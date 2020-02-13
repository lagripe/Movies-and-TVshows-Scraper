using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordPressSharp;
using WordPressSharp.Models;

namespace Stream_Scraper
{
	class AutoPoster
	{
		public static WordPressClient WordpressCL(string url, string usr, string pwd)
		{
			WordPressSiteConfig config = new WordPressSiteConfig
			{
				Username = usr,
				Password = pwd,
				BaseUrl = url
			};
			WordPressClient client;
			try
			{
				client = new WordPressClient(config);
			}
			catch { return null; }
			return client;
		}
		public static List<Post> getPosts(WordPressClient client,string type)
		{
			List<Post> Posts = new List<Post>();
			int offset = 0;
			while (true)
			{
				var filter = new PostFilter { PostType = type, Number = 100, Offset = offset };
				List<Post> temp = client.GetPosts(filter).ToList();
				if (temp.Count == 0)
					return Posts;
				Posts.AddRange(temp);
				offset += 100;
				
			}

		}
		public static Post findPost(string tmdb, List<Post> Posts)
		{
            
			foreach (Post post in Posts)
			{
				foreach (CustomField item in post.CustomFields.ToList())
				{
  
                    if (item.Key == "idtmdb" && item.Value.ToString() == tmdb)
						return post;
                        
                    
				}

			}
			return null;
		}
        public static Post findPostTV(string tmdb, List<Post> Posts)
        {

            foreach (Post post in Posts)
            {
                foreach (CustomField item in post.CustomFields.ToList())
                {

                    if (item.Key == "ids" && item.Value.ToString() == tmdb)
                        return post;


                }

            }
            return null;
        }
        public static Post findPostEpisode(string tmdb,string season,string episode, List<Post> Posts)
        {
            
            foreach (Post post in Posts)
            {
                int cpt = 0;
                foreach (CustomField item in post.CustomFields.ToList())
                {
                    
                    if (item.Key == "ids" && item.Value.ToString() == tmdb)
                        cpt++;
                    if (item.Key == "episodio" && item.Value.ToString() == episode)
                        cpt++;
                    if (item.Key == "temporada" && item.Value.ToString() == season)
                        cpt++;
                    
                }
                if (cpt == 3)
                    return post;

            }
            return null;
        }
        public static Post findPostSeason(string tmdb, string season, List<Post> Posts)
        {

            foreach (Post post in Posts)
            {
                int cpt = 0;

                foreach (CustomField item in post.CustomFields.ToList())
                {
                    if (item.Key == "ids" && item.Value.ToString() == tmdb)
                        cpt++;
                    
                    if (item.Key == "temporada" && item.Value.ToString() == season)
                        cpt++;
                    
                }
                if (cpt == 2)
                    return post;

            }
            return null;
        }
        public static Dictionary<string,Term> getTerms(WordPressClient client,string Taxonomy) {
            return client.GetTerms(Taxonomy, new TermFilter()).ToList().ToDictionary(term => term.Name, term => term);
        }
        public static List<Dictionary<string, string>> constructRCF(string urls){

            List<Dictionary<string, string>> repeatable_fields = new List<Dictionary<string, string>>();

            int counter = 1;
            foreach (string url in urls.Split('\n'))
            {

                repeatable_fields.Add(new Dictionary<string, string> {

                                {"name", $"Server {counter}" },
                                {"select", "iframe" },
                                {"idioma", "" },
                                {"url",url.Trim() }
                                            }); counter++;

            }
            return repeatable_fields;

        }

