using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RestComunication
{
    class Program
    {
        static void Main(string[] args)
        {
            RestTools rT = new RestTools();

            bool isUser = rT.isUser("Arturito");

            bool isPass = rT.isPassword("Braisman", "ojetedegringa");

            bool isUsrC = rT.createUser("Hector", "elmascapo");

            Console.WriteLine("isUser {0}, isPass {1}, isUsrC {2}", isUser, isPass, isUsrC);

            Console.Read();
        }
    }

    public class Credentials
    {
        public string[] propiedades_tbl { set; get; }
        public string user_name { set; get; }
        public string pass { set; get; }
    }

    public class RestTools
    {
        private string server_url = "http://odysseyop.azurewebsites.net/";
        private string format = "application/json";
        //direccion de credenciales
        private string credentials_path = "api/Credenciales";
        //bandera booleana 
        private bool flag = false;
        /**
        *Constructor vacío
        */
        public RestTools()
        {
            //constructor vacion con valores default
        }

        /**
         * Cambio de url de server
         */
        public RestTools(string p_server_url)
        {
            this.server_url = p_server_url;
        }

        /**
         * Cambio de formato
         */
        public void setFormat(string p_format)
        {
            format = p_format;
        }

        public bool isUser(string usr_name)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri(server_url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

            isUserAux(client, usr_name).Wait();

            return flag;

        }

        private async Task isUserAux(HttpClient client, string usr_name)
        {
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

        public bool isPassword(string usr_name, string pass)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri(server_url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

            isPasswordAux(client, usr_name, pass).Wait();

            return flag;
        }

        private async Task isPasswordAux(HttpClient client, string usr_name, string password)
        {
            HttpResponseMessage response = await client.GetAsync(credentials_path + "/"+ usr_name);

            if (response.IsSuccessStatusCode)
            {
                Credentials cred = await response.Content.ReadAsAsync<Credentials>();

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

        public bool createUser(string p_usr_name, string p_pass)
        {

            createUserAux(p_usr_name, p_pass).Wait();

            return flag;

        }

        private async Task createUserAux(string p_usr_name, string p_pass)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(server_url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(format));

                Credentials cred = new Credentials() { user_name = p_usr_name, pass = p_pass };

                HttpResponseMessage response = await client.PostAsJsonAsync(credentials_path, cred);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Sirvio el post :-D");
                    flag = true;
                }
                else
                {
                    Console.WriteLine("No sirvio el post D-: {0}", response.StatusCode);
                    flag = false;
                }
            }

        }
    }

}
