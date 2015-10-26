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

            Console.Read();
            
        }

        static async Task RunAsync()
        {
            RestTools rT = new RestTools();

            Song result = await rT.getSongById(5);

            if(result != null)
            {
                Console.WriteLine("id de la cancion {0} metadata {1} directorio {2}", 
                    result.song_id, result.metadata_id, result.song_directory);

                List<string> lista = new List<string>();

                lista.Add("2005-04-26 03:00:00");
                lista.Add("sample song");
                lista.Add("Braisman");
                lista.Add("#yolo #suaj #uknow");
                lista.Add("The best of the best of Braisman");
                lista.Add("braisman style");
                lista.Add("2001");

                result = await rT.createVersion(lista, result);

                Console.WriteLine("\nid de la cancion {0} metadata {1} directorio {2}",
                    result.song_id, result.metadata_id, result.song_directory);

            }
            else
            {
                Console.WriteLine("Algo salió mal D-: 0");
            }
        }
    }

}
