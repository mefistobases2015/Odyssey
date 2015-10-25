using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace RestComunication
{
    class RestTools
    {
        private string server_url = "http://odysseyop.azurewebsites.net/";
        private string format = "application/json";

        private string credentials_path = "api/Credenciales";
        private string songs_path = "api/Canciones";
        private string versions_path = "api/Versiones";
        private string properties_path = "api/Propiedades";        
       /**
        *Constructor vacío
        */
        public RestTools()
        {
            //constructor vacion con valores default
        }

        public RestTools(string p_server_url)
        {
            this.server_url = p_server_url;
        }

        public void setFormat(string p_format)
        {
            format = p_format;
        }

        public async Task<bool> isUser(string usr_name)
        {
            bool flag = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(credentials_path + "/" + usr_name);

                if (response.IsSuccessStatusCode)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;

        }

        public async Task<bool> isPassword(string usr_name, string password)
        {
            bool flag = false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(credentials_path + "/" + usr_name);

                if (response.IsSuccessStatusCode)
                {
                    Credential cred = await response.Content.ReadAsAsync<Credential>();

                    string repassword = cred.pass;

                    if (repassword.CompareTo(password) == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }

                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

        public async Task<bool> createUser(string p_usr_name, string p_pass)
        {
            bool flag = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                Credential cred = new Credential() { user_name = p_usr_name, pass = p_pass };

                HttpResponseMessage response = await client.PostAsJsonAsync(credentials_path, cred);

                if (response.IsSuccessStatusCode)
                {
                    //Console.WriteLine("Sirvio el post :-D");
                    flag = true;
                }
                else
                {
                    //Console.WriteLine("No sirvio el post D-: {0}", response.StatusCode);
                    flag = false;
                }
            }

            return flag;

        }

        public async Task<Song> createSong(string p_song_directory)
        {

            Song result= new Song();

            Song song = new Song() { song_id = -1, metadata_id = -1, song_directory = p_song_directory };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync(songs_path, song);

                if (response.IsSuccessStatusCode)
                {
                    Uri uri = response.Headers.Location;

                    HttpResponseMessage response2 = await client.GetAsync(uri);

                    result = await response2.Content.ReadAsAsync<Song>();

                }
                else
                {
                    result = null;
                }
            }

            return result;
        }

        public async Task<Version> createVersion(List<String> new_version, Song song)
        {
            Version result = new Version();

            Version ver = new Version(new_version, song.song_id);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync(versions_path, ver);

                if (response.IsSuccessStatusCode)
                {
                    Uri uri = response.Headers.Location;

                    HttpResponseMessage response2 = await client.GetAsync(uri);

                    result = await response2.Content.ReadAsAsync<Version>();
                }
                else
                {
                    result = null;
                }
            }

            return result;

        }
        
        public async Task<Boolean> addSong2User(string p_user_name, string p_song_name, Song p_song)
        {
            bool flag = false;

            Property prop = new Property() { user_name =  p_user_name, song_id = p_song.song_id, song_name = p_song_name}; 

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync<Property>(properties_path, prop);

                if (response.IsSuccessStatusCode)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }

            return flag;
        }

        public async Task<Boolean> add2SongList(string p_user_name, string p_song_name, 
            string p_song_directory, List<String> new_version)
        {
            bool flag = false;
            //se crea la cancion
            Song song = await createSong(p_song_directory);
            //se crea la version
            Version ver = await createVersion(new_version, song);
            //se actualiza el song metadata id
            song.metadata_id = ver.version_id;
            //se hace put de la actualizacion
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PutAsJsonAsync<Song>(songs_path + song.song_id, song);

                if (response.IsSuccessStatusCode)
                {
                    flag = await addSong2User(p_user_name, p_song_name, song);
                }
                else
                {
                    flag = false;
                }
            }

            return flag;
        }


    }
}
