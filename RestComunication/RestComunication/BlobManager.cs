using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Security.Permissions;

namespace RestComunication
{
    class BlobManager
    {
        private static string accountName = "odysseyblob";
        private static string accountKey = "+LNzgFo5XOB7J7lFffed0oEha5qUDN+8aV3fIbBk8B2eeAFk/VxAwmfyBv0gk7WuSMJewyOu5R2Bnu8O0jUtKA==";

        public bool uploadSong(int song_id, string song_path)
        {
            bool flag = false;

            //hace la cuenta
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

            //crea el cliente
            CloudBlobClient client = account.CreateCloudBlobClient();

            //crae el contenedor
            CloudBlobContainer container = client.GetContainerReference("music");
            container.CreateIfNotExists();
            //
            CloudBlockBlob blob = container.GetBlockBlobReference(song_id.ToString() + ".mp3");
            using (System.IO.Stream file = System.IO.File.OpenRead(song_path))
            {
                try
                {
                    blob.UploadFromStream(file);
                    flag = true;

                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    flag = false;
                }
                
            }

            return flag;
        }

        public bool downloadSong(int song_id, string song_name, string song_path)
        {
            bool flag = false;

            //hace la cuenta
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

            //crea el cliente
            CloudBlobClient client = account.CreateCloudBlobClient();

            //crae el contenedor
            CloudBlobContainer sampleContainer = client.GetContainerReference("music");

            CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(song_id.ToString() + ".mp3");

            

            try
            {
                //FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, "C:\\Users\\Andres\\Music");
                Console.WriteLine("Path: {0}", song_path + "\\" + song_name);
                Stream outputFile = new FileStream(song_path + "\\" + song_name, FileMode.Create);

                blob.DownloadToStream(outputFile);
                flag = true;
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
               flag = false;
            }

            return flag;
        }


    }
}