		public static void BotMovies(WordPressClient client, string type,string table_db) {
			//Website Posts
			List<Post> Posts = getPosts(client, type);
            Dictionary<string, Term> ExistedGenres = getTerms(client, "genres");

           
            List<Dictionary<string, string>> movies = Information.getMovies_db(table_db);
			foreach (Dictionary<string, string> movie in movies)
			{
                try { 
				    Console.WriteLine(movie["tmdb"]);
				    Post post = findPost(movie["tmdb"], Posts);
                    Serializer sr = new Serializer();

                    if (post == null)
                    {
                        Console.WriteLine("INSERT");
                        //INSERT

                        Post newPost = new Post {
                            Title = movie["title"],
                            Content = movie["overview"],
                            PostType = type,
                            Status = "publish",
                            CommentStatus = "open"

                        };



                       

                        //Console.WriteLine(repeatable_fields[2]["name"]);
                        string RCF = sr.Serialize(constructRCF(movie["streams"])).Replace("\"", "\\\"");
                        //Console.WriteLine(RCF);
                        List<CustomField> cf = new List<CustomField>();
                        cf.Add(new CustomField { Key = "idtmdb", Value = movie["tmdb"] });
                        cf.Add(new CustomField { Key = "repeatable_fields", Value = "" });

                        cf.Add(new CustomField { Key = "dt_poster", Value = movie["poster_path"] });
                        cf.Add(new CustomField { Key = "dt_backdrop", Value = movie["backdrop_path"] });
                        cf.Add(new CustomField { Key = "imagenes", Value = movie["backdrop_path"] });
                        DateTime dt = DateTime.Parse(movie["release_date"]);
                        cf.Add(new CustomField { Key = "release_date", Value = string.Format($"{dt:yyyy-MM-dd}") });
                        cf.Add(new CustomField { Key = "original_title", Value = "" });
                        cf.Add(new CustomField { Key = "vote_count", Value = movie["vote_count"] });
                        cf.Add(new CustomField { Key = "vote_average", Value = movie["vote_average"] == "0" ? "" : movie["vote_average"] });
                        cf.Add(new CustomField { Key = "ids", Value = movie["tmdb"] });
                        cf.Add(new CustomField { Key = "original_language", Value = movie["original_language"] });
                        cf.Add(new CustomField { Key = "tagline", Value = movie["tagline"] });
                        cf.Add(new CustomField { Key = "runtime", Value = movie["runtime"] });
                        cf.Add(new CustomField { Key = "youtube_id", Value = movie["videos"] });
                        cf.Add(new CustomField { Key = "imdbRating", Value = movie["vote_average"] == "0" ? "" : movie["vote_average"] });
                        cf.Add(new CustomField { Key = "imdbVotes", Value = movie["vote_count"] });
                        cf.Add(new CustomField { Key = "Country", Value = movie["production_countries"] });
                        cf.Add(new CustomField { Key = "dt_cast", Value = string.Join("", movie["cast"].Split('\n').Select(c => c.Trim())) });
                        cf.Add(new CustomField { Key = "dt_dir", Value = string.Join("", movie["crew"].Split('\n').Select(c => c.Trim())) });


                        //YOAST SEO
                        cf.Add(new CustomField { Key = "_yoast_wpseo_title", Value = "Regarder " + movie["title"].Trim() + " en vf streaming" });
                        cf.Add(new CustomField { Key = "_yoast_wpseo_metadesc", Value = movie["overview"] });

                        cf.Add(new CustomField { Key = "_yoast_wpseo_focuskw", Value = movie["title"].Trim() + " (" + dt.Year + ")" });

                        newPost.CustomFields = cf.ToArray();

                        //Add Genres
                        List<Term> Terms = new List<Term>();
                        foreach (string genre in movie["genres"].Split(','))
                        {
                            if (genre.Length < 2)
                                continue;
                            if (ExistedGenres.ContainsKey(genre))
                                Terms.Add(ExistedGenres[genre]);
                            else
                            {
                                Term newTerm = new Term { Taxonomy = "genres", Name = genre };
                                client.NewTerm(newTerm);
                                ExistedGenres = getTerms(client, "genres");
                                Terms.Add(ExistedGenres[genre]);

                            }

                        }
                        //Add Year
                        if (getTerms(client, "dtyear").ContainsKey(dt.Year.ToString()))
                            Terms.Add(getTerms(client, "dtyear")[dt.Year.ToString()]);
                        else
                        {
                            Term newTerm = new Term { Taxonomy = "dtyear", Name = dt.Year.ToString() };
                            client.NewTerm(newTerm);
                            Terms.Add(
                                getTerms(client, "dtyear")[dt.Year.ToString()]);
                        }

                        newPost.Terms = Terms.ToArray();



                        Information.uploadRCF(RCF, client.NewPost(newPost));
                        


                    }
                    else
                    {
                        //UPDATE
                        Console.WriteLine("UPDATE");
                        // Update rating
                        Information.UpdateRate(movie["vote_average"] == "0" ? "" : movie["vote_average"], movie["vote_count"], movie["tmdb"]);
                        //Update Streams
                        Information.uploadRCF(sr.Serialize(constructRCF(movie["streams"])).Replace("\"", "\\\"")
                                                , movie["tmdb"]);

                    }
                    Information.updatePostedStatusMovies(movie["tmdb"],table_db);

                }
                catch { }

            }

        }

