using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RestComunication
{
    class RestTools
    {
        private string server_url = "http://odysseyop.azurewebsites.net/";
        private string format = "application/json";
        private string credentials_path = "api/Credenciales";
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

                Credentials cred = new Credentials() { user_name = p_usr_name, pass = p_pass };

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



    }
}
