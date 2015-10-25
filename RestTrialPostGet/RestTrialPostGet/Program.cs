using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RestTrialPostGet
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                runAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.Read();
            }
        }

        static async Task runAsync()
        {
            using (var client = new HttpClient())
            {
                // Se crea la conexion del cliente
                client.BaseAddress = new Uri("http://odysseyop.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Prueba de get
                HttpResponseMessage response = await client.GetAsync("api/Credenciales/manuelito");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Estado del resquest {0}", response.StatusCode);

                    Credentials cred = await response.Content.ReadAsAsync<Credentials>();

                    Console.WriteLine("User name {0}, password {1}", cred.user_name, cred.pass);

                    /*string result = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("\n" + result);*/
                }
                else
                {
                    Console.WriteLine("Estado del resquest {0}", response.StatusCode);
                }

                //Prueba de post

                /*  Credentials cred2 = new Credentials() { user_name = "Manuelito", pass = "batmanuel" };

                  HttpResponseMessage responseP = await client.PostAsJsonAsync("api/Credenciales", cred2);

                  if (responseP.IsSuccessStatusCode)
                  {
                      Console.WriteLine("Post correcto, estado {0}", responseP.StatusCode);
                  }
                  else
                  {
                      Console.WriteLine("No sirvio D-:");
                  }*/

                Console.Read();
            }
        }
    }

    class Credentials
    {
        public string[] propiedades_tbl { get; set; }
        public string user_name { get; set; }
        public string pass { get; set; }
    }
}