        public static void BotEpisodes(WordPressClient client, string type, string episode_db,string seasons_db, string tvshows_db)
        {
            //Website Posts episodes
            List<Post> Posts = getPosts(client, type);
            Dictionary<string, Term> ExistedGenres = getTerms(client, "genres");

            
            List<Dictionary<string, string>> episodes = Information.getEpisodes_db(episode_db);
            Console.WriteLine(episodes.Count);
            foreach (Dictionary<string, string> episode in episodes)
            {
                try
                {
                    Console.WriteLine(episode["tmdb"]);
                    Post post = findPostEpisode(episode["tmdb"], episode["season"], episode["episode"], Posts);
                    Serializer sr = new Serializer();

                    if (post == null)
                    {
                        Console.WriteLine("INSERT");
                        //INSERT

                        Post newPost = new Post
                        {
                            Title = episode["tv_title"] + " S" + episode["season"] + "xE" + episode["episode"],
                            Content = episode["overview"],
                            PostType = type,
                            Status = "publish",
                            CommentStatus = "open"

                        };


                        #region InsertEpisode

                        ///<summary>
                        ///-----Insert Episode
                        Console.WriteLine(episode["tv_title"] + " "+ episode["season"] + "x"+ episode["episode"]);
                        string RCF = sr.Serialize(constructRCF(episode["streams"])).Replace("\"", "\\\"");
                        List<CustomField> cf = new List<CustomField>();
                        cf.Add(new CustomField { Key = "temporada", Value = episode["season"] });
                        cf.Add(new CustomField { Key = "episodio", Value = episode["episode"] });
                        cf.Add(new CustomField { Key = "ids", Value = episode["tmdb"] });
                        cf.Add(new CustomField { Key = "episode_name", Value = episode["name"] });
                        cf.Add(new CustomField { Key = "serie", Value = episode["tv_title"] });
                        cf.Add(new CustomField { Key = "dt_backdrop", Value = episode["still_path"] });
                        cf.Add(new CustomField { Key = "imagenes", Value = episode["still_path"] });
                        DateTime dt = DateTime.Parse(episode["air_date"]);
                        cf.Add(new CustomField { Key = "air_date", Value = string.Format($"{dt:yyyy-MM-dd}") });
                        cf.Add(new CustomField { Key = "repeatable_fields", Value = "" });

                            //YOAST SEO
                        cf.Add(new CustomField { Key = "_yoast_wpseo_title", Value = "Regarder " + episode["tv_title"] + $": {episode["season"]}x{episode["episode"]}" + " en vf streaming" });
                        cf.Add(new CustomField { Key = "_yoast_wpseo_metadesc", Value = episode["overview"] });

                        cf.Add(new CustomField { Key = "_yoast_wpseo_focuskw", Value = "Regarder " + episode["tv_title"] + $": {episode["season"]}x{episode["episode"]}" });
                        newPost.CustomFields = cf.ToArray();
                        Information.uploadRCF(RCF, client.NewPost(newPost));
                        Console.WriteLine("Episode Inserted");
                        ///--- END Insert Episode </summary>
                        #endregion
                        
                        #region Season
                        if (findPostSeason(episode["tmdb"], episode["season"], getPosts(client, "seasons")) != null)
                            goto None;
                        Dictionary<string, string> SEASON = Information.getSeason_db(episode["tmdb"], episode["season"], seasons_db);
                        newPost = new Post
                        {
                            Title = episode["tv_title"] + $": Season {episode["season"]}",
                            Content = SEASON["overview"],
                            PostType = "seasons",
                            Status = "publish",
                            CommentStatus = "open"

                        };
                        Console.WriteLine("OK");
                        ///<summary>
                        ///-----Insert Season
                        cf = new List<CustomField>();
                        cf.Add(new CustomField { Key = "temporada", Value = episode["season"] });
                        cf.Add(new CustomField { Key = "ids", Value = episode["tmdb"] });
                        cf.Add(new CustomField { Key = "serie", Value = episode["tv_title"] });
                        cf.Add(new CustomField { Key = "dt_poster", Value = SEASON["poster_path"] });

                        dt = DateTime.Parse(SEASON["air_date"]);
                        cf.Add(new CustomField { Key = "air_date", Value = string.Format($"{dt:yyyy-MM-dd}") });
                        cf.Add(new CustomField { Key = "clgnrt", Value = "1" });

                        //YOAST SEO
                        cf.Add(new CustomField { Key = "_yoast_wpseo_title", Value = "Regarder " + episode["tv_title"] + $": Season {episode["season"]}" + " en vf streaming" });
                        cf.Add(new CustomField { Key = "_yoast_wpseo_metadesc", Value = episode["overview"] });

                        cf.Add(new CustomField { Key = "_yoast_wpseo_focuskw", Value = "Regarder " + episode["tv_title"] + $": Season {episode["season"]}" });

                        newPost.CustomFields = cf.ToArray();
                        client.NewPost(newPost);
                        ///--- END Season
                        #endregion
                        
                        #region TVShow
                        if (findPostTV(episode["tmdb"], getPosts(client, "tvshows")) != null)
                            goto None;
                        Console.WriteLine("TVSHOW not found ------------------------------");



                        Dictionary<string, string> TVSHOW = Information.getTvshow_db(episode["tmdb"], tvshows_db);
                        newPost = new Post
                        {
                            Title = episode["tv_title"],
                            Content = TVSHOW["overview"],
                            PostType = "tvshows",
                            Status = "publish",
                            CommentStatus = "open"

                        };

                        cf = new List<CustomField>();
                        cf.Add(new CustomField { Key = "dt_poster", Value = TVSHOW["poster_path"] });
                        cf.Add(new CustomField { Key = "dt_backdrop", Value = TVSHOW["backdrop_path"] });
                        cf.Add(new CustomField { Key = "imagenes", Value = TVSHOW["backdrop_path"] });
                        cf.Add(new CustomField { Key = "number_of_seasons", Value = TVSHOW["number_of_seasons"] });
                        cf.Add(new CustomField { Key = "number_of_episodes", Value = TVSHOW["number_of_episodes"] });
                        dt = DateTime.Parse(TVSHOW["release_date"]);
                        cf.Add(new CustomField { Key = "first_air_date", Value = string.Format($"{dt:yyyy-MM-dd}") });
                        cf.Add(new CustomField { Key = "original_title", Value = "" });
                        cf.Add(new CustomField { Key = "ids", Value = TVSHOW["tmdb"] });
                        cf.Add(new CustomField { Key = "original_language", Value = TVSHOW["original_language"] });
                        cf.Add(new CustomField { Key = "episode_run_time", Value = TVSHOW["runtime"] });
                        cf.Add(new CustomField { Key = "youtube_id", Value = TVSHOW["videos"] });
                        cf.Add(new CustomField { Key = "imdbRating", Value = TVSHOW["vote_average"] == "0" ? "" : TVSHOW["vote_average"] });
                        cf.Add(new CustomField { Key = "imdbVotes", Value = TVSHOW["vote_count"] });
                        cf.Add(new CustomField { Key = "dt_cast", Value = string.Join("", TVSHOW["cast"].Split('\n').Select(c => c.Trim())) });
                        cf.Add(new CustomField { Key = "dt_creator", Value = string.Join("", TVSHOW["crew"].Split('\n').Select(c => c.Trim())) });
                        cf.Add(new CustomField { Key = "clgnrt", Value = "1" });


                        //YOAST SEO
                        cf.Add(new CustomField { Key = "_yoast_wpseo_title", Value = "Regarder " + TVSHOW["title"].Trim() + " en vf streaming" });
                        cf.Add(new CustomField { Key = "_yoast_wpseo_metadesc", Value = TVSHOW["overview"] });

                        cf.Add(new CustomField { Key = "_yoast_wpseo_focuskw", Value = "Regarder " + TVSHOW["title"].Trim() });

                        newPost.CustomFields = cf.ToArray();

                        
                        //Add Genres
                        List<Term> Terms = new List<Term>();
                        foreach (string genre in TVSHOW["genres"].Split(','))
                        {
                            if (genre.Length < 2)
                                continue;
                            if (ExistedGenres.ContainsKey(genre))
                                Terms.Add(ExistedGenres[genre]);
                            else
                            {
                                Term newTerm = new Term { Taxonomy = "genres", Name = genre };
                                client.NewTerm(newTerm);
                                ExistedGenres = getTerms(client, "genres");
                                Terms.Add(ExistedGenres[genre]);

                            }

                        }
                        
                        //Add Year
                        if (getTerms(client, "dtyear").ContainsKey(dt.Year.ToString()))
                            Terms.Add(getTerms(client, "dtyear")[dt.Year.ToString()]);
                        else
                        {
                            Term newTerm = new Term { Taxonomy = "dtyear", Name = dt.Year.ToString() };
                            client.NewTerm(newTerm);
                            Terms.Add(
                                getTerms(client, "dtyear")[dt.Year.ToString()]);
                        }
                        
                        newPost.Terms = Terms.ToArray();

                        client.NewPost(newPost);
                        Console.WriteLine("TVSHOW INSERTED #####");
                        #endregion

                        None:
                        UpdateTVGeneration( findPostTV(episode["tmdb"], getPosts(client, "tvshows")),client);
                        
                            Console.WriteLine("-------------------------------");
                    }
                    else
                    {
                        //UPDATE
                        Console.WriteLine("UPDATE");
                        // Update rating
                        //Information.UpdateRate(movie["vote_average"] == "0" ? "" : movie["vote_average"], movie["vote_count"], movie["tmdb"]);
                        //Update Streams
                        //Information.uploadRCF(sr.Serialize(constructRCF(movie["streams"])).Replace("\"", "\\\"")
                        //                       , movie["tmdb"]);

                    }

                }
                catch (Exception e){ Console.WriteLine(e.Message); }

            }

        }
        public static void UpdateTVGeneration(Post p,WordPressClient client)
        {
            CustomField f = null;
            foreach (CustomField item in p.CustomFields)
            {
                if (item.Key.Equals("clgnrt"))
                { f = item; break; }
                    

            }
            f.Value = 0;
            client.EditPost(p);
            f.Value = 1;
            client.EditPost(p);

        }
    }
}
