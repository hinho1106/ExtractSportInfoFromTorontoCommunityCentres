using Newtonsoft.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ExtractSportInfo
{
    internal class Program_dropin_general
    {

        //JSON format for macro info
        public class RootObj
        {
            public string? objectIdFieldName { get; set; }
            public UniqueIdField? uniqueIdField { get; set; }
            public string? globalIdFieldName { get; set; }
            public string? geometryType { get; set; }
            public SpatialReference? spatialReference { get; set; }
            public List<Field>? fields { get; set; }
            public List<Feature>? features { get; set; }
        }

        public class Attributes
        {
            public int objectid_1 { get; set; }
            public int? objectid { get; set; }
            public int locationid { get; set; }
            public int facility_master_id { get; set; }
            public string? complexname { get; set; }
            public string? address { get; set; }
            public string? website { get; set; }
            public string? location_type { get; set; }
            public string? show_on_sports_map { get; set; }
            public string? sports_activities_a_d { get; set; }
            public string? sports_activities_e_p { get; set; }
            public string? sports_activities_s_z { get; set; }
            public double x { get; set; }
            public double y { get; set; }
            public string? globalid { get; set; }
        }

        public class Feature
        {
            public Attributes? attributes { get; set; }
            public Geometry? geometry { get; set; }
        }

        public class Field
        {
            public string? name { get; set; }
            public string? type { get; set; }
            public string? alias { get; set; }
            public string? sqlType { get; set; }
            public object? domain { get; set; }
            public object? defaultValue { get; set; }
            public int? length { get; set; }
        }

        public class Geometry
        {
            public double x { get; set; }
            public double y { get; set; }
        }

        public class SpatialReference
        {
            public int wkid { get; set; }
            public int latestWkid { get; set; }
        }

        public class UniqueIdField
        {
            public string? name { get; set; }
            public bool? isSystemMaintained { get; set; }
        }

        //JSON format for particular sport info of a comm centre
        public class Day
        {
            public int id { get; set; }
            public string? day { get; set; }
            public string? title { get; set; }
            public string? age { get; set; }
            public string? status { get; set; }
            public string? comment { get; set; }
            public string? link { get; set; }
            public List<Time>? times { get; set; }
        }

        public class Program
        {
            public string? program { get; set; }
            public List<Day>? days { get; set; }
        }

        public class RootPrograms
        {
            public List<Program>? programs { get; set; }
        }

        public class Time
        {
            public string? id { get; set; }
            public string? day { get; set; }
            public string? title { get; set; }
            public string? status { get; set; }
            public string? comment { get; set; }
            public string? link { get; set; }
        }

        //Global variables
        const string sport = "Pickleball";
        const string week = "week2";
        const string excludedTitle1 = "LGBT";
        const string excludedTitle2 = "Family";
        const string weekday = "monday";

        static async Task<string> GETResponse()
        {
            HttpClient client = new HttpClient();

            const string url = "https://services3.arcgis.com/b9WvedVPoizGfvfD/arcgis/rest/services/COT_Sports_DropIn_View/FeatureServer/0/query?f=json&where=show_on_sports_map%20=%20%27Yes%27&returnGeometry=true&spatialRel=esriSpatialRelIntersects&outFields=*&outSR=102100&resultOffset=0&resultRecordCount=4000";

            //Task task = client.GetAsync(url);

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

        static async Task<string> GETCommPickleballInfo(int centreID)
        {
            HttpClient client = new HttpClient();


            string url = "https://www.toronto.ca/data/parks/live/locations/" + centreID + "/sports/" + week + ".json";

            HttpResponseMessage response = await client.GetAsync(url);


            if (response.IsSuccessStatusCode)
            {
                //Console.WriteLine("Request Success");
                //Console.WriteLine();
                return await response.Content.ReadAsStringAsync();

            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
                return "";

            }
        }

        static async Task<int> Main1()
        {
            

            string content = await GETResponse();
            //Console.WriteLine(content);

            RootObj? rootObj;

            //Convert response to JSON format
            try
            {
                rootObj = JsonConvert.DeserializeObject<RootObj>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }


            Console.WriteLine();

            //Read JSON format

            foreach (var commCentre in rootObj.features)
            {
                string? sportlist = commCentre.attributes.sports_activities_e_p;
                string? centreName = commCentre.attributes.complexname;
                int id = commCentre.attributes.locationid;

                if (!string.IsNullOrEmpty(sportlist) ? sportlist.Contains(sport) : false)
                {
                    //Console.WriteLine(centreName);

                    //control variables
                    bool showCommInfo = true;

                    //Testing
                    //Task<string> getPickleballInfo = Task.Run(() => GETCommPickleballInfo(id));

                    //getPickleballInfo.Wait();
                    //string info = getPickleballInfo.Result;

                    string info = await GETCommPickleballInfo(id);

                    RootPrograms? infoObj;

                    //Convert response to JSON format
                    try
                    {
                        infoObj = JsonConvert.DeserializeObject<RootPrograms>(info);
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

                                        if (showCommInfo)
                                        {
                                            Console.WriteLine(centreName);
                                            showCommInfo = false;
                                        }

                                        Console.WriteLine(program.title);
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

                    

                    if (!showCommInfo)
                    {
                        Console.WriteLine();
                        showCommInfo = true;
                    }

                    //Console.WriteLine(info);
                    //break;
                }



            }

            return 1;

        }
    }
}
