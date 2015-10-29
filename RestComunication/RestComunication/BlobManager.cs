using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace RestComunication
{
    class BlobManager
    {
        static string accountName = "odysseyblob";
        static string accountKey = "+LNzgFo5XOB7J7lFffed0oEha5qUDN+8aV3fIbBk8B2eeAFk/VxAwmfyBv0gk7WuSMJewyOu5R2Bnu8O0jUtKA==";

        public bool uploadSong(string song_path, string song_name)
        {
            bool flag = false;

            //hace la cuenta
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

            //crea el cliente
            CloudBlobClient client = account.CreateCloudBlobClient();

            //crae el contenedor
            CloudBlobContainer sampleContainer = client.GetContainerReference("music");
            sampleContainer.CreateIfNotExists();
            //
            CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(song_name);
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

        public bool downloadSong(string song_path, string song_name)
        {
            bool flag = false;

            //hace la cuenta
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

            //crea el cliente
            CloudBlobClient client = account.CreateCloudBlobClient();

            //crae el contenedor
            CloudBlobContainer sampleContainer = client.GetContainerReference("music");

            CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(song_name);

            using (Stream outputFile = new FileStream(song_path, FileMode.Create))
            {
                try
                {
                    blob.DownloadToStream(outputFile);
                    flag = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    flag = false;
                }
                
            }

            return flag;
        }

    }
}
