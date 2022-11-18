using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DataStorageHandler.StorageFileLocation = "DataStorage.json";
            DataStorageHandler.Init();


            for (int i = 4509300; i < 4700000; i++)
            {
                string response = await GetAsync("https://www.gis.rotterdam.nl/msb.meldingenopkaart/BestaandeInternetMeldingen.ashx?id=" + i + "&details=1");
    


                if (response != "null")
                {
                    Console.WriteLine(response);

                    string DatumMelding = Regex.Match(response, @"DatumMelding"":new Date\((\d+)\)").Groups[1].Value;
                    string DatumRappel = Regex.Match(response, @"DatumRappel"":new Date\((\d+)\)").Groups[1].Value;
                    string DatumAfhandeling = Regex.Match(response, @"DatumAfhandeling"":new Date\((\d+)\)").Groups[1].Value;
              
                    Console.WriteLine(DatumMelding);
                    Console.WriteLine(DatumRappel);
                    Console.WriteLine(DatumAfhandeling);

                    int DatumAfhandelingInt = 0;
                    int DatumMeldingInt = 0;
                    int DatumRappelInt = 0;
                    try
                    {
                        if (DatumMelding.Trim() != "")
                        {
                            DatumMeldingInt = int.Parse(DatumMelding.Substring(0, DatumMelding.Length - 3));
                            Console.WriteLine("datum aangepast");
                        }
                        else
                        {
                            DatumMeldingInt = 0;
                            Console.WriteLine("datum fout");
                        }

                    }
                    catch (Exception e)
                    {
                        DatumMeldingInt = 0;

                        Console.WriteLine(e.ToString());

                    }

                    try
                    {
                        if (DatumRappel.Trim() != "")
                        {
                            DatumRappelInt = int.Parse(DatumRappel.Substring(0, DatumRappel.Length - 3));

                        }
                        else
                        {
                            DatumRappelInt = 0;


                        }
                    }
                    catch (Exception e)
                    {
                        DatumRappelInt = 0;

                        Console.WriteLine(e.ToString());

                    }

                    try
                    {

                        if (DatumAfhandeling.Trim() != "")
                        {
                            DatumAfhandelingInt = int.Parse(DatumAfhandeling.Substring(0, DatumAfhandeling.Length - 3));

                        }
                        else
                        {
                            DatumAfhandelingInt = 0;


                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }

                    Console.WriteLine("datum definitief" + DatumMeldingInt.ToString());
                    Notification notification = JsonConvert.DeserializeObject<Notification>(response);
                    notification.DataAfhandeling = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(DatumAfhandelingInt);
                    notification.DataRappel = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(DatumRappelInt);
                    notification.Datum = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(DatumMeldingInt);

                    Console.WriteLine("datum in noti" + notification.Datum.ToString());
                    // from string date to datetime where date is ticks since 1970
                    notification.BuurtNaam = notification.BuurtNaam.TrimEnd();
                    DataStorageHandler.Storage.notifications.Add(notification);

                    if (i % 50 == 0)
                    {
                        DataStorageHandler.SaveChanges();
                    }
                }
            }
        }
        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }



        public class DataStorageHandler
        {
            public static string StorageFileLocation { get; set; }
            public static DataStorage Storage { get; set; }

            public static void Init()
            {
                string fileContent = File.ReadAllText(StorageFileLocation);
                try
                {
                    Storage = JsonConvert.DeserializeObject<DataStorage>(fileContent);
                }
                catch (Exception)
                {
                    Storage = new DataStorage();
                }
            }

            public static void SaveChanges()
            {
                string res = JsonConvert.SerializeObject(Storage, Formatting.Indented,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                File.WriteAllText(StorageFileLocation, res);
            }
        }


        public class DataStorage
        {
            public List<Notification> notifications { get; set; } = new List<Notification>();
        }


        public class Notification
        {
            public int id { get; set; }

            public string Onderwerp { get; set; }
   
            public DateTime Datum { get; set; }
            public string StraatNaam { get; set; }

            public string BuurtNaam { get; set; }
            
            public string InvoerType { get; set; }

            public string Status { get; set; }
            public string HerkomstCode { get; set; }

            public string StatusOms { get; set; }

            public double CoordX { get; set; }
            public double CoordY { get; set; }

            public string VertragingsRedenOms { get; set; }
            public string VertragingsRedenCode { get; set; }
            public DateTime DataAfhandeling { get; set; }

            // create property 'DatumRappel' which return the value datetime in 1970 and set is default
            public DateTime DataRappel { get; set; }
        
            public string AfhandelingTekst { get; set; }
        }
    }
}