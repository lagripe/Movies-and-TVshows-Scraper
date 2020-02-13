using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stream_Scraper
{
    public class Genre
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class ProductionCompany
    {
        public int id { get; set; }
        public string logo_path { get; set; }
        public string name { get; set; }
        public string origin_country { get; set; }
    }
    public class GuestStar
    {
        public int id { get; set; }
        public string name { get; set; }
        public string credit_id { get; set; }
        public string character { get; set; }
        public int order { get; set; }
        public int gender { get; set; }
        public string profile_path { get; set; }
    }
    public class ProductionCountry
    {
        public string iso_3166_1 { get; set; }
        public string name { get; set; }
    }

    public class SpokenLanguage
    {
        public string iso_639_1 { get; set; }
        public string name { get; set; }
    }

    public class Cast
    {
        public int cast_id { get; set; }
        public string character { get; set; }
        public string credit_id { get; set; }
        public int gender { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public string profile_path { get; set; }
    }

    public class Crew
    {
        public string credit_id { get; set; }
        public string department { get; set; }
        public int gender { get; set; }
        public int id { get; set; }
        public string job { get; set; }
        public string name { get; set; }
        public string profile_path { get; set; }
    }

    public class Credits
    {
        public List<Cast> cast { get; set; }
        public List<Crew> crew { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public string iso_639_1 { get; set; }
        public string iso_3166_1 { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string site { get; set; }
        public int size { get; set; }
        public string type { get; set; }
    }

    public class Videos
    {
        public List<Result> results { get; set; }
    }

    public class Images
    {
        public List<string> backdrops { get; set; }
        public List<string> posters { get; set; }
    }
    public class Season
    {
        public string air_date { get; set; }
        public int episode_count { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public int season_number { get; set; }
    }
    public class CreatedBy
    {
        public int id { get; set; }
        public string credit_id { get; set; }
        public string name { get; set; }
        public int gender { get; set; }
        public string profile_path { get; set; }
    }
    public class LastEpisodeToAir
    {
        public string air_date { get; set; }
        public int episode_number { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public string production_code { get; set; }
        public int season_number { get; set; }
        public int show_id { get; set; }
        public string still_path { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }

    public class NextEpisodeToAir
    {
        public string air_date { get; set; }
        public int episode_number { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public string production_code { get; set; }
        public int season_number { get; set; }
        public int show_id { get; set; }
        public object still_path { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }

    public class Network
    {
        public string name { get; set; }
        public int id { get; set; }
        public string logo_path { get; set; }
        public string origin_country { get; set; }
    }
    public class Movie
    {
        public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public object belongs_to_collection { get; set; }
        public int budget { get; set; }
        public List<Genre> genres { get; set; }
        public string homepage { get; set; }
        public int id { get; set; }
        public string imdb_id { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public List<ProductionCompany> production_companies { get; set; }
        public List<ProductionCountry> production_countries { get; set; }
        public string release_date { get; set; }
        public int revenue { get; set; }
        public int runtime { get; set; }
        public List<SpokenLanguage> spoken_languages { get; set; }
        public string status { get; set; }
        public string tagline { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public Credits credits { get; set; }
        public Videos videos { get; set; }
        public Images images { get; set; }
    }
    public class TV
    {
        public string backdrop_path { get; set; }
        public List<CreatedBy> created_by { get; set; }
        public List<int> episode_run_time { get; set; }
        public string first_air_date { get; set; }
        public List<Genre> genres { get; set; }
        public string homepage { get; set; }
        public int id { get; set; }
        public bool in_production { get; set; }
        public List<string> languages { get; set; }
        public string last_air_date { get; set; }
        public LastEpisodeToAir last_episode_to_air { get; set; }
        public string name { get; set; }
        public NextEpisodeToAir next_episode_to_air { get; set; }
        public List<Network> networks { get; set; }
        public int number_of_episodes { get; set; }
        public int number_of_seasons { get; set; }
        public List<string> origin_country { get; set; }
        public string original_language { get; set; }
        public string original_name { get; set; }
        public string overview { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public List<ProductionCompany> production_companies { get; set; }
        public List<Season> seasons { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public Credits credits { get; set; }
        public Videos videos { get; set; }
        public Images images { get; set; }
    }
    public class Episode
    {
        public string air_date { get; set; }
        public List<Crew> crew { get; set; }
        public int episode_number { get; set; }
        public List<GuestStar> guest_stars { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public int id { get; set; }
        public string production_code { get; set; }
        public int season_number { get; set; }
        public string still_path { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }
    class Information
    {
        static MySqlConnection con = new MySqlConnection("server=127.0.0.1;uid=root;pwd=;database=collection_db");

        public static MySqlConnection Externalconnection=null;
        public static ForwardedPortLocal portFwld;
        public static SshClient sshclient;

        public static bool EstablishExternalConnection()

        {
            if (Externalconnection != null)
                return false;
            try {
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo("premium59.web-hosting.com", 21098, "tfariyiy", "streamingvf@123");
                connectionInfo.Timeout = TimeSpan.FromSeconds(30);
                sshclient = new SshClient(connectionInfo);
                sshclient.Connect();
                portFwld = new ForwardedPortLocal("127.0.0.1"/*your computer ip*/, "127.0.0.1" /*server ip*/, 3306 /*server mysql port*/);
                sshclient.AddForwardedPort(portFwld);
                portFwld.Start();
                //// using Renci.sshNet 
                Externalconnection = new MySqlConnection("server = " + "127.0.0.1" /*you computer ip*/ + "; Database = tfariyiy_wp443; UID = tfariyiy_wp443; PWD =D)1pS14(Ts; Port = " + portFwld.BoundPort /*very important !!*/);
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

        }
        public static void uploadRCF(string serialized,string postID) {
            if (Externalconnection.State == System.Data.ConnectionState.Open)
                Externalconnection.Close();
            Externalconnection.Open();
            MySqlCommand cmd = new MySqlCommand($"UPDATE wpde_postmeta SET meta_value = '{serialized}' where post_id = {postID} and meta_key = 'repeatable_fields'", Externalconnection);
            cmd.ExecuteNonQuery();
            Externalconnection.Close();
        }

        public static List<String> filterLiveStreams(string streams)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc;
            List<string> output = new List<string>();
            foreach(string stream in streams.Split('\n'))
            {
                try { 
                    doc = web.Load(stream);
                    if (doc.DocumentNode.WriteContentTo().Contains("video") || doc.DocumentNode.WriteContentTo().Contains("iframe"))
                        output.Add(stream);
                }
                catch
                {

                }
                
            }
            return output;
        }
        public static void UpdateRate(string rate, string count, string postID)
        {
            if (Externalconnection.State == System.Data.ConnectionState.Open)
                Externalconnection.Close();
            Externalconnection.Open();
            MySqlCommand cmd = new MySqlCommand($"UPDATE wpde_postmeta SET meta_value = '{rate}' where post_id = {postID} and meta_key = 'vote_average'", Externalconnection);
            cmd.ExecuteNonQuery();
            cmd = new MySqlCommand($"UPDATE wpde_postmeta SET meta_value = '{count}' where post_id = {postID} and meta_key = 'vote_count'", Externalconnection);
            cmd.ExecuteNonQuery();
            Externalconnection.Close();
        }
        static string API_KEY()
        {
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandText = $"SELECT tmdb_key from config where id_config = 1";
                string _API = cmd.ExecuteScalar().ToString();
                con.Close();
                return _API;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Exit();
                return null;
            }
        }
        public static HttpClient getClient()
        {
            string baseUrl = "https://french-serie.co/";
            string loginUrl = "";
            

            var uri = new Uri(baseUrl);

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            HttpClient client = new HttpClient(handler);
            
            client.BaseAddress = uri;

            var request = new FormUrlEncodedContent(new[]
                                    {
                                        new KeyValuePair<string, string>("login", "submit"),
                                        new KeyValuePair<string, string>("login_name", "PHP52"),
                                        new KeyValuePair<string, string>("login_password", "123456789")
                                    });
            var resLogin = client.PostAsync(loginUrl, request).Result;
            if (resLogin.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Could not login -> StatusCode = " + resLogin.StatusCode);

            // see what cookies are returned   
            IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
                Console.WriteLine(cookie.Name + ": " + cookie.Value);
            //Request Page
            /*var resData = client.GetAsync("https://french-serie.co/9356-serie-game-of-thrones-saison-1-stream-complet-vf.html").Result;
            if (resData.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Could not get data html -> StatusCode = " + resData.StatusCode);

            var html = resData.Content.ReadAsStringAsync().Result;

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            Console.WriteLine(doc.DocumentNode.SelectSingleNode(".//div[@class='elink']").SelectNodes(".//a").Count);
            */
            return client;
            
        }
        public static HtmlAgilityPack.HtmlDocument getPageData(HttpClient client, string url)
        {
            var resData = client.GetAsync(url).Result;
            if (resData.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Could not get data html -> StatusCode = " + resData.StatusCode);

            var html = resData.Content.ReadAsStringAsync().Result;
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
        public static Movie getTMDB_Movie(string id_tmdb,string lang)
        {

            using (System.Net.WebClient client = new System.Net.WebClient()) // WebClient class inherits IDisposable
            {
                try
                {
                    var htmlData = client.DownloadData($"https://api.themoviedb.org/3/movie/{id_tmdb}?api_key={API_KEY()}&language={lang}&append_to_response=credits,videos,images");
                var json = Encoding.UTF8.GetString(htmlData);
                
                
                    return JsonConvert.DeserializeObject<Movie>(json);
                }
                catch
                {
                    return null;
                }
            }
            
        }
        public static TV getTMDB_TV(string id_tmdb, string lang)
        {
            using (System.Net.WebClient client = new System.Net.WebClient()) // WebClient class inherits IDisposable
            {
                var htmlData = client.DownloadData($"https://api.themoviedb.org/3/tv/{id_tmdb}?api_key={API_KEY()}&language={lang}&append_to_response=credits,videos,images");
                var json = Encoding.UTF8.GetString(htmlData);

                try
                {
                    return JsonConvert.DeserializeObject<TV>(json);
                }
                catch
                {
                    return null;
                }
            }


        }
        public static Episode getTMDB_Episode(string id_tmdb, string lang,int s,int e)
        {

           
            using (System.Net.WebClient client = new System.Net.WebClient()) // WebClient class inherits IDisposable
            {
                var htmlData = client.DownloadData($"https://api.themoviedb.org/3/tv/{id_tmdb}/season/{s}/episode/{e}?api_key={API_KEY()}&language={lang}");
                var json = Encoding.UTF8.GetString(htmlData);

                try
                {
                    return JsonConvert.DeserializeObject<Episode>(json);
                }
                catch
                {
                    return null;
                }
            }

        }
        public static  string getTMDBId_movie(string title) {
            HtmlWeb browser = new HtmlWeb();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HtmlAgilityPack.HtmlDocument document = browser.Load($"https://www.themoviedb.org/search/movie?query={title}");
            HtmlNode id = document.DocumentNode.SelectSingleNode("//div[@class='item poster card']");
            if (id == null)
                return null;
            id = id.SelectSingleNode("//a[@class='title result']");
            return id.Attributes["id"].Value.Split('_')[1];

        }
        public static string getTMDBId_TV(string title)
        {
            HtmlWeb browser = new HtmlWeb();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HtmlAgilityPack.HtmlDocument document = browser.Load($"https://www.themoviedb.org/search/tv?query={title}");
            HtmlNode id = document.DocumentNode.SelectSingleNode("//div[@class='item poster card']");
            if (id == null)
                return null;
            id = id.SelectSingleNode("//a[@class='title result']");
            return id.Attributes["id"].Value.Split('_')[1];

        }

        public static void dumpMovie_db(Bunifu.Framework.UI.BunifuCheckbox UpdateCheck,Movie movie, List<string> streams,string table_db, Bunifu.Framework.UI.BunifuCustomDataGrid dataGrid)
        {
            
            if (!DataExist_db(movie.id.ToString(), table_db)){
                //---- insertion
                Console.WriteLine("insert");
                insertMovie(movie, table_db,streams);
                if (dataGrid.InvokeRequired) dataGrid.Invoke(new Action(() => dataGrid.Rows.Add(movie.title, "INSERTED")));

            }
            else
            {
                if (UpdateCheck.Checked)
                {
                    //---- update
                    Console.WriteLine("update");
                    UpdateMovie(streams, movie.id, table_db);
                    if (dataGrid.InvokeRequired) dataGrid.Invoke(new Action(() => dataGrid.Rows.Add(movie.title, "UPDATED")));
                }


            }




        }
        public static bool DataExist_db(string tmdb,string table) {
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"SELECT count(*) from {table} where tmdb = {tmdb}";
            if (cmd.ExecuteScalar().ToString().Equals("0"))
            {
                con.Close();
                return false;
            }

            con.Close();
            return true;
        }
        public static bool EpisodeExist_db(string tmdb,int Snumber,int e, string table)
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"SELECT count(*) from {table} where tmdb = {tmdb} and Season = {Snumber} and episode = {e}";
            if (cmd.ExecuteScalar().ToString().Equals("0"))
            {
                con.Close();
                return false;
            }

            con.Close();
            return true;
        }
        public static bool SeasonExist_db(int tmdb, int season ,string table)
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"SELECT count(*) from {table} where tmdb = {tmdb} and season = {season}";
            if (cmd.ExecuteScalar().ToString().Equals("0"))
            {
                con.Close();
                return false;
            }

            con.Close();
            return true;
        }
        static void insertMovie(Movie movie, string table, List<string> streams)
        {
            if (streams == null)
                streams.Add("");
            Console.WriteLine("DUMP!!");
            try {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                Console.WriteLine(movie.id);
                cmd.CommandText = $"INSERT INTO {table} (tmdb,imdb,title,original_title,original_language,overview,genres,release_date,status,vote_average,poster_path,backdrop_path,production_countries,production_companies,runtime,spoken_languages,tagline,vote_count,cast,crew,videos,images,streams,insert_time)"
                                    + "VALUES("
                                    + $@"{movie.id},"
                                    + $@"'{movie.imdb_id}',"
                                    + $@"'{movie.title.Replace(@"'", @"\'")}',"
                                    + $@"'{movie.original_title.Replace(@"'", @"\'")}',"
                                    + $@"'{movie.original_language}',"
                                    + $@"'{movie.overview.Replace(@"'", @"\'")}',"
                                    + $@"'{string.Join(",", movie.genres.Select(genre => genre.name))}',"
                                    + $@"'{movie.release_date}',"
                                    + $@"'{movie.status.Replace(@"'", @"\'")}',"
                                    + $@"'{movie.vote_average}',"
                                    + $@"'{movie.poster_path}',"
                                    + $@"'{movie.backdrop_path}',"
                                    + $@"'{string.Join(",", movie.production_countries.Select(country => country.name))}',"
                                    + $@"'{string.Join(",", movie.production_companies.Select(company => company.name)).Replace(@"'", @"\'")}',"
                                    + $@"'{movie.runtime}',"
                                    + $@"'{string.Join(",", movie.spoken_languages.Select(lang => lang.name))}',"
                                    + $@"'{movie.tagline.Replace(@"'", @"\'")}',"
                                    + $@"'{movie.vote_count}',"
                                    + $@"'{string.Join("\n", movie.credits.cast.Select(c => "[" + c.profile_path + ";" + c.character + "," + c.name + "]")).Replace(@"'", @"\'")}',"
                                    + $@"'{string.Join("\n", movie.credits.crew.Where(c=> c.job == "Director").Select(c => "[" + c.profile_path + ";" + c.name + "]")).Replace(@"'", @"\'")}',"
                                    + $@"'{movie.videos.results.Where(vd => vd.site.Equals("YouTube")).Select(vd => "[" + vd.key + "]").FirstOrDefault()}',"
                                    + $@"'{string.Join("\n", movie.images.backdrops)}',"
                                    + $@"'{string.Join("\n", filterLiveStreams(string.Join("\n", streams)))}',"
                                    + $@"'{DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd hh:mm:ss")}'"
                                + ")";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e){ Console.WriteLine(e.Message); }
            finally { con.Close(); }
            

        }
        public static void insertTV(TV tv, string table)
        {
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                Console.WriteLine(tv.id);
                cmd.CommandText = $"INSERT INTO {table} (tmdb,title,original_title,original_language,overview,genres,number_of_seasons,number_of_episodes,vote_average,vote_count,release_date,status,poster_path,backdrop_path,production_companies,languages,runtime,cast,crew,videos,images)"
                                    + "VALUES("
                                    + $@"{tv.id},"
                                    + $@"'{tv.name.Replace(@"'", @"\'")}',"
                                    + $@"'{tv.original_name.Replace(@"'", @"\'")}',"
                                    + $@"'{tv.original_language}',"
                                    + $@"'{tv.overview.Replace(@"'", @"\'")}',"
                                    + $@"'{string.Join(",", tv.genres.Select(genre => genre.name))}',"
                                    + $@"'{tv.number_of_seasons}',"
                                    + $@"'{tv.number_of_episodes}',"
                                    + $@"'{tv.vote_average}',"
                                    + $@"'{tv.vote_count}',"
                                    + $@"'{tv.first_air_date}',"
                                    + $@"'{tv.status.Replace(@"'", @"\'")}',"
                                    + $@"'{tv.poster_path}',"
                                    + $@"'{tv.backdrop_path}',"
                                    + $@"'{string.Join(",", tv.production_companies.Select(company => company.name)).Replace(@"'", @"\'")}',"
                                    + $@"'{string.Join(",", tv.languages)}',"
                                    + $@"'{tv.episode_run_time.First()}',"
                                    + $@"'{string.Join("\n", tv.credits.cast.Select(c => "[" + c.profile_path + ";" + c.character + "," + c.name + "]")).Replace(@"'", @"\'")}',"
                                    + $@"'{string.Join("\n", tv.credits.crew.Select(c => "[" + c.profile_path + ";" + c.job + "," + c.name + "]")).Replace(@"'", @"\'")}',"
                                    + $@"'{string.Join("\n", tv.videos.results.Where(vd => vd.site.Equals("YouTube")).Select(vd => "[" + vd.key + "]")).Replace(@"'", @"\'")}',"
                                    + $@"'{string.Join("\n", tv.images.backdrops)}'"
                                + ")";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            finally { con.Close(); }


        }
        static void UpdateMovie(List<string> streams,int tmdb,string table_db)
        {

            
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.Text;

                ///--------
                /// <summary>
                /// filter live streams only
                /// </summary>

                cmd.CommandText = $"select streams from {table_db}  where tmdb = {tmdb}";
                List<string> strms = filterLiveStreams(cmd.ExecuteScalar().ToString() + "\n"+
                                            string.Join("\n",streams));
                ///-------
                cmd.CommandText = $"UPDATE {table_db} SET streams = {string.Join("\n", strms)}   where tmdb = {tmdb}";
                cmd.ExecuteNonQuery();
                cmd.CommandText = $"UPDATE {table_db} SET posted = 0 where tmdb = {tmdb}";
                cmd.ExecuteNonQuery();
            }
                catch (Exception e) { Console.WriteLine(e.Message); }
                finally { con.Close(); }
            
        }

        public static void dumpSeasonsEpisodes_db(TV tv ,Dictionary<int, Dictionary<int, List<string>>> Data, string season_db,string episode_db,string lang, Bunifu.Framework.UI.BunifuCheckbox UpdateCheck)
        {
            foreach (KeyValuePair<int, Dictionary<int, List<string>>> Season in Data)
            {
                //<summary>
                // insert season if missing
                //
                //<summary>
                if (!SeasonExist_db(tv.id, Season.Key, season_db))
                {
                    InsertSeason(tv.id, tv.name,getSeasonDetails(tv.seasons,Season.Key), season_db);
                }
                //<summary>
                // Insert episodes
                // if update checked , update all streams
                // else insert new only
                //<summary>
                InsertEpisodes(tv.id, tv.name, Season.Key,Season.Value,episode_db,lang, UpdateCheck);

               

            }

        }
        static Season getSeasonDetails(List<Season> seasons,int Snumber)
        {
            foreach (Season s in seasons)
                if (s.season_number == Snumber)
                    return s;
            return null;

        }
        static void InsertSeason(int tmdb,string tv_name,Season details,string season_db)
        {
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"INSERT INTO {season_db} (tmdb,season,tv_name,air_date,poster_path,name,overview)"
                                    + "VALUES("
                                    + $@"{tmdb},"
                                    + $@"{details.season_number},"
                                    + $@"'{tv_name.Replace(@"'", @"\'")}',"
                                    + $@"'{details.air_date}',"
                                    + $@"'{details.poster_path}',"
                                    + $@"'{details.name.Replace(@"'", @"\'")}',"
                                    + $@"'{details.overview.Replace(@"'", @"\'")}'"
                                + ")";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            finally { con.Close(); }
        }
        static void InsertEpisodes(int tmdb, string tv_name, int Snumber, Dictionary<int, List<string>> EPs, string episode_db,string lang, Bunifu.Framework.UI.BunifuCheckbox UpdateCheck)
        {
            foreach (KeyValuePair<int,List<string>> EP in EPs)
            {
                if (EP.Value.Count == 0)
                    goto None;
                if (EpisodeExist_db(tmdb.ToString(), Snumber, EP.Key,episode_db))
                {
                    
                    if (UpdateCheck.Checked)
                    {
                        //<summary>
                        // Update Episode
                        //<summary>
                        try
                        {
                            
                            con.Open();
                            MySqlCommand cmd = new MySqlCommand();
                            cmd.Connection = con;
                            cmd.CommandType = System.Data.CommandType.Text;
                            Regex pattern = new Regex("[\\\\\"]");
                            
                            cmd.CommandText = $"UPDATE  {episode_db} SET streams =  '{string.Join("\n", filterLiveStreams(string.Join("\n", EP.Value.Select(s => pattern.Replace(s, "")))))}' "
                                                + $"WHERE tmdb ={tmdb} and season = {Snumber} and episode = {EP.Key}";

                            cmd.ExecuteNonQuery();
                            cmd.CommandText = $"UPDATE  {episode_db} SET posted =  0 "
                                                + $"WHERE tmdb ={tmdb} and season = {Snumber} and episode = {EP.Key}";

                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception z) { Console.WriteLine(z.Message); }
                        finally { con.Close(); }
                    }
                    
                    
                
                }
                else
                {
                    //<summary>
                    // Insert New Episode
                    //<summary>
                    try
                    {
                        Episode e = getTMDB_Episode(tmdb.ToString(), lang, Snumber, EP.Key);
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = con;
                        cmd.CommandType = System.Data.CommandType.Text;
                        Regex pattern = new Regex("[\\\\\"]");

                        cmd.CommandText = $"INSERT INTO {episode_db} (tmdb,season,episode,overview,name,still_path,vote_average,vote_count,tv_title,air_date,streams,insert_time)"
                                            + "VALUES("
                                            + $@"{tmdb},"
                                            + $@"{Snumber},"
                                            + $@"{EP.Key},"
                                            + $@"'{e.overview.Replace(@"'", @"\'")}',"
                                            + $@"'{e.name.Replace(@"'", @"\'")}',"
                                            + $@"'{e.still_path}',"
                                            + $@"'{e.vote_average}',"
                                            + $@"'{e.vote_count}',"
                                            + $@"'{tv_name.Replace(@"'", @"\'")}',"
                                            + $@"'{e.air_date}',"
                                            + $@"'{string.Join("\n", filterLiveStreams(string.Join("\n", EP.Value.Select(s => pattern.Replace(s, "")))))}',"
                                            + $@"'{DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd hh:mm:ss")}'"

                                        + ")";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception z) { Console.WriteLine(z.Message); }
                    finally { con.Close(); }
                }
                
                
            }
            None:
            return;
            
        }
        public static List<Dictionary<string, string>> getMovies_db(string table_db)
        {
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"select * from {table_db} where posted = 0 order by insert_time ASC";
                MySqlDataReader rd = cmd.ExecuteReader();
                List<Dictionary<string, string>> Rows = new List<Dictionary<string, string>>();
                while (rd.Read())
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();
                    for (int i = 0; i < rd.FieldCount; i++)
                    {
                        row.Add(rd.GetName(i), rd.GetValue(i).ToString());
                    }
                    Rows.Add(row);
                    
                }
                con.Close();
                return Rows; 
            }
            catch (Exception e) { Console.WriteLine(e.Message); con.Close(); return null;  }
            
           
        }
        public static List<Dictionary<string, string>> getEpisodes_db(string table_db)
        {
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"select * from {table_db} where posted = 0 order by insert_time ASC";
                MySqlDataReader rd = cmd.ExecuteReader();
                List<Dictionary<string, string>> Rows = new List<Dictionary<string, string>>();
                while (rd.Read())
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();
                    for (int i = 0; i < rd.FieldCount; i++)
                    {
                        row.Add(rd.GetName(i), rd.GetValue(i).ToString());
                    }
                    Rows.Add(row);

                }
                con.Close();
                return Rows;
            }
            catch (Exception e) { Console.WriteLine(e.Message); con.Close(); return null; }


        }
        public static Dictionary<string, string> getSeason_db(string tmdb, string S,string table_db)
        {
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"select * from {table_db} where tmdb = {tmdb} and season = {S}";
                MySqlDataReader rd = cmd.ExecuteReader();
                rd.Read();
                Dictionary<string, string> row = new Dictionary<string, string>();
                for (int i = 0; i < rd.FieldCount; i++)
                {
                    row.Add(rd.GetName(i), rd.GetValue(i).ToString());
                }

                
                con.Close();
                return row;
            }
            catch (Exception e) { Console.WriteLine(e.Message); con.Close(); return null; }


        }
        public static Dictionary<string, string> getTvshow_db(string tmdb, string table_db)
        {
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"select * from {table_db} where tmdb = {tmdb}";
                MySqlDataReader rd = cmd.ExecuteReader();
                rd.Read();
                Dictionary<string, string> row = new Dictionary<string, string>();
                for (int i = 0; i < rd.FieldCount; i++)
                {
                    row.Add(rd.GetName(i), rd.GetValue(i).ToString());
                }


                con.Close();
                return row;
            }
            catch (Exception e) { Console.WriteLine(e.Message); con.Close(); return null; }


        }
        public static void updatePostedStatusMovies(string tmdb,string table_db)
        {
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"UPDATE {table_db} SET posted = 0 where tmdb = {tmdb}";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            finally { con.Close(); }
        }
    }

}
