using System;
using System.Collections.Generic;

namespace RestComunication
{
    class JsonObjects
    {
    }

    public class Credential
    {
        public string user_name { set; get; }
        public string pass { set; get; }
    }

    public class Song
    {
        public int song_id { set; get; }
        public int metadata_id { set; get; }
        public string song_directory { set; get; }

    }

    public class Version
    {
        public int version_id { set; get; }
        public int song_id { set; get; }
        public string submission_date { set; get; }
        public string id3v2_title { set; get; }
        public string id3v2_author { set; get; }
        public string id3v2_lyrics { set; get; }
        public string id3v2_album { set; get; }
        public string id3v2_genre { set; get; }
        public int id3v2_year { set; get; }

        public Version()
        {
            //constructor vacio
        }

        public Version(List<string> version, int p_song_id)
        {
            song_id = p_song_id;

            id3v2_author = version[0];
            id3v2_title = version[1];
            id3v2_album = version[2];
            id3v2_year = Convert.ToInt32(version[3]);
            id3v2_genre = version[4];
            id3v2_lyrics = version[5];
            submission_date = version[6];
        }

    }

    public class Property
    {
        public string user_name { set; get; }
        public int song_id { set; get; }
        public string song_name { set; get; }
    }

    public class MetadataAndSong
    {
        public string user_name { get; set; }
        public int song_id { get; set; }
        public string song_name { get; set; }
        public int metadata_id { get; set; }
        public string song_directory { get; set; }
        public string submission_date { get; set; }
        public string id3v2_title { get; set; }
        public string id3v2_author { get; set; }
        public string id3v2_lyrics { get; set; }
        public string id3v2_album { get; set; }
        public string id3v2_genre { get; set; }
        public int year { get; set; }

    }

    public class Metadata
    {
        public string _ID3Title { get; set; }
        public string _ID3Artist { get; set; }
        public string _ID3Album { get; set; }
        public string _ID3Year { get; set; }
        public string _ID3Genre { get; set; }
        public string _ID3Comment { get; set; }
        public string _ID3Lyrics { get; set; }
        public string _SubmissionDate { get; set; }
        public string _SongDirectory { get; set; }
        public string _SongID { get; set; }
    }

    public class Solicitud
    {
        public string emisor { get; set; }
        public string receptor { get; set; }
    }

}
