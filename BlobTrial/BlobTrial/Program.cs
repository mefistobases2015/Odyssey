using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobTrial
{
    class Program
    {
        static string accountName = "odysseyblob";
        static string accountKey = "+LNzgFo5XOB7J7lFffed0oEha5qUDN+8aV3fIbBk8B2eeAFk/VxAwmfyBv0gk7WuSMJewyOu5R2Bnu8O0jUtKA==";

        static void Main(string[] args)
        {
            try
            {
                //hace la cuenta
                StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);
                //crea el cliente
                CloudBlobClient client = account.CreateCloudBlobClient();
                //crae el contenedor
                CloudBlobContainer sampleContainer = client.GetContainerReference("music");
                sampleContainer.CreateIfNotExists();
                //
                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference("9.mp3");
                using (System.IO.Stream file = System.IO.File.OpenRead("C:\\Users\\Andres\\Downloads\\9.mp3"))
                {
                    blob.UploadFromStream(file);
                }

                /*CloudBlockBlob blob = sampleContainer.GetBlockBlobReference("APictureFile.jpg");
                using (Stream outputFile = new FileStream("Downloaded.jpg", FileMode.Create))
                {
                    blob.DownloadToStream(outputFile);
                }*/

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.Read();
        }
    }
}
