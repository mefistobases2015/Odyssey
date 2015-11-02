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

            List<string> res = await rT.getRequests("Arturito");

            if (res.Count > 0)
            {
                Console.WriteLine("#YOLO#SOYUNPRO#OP#:-D");

                for(int i = 0; i < res.Count; i++)
                {
                    Console.Write(res[i] + ", ");
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("#MeLaCorto#PutaMierda#QueSal#D-:");
            }

        }
    }

}
