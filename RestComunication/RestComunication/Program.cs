using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace RestComunication
{
    class Program
    {
        static void Main(string[] args)
        {
            /*try
            {
                RunAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }*/

            BlobManager bm = new BlobManager();
            bm.downloadSong("Braiman", 6, "Magnets.mp3", "C:\\Users\\Andres\\Music");

            Console.Read();
            
        }

        static async Task RunAsync()
        {
            RestTools rT = new RestTools();

            Song song = await rT.getSongById(6);

            if(song == null)
            {
                Console.WriteLine("Algo salió mal D:");
            }
            else
            {
                BlobManager bm = new BlobManager();

                bool result = bm.uploadSong("Braisman", song.song_id, song.song_directory);

                if (!result)
                {
                    Console.WriteLine("No se pudo subir la canción D-:");
                }
                else
                {
                    Console.WriteLine("La canción se subió correctamente :-D");
                }
            }

        }
    }

}
