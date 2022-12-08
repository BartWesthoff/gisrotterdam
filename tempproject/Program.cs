using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using static Test.Program;
using Formatting = Newtonsoft.Json.Formatting;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //DataStorageHandler.StorageFileLocation = "DataStorage.json";
            //DataStorageHandler.Init();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            bool isFeatureMember = false;
            bool isCode = false;
            bool isDescription = false;
            bool isTitle = false;
            FeatureMember featureMember = new FeatureMember();
            using (var fileStream = File.OpenText("data.xml"))
            using (XmlReader reader = XmlReader.Create(fileStream, settings))

            {
                while (reader.Read())
                {


                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.WriteLine($"Start Element: {reader.Name}. Has Attributes? : {reader.HasAttributes}");
                            if (reader.Name == "gml:featureMember")
                            {
                                isFeatureMember = true;
                            }
                            if (reader.HasAttributes)
                            {
                                Console.WriteLine($"Attribute Count: {reader.AttributeCount}");
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    Console.WriteLine($"Attribute Name: {reader.Name}, Attribute Value: {reader.Value}");
                                    if (reader.Name == "fid")
                                    {
                                        // cast reader.value to int


                                        featureMember.Fid = int.Parse(reader.Value);
                                    }
                                }
                            }

                            if (reader.Name == "Code")
                            {
                                isCode = true;
                            }

                            if (reader.Name == "Description")
                            {
                                isDescription = true;

                            }
                            if (reader.Name == "Title")
                            {
                                isTitle = true;
                            }

                            break;
                        case XmlNodeType.Text:
                            Console.WriteLine($"Inner Text: {reader.Value}");
                            if (reader.Value.Contains(",") && isFeatureMember)
                            {
                                featureMember.CoordX = int.Parse(reader.Value.Split(",")[0]);
                                featureMember.CoordY = int.Parse(reader.Value.Split(",")[1]);
                            }
                            if (reader.Value.Contains(".") && isCode)
                            {
                                featureMember.Code = reader.Value;
                            }
                            if (isDescription)
                            {
                                featureMember.Description = reader.Value;
                            }
                            if (isTitle)
                            {
                                featureMember.title = reader.Value;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            Console.WriteLine($"End Element: {reader.Name}");
                            if (reader.Name == "gml:featureMember")
                            {
                                // stop program
                                isFeatureMember = false;

                                Console.WriteLine(featureMember);
                                return;
                            }
                            if (reader.Name == "Code")
                            {
                                isCode = false;
                            }
                            if (reader.Name == "Description")
                            {
                                isDescription = false;
                            }
                            if (reader.Name == "Title")
                            {
                                isTitle = false;
                            }


                            break;
                        default:
                            Console.WriteLine($"Unknown: {reader.NodeType}");
                            break;
                    }
                }
            }
        }
        //public static async Task<string> GetAsync(string uri)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        //    using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
        //    using (Stream stream = response.GetResponseStream())
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        return await reader.ReadToEndAsync();
        //    }
        //}

        public class FeatureMember
        {
            public int Fid { get; set; }
            public string Item { get; set; }
            public int CoordX { get; set; }
            public int CoordY { get; set; }
            public string Code { get; set; }
            public string title { get; set; }

            public string Summary { get; set; }

            public string Description { get; set; }

            public override string ToString()
            {
                return $"Fid: {this.Fid} Item: {this.Item} CoordX: {this.CoordX} CoordY: {this.CoordY} Code: {this.Code} Title: {this.title} Summary: {this.Summary} Description: {this.Description}";
            }

        }



        //public class DataStorageHandler
        //{
        //    public static string StorageFileLocation { get; set; }
        //    public static DataStorage Storage { get; set; }

        //    public static void Init()
        //    {
        //        string fileContent = File.ReadAllText(StorageFileLocation);
        //        try
        //        {
        //            Storage = JsonConvert.DeserializeObject<DataStorage>(fileContent);
        //        }
        //        catch (Exception)
        //        {
        //            Storage = new DataStorage();
        //        }
        //    }

        //    public static void SaveChanges()
        //    {
        //        string res = JsonConvert.SerializeObject(Storage, Formatting.Indented,
        //                    new JsonSerializerSettings()
        //                    {
        //                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //                    });
        //        File.WriteAllText(StorageFileLocation, res);
        //    }
        //}


        //public class DataStorage
        //{
        //    public List<Notification> notifications { get; set; } = new List<Notification>();
        //}


        //public class Notification
        //{
        //    public int id { get; set; }

        //    public string Onderwerp { get; set; }

        //    public DateTime Datum { get; set; }
        //    public string StraatNaam { get; set; }

        //    public string BuurtNaam { get; set; }

        //    public string InvoerType { get; set; }

        //    public string Status { get; set; }
        //    public string HerkomstCode { get; set; }

        //    public string StatusOms { get; set; }

        //    public double CoordX { get; set; }
        //    public double CoordY { get; set; }

        //    public string VertragingsRedenOms { get; set; }
        //    public string VertragingsRedenCode { get; set; }
        //    public DateTime DataAfhandeling { get; set; }

        //    // create property 'DatumRappel' which return the value datetime in 1970 and set is default
        //    public DateTime DataRappel { get; set; }

        //    public string AfhandelingTekst { get; set; }
        //}


        //public class Info
        //{
        //    public object request { get; set; }
        //    public int MyProperty { get; set; }
        //}
    }
}