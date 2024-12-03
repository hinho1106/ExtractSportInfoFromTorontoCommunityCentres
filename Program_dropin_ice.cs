using Newtonsoft.Json;
using static ExtractSportInfo.Program_dropin_general;

namespace ExtractSportInfo
{
    //General Ice drop-in info
    public class Attributes
    {
        public int objectid { get; set; }
        public string location { get; set; }
        public int? facility_master_id { get; set; }
        public int locationid { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public string address { get; set; }
        public string amenities { get; set; }
        public object operational_hours { get; set; }
        public object funguide_url { get; set; }
        public string website { get; set; }
        public string show_on_map { get; set; }
        public string location_type { get; set; }
        public object dropins { get; set; }
        public string globalid { get; set; }
        public string activity_type { get; set; }
    }

    public class Feature
    {
        public Attributes attributes { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Field
    {
        public string name { get; set; }
        public string type { get; set; }
        public string alias { get; set; }
        public string sqlType { get; set; }
        public object domain { get; set; }
        public object defaultValue { get; set; }
        public int? length { get; set; }
    }

    public class Geometry
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class GeneralIce
    {
        public string objectIdFieldName { get; set; }
        public UniqueIdField uniqueIdField { get; set; }
        public string globalIdFieldName { get; set; }
        public string geometryType { get; set; }
        public SpatialReference spatialReference { get; set; }
        public List<Field> fields { get; set; }
        public List<Feature> features { get; set; }
    }

    //Particular Ice location
    public class Day
    {
        public int id { get; set; }
        public string day { get; set; }
        public string title { get; set; }
        public string age { get; set; }
        public string status { get; set; }
        public string comment { get; set; }
        public string link { get; set; }
        public List<Time> times { get; set; }
    }

    public class Program
    {
        public string program { get; set; }
        public List<Day> days { get; set; }
    }

    public class Location
    {
        public List<Program> programs { get; set; }
    }

    public class Time
    {
        public string id { get; set; }
        public string day { get; set; }
        public string title { get; set; }
        public string status { get; set; }
        public string comment { get; set; }
        public string link { get; set; }
    }

    public class SpatialReference
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }
    }

    public class UniqueIdField
    {
        public string name { get; set; }
        public bool isSystemMaintained { get; set; }
    }

    internal class Program_dropin_ice
    {

        //Global variables
        const string sport = "Leisure Skate";
        const string week = "week1";
        const string excludedTitle1 = "LGBT";
        const string excludedTitle2 = "Family";
        const string weekday = "wednesday";

        static async Task<string> GETGeneralIceResponse()
        {
            HttpClient client = new HttpClient();

            const string url = "https://services3.arcgis.com/b9WvedVPoizGfvfD/arcgis/rest/services/Skate_Locations_v2/FeatureServer/0/query?f=json&where=Show_On_Map%20=%20%27Yes%27&returnGeometry=true&spatialRel=esriSpatialRelIntersects&outFields=*&outSR=102100&resultOffset=0&resultRecordCount=4000";

            HttpResponseMessage response = await client.GetAsync(url);


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Success");
                return await response.Content.ReadAsStringAsync();

            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
                return "";

            }



        }

        static async Task<string> GETIceLocationResponse(int locationID)
        {
            HttpClient client = new HttpClient();

            string url = "https://www.toronto.ca/data/parks/live/locations/" + locationID +"/skate/" + week + ".json";

            HttpResponseMessage response = await client.GetAsync(url);


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Success");
                return await response.Content.ReadAsStringAsync();

            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
                return "";

            }



        }

        

        async static Task<int> Main(string[] args)
        {
            string content = await GETGeneralIceResponse();
            //Console.WriteLine(content);

            GeneralIce? rootObj;

            //Convert response to JSON format
            try
            {
                rootObj = JsonConvert.DeserializeObject<GeneralIce>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }

            Console.WriteLine();

            //Read JSON format

            foreach (var location in rootObj.features)
            {
                string? activitylist = location.attributes.activity_type;
                
                string? locationName = location.attributes.location;
                int id = location.attributes.locationid;

                if (!string.IsNullOrEmpty(activitylist) ?activitylist.Contains(sport) : false)
                {
                    //Console.WriteLine(centreName);

                    //control variables
                    bool showLocationInfo = true;

                    //Testing
                    //Task<string> getPickleballInfo = Task.Run(() => GETCommPickleballInfo(id));

                    //getPickleballInfo.Wait();
                    //string info = getPickleballInfo.Result;

                    string info = await GETIceLocationResponse(id);

                    Location? infoObj;

                    //Convert response to JSON format
                    try
                    {
                        infoObj = JsonConvert.DeserializeObject<Location>(info);
                        if (infoObj == null)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 0;
                    }


                    try
                    {
                        //Console.WriteLine(centreName);
                        //Console.WriteLine(id);
                        List<Day>? programs = infoObj.programs[0].days;
                        foreach (Day program in programs)
                        {
                            if (program.title.Contains(sport) && !program.age.Contains("60") && !program.age.Contains("55") && !program.title.Contains(excludedTitle1) && !program.title.Contains(excludedTitle2))
                            {



                                foreach (Time time in program.times)
                                {
                                    if (time.day.Contains(weekday))
                                    {

                                        if (showLocationInfo)
                                        {
                                            Console.WriteLine(locationName);
                                            showLocationInfo = false;
                                        }

                                        Console.WriteLine(program.title + " " + program.age);
                                        Console.WriteLine(time.day + "\t" + time.title);
                                    }


                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }



                    if (!showLocationInfo)
                    {
                        Console.WriteLine();
                        showLocationInfo = true;
                    }

                    //Console.WriteLine(info);
                    //break;
                }



            }

            return 1;
        }
    }
}
