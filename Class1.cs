using RestSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace ConsoleApplication2
{

    class Rootobject
    {
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public Issue[] issues { get; set; }
    }

    public class Issue
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public Fields fields { get; set; }
    }

    public class Fields
    {
        public Assignee assignee { get; set; }
    }

    public class Assignee
    {
        public string self { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string accountId { get; set; }
        public string emailAddress { get; set; }
        public Avatarurls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
    }

    public class Avatarurls
    {
        public string _48x48 { get; set; }
        public string _24x24 { get; set; }
        public string _16x16 { get; set; }
        public string _32x32 { get; set; }
    }

    class Program
    {
        //this fellow gets you your base 64 encodin
        private static string GetEncodedCredentials(string m_Username, string m_Password)
        {
            string mergedCredentials = string.Format("{0}:{1}", m_Username, m_Password);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }
        static Dictionary<string, List<Dictionary<string, string>>> getCritical(string[] gs)
        //   public static void Main(string[] gs)
        {
            //bhai ke creds  . can be made modular when required.
            string apikey = GetEncodedCredentials(" ", " ");
            string rawjsonresponse = consumeAPI(apikey); //bhai ka cryptokey
            Rootobject robj = deserializelejson(rawjsonresponse);  // the response, objectified. ;P
            var dict = squashtoDict(robj);  // voila!
            return dict;
        }
        static Rootobject deserializelejson(string js)
        {
            return JsonConvert.DeserializeObject<Rootobject>(js);

        }
        static Dictionary<string, List<Dictionary<string, string>>> squashtoDict(Rootobject r)
        {
            var x = r.issues[0].self;
            var d = new Dictionary<string, List<Dictionary<string, string>>>();
            int numberofissues = r.issues.Length;
            for (int i = 0; i < numberofissues; i++)
            {
                string candidatetobeakey = r.issues[i].fields.assignee.displayName;

                var issuedata = new Dictionary<string, string>();
                issuedata.Add(r.issues[i].key, r.issues[i].self);

                if (d.ContainsKey(candidatetobeakey))
                {
                    d[candidatetobeakey].Add(issuedata);
                }
                else
                {

                    d.Add(candidatetobeakey, new List<Dictionary<string, string>>());
                    d[candidatetobeakey].Add(issuedata);
                }
            }
            return d;
        }
        //this champ does the request response (in raw json string) with a use case query hard coded in which would've other wise taken in both auth key and stringified jql
        static string consumeAPI(string apikey)
        {
            //here below is the endpoint provided by jira's rest api.
            var client = new RestClient("https://companyname.jira.com/rest/api/2/search");

            var request = new RestRequest(Method.POST);
            //use pacman chrome app to perform request, responses before converting it to restsharp here.
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Basic " + apikey);
            //FOR DEPLOYMENT request.AddParameter("application/json", "{\"jql\":\"project=\\\"\\\" and duedate=startOfDay() ORDER BY assignee\"}  ", ParameterType.RequestBody);
            /*FOR DEMO:*/
            request.AddParameter("application/json", "{\"jql\":\"project = \\\"\\\" AND duedate =\\\"2017/09/22\\\" ORDER BY assignee\", \"startAt\":0,\"maxResults\":7,\"fields\":[\"id\",\"key\",\"assignee\"]}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.StatusCode());
            var rpns = response.Content; // your raw json response
                                         // Console.WriteLine(rpns);

            //  Console.ReadLine();
            return rpns;
            //var xy = "Hello";
            //var path = @"‪C:\Users\mypath\" + "abcde.txt";
            //var fileName = "something.txt";
            //Path.Combine(str_uploadpath, fileName);
            // File.WriteAllText(Path.Combine(str_uploadpath, fileName), xy);
            //if (!File.Exists(path))
            //{
            //    File.WriteAllText(path, xy);
            //}
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            //{
            //    file.WriteLine("test");
            //}
        }
    }
}
