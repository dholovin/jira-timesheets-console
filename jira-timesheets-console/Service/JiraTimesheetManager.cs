using jiratimesheetsconsole.Model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace jiratimesheetsconsole.Service
{
    class JiraTimesheetManager
    {
        const string API_SEARCH = "rest/api/latest/search";

        public string Server { get; set; }

        private readonly string _userName;
        private readonly string _password;

        public JiraTimesheetManager(string server, string username, string password)
        {
            _userName = username;
            _password = password;
            Server = server;
        }

        public async Task<JiraTimecards> GetTimecards(DateTime startDate, DateTime endDate)
        {
            //Example:  //Example: https://avid-ondemand.atlassian.net/rest/api/2/search?jql=issuetype in (subTaskIssueTypes())  AND (worklogDate >= "2017/06/16" AND worklogDate <= "2017/06/16") AND worklogAuthor = currentUser()&maxResults=1000&fields=issuetype,parent,summary,worklog,project
            string querystring = string.Format("jql=(worklogDate >= '{0}' AND worklogDate <= '{1}') AND worklogAuthor = currentUser()&fields=issuetype,parent,summary,worklog,project&maxResults={2}",
                                //string.Format(jql=issuetype in (subTaskIssueTypes()) AND (worklogDate >= '{0}' AND worklogDate <= '{1}') AND worklogAuthor = currentUser()&fields=issuetype,parent,summary,worklog,project&maxResults={2}",
                                //string.Format(jql=issuetype in (subTaskIssueTypes()) AND (worklogDate >= '{0}' AND worklogDate <= '{1}') AND worklogAuthor = currentUser()&maxResults={2}",
                                startDate.ToString("yyyy/MM/dd"),
                                endDate.ToString("yyyy/MM/dd"),
                                1000);

            IRESTClient client = new RESTClient(Server, _userName, _password);

            HttpResponseMessage response = await client.GetAsync(API_SEARCH, querystring);

            HttpContent content = response.Content;

            // ... Read the string.
            string resultAsString = await content.ReadAsStringAsync();

            // ... Deserialize response
            try
            {
                var timecardsLogged = JsonConvert.DeserializeObject<JiraTimecards>(resultAsString);

                return timecardsLogged;
            }
            catch (JsonSerializationException exSerialization)
            {
                Console.WriteLine("Error: " + exSerialization.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }
    }
}
