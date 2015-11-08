using System;
using System.Data.SqlClient;
using System.IO;

namespace DatabaseTrial
{
    class Program
    {
        static void Main(string[] args)
        {

            //string sql_path = "../../../OdysseyOffline.sql";

           // string connec_str = "Server=localhost;Integrated security=SSPI;database=master";

            string connec_str2 = "Server=localhost;Integrated security=SSPI;database=OdysseyOffline";

            /*string script = File.ReadAllText(sql_path);

            using (SqlConnection conn = new SqlConnection(connec_str))
            {
                SqlCommand command = new SqlCommand("CREATE DATABASE OdysseyOffline", conn);
                try
                {
                    conn.Open();
                    command.ExecuteNonQuery();

                    Console.WriteLine("Todo salio bien");

                    conn.Close();
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }



            using (SqlConnection conn = new SqlConnection(connec_str2))
            {
                SqlCommand command = new SqlCommand(script, conn);
                try
                {
                    conn.Open();
                    command.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }*/

            using (SqlConnection connection = new SqlConnection(connec_str2))
            {
                SqlCommand command = new SqlCommand();

                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = "INSERT canciones_tbl (song_directory) output INSERTED.local_song_id VALUES( @directory )";
                command.Parameters.AddWithValue("@directory", "C:\\Users\\Yolo");

                command.Connection = connection;

                connection.Open();

                if(command == null)
                {
                    Console.WriteLine("NULL");

                }
                else
                {
                    object obj = command.ExecuteScalar();

                    int row_id = (int) obj;

                    Console.WriteLine("id {0}", row_id);
                }

                connection.Close();

            }

            Console.Read();
        }
    }
}
