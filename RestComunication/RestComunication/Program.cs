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

            bool res = await rT.setPlay2ASong(7);

            if (!res)
            {
                Console.WriteLine("#No salió bien");
            }
            else
            {
                Console.WriteLine("#TodoSalióBien:-D");
            }

        }
    }

}
