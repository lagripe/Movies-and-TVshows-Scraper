using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stream_Scraper
{
    class frenchStream
    {
        static int getPagesCount(string url)
        {
            HtmlWeb browser = new HtmlWeb();
            HtmlDocument doc = browser.Load(url);
            
            return int.Parse(doc.DocumentNode.SelectSingleNode("//span[@class='navigation']").SelectNodes(".//a").Last().InnerText);
        }
        static List<string> getMoviesURLs(string url)
        {
            HtmlWeb browser = new HtmlWeb();

            HtmlDocument doc = browser.Load(url);

            List<string> URLs = new List<string>();
            HtmlNodeCollection tags = doc.DocumentNode.SelectNodes("//div[@class='short']");

            foreach (HtmlNode tag in tags)
            {
                URLs.Add(tag.SelectSingleNode(".//a[@class='short-poster img-box with-mask']").Attributes["href"].Value);
            }


            return URLs;

        }
        static List<string> getMovieStreams(HtmlDocument doc)
        {
            List<string> streams = new List<string>();
            try
            {
                foreach (HtmlNode stream in doc.DocumentNode.SelectNodes("//a[@target='seriePlayer']"))
                {
                    if(!stream.Attributes["href"].Value.Contains("hqq"))
                        try {
                            string r = getRealStream(stream.Attributes["href"].Value);
                            if(!r.Contains("google"))
                                streams.Add(r);
                        }
                        catch { }
                }
                return streams;
            }
            catch { return null; }
        }

        static string getRealStream(string url)
        {
            /*var request = (HttpWebRequest)WebRequest.Create(url);
            
            request.Proxy = null;
            WebResponse res = request.GetResponse();
            return res.ResponseUri.AbsoluteUri;*/
            
            HtmlWeb browser = new HtmlWeb();
            browser.PreRequest = OnPreRequest;
            browser.Load(url);
            return browser.ResponseUri.AbsoluteUri;
        }
        private static bool OnPreRequest(HttpWebRequest request)
        {
            request.AllowAutoRedirect = true;
            return true;
        }
        static void getMoviesPerPage(Bunifu.Framework.UI.BunifuCheckbox UpdateCheck,System.Windows.Forms.PictureBox preview, List<string> URLs, string table_db, string stream_db, Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid)
        {

            foreach (string URL in URLs)
            {
                HtmlWeb browser = new HtmlWeb();
                try
                {
                    Console.WriteLine(URL);
                    HtmlDocument doc = browser.Load(URL);
                    Regex pattern0 = new Regex(@"\(.*\)");
                    string title = doc.DocumentNode.SelectSingleNode("//h1[@id='s-title']").InnerText.Trim();
                    title = pattern0.Replace(title, "").Trim();
                    Console.WriteLine(title);
                    Movie movie = Information.getTMDB_Movie(Information.getTMDBId_movie(title), "fr-FR");
                    if (movie == null)
                        continue;

                    preview.ImageLocation = "https://image.tmdb.org/t/p/w300_and_h450_bestv2" + movie.poster_path;
                    
                    // ---- get streams ------
                    List<string> streams = getMovieStreams(doc);
                    // ---- save into db ------
                    Information.dumpMovie_db(UpdateCheck,movie, streams, table_db, dataGrid);
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }

            }
        }

        public static Task getAllMovies(Bunifu.Framework.UI.BunifuCheckbox UpdateCheck,System.Windows.Forms.PictureBox preview, Bunifu.Framework.UI.BunifuFlatButton Start, Bunifu.Framework.UI.BunifuFlatButton Stop, Bunifu.Framework.UI.BunifuCircleProgressbar Progress, Stream_Scraper.Form1.StopWatch Watch, Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid, System.Windows.Forms.Label Total, System.Windows.Forms.Label Scraped, string url, string table_db, string stream_db)
        {
            int PagesCount = getPagesCount(url);
            Total.Invoke(new Action(() => Total.Text = PagesCount.ToString()));

            for (int i = 1; i <= PagesCount; i++)
            {
                try {
                    List<string> moviesURLs = null;

                    moviesURLs = getMoviesURLs($"https://french-film.co/film-streaming/page/{i}/");
                    getMoviesPerPage(UpdateCheck, preview, moviesURLs, table_db, stream_db, dataGrid);
                    Scraped.Invoke(new Action(() => Scraped.Text = i.ToString()));
                }
                catch { }
            }
            Start.Invoke(new Action(() => Start.Enabled = true));
            Stop.Invoke(new Action(() => Stop.Enabled = false));
            Progress.Invoke(new Action(() => Progress.animated = false));
            Progress.Invoke(new Action(() => Progress.Visible = false));
            return null;
        }

        static List<string> getTVsURLs(string url)
        {
            HtmlWeb browser = new HtmlWeb();

            HtmlDocument doc = browser.Load(url);

            List<string> URLs = new List<string>();
            HtmlNodeCollection tags = doc.DocumentNode.SelectNodes("//div[@class='short']");

            foreach (HtmlNode tag in tags)
            {
                URLs.Add(tag.SelectSingleNode(".//a[@class='short-poster img-box with-mask']").Attributes["href"].Value);
            }

            return URLs;

        }
        static List<string> getSeasonsURLs(string url)
        {
            HtmlWeb browser = new HtmlWeb();
            HtmlDocument doc = browser.Load(url);

            List<string> URLs = new List<string>();
            HtmlNodeCollection tags = doc.DocumentNode.SelectNodes("//div[@class='short']");
            foreach (HtmlNode tag in tags)
            {
                URLs.Add(tag.SelectSingleNode(".//a[@class='short-poster img-box with-mask']").Attributes["href"].Value);
            }

            return URLs;

        }
        static Dictionary<int, Dictionary<int, List<string>>> getTVData(string URL)
        {
            Dictionary<int, Dictionary<int, List<string>>> Data = new Dictionary<int, Dictionary<int, List<string>>>();
            // GET Seasons
            HttpClient browser = Information.getClient();

            foreach (String sURL in getSeasonsURLs(URL))
            {
                try
                {
                    HtmlDocument doc = Information.getPageData(browser, sURL);
                    Regex p = new Regex("Saison ([0-9]+)");
                    Regex e = new Regex("pisode ([0-9]+)");
                    string info = doc.DocumentNode.SelectSingleNode(".//h1[@id='s-title']").InnerText.Trim();
                    string season = p.Match(info).Groups[1].Value;
                    int SE = int.Parse(season);
                    Console.WriteLine("Season : " + SE);
                    HtmlNode container = doc.DocumentNode.SelectSingleNode("//div[@class='series-center']");
                    if (container == null)
                        continue;
                    Dictionary<int, List<string>> EPISODES = new Dictionary<int, List<string>>();
                    foreach (HtmlNode divParent in container.SelectNodes(".//div[@class='selink']"))
                    {
                        

                        try
                        {

                            if (divParent.SelectSingleNode(".//span").InnerText.Contains("VOSTFR"))
                                continue;
                            string EPI = e.Match(divParent.SelectSingleNode(".//span").InnerText).Groups[1].Value;
                            int EP = int.Parse(EPI);
                            
                            string realS = "";
                            List<string> streams = new List<string>();
                            foreach (HtmlNode links in divParent.SelectSingleNode(".//ul[@class='btnss']").SelectNodes(".//a"))
                            {
                                try
                                {
                                    if (links.InnerText.Contains("Lecteur")) {
                                        if (!links.Attributes["href"].Value.Contains("hqq") && links.Attributes["href"].Value.Contains("http")) {
                                            realS = getRealStream(links.Attributes["href"].Value);
                                            if (!realS.Contains("google"))
                                                streams.Add(realS);
                                        }
                                        

                                    }
                                    
                                }
                                catch { }
                                

                            }
                            
                            EPISODES.Add(EP, streams);
                            Console.WriteLine($"{SE}x{EP} --> {string.Join(" , ", streams)}");

                        }
                        catch(Exception z) { Console.WriteLine(z.Message); }
                    }

                    Data.Add(SE, EPISODES);
                }
                catch { }

                
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
                    //Regex p = new Regex("Saison ([0-9]+)");
                    Regex t = new Regex("- Saison [0-9]+");
                    string info = doc.DocumentNode.SelectSingleNode("//h1[@id='s-title']").InnerText.Trim();
                    info = pattern0.Replace(info, "").Trim();
                    //string season = p.Match(info).Groups[1].Value;
                    string title = t.Replace(info, "").Trim();
                    TV tv = Information.getTMDB_TV(Information.getTMDBId_TV(title), "fr-FR");
                    if (tv == null)
                        continue;
                    if (!Information.DataExist_db(tv.id.ToString(), table_db))
                    {
                        Information.insertTV(tv, table_db);
                        if (dataGrid.InvokeRequired) dataGrid.Invoke(new Action(() => dataGrid.Rows.Add(tv.name, "INSERTED")));
                    }
                    else
                    {
                        if (UpdateCheck.Checked)
                        {
                            if (dataGrid.InvokeRequired) dataGrid.Invoke(new Action(() => dataGrid.Rows.Add(tv.name, "UPDATED")));
                        }
                        else
                            if (dataGrid.InvokeRequired) dataGrid.Invoke(new Action(() => dataGrid.Rows.Add(tv.name, "SEARCHING NEW EPs")));
                    }

                    preview.ImageLocation = "https://image.tmdb.org/t/p/w300_and_h450_bestv2" + tv.poster_path;
                    

                    string seasonsURL = "https://french-serie.co" + doc.DocumentNode.SelectSingleNode(".//div[@class='fmeta icon-l']")
                                                        .SelectSingleNode(".//strong").ParentNode.ParentNode.Attributes["href"].Value;
                    seasonsURL = Uri.EscapeUriString(seasonsURL);
                    //-- get seasons episodes <Season,<Episode,Streams>>
                    Dictionary<int, Dictionary<int, List<string>>> tvData = getTVData(seasonsURL);
                    // ---- save into db ------
                    Information.dumpSeasonsEpisodes_db(tv, tvData, seasons_db, stream_db, "fr-FR", UpdateCheck);
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }

            }
           
        }
        
        public static Task getAllTV(Bunifu.Framework.UI.BunifuCheckbox UpdateCheck, System.Windows.Forms.PictureBox preview, Bunifu.Framework.UI.BunifuFlatButton Start, Bunifu.Framework.UI.BunifuFlatButton Stop, Bunifu.Framework.UI.BunifuCircleProgressbar Progress, Stream_Scraper.Form1.StopWatch Watch, Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid, System.Windows.Forms.Label Total, System.Windows.Forms.Label Scraped, string url, string table_db, string seasons_db, string stream_db)
        {
            int PagesCount = getPagesCount(url);
            Total.Invoke(new Action(() => Total.Text = PagesCount.ToString()));
            for (int i = 1; i <= PagesCount; i++)
            {
                List<string> TVURLs = null;
                TVURLs = getTVsURLs($"https://french-serie.co/serie-streaming/serie-en-vf-streaming/page/{i}/");
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
