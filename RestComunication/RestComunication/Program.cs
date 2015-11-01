using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace RestComunication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RunAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            /*BlobManager bm = new BlobManager();
            bool flag = bm.downloadSong("Braisman", 6, "Magnets.mp3", "C:\\Users\\Andres\\Music");

            if (flag)
            {
                Console.WriteLine("Descarga completada");
            }
            else
            {
                Console.WriteLine("No se pudo realizar la descarga");
            }*/

            Console.Read();
            
        }

        static async Task RunAsync()
        {
            RestTools rT = new RestTools();

            List<List<string>> res = await rT.getMetadataSongByUser("Braisman");

            if (res.Count > 0)
            {
                Console.WriteLine("#TodoSalioBien:-D");

                for(int i = 0; i < res.Count; i++)
                {
                    for(int j = 0; j < res[i].Count; j++)
                    {
                        Console.Write(res[i][j] + " ");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("#MeCagoEnTodoD-:");
            }

          //  await rT.getMetadataSong("Braisman");

           /* String result = await rT.getMusicalByUserName("Satan");

            if (result.Length > 0)
            {
                Console.WriteLine("Todo salio bien :-D musical = {0}", result);
            }
            else
            {
                Console.WriteLine("Error satanico D-:");
            }*/

            /*Song song = await rT.getSongById(6);

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
            }*/

        }
    }

}
