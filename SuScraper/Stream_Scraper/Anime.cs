using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stream_Scraper
{
    class Anime
    {
        public string Unique { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Aired { get; set; }
        public List<string> Genres { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }

        public Dictionary<string, List<string>> Streams { get; set; }
        public Anime()
        {
            Streams = new Dictionary<string, List<string>>();
            Genres = new List<string>();

        }
        public string ToString()
        {
            return string.Format("Unique : {0}\nTitle : {1}\nType : {2}\nReleased : {3}\n Genres : {4}"
                ,Unique, Title,  Type, Aired, string.Join(",",Genres)
                );
        }
    }
}
