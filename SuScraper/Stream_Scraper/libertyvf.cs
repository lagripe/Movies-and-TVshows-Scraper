using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stream_Scraper
{
    class libertyvf
    {

        static int getPagesCount(string url)
        {
            HtmlWeb browser = new HtmlWeb();
            HtmlDocument doc = browser.Load(url);
            return int.Parse(doc.DocumentNode.SelectSingleNode("//ul[@class='pagination']").SelectNodes(".//a")[10].InnerText);
        }
        static List<string> getMoviesURLs(string url)
        {
            HtmlWeb browser = new HtmlWeb();
            HtmlDocument doc = browser.Load(url);
            List<string> URLs = new List<string>();
            HtmlNodeCollection tags = doc.DocumentNode.SelectNodes("//figure");
            Regex pattern = new Regex(@"films\/");
            foreach (HtmlNode tag in tags)
                URLs.Add(pattern.Replace(tag.SelectSingleNode(".//a").Attributes["href"].Value,"films/streaming/"));
            return URLs;

        }
        static List<string> getTVsURLs(string url)
        {
            HtmlWeb browser = new HtmlWeb();
            HtmlDocument doc = browser.Load(url);
            List<string> URLs = new List<string>();
            HtmlNodeCollection tags = doc.DocumentNode.SelectNodes("//div[@class='divtelecharger']");
            Regex pattern = new Regex(@"telecharger");
            foreach (HtmlNode tag in tags)
                URLs.Add(pattern.Replace(tag.SelectSingleNode(".//a").Attributes["href"].Value, "streaming"));
            return URLs;

        }
        static List<string> getMovieStreams(HtmlDocument doc,HtmlWeb browser, string URL)
        {
            List<string> streams = new List<string>();
            Regex pattern = new Regex("[\\\\\"]");
            try { 
                foreach (HtmlNode stream in doc.DocumentNode.SelectSingleNode("//div[@class='for-scroll']").SelectNodes(".//div"))
                {
                    HtmlDocument stream_doc = browser.Load("http://libertyvf.co/v2/video.php?id="
                                                            + stream.Attributes["streaming"].Value
                                                            + "&id_movie=" + URL.Split('/').Last().Split('-').First()
                                                            + "&type=films");

                    streams.Add(pattern.Replace(stream_doc.DocumentNode.SelectSingleNode("//iframe").Attributes["src"].Value, ""));

                }
                return streams;
            }
            catch { return null; }
        }
        static List<string> getEpisodeStreams( string URL)
        {
            List<string> streams = new List<string>();
            Regex pattern = new Regex("[\\\\\"]");
            HtmlWeb browser = new HtmlWeb();
            HtmlDocument doc = browser.Load(URL);

            try
            {
                foreach (HtmlNode scroll in doc.DocumentNode.SelectNodes("//div[@class='for-scroll']"))
                {
                    foreach (HtmlNode stream in scroll.SelectNodes(".//div"))
                    {
                        HtmlDocument stream_doc = browser.Load("http://libertyvf.co/v2/video.php?id="
                                                            + stream.Attributes["streaming"].Value
                                                            + "&type=series");

                        streams.Add(pattern.Replace(stream_doc.DocumentNode.SelectSingleNode("//iframe").Attributes["src"].Value, ""));
                    }
                    

                }
                return streams;
            }
            catch { return null; }
        }

        static void getMoviesPerPage(Bunifu.Framework.UI.BunifuCheckbox UpdateCheck,System.Windows.Forms.PictureBox preview,List<string> URLs, string table_db,Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid)
        {
            
            foreach (string URL in URLs)
            {
                HtmlWeb browser = new HtmlWeb();
                try
                {
                    Console.WriteLine(URL);
                    HtmlDocument doc = browser.Load(URL);
                    Regex pattern0 = new Regex(@"\(.*\)");
                    string title = doc.DocumentNode.SelectSingleNode("//h2[@class='heading']").InnerText.Trim();
                    title = pattern0.Replace(title, "").Trim();
                    Movie movie = Information.getTMDB_Movie(Information.getTMDBId_movie(title), "fr-FR");
                    if (movie == null)
                        continue;
                    
                    preview.ImageLocation = "https://image.tmdb.org/t/p/w300_and_h450_bestv2" + movie.poster_path;
                    // ---- get streams ------
                    List<string> streams = getMovieStreams(doc, browser, URL);
                    // ---- save into db ------
                    Information.dumpMovie_db(UpdateCheck,movie, streams, table_db, dataGrid);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    
                }
                
            }
        }
        static Dictionary<int, Dictionary<int, List<string>>>  getTVData(string URL)
        {
            Dictionary<int, Dictionary<int, List<string>>> Data = new Dictionary<int, Dictionary<int, List<string>>>();
            HtmlWeb browser = new HtmlWeb();
            // GET Seasons
            HtmlDocument doc = browser.Load(URL);
            foreach (HtmlNode heading in doc.DocumentNode.SelectNodes("//h2[@class='heading-small']"))
            {
                HtmlNode Season = heading.ParentNode;
                Regex pattern = new Regex("[^0-9]");
                int SE = int.Parse(pattern.Replace(Season.SelectSingleNode(".//h2").InnerText,""));
                Console.WriteLine(Season.SelectSingleNode(".//h2").InnerText);
                Dictionary<int, List<string>> Episodes = new Dictionary<int, List<string>>();
                foreach (HtmlNode episode in Season.SelectNodes(".//a"))
                {
                    try {
                        string EP_URL = episode.Attributes["href"].Value;
                        List<string> streams = getEpisodeStreams(EP_URL);
                        if (streams.Count > 0)
                            Episodes.Add(int.Parse(episode.InnerText), streams);
                        Console.WriteLine($"{SE}x{episode.InnerText} --> {string.Join(" , ", streams)}");
                    }
                    catch {
                        Episodes.Add(int.Parse(episode.InnerText), new List<string>());
                        Console.WriteLine($"{SE}x{episode.InnerText} --> ");
                    }
                }
                Data.Add(SE, Episodes);
            }
            return Data;
        }
        static void getTVPerPage(System.Windows.Forms.PictureBox preview, List<string> URLs, string table_db, string seasons_db, string stream_db, Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid, Bunifu.Framework.UI.BunifuCheckbox UpdateCheck)
        {

            foreach (string URL in URLs)
            {
                HtmlWeb browser = new HtmlWeb();
                try
                {
                    Console.WriteLine(URL);
                    HtmlDocument doc = browser.Load(URL);
                    Regex pattern0 = new Regex(@"\(.*\)");
                    string title = doc.DocumentNode.SelectSingleNode("//h2[@class='heading']").InnerText.Trim();
                    title = pattern0.Replace(title, "").Trim();
                    TV tv = Information.getTMDB_TV(Information.getTMDBId_TV(title), "fr-FR");
                    if (tv == null)
                        continue;
                    if (!Information.DataExist_db(tv.id.ToString(), table_db))
                        Information.insertTV(tv,table_db);
                    if (dataGrid.InvokeRequired) dataGrid.Invoke(new Action(() => dataGrid.Rows.Add(title, "UPDATED")));
                    preview.ImageLocation = "https://image.tmdb.org/t/p/w300_and_h450_bestv2" + tv.poster_path;
                    //-- get seasons episodes <Season,<Episode,Streams>>
                    Dictionary<int, Dictionary<int, List<string>>> tvData = getTVData(URL);
                    // ---- save into db ------
                    Information.dumpSeasonsEpisodes_db(tv,tvData, seasons_db,stream_db, "fr-FR", UpdateCheck);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }

            }
        }
        public static Task getAllMovies(Bunifu.Framework.UI.BunifuCheckbox UpdateCheck,System.Windows.Forms.PictureBox preview, Bunifu.Framework.UI.BunifuFlatButton Start, Bunifu.Framework.UI.BunifuFlatButton Stop, Bunifu.Framework.UI.BunifuCircleProgressbar Progress, Stream_Scraper.Form1.StopWatch Watch, Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid, System.Windows.Forms.Label Total, System.Windows.Forms.Label Scraped, string url,string table_db)
        {
            int PagesCount = getPagesCount(url);
            Total.Invoke(new Action(() => Total.Text = PagesCount.ToString()));
            for (int i = 1297; i <= PagesCount; i++)
            {
                List<string> moviesURLs = null;
                if (i==1)
                    moviesURLs = getMoviesURLs($"https://libertyvf.co/films/");
                else
                   moviesURLs = getMoviesURLs($"https://libertyvf.co/films-p{i}.html");
                getMoviesPerPage(UpdateCheck,preview, moviesURLs, table_db, dataGrid);
                Scraped.Invoke(new Action(() => Scraped.Text = i.ToString()));
            }
            Start.Invoke(new Action(() => Start.Enabled = true));
            Stop.Invoke(new Action(() => Stop.Enabled = false));
            Progress.Invoke(new Action(() => Progress.animated = false));
            Progress.Invoke(new Action(() => Progress.Visible = false));
            return null;
        }
        public static Task getAllTV(Bunifu.Framework.UI.BunifuCheckbox UpdateCheck, System.Windows.Forms.PictureBox preview, Bunifu.Framework.UI.BunifuFlatButton Start, Bunifu.Framework.UI.BunifuFlatButton Stop, Bunifu.Framework.UI.BunifuCircleProgressbar Progress, Stream_Scraper.Form1.StopWatch Watch, Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid, System.Windows.Forms.Label Total, System.Windows.Forms.Label Scraped, string url, string table_db, string seasons_db, string stream_db)
        {
            int PagesCount = getPagesCount(url);
            Total.Invoke(new Action(() => Total.Text = PagesCount.ToString()));
            for (int i = 1; i <= PagesCount; i++)
            {
                List<string> TVURLs = null;
                

                if (i == 1)
                    TVURLs = getTVsURLs($"https://libertyvf.co/series/");
                else
                    TVURLs = getTVsURLs($"https://libertyvf.co/series/page-{i}.html");
                getTVPerPage(preview, TVURLs, table_db, seasons_db, stream_db, dataGrid, UpdateCheck);
                Scraped.Invoke(new Action(() => Scraped.Text = i.ToString()));
            }
            Start.Invoke(new Action(() => Start.Enabled = true));
            Stop.Invoke(new Action(() => Stop.Enabled = false));
            Progress.Invoke(new Action(() => Progress.animated = false));
            Progress.Invoke(new Action(() => Progress.Visible = false));
            return null;
        }
    }
}
