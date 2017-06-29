using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace jiratimesheetsconsole.Service
{
    class RESTClient: IRESTClient
    {
        private readonly string _baseAddress;
        private readonly string _mediaType;
        private readonly string _charSet;
        private readonly string _userName;
        private readonly string _password;

        //TODO: pass logger into constructor 
        public RESTClient(string baseAddress, string userName, string password, string mediaType = "application/json", string charSet = "utf-8")
        {
            if (string.IsNullOrEmpty(baseAddress))
                throw new ArgumentException("Parameter 'baseAddress' cannot be empty!");

            _baseAddress = baseAddress;
            _mediaType = mediaType;
            _charSet = charSet;
            _userName = userName;
            _password = password;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, string queryParams)
        {
            if (string.IsNullOrEmpty(url))
            {
                //TODO: Log exception
                throw new ArgumentException("Parameter 'url' cannot be empty!");
            }

            if (string.IsNullOrEmpty(queryParams))
            {
                //TODO: Log exception
                throw new ArgumentException("Parameter 'queryParams' cannot be empty!");
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseAddress);

                client.DefaultRequestHeaders.Accept.Clear();
                MediaTypeWithQualityHeaderValue acceptHeader = new MediaTypeWithQualityHeaderValue(_mediaType);
                acceptHeader.CharSet = _charSet;
                client.DefaultRequestHeaders.Accept.Add(acceptHeader);

                if (!string.IsNullOrEmpty(_userName) && !string.IsNullOrEmpty(_password))
                {
                    //Authenticate Request (Basic)
                    var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_userName}:{_password}")));
                    client.DefaultRequestHeaders.Authorization = authValue;
                }

                //Send Async GET Request
                string requestUrl = PrepareGetRequestUrl(url, queryParams);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    HandleStatusCode(response);

                    return response;
                }
                catch (Exception ex)
                {
                    //TODO: Log Exception

                }


                //TODO: what to return?
                return null;
            }
        }

        //TODO: implement data driven unit test
        public string PrepareGetRequestUrl(string url, string queryParams)
        {
            string result = string.Empty;
            
            //Ensure first '/' char, assume BaseUrl has no '/' 
            result = url.StartsWith("/") ? url : "/" + url;

            //Trim last '/' char if any before appending query string
            result = result.EndsWith("/") ? result.TrimEnd('/') : result;

            //Append '?' char before appending query string
            result = result.EndsWith("?") ? result : result += "?";

            //Trim first '?' char in queryParams if any before appending
            result = queryParams.StartsWith("?") ? result += (queryParams.TrimEnd('?')) : result += queryParams;

            //Example: https://avid-ondemand.atlassian.net/rest/api/2/search?jql=issuetype in (subTaskIssueTypes())  AND (worklogDate >= "2017/06/16" AND worklogDate <= "2017/06/16") AND worklogAuthor = currentUser()&maxResults=1000&fields=issuetype,parent,summary,worklog,project
            return result;
        }

        private void HandleStatusCode(HttpResponseMessage response)
        {
            switch(response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.BadRequest:
                    throw new HttpRequestException("Error: Status Code 400. The request was invalid. You may be missing a required argument or provided bad data. An error message will be returned explaining what happened.");
                case HttpStatusCode.Forbidden:
                    throw new HttpRequestException("Error: Status Code 403. You don't have permission to complete the operation or access the resource.");
                case HttpStatusCode.NotFound:
                    throw new HttpRequestException("Error: Status Code 404. You requested an invalid method.");
                case HttpStatusCode.MethodNotAllowed:
                    throw new HttpRequestException("Error: Status Code 405. The method specified in the Request-Line is not allowed for the resource identified by the Request-URI. (used POST instead of PUT).");
                case HttpStatusCode.InternalServerError:
                    throw new HttpRequestException("Error: Status Code 500. Something is wrong on our end. We'll investigate what happened. Feel free to contact us.");
                case HttpStatusCode.ServiceUnavailable:
                    throw new HttpRequestException("Error: Status Code 503. The method you requested is currently unavailable (due to maintenance or high load).");
                default:
                    break;
            }

        }
    }
}
