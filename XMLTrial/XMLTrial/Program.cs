using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace XMLTrial
{
    class Program
    {
        static void Main(string[] args)
        {

            CreateXML();

            Console.Read();
        }

        static void CreateXML()
        {
            DatabaseSettings pDatabaseSettings = new DatabaseSettings() { databaseName = "OdysseyOffline", isDatabase = false };

            BlobManagerSettings pBlobManagerSettings = new BlobManagerSettings() { accountName = "Prueba", accountKey = "Password" };

            //se crea el objeto que se va a serializar
            Settings settings = new Settings() { databaseSettings = pDatabaseSettings, blobManagerSettings = pBlobManagerSettings };

            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            TextWriter t_writer = new StringWriter();
            //convierte el objeto en string
            serializer.Serialize(t_writer, settings);
            t_writer.Flush();
            string xml = t_writer.ToString();
            t_writer.Close();
            //Se imprime prueba
            Console.WriteLine(xml + "\n");
            //quita los espacios
            xml = xml.Trim();
            //se imprime despues del Trim
            Console.WriteLine(xml + "\n");
            //Documento
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xml);
            //no se que hace
            string xml_file = doc.OuterXml;
            //imprime lo que hizo
            Console.WriteLine(xml_file + "\n");
            //
            try
            {
                using (StreamWriter writer = new StreamWriter("settings.xml", false))
                {
                    writer.Write(xml_file.Trim());
                    writer.Flush();
                    writer.Close();
                }

                Console.WriteLine("Todo salio bien");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /*static void ReadXML()
        {
            StreamReader reader = new StreamReader("settings.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            Settings settings = (Settings) serializer.Deserialize(reader);

            Console.WriteLine("Hay base? {0}", settings.isDatabase);
        }*/

        /* static void Change()
         {
             StreamReader reader = new StreamReader("settings.xml");

             XmlSerializer serializer = new XmlSerializer(typeof(Settings));

             Settings settings = (Settings)serializer.Deserialize(reader);


             reader.Close();

             settings.isDatabase = true;

             //-------------------------------------------------------------------------

             XmlSerializer serializer2 = new XmlSerializer(typeof(Settings));
             TextWriter t_writer = new StringWriter();

             serializer.Serialize(t_writer, settings);
             t_writer.Flush();
             string xml = t_writer.ToString();
             t_writer.Close();

             xml = xml.Trim();

             XmlDocument doc = new XmlDocument();
             doc.PreserveWhitespace = true;
             doc.LoadXml(xml);

             string xml_file = doc.OuterXml;

             try
             {
                 using (StreamWriter writer = new StreamWriter("settings.xml", false))
                 {
                     writer.Write(xml_file.Trim());
                     writer.Flush();
                     writer.Close();
                 }

                 Console.WriteLine("Todo salio bien");
             }
             catch (Exception e)
             {
                 Console.WriteLine(e);
             }
         }*/
    }
    [Serializable()]
    [XmlRoot("Settings")]
    public class Settings
    {

        [XmlElement("DatabaseSettings")]
        public DatabaseSettings databaseSettings { set;  get; }

        [XmlElement("BlobManagerSettings")]
        public BlobManagerSettings blobManagerSettings{set;  get;}
        
    }

    [Serializable()]
    public class DatabaseSettings
    {
        [XmlAttribute("databaseName")]
        public string databaseName { set; get; }
        [XmlAttribute("isDatabase")]
        public bool isDatabase { set; get; }
    }

    [Serializable()]
    public class BlobManagerSettings
    {
        [XmlAttribute("accountName")]
        public string accountName { set; get; }
        [XmlAttribute("accountKey")]
        public string accountKey { set; get; }
    }

}
