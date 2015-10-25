using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestComunication
{
    class JsonObjects
    {
    }

    public class Credentials
    {
        public string user_name { set; get; }
        public string pass { set; get; }
    }

    public class Songs
    {
        public string song_id { set; get; }
        public string metadata_id { set; get; }
        public string song_directory { set; get; }

    }

    public class Versions
    {
        public string version_id { set; get; }
        public string song_id { set; get; }
        public string submission_date { set; get; }
        public string id3v2_title { set; get; }
        public string id3v2_author { set; get; }
        public string id3v2_lyrics { set; get; }
        public string id3v2_album { set; get; }
        public string id3v2_genre { set; get; }
        public string id3v2_year { set; get; }
    }

    public class Properties
    {
        public string user_name { set; get; }
        public string song_id { set; get; }
        public string song_name { set; get; }
    }
}
