﻿using System;
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

            submission_date = version[0];
            id3v2_title = version[1];
            id3v2_author = version[2];
            id3v2_lyrics = version[3];
            id3v2_album = version[4];
            id3v2_genre = version[5];

            id3v2_year = Convert.ToInt32(version[6]);

        }

    }

    public class Property
    {
        public string user_name { set; get; }
        public int song_id { set; get; }
        public string song_name { set; get; }
    }

    class MetadataAndSong
    {
        public string user_name { get; set; }
        public int song_id { get; set; }
        public string song_name { get; set; }
        public int metadata_id { get; set; }
        public string song_dir { get; set; }
        public string date { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string lyrics { get; set; }
        public string album { get; set; }
        public string genre { get; set; }
        public int year { get; set; }

    }

}
