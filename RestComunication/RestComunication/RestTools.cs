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
        private string songs_by_user_path = "api/CancionesUsuario";
        private const string mongo_songs_path = "api/CancionesMongo";
        private const string friend_request_path = "api/Solicitud";
        private const string mongo_users_path = "api/Usuarios";

        /// <summary>
        /// Constructor vacío, con valores por 
        /// defecto para conectarse al servidor
        /// </summary>
        public RestTools()
        {
            //constructor vacion con valores default
        }

        /// <summary>
        /// Constructor que cambia el url con el que se
        /// conecta al servidor.
        /// </summary>
        /// <param name="p_server_url">
        /// Dirección url del servidor.
        /// </param>
        public RestTools(string p_server_url)
        {
            this.server_url = p_server_url;
        }

        /// <summary>
        /// Cambia como el formato del contenido de 
        /// las consultas rest
        /// </summary>
        /// <param name="p_format">
        /// String con nuevo formato de envio
        /// </param>
        public void setFormat(string p_format)
        {
            format = p_format;
        }

        /// <summary>
        /// Verifica si un nombre de usuario existe en la base de datos 
        /// remota
        /// </summary>
        /// <param name="usr_name">
        /// string con el nombre de usuario que se va a consultar 
        /// si existe
        /// </param>
        /// <returns>
        /// bool que resulta true si el usuario existe y false cualquier
        /// otro caso
        /// </returns>
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
                    Console.WriteLine("\nCodigo de error {0}", response.StatusCode);
                    flag = false;
                }
            }
            return flag;

        }

        /// <summary>
        /// Verifica que el password que se le especifica a un 
        /// usuario este correcto.
        /// </summary>
        /// <param name="usr_name">
        /// string con el nombre de usuario al que 
        /// se le va a consultar el password.
        /// </param>
        /// string con el password a consultar.
        /// <param name="password">
        /// </param>
        /// <returns>
        /// bool que es true si el password coincide con el usuario,
        /// false en cualquier otro caso.
        /// </returns>
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

        /// <summary>
        /// Crea un nuevo usuario con el nombre de usuario y
        /// el password del nuevo usuario
        /// </summary>
        /// <param name="p_usr_name">
        /// Nuevo nombre de usuario
        /// </param>
        /// <param name="p_pass">
        /// Password del nuevo usuario
        /// </param>
        /// <returns>
        /// bool que es true en caso de que se haya creado el usuario con 
        /// exito, false en cualquier otro caso
        /// </returns>
        public async Task<bool> createUser(string p_usr_name, string p_pass)
        {
            bool flag = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                Credential cred = new Credential() { user_name = p_usr_name, pass = p_pass };

                HttpResponseMessage response = await client.PostAsJsonAsync<Credential>(credentials_path, cred);

                if (response.IsSuccessStatusCode)
                {
                    flag = true;

                    cred = await response.Content.ReadAsAsync<Credential>();

                    bool result2 = await addMongoUser(cred.user_name);

                    if (!result2)
                    {
                        Console.WriteLine("No se pudo agregar el usurio {0} a Mongo", cred.user_name);
                    }
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                    flag = false;
                }
            }

            return flag;

        }

        /// <summary>
        /// Crea un usario en los usarios de mongo
        /// </summary>
        /// <param name="user_name">
        /// Nombre de usuario
        /// </param>
        /// <returns>
        /// bool true si se logra crear, en cualquier otro caso 
        /// false
        /// </returns>
        public async Task<bool> addMongoUser(string user_name)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync<string>(mongo_users_path + "?value=" + user_name, "");

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Crea una nueva canción
        /// </summary>
        /// <param name="p_song_directory">
        /// Directorio donde se encuetra la canción.
        /// </param>
        /// <returns>
        /// Un objeto song con la información de la canción
        /// </returns>
        public async Task<Song> createSong(string p_song_directory)
        {

            Song result = new Song();

            Song song = new Song() { song_id = -1, metadata_id = -1, song_directory = p_song_directory };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync(songs_path, song);

                if (response.IsSuccessStatusCode)
                {

                    result = await response.Content.ReadAsAsync<Song>();

                    bool res = await addMongoSong(result.song_id);

                    if (!res)
                    {
                        Console.WriteLine("No se puedo agregar a Mongo la cancion id {0}", result.song_id);
                    }

                }
                else
                {
                    result = null;
                    Console.WriteLine("\nCodigo de error {0}", response.StatusCode);
                }
            }

            return result;
        }

        /// <summary>
        /// Agrega una cancion que se agrego a la base de datos de cancioens a 
        /// Mongo
        /// </summary>
        /// <param name="song_id">
        /// int que es el id de la cancion a agregar
        /// </param>
        /// <returns>
        /// bool que es true si se logró agregar, false en cualquier otro caso
        /// </returns>
        public async Task<bool> addMongoSong(int song_id)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync<string>(mongo_songs_path + "/" + song_id.ToString(), "");

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    result = false;

                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return result;
        }

        /// <summary>
        /// Crea una version de metadata
        /// </summary>
        /// <param name="new_version">
        /// Lista con los valores de metadata, viene con los 
        /// datos necesario para crear una version
        /// </param>
        /// <param name="p_song_directory">
        /// Directorio de la cancion, para ser
        /// creada y agregarle la version de metadata
        /// </param>
        /// <returns>
        /// Objeto Song con los parametros de la canción
        /// </returns>
        public async Task<Song> createVersion(List<string> new_version, string p_song_directory)
        {

            Song song = await createSong(p_song_directory);

            Version ver = new Version(new_version, song.song_id);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync(versions_path, ver);

                if (response.IsSuccessStatusCode)
                {

                    ver = await response.Content.ReadAsAsync<Version>();

                    song.metadata_id = ver.version_id;

                    HttpResponseMessage updsng = await client.PutAsJsonAsync<Song>(songs_path + "/" + song.song_id, song);

                    if (updsng.IsSuccessStatusCode)
                    {
                        song = await updsng.Content.ReadAsAsync<Song>();
                        Console.WriteLine("\nSe creo correctamente, metadata_id");
                    }

                    else
                    {
                        song = null;
                        Console.WriteLine("\nError {0}", updsng.StatusCode);
                    }
                }
                else
                {
                    song = null;
                }
            }

            return song;

        }

        /// <summary>
        /// Crea una version nueva pero con un objeto Song
        /// </summary>
        /// <param name="new_version">
        /// Version que se le va a agregar a la canción
        /// </param>
        /// <param name="song">
        /// Canción a la que se le asigna la version
        /// </param>
        /// <returns>
        /// Objeto Song que contiene los valores de la cancion
        /// y con una version asignada
        /// </returns>
        public async Task<Song> createVersion(List<string> new_version, Song song)
        {
            Version ver = new Version(new_version, song.song_id);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync(versions_path, ver);

                if (response.IsSuccessStatusCode)
                {

                    ver = await response.Content.ReadAsAsync<Version>();

                    song.metadata_id = ver.version_id;

                    HttpResponseMessage updsng = await client.PutAsJsonAsync<Song>(songs_path + "/" + song.song_id, song);

                    if (updsng.IsSuccessStatusCode)
                    {
                        Console.WriteLine("\nSe creo correctamente, metadata_id {0}", song.metadata_id);
                    }
                    else
                    {
                        song = null;
                        Console.WriteLine("\nError {0}", updsng.StatusCode);
                    }
                }
                else
                {
                    song = null;
                }
            }

            return song;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="met"></param>
        /// <returns></returns>
        public async Task<Song> createVersion(Metadata met)
        {
            Song song;

            Version ver = new Version(met);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync(versions_path, ver);

                if (response.IsSuccessStatusCode)
                {
                    ver = await response.Content.ReadAsAsync<Version>();

                    song = await getSongById(Convert.ToInt32(met._SongID));

                    song.metadata_id = ver.version_id;

                    HttpResponseMessage updsng = await client.PutAsJsonAsync<Song>(songs_path + "/" + song.song_id, song);

                    if (updsng.IsSuccessStatusCode)
                    {
                        Console.WriteLine("\nSe creo correctamente, metadata_id {0}", song.metadata_id);
                    }
                    else
                    {
                        song = null;
                        Console.WriteLine("\nError {0}", updsng.StatusCode);
                    }
                }
                else
                {
                    song = null;
                }

                return song;
            }
        }

        /// <summary>
        /// Agrega una canción a un usuario al crear la canción
        /// </summary>
        /// <param name="p_user_name">
        /// Nombre de usuario al que se le agrega la cancion
        /// </param>
        /// <param name="p_song_name">
        /// nombre de la cancion
        /// </param>
        /// <param name="new_version">
        /// lista con la información de una versión
        /// </param>
        /// <param name="p_song_directory">
        /// dirección de la canción a agregar
        /// </param>
        /// <returns>
        /// bool que es true si se logró crear la canción, en otro caso false
        /// </returns>
        public async Task<bool> addSong2user(string p_user_name, string p_song_name,
            List<string> new_version, string p_song_directory)
        {
            bool flag = false;

            Song song = await createVersion(new_version, p_song_directory);

            Property prop = new Property() { user_name = p_user_name, song_name = p_song_name, song_id = song.song_id };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync<Property>(properties_path, prop);

                if (response.IsSuccessStatusCode)
                {
                    flag = true;
                    Console.WriteLine("\nSe agrego bien la cancion");
                }
                else
                {
                    flag = false;
                    Console.WriteLine("\nEl codigo de error: {0}", response.StatusCode);
                }
            }

            return flag;
        }

        /// <summary>
        /// Agrega una canción a un usuario
        /// </summary>
        /// <param name="p_user_name">
        /// Nombre del usaurio al que se le va a agregar 
        /// la canción
        /// </param>
        /// <param name="p_song_name">
        /// Nombre de la canción que se va a agregar
        /// </param>
        /// <param name="song">
        /// Objeto Song que tiene los parametros necearios 
        /// para unir las canciones
        /// </param>
        /// <returns>
        /// bool true si se logra crear el enlace, false en cualquier 
        /// otro caso
        /// </returns>
        public async Task<bool> addSong2user(string p_user_name, string p_song_name, Song song)
        {
            bool flag = false;

            Property prop = new Property() { user_name = p_user_name, song_name = p_song_name, song_id = song.song_id };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync<Property>(properties_path, prop);

                if (response.IsSuccessStatusCode)
                {
                    flag = true;
                    Console.WriteLine("\nSe agrego bien la cancion");
                }
                else
                {
                    flag = false;
                    Console.WriteLine("\nEl codigo de error: {0}", response.StatusCode);
                }
            }

            return flag;
        }

        /// <summary>
        /// Le asigna una una metada a una canción
        /// </summary>
        /// <param name="p_song_id">
        /// id de la canción
        /// </param>
        /// <param name="p_version_id">
        /// id de la versión que se le va a asignar
        /// </param>
        /// <returns>
        /// bool true si se logra asignar, false en cualquier otro caso
        /// </returns>
        public async Task<bool> setMetadataSong(int p_song_id, int p_version_id)
        {
            bool flag = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage song_res = await client.GetAsync(songs_path);

                Song song = await song_res.Content.ReadAsAsync<Song>();

                HttpResponseMessage ver_res = await client.GetAsync(versions_path);

                Version ver = await ver_res.Content.ReadAsAsync<Version>();

                song.metadata_id = ver.version_id;

                HttpResponseMessage sng_upd = await client.PutAsJsonAsync<Song>(songs_path + "/" + song.song_id, song);

                if (sng_upd.IsSuccessStatusCode)
                {
                    flag = true;
                    Console.WriteLine("\nSe agrego bien la cancion");
                }
                else
                {
                    flag = false;
                    Console.WriteLine("\nEl codigo de error: {0}", sng_upd.StatusCode);
                }
            }

            return flag;
        }

        /// <summary>
        /// Obtiene una canción
        /// </summary>
        /// <param name="p_song_id">
        /// int que es el id de la canción
        /// </param>
        /// <returns>
        /// Objeto Song que contiene los parametros 
        /// de una canción
        /// </returns>
        public async Task<Song> getSongById(int p_song_id)
        {
            Song song = new Song();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(songs_path + "/" + p_song_id);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\nHubo ok con el server");
                    song = await response.Content.ReadAsAsync<Song>();
                }
                else
                {
                    Console.WriteLine("\nAlgo salio mal D-: codigo {0}", response.StatusCode);
                    song = null;
                }
            }

            return song;
        }

        /// <summary>
        /// Obtiene todas las canciones de un usario y su metadata
        /// </summary>
        /// <param name="user_name">
        /// nombre del usuario
        /// </param>
        /// <returns>
        /// Lista de objetos metadata 
        /// </returns>
        public async Task<List<Metadata>> getMetadataSongByUser(string user_name)
        {
            List<Metadata> songs_metadata = new List<Metadata>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(songs_by_user_path + "/" + user_name);

                MetadataAndSong[] sngs_n_met = await response.Content.ReadAsAsync<MetadataAndSong[]>();

                for (int i = 0; i < sngs_n_met.Length; i++)
                {
                    Metadata song_met = new Metadata();

                    song_met._ID3Artist = (sngs_n_met[i].id3v2_author);

                    song_met._ID3Title = (sngs_n_met[i].id3v2_title);
                    song_met._ID3Album = (sngs_n_met[i].id3v2_album);
                    song_met._ID3Year = (sngs_n_met[i].year.ToString());
                    song_met._ID3Genre = (sngs_n_met[i].id3v2_genre);
                    song_met._ID3Lyrics = (sngs_n_met[i].id3v2_lyrics);
                    song_met._SongID = (sngs_n_met[i].song_id.ToString());
                    song_met._ID3Title = (sngs_n_met[i].song_name);
                    song_met._SongDirectory = (sngs_n_met[i].song_directory);
                    song_met._SubmissionDate = (sngs_n_met[i].submission_date);
                    songs_metadata.Add(song_met);
                }
            }
            return songs_metadata;

        }

        /// <summary>
        /// Obtiene la clasificación musical de un usuario
        /// </summary>
        /// <param name="usr_id">
        /// Nombre del usuario
        /// </param>
        /// <returns>
        /// string con la clasificación del usuario
        /// </returns>
        public async Task<string> getMusicalByUserName(string usr_id)
        {
            string res = "";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_users_path + "/Musical?id=" + usr_id);

                if (response.IsSuccessStatusCode)
                {
                    res = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    res = "";
                }
            }

            return res;
        }

        /// <summary>
        /// Obtiene la clasificacion como resultado de los gustos de 
        /// los amigos
        /// </summary>
        /// <param name="usr_name">
        /// nombre del usuario
        /// </param>
        /// <returns>
        /// retorna un string con la clasificación
        /// </returns>
        public async Task<string> getSocialByUserName(string usr_name)
        {
            string res = "";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_users_path + "/Musical?id=" + usr_name);

                if (response.IsSuccessStatusCode)
                {
                    res = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("Codigo de respuesta {0}", response.StatusCode);
                    res = "";
                }
            }

            return res;
        }

        /// <summary>
        /// Envia una solicitud de amista a un usuario
        /// </summary>
        /// <param name="usr_name">
        /// Nombre del usuario que hace la solicitud
        /// </param>
        /// <param name="friend_usr_name">
        /// nombre del usario que se le solicita
        /// </param>
        /// <returns>
        /// bool que si se envia la solicitud es true, en cualquier otro caso es false
        /// </returns>
        public async Task<bool> addFriendByUserName(string usr_name, string friend_usr_name)
        {
            bool res_status = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PutAsJsonAsync<string>(mongo_users_path + "/Amigos?id=" + usr_name,
                    friend_usr_name);

                if (response.IsSuccessStatusCode)
                {
                    res_status = true;
                }
                else
                {
                    res_status = false;
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return res_status;
        }

        /// <summary>
        /// Se le da like a una canción
        /// </summary>
        /// <param name="song_id">
        /// Id de la canción a la que se le va a dar like
        /// </param>
        /// <returns>
        /// un bool que es true si se completa la acción, false 
        /// en cualquier otro caso
        /// </returns>
        public async Task<bool> setLike2ASong(int song_id)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PutAsJsonAsync<String>(mongo_songs_path + "/Like?id=" + song_id.ToString(),
                    "");

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return result;
        }

        /// <summary>
        /// Se le da dislike a una canción
        /// </summary>
        /// <param name="song_id">
        /// nombre de la cacion a la que se le va a dar 
        /// un dislike
        /// </param>
        /// <returns>
        /// bool que es true si se completa el request, false
        /// en cualqueir otro caso
        /// </returns>
        public async Task<bool> setDislike2ASong(int song_id)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PutAsJsonAsync<String>(mongo_songs_path + "/Dislike?id=" + song_id.ToString(),
                    "");

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return result;
        }

        /// <summary>
        /// Se le suma una reproducción a la cancion
        /// </summary>
        /// <param name="song_id">
        /// id de la canción 
        /// </param>
        /// <returns>
        /// bool que es true si se termina de subir 
        /// el numero, false en cualquier otro caso
        /// </returns>
        public async Task<bool> setPlay2ASong(int song_id)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PutAsJsonAsync<String>(mongo_songs_path + "/Play?id=" + song_id.ToString(),
                    "");

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return result;
        }

        /// <summary>
        /// Se le agrega un comentario a una canción
        /// </summary>
        /// <param name="song_id">
        /// id de la canción
        /// </param>
        /// <param name="usr_name">
        /// nombre de usuario del que hace el comentario
        /// </param>
        /// <param name="p_comment">
        /// comentario
        /// </param>
        /// <returns>
        /// bool que es true si se completa la acción, 
        /// es false si no se logra.
        /// </returns>
        public async Task<bool> setComment2ASong(int song_id, string usr_name, string p_comment)
        {
            bool result = false;

            Comment comm = new Comment() { autor = usr_name, cmt = p_comment };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PutAsJsonAsync<Comment>(mongo_songs_path + "/Comment?id=" + song_id.ToString(),
                    comm);

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return result;
        }

        /// <summary>
        /// Obtiene la cantidad de likes que tiene una canción
        /// </summary>
        /// <param name="song_id">
        /// identificador de la canción a la que se le obtiene la
        /// cantidad de likes
        /// </param>
        /// <returns>
        /// int con la cantiad de likes, si es -1 
        /// hubo un error
        /// </returns>
        public async Task<int> getSongLikes(int song_id)
        {
            int result = 0;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_songs_path + "/Like?id=" + song_id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    result = Convert.ToInt32(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    result = -1;
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }

                return result;
            }

        }

        /// <summary>
        /// Obtiene los dislikes de una canción
        /// </summary>
        /// <param name="song_id">
        /// identificadro de la canción a la que se le 
        /// van a obtener los dislikes
        /// </param>
        /// <returns>
        /// int que es la cantidad de dislikes, si es -1 
        /// hubo un error
        /// </returns>
        public async Task<int> getSongDislkes(int song_id)
        {
            int result = 0;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_songs_path + "/Dislike?id=" + song_id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    result = Convert.ToInt32(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    result = -1;
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }

                return result;
            }
        }

        /// <summary>
        /// Obtiene las veces que ha sido reproducida una canción
        /// </summary>
        /// <param name="song_id">
        /// Identificador de la canción
        /// </param>
        /// <returns>
        /// int que son las cantidades de veces que ha sido reproducida
        /// una canción
        /// </returns>
        public async Task<int> getSongPlays(int song_id)
        {
            int result = 0;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_songs_path + "/Play?id=" + song_id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    result = Convert.ToInt32(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    result = -1;
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return result;
        }

        /// <summary>
        /// Obtiene los comentarios y que los hizo
        /// </summary>
        /// <param name="song_id">
        /// Identificador de la canción de la que se 
        /// quieren obtener los comentarios
        /// </param>
        /// <returns>
        /// List de string que tiene comentarios y quien los hizo
        /// </returns>
        public async Task<List<string>> getSongComments(int song_id)
        {
            List<string> result = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_songs_path + "/Comment?id=" + song_id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    string[] res = await response.Content.ReadAsAsync<string[]>();

                    for (int i = 0; i < res.Length; i++)
                    {
                        result.Add(res[i]);
                    }
                }
                else
                {
                    result = null;
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return result;
        }

        /// <summary>
        /// Obtiene una lista de solicitudes que tiene 
        /// un usuario
        /// </summary>
        /// <param name="usr_name">
        /// nombre de usuario al que se le van a obtener
        /// las solicitudes
        /// </param>
        /// <returns>
        /// List<Solicitud> que tiene las solicitudes que se le hicieron a un
        /// usuario
        /// </returns>
        public async Task<List<string>> getRequests(string usr_name)
        {
            List<string> requests = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(friend_request_path + "/" + usr_name);

                if (response.IsSuccessStatusCode)
                {
                    string[] requests_array = await response.Content.ReadAsAsync<string[]>();

                    for (int i = 0; i < requests_array.Length; i++)
                    {
                        requests.Add(requests_array[i]);
                    }
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return requests;
        }

        /// <summary>
        /// Se envia una solicitud a un usuario
        /// </summary>
        /// <param name="p_emisor">
        /// usario que hace la solicitud
        /// </param>
        /// <param name="p_receptor">
        /// usuario al que le solicitan
        /// </param>
        /// <returns>
        /// bool que es true si se completa la acción
        /// </returns>
        public async Task<bool> setRequest(string p_emisor, string p_receptor)
        {
            bool result = false;

            Solicitud request = new Solicitud() { emisor = p_emisor, receptor = p_receptor };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PostAsJsonAsync<Solicitud>(friend_request_path, request);

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Elimina solicitud de amigo
        /// </summary>
        /// <param name="p_emisor">
        /// el que enviaba la solicitud
        /// </param>
        /// <param name="p_receptor">
        /// usario que recibia la solicitud
        /// </param>
        /// <returns>
        /// bool que es true si se completa la acción
        /// </returns>
        public async Task<bool> deleteRequest(string p_emisor, string p_receptor)
        {
            bool result = false;

            Solicitud request = new Solicitud() { emisor = p_emisor, receptor = p_receptor };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PutAsJsonAsync(friend_request_path, request);

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Retorna una lista de amigos
        /// </summary>
        /// <param name="usr_name">
        /// Nombre de usario al que se le van a ver los amigos
        /// </param>
        /// <returns>
        /// List de string que tiene todos los amigos
        /// </returns>
        public async Task<List<string>> getFriends(string usr_name)
        {
            List<string> result = new List<string>();

            using(HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_users_path+"/Amigos?id="+usr_name);

                if (response.IsSuccessStatusCode)
                {
                    string[] res = await response.Content.ReadAsAsync<string[]>();

                    for(int i = 0; i < res.Length; i++)
                    {
                        result.Add(res[i]);
                    }
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);

                    result = null;
                }

                
            }

            return result;
        }

        /// <summary>
        /// Obtiene las recomendaciones de amistad de un usuario
        /// </summary>
        /// <param name="usr_name">
        /// Nombre de usuario al que se le van a recomendar amigos
        /// </param>
        /// <returns>
        /// Lista con los nombres de los amigos que se le recomendaron
        /// </returns>
        public async Task<List<string>> getRecomendedFriends(string usr_name)
        {
            List<string> list = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_users_path+"/Recomendar?id="+usr_name);

                if (response.IsSuccessStatusCode)
                {
                    string[] tmp = await response.Content.ReadAsAsync<string[]>();

                    for(int i = 0; i < tmp.Length; i++)
                    {
                        list.Add(tmp[i]);
                    }
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

            return list;
         }

        /// <summary>
        /// Se obtiene la cantidad amigos
        /// </summary>
        /// <param name="usr_name">
        /// Nombre de usuario al que se le quiere ver la cantidad
        /// de amigos
        /// </param>
        /// <returns>
        /// int que es la cantidad de amigos
        /// </returns>
        public async Task<int> getFriendsQuantity(string usr_name)
        {
            int result = -1;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_users_path+"CantidadAmigos?id="+usr_name);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<int>();
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

                return result;
        }

        /// <summary>
        /// Cambia el estado de conexión del usuario en la aplicación
        /// web
        /// </summary>
        /// <param name="state">
        /// bool que es el estado de conexion
        /// </param>
        /// <returns>
        /// bool que es true si se completa la acción, en cualquier otro
        /// caso es false;
        /// </returns>
        public async Task<bool> setConnectedState(bool state)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PutAsJsonAsync<string>(mongo_users_path+"/Conectado?id="+state.ToString()
                    ,"");
                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

                return result;
        } 

        /// <summary>
        /// Obtiene la popularidad de un usuario
        /// </summary>
        /// <param name="usr_id">
        /// Identificador de usuario
        /// </param>
        /// <returns>
        /// indice de popularidad debido a los amigos
        /// </returns>
        public async Task<int> getPopularityByFriends(string usr_id)
        {
            int popularity = -1;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_users_path+"/Popular?id="+usr_id);

                if (response.IsSuccessStatusCode)
                {
                    popularity = await response.Content.ReadAsAsync<int>();
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                }
            }

                return popularity;
        }

        /// <summary>
        /// Establece un indice de popularidad a un usario
        /// </summary>
        /// <param name="popularity"></param>
        /// <returns></returns>
        public async Task<bool> setPopularity(string usr_name, int popularity)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.PutAsJsonAsync<int>(mongo_users_path+"/Popular?id="+usr_name,popularity);

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                    result = false;
                }

            }

                return result;
        }

        /// <summary>
        /// Elimina un usuario de la base
        /// </summary>
        /// <param name="usr_id">
        /// Identificador del usuario 
        /// </param>
        /// <returns>
        /// bool que es true si se cumple el delete, false 
        /// en cualquier otro caso.
        /// </returns>
        public async Task<bool> deleteUser(string usr_id)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.DeleteAsync(mongo_users_path+"/"+usr_id);

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                    result = false;
                }

            }

            return result;
        }

        /// <summary>
        /// Obtiene la popularidad debido a comentarios
        /// </summary>
        /// <param name="user_id">
        /// nombre del usuario al que se le va a obtener las
        /// la popularidad
        /// </param>
        /// <returns>
        /// int que es el nivel de popularidad
        /// </returns>
        public async Task<int> getPopularityByComments(string user_id)
        {
            int result = -1;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.GetAsync(mongo_songs_path+"/Popular?id="+user_id);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<int>();
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                    result = -1;
                }
            }

                return result;
        }

        /// <summary>
        /// Se elimina una credencial de la base 
        /// </summary>
        /// <param name="user_id">
        /// Identificador de la credencial a borrar
        /// </param>
        /// <returns>
        /// retorna un bool que es true si se logra borrar la credencial. En
        /// cualquier otro caso es false.
        /// </returns>
        public async Task<bool> deleteCredential(string user_id)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.DeleteAsync(credentials_path +"/"+user_id);

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Borra una version de la tabla de versiones
        /// </summary>
        /// <param name="version_id">
        /// Version a borrar 
        /// </param>
        /// <returns>
        /// bool que es true si se ejecuta la acción, en 
        /// caso contrario retorna false
        /// </returns>
        public async Task<bool> deleteVersion(int version_id)
        {
            bool result = false;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                HttpResponseMessage response = await client.DeleteAsync(versions_path+"/"+version_id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    Console.WriteLine("Status Code {0}", response.StatusCode);
                    result = false;
                }
            }

            return result;
        }

    }
}
