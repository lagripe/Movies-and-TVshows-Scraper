using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace Stream_Scraper
{
    class ryuanime
    {

        private static List<string> getAnimeList(string url)
        {
            HtmlWeb browser = new HtmlWeb();
            HtmlDocument document = browser.Load(url);
            HtmlNodeCollection List = document.DocumentNode.SelectNodes("//div[@class='anime_list_body']//a");
            List<string> AnimeList = new List<string>();
            foreach (HtmlNode item in List)
            {
                AnimeList.Add("https://www2.gogoanime.io"+item.Attributes["href"].Value);
            }
            return AnimeList;
        }
        
        private static Dictionary<string, List<string>> getEpisodesPerAnime(string link,int num)
        {
            Dictionary<string, List<string>> Episodes = new Dictionary<string, List<string>>();
            HtmlWeb browser = new HtmlWeb();
            for (int i = 1; i <= num; i++)
            {
                string TotalLink = "https://www2.gogoanime.io/" + link + $"-episode-{i}";
                HtmlDocument document = browser.Load(TotalLink);
                List<string> StreamsPerEp = new List<string>();
                foreach (HtmlNode stream in document.DocumentNode.SelectNodes("//div[@class='anime_muti_link']//a"))
                    StreamsPerEp.Add(stream.Attributes["data-video"].Value);
                //Console.WriteLine($"Episode {i} ===> Streams {StreamsPerEp.Count}");
                Episodes.Add(i.ToString(), StreamsPerEp);
            }

            return Episodes;
        }
        private static List<Anime> getAnimesPerPage(Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid,System.Windows.Forms.PictureBox preview,List<string> Links)
        {
            List<Anime> animes = new List<Anime>();
            HtmlWeb browser = new HtmlWeb();


            foreach (string link in Links)
            {
                try {
                    //Console.WriteLine(link);
                    HtmlDocument document = browser.Load(link);
                    Anime anime = new Anime();
                    HtmlNodeCollection info = document.DocumentNode.SelectSingleNode("//div[@class='anime_info_body_bg']").SelectNodes("p[@class='type']");
                    anime.Title = document.DocumentNode.SelectSingleNode("//div[@class='anime_info_body_bg']").SelectSingleNode("h1").InnerText;
                    anime.Image = document.DocumentNode.SelectSingleNode("//div[@class='anime_info_body_bg']").SelectSingleNode("img").Attributes["src"].Value;
                    preview.Invoke(new Action(() => preview.ImageLocation = anime.Image));
                    foreach (HtmlNode item in info)
                    {
                        string choice = item.FirstChild.InnerText.Trim();
                        item.FirstChild.Remove();
                        switch (choice)
                        {


                            case "Type:":
                                anime.Type = item.SelectSingleNode("a").Attributes["title"].Value;
                                break;

                            case "Status:":
                                anime.Status = item.InnerText;
                                break;
                            case "Released:":
                                anime.Aired = item.InnerText;
                                break;
                            case "Genre:":
                                foreach (HtmlNode genre in item.SelectNodes("a"))
                                    anime.Genres.Add(genre.Attributes["title"].Value);
                                break;

                            case "Plot Summary:":
                                anime.Summary = item.InnerText;
                                break;
                            default: break;
                        }
                    }
                    //------get Episodes

                    int numEpisodes = int.Parse(document.DocumentNode.SelectSingleNode("//ul[@id='episode_page']")
                                                .SelectNodes("li")
                                                .Last()
                                                .SelectSingleNode("a")
                                                .Attributes["ep_end"].Value);
                    string unq = link.Split(new[] { "category/" }, StringSplitOptions.None)[1];
                    anime.Streams = getEpisodesPerAnime(unq, numEpisodes);
                    anime.Unique = unq;
                    dumpIntodb(dataGrid,anime);
                    //dumpScrapedAnimeGrid(dataGrid,anime);
                    //Console.WriteLine(anime.ToString());
                    animes.Add(anime);

                }
                catch
                {

                }
                
            }

            
            
            return animes;
        }
        
        public static Task getAnimes(System.Windows.Forms.PictureBox preview, Bunifu.Framework.UI.BunifuFlatButton Start, Bunifu.Framework.UI.BunifuFlatButton Stop, Bunifu.Framework.UI.BunifuCircleProgressbar Progress, Stream_Scraper.Form1.StopWatch Watch, Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid, System.Windows.Forms.Label Total, System.Windows.Forms.Label Scraped, string url)
        {
            int PageCount = 45;
            
            
            Total.Invoke(new Action(() => Total.Text = PageCount.ToString()));
            try
            {
                for (int i = 1; i < PageCount; i++)
                {

                    List<string> AnimeList = getAnimeList(url+ $"?page={i}");
                    List<Anime> animes = getAnimesPerPage(dataGrid,preview, AnimeList);
                    Scraped.Invoke(new Action(() => Scraped.Text = (i).ToString()));
                }
            }
            catch
            {

            }
            finally
            {
                Start.Invoke(new Action(() => Start.Enabled = true));
                Stop.Invoke(new Action(() => Stop.Enabled = false));
                Progress.Invoke(new Action(() => Progress.animated = false));
                Progress.Invoke(new Action(() => Progress.Visible = false));
            }


            return null;
        }

        private static void dumpIntodb(Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid,Anime anime)
        {
            MySqlConnection con = new MySqlConnection("server=127.0.0.1;uid=root;pwd=;database=anime_db");
            string idAnime = "";
            try
            {
                //---- dump anime info
                
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"INSERT INTO animes (Uniq,Title,Type,Status,Aired,Summary,Image) values ('{anime.Unique.Trim()}','{anime.Title.Trim()}','{anime.Type.Trim()}','{anime.Status.Trim()}','{anime.Aired.Trim()}','{anime.Summary.Trim().Replace(@"'",@"\'")}','{anime.Image}')";
                cmd.ExecuteNonQuery();

                //--------- get Anime Id_anime
                cmd.CommandText = "SELECT id_anime FROM animes ORDER BY id_anime DESC LIMIT 1";
                idAnime = cmd.ExecuteScalar().ToString();
                //--------- dump anime genres
                foreach (string item in anime.Genres)
                {
                    cmd.CommandText = $"select id_genre from genres where genre_desc = '{item}'";
                    string idGenre = cmd.ExecuteScalar().ToString();
                    //Console.WriteLine(cmd.CommandText);
                    cmd.CommandText = $"INSERT INTO anime_genre (id_anime,id_genre) values({idAnime},{idGenre})";
                    cmd.ExecuteNonQuery();
                }
                //------- dump animes episodes
                foreach (KeyValuePair<string,List<string>> item in anime.Streams)
                {
                    cmd.CommandText = $"INSERT INTO episodes (id_anime,ep_number,streams) values ({idAnime},{item.Key},'{string.Join(",", item.Value)}')";
                    cmd.ExecuteNonQuery();
                }

                dataGrid.Invoke(new Action(() => dataGrid.Rows.Add(anime.Title, anime.Type, anime.Aired, anime.Status)));

            }
            catch (MySqlException e){
                Console.WriteLine(e.Message);
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"DELETE FROM animes WHERE id_anime = {idAnime}";
                cmd.ExecuteNonQuery();
                cmd.CommandText = $"DELETE FROM anime_genre WHERE id_anime = {idAnime}";
                cmd.ExecuteNonQuery();
                cmd.CommandText = $"DELETE FROM episodes WHERE id_anime = {idAnime}";
                cmd.ExecuteNonQuery();


            }
            finally
            {
                
                con.Close();
            }
            
        }

    }
}
