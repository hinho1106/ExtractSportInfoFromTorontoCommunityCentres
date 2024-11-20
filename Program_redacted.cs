using System;
using System.Net.Http;
using HtmlAgilityPack;

namespace PickleballScraper
{
    class Program_redacted
    {

        /* global variable*/

        const int dayNum = 6; // 1:Mon, 2:Tue, ... , 7:Sun
        
        //const string must be contained in the search
        const string dateRange = "Nov 11 to Nov 17";
        const string targetSport = "Pickleball";
        const string excludedPhrase = "60";

        static string numToDay(int num)
        {
            switch (num)
            {
                case 1:
                    return "Mon";
                case 2:
                    return "Tue";
                case 3:
                    return "Wed";
                case 4:
                    return "Thu";
                case 5:
                    return "Fri";
                case 6:
                    return "Sat";
                case 7:
                    return "Sun";
                default:
                    return "None";
            }
        }

        static async Task Main1()
        {
            var httpClient = new HttpClient();
            var url = "https://www.toronto.ca/data/parks/prd/sports/dropin/racquet/index.html";
            var response = await httpClient.GetAsync(url);
            var htmlContent = await response.Content.ReadAsStringAsync();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            //control variables
            bool showCommInfo = true;

            /* Debug */
            // Console.WriteLine(htmlContent);

            // Assuming the program details are in a table with specific class names
            var table = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='pfrProgramDescrList dropinbox']");

            /* Debug */
            // Console.WriteLine(table);
            if (table != null)
            {
                var communityCentreDivs = table.SelectNodes("div");
                foreach (var div in communityCentreDivs)
                {

                    /* Debug */
                    //Console.WriteLine(div.Attributes["data-id"].Value);

                    //Search community centre
                    string communityCentre = div.SelectNodes("h2")[0].InnerText;
                    //Console.WriteLine(communityCentre);

                    //Search each of the sport drop-in according to community centre, check if it matches the pickleball and desired day
                    var sportRows = div.SelectNodes("table/tbody/tr");
                    
                    foreach (var row in sportRows)
                    {
                        string sport = row.SelectNodes("th")[0].InnerText;
                        
                        if (sport.Contains(targetSport) && sport.Contains(dateRange) && !sport.Contains(excludedPhrase))
                        {
                            string time = row.SelectNodes("td")[dayNum-1].InnerText;
                            if (!time.Contains("&nbsp;"))
                            {
                                if(showCommInfo)
                                {
                                    Console.WriteLine(communityCentre);
                                    showCommInfo = false;
                                }
                                Console.Write(sport);
                                Console.Write("\t");
                                Console.Write(numToDay(dayNum));
                                Console.Write("\t");
                                Console.Write(time);
                                Console.WriteLine();
                            }
                            
                        }
                        //var sport = row.SelectNodes("tr")[0].InnerHtml;
                        //Console.WriteLine(sport);
                    }

                    if (!showCommInfo)
                    {
                        Console.WriteLine();
                        showCommInfo = true;
                    }
                    

                    
                    
                }
            }
            else
            {
                Console.WriteLine("Table not found");
            }
        }
    }
}