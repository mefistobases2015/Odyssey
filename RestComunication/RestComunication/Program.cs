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

            List<string> metadata = new List<string>();

            metadata.Add("Hozier");
            metadata.Add("Someone New");
            metadata.Add("Braisman Songs");
            metadata.Add("2013");
            metadata.Add("Braisman style");
            metadata.Add("I fall in love just a little o little bit...");
            metadata.Add("2013-06-05 00:00:12");


                       

            if (result)
            {
                Console.WriteLine("Sirvio");
            }
            else
            {
                Console.WriteLine("NI MERGAS");
            }
        }
    }

}
