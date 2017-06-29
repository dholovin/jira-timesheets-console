using System.Net.Http;
using System.Threading.Tasks;

namespace jiratimesheetsconsole.Service
{
    interface IRESTClient
    {
        string PrepareGetRequestUrl(string url, string queryParams);
        Task<HttpResponseMessage> GetAsync(string url, string queryParams);
    }   
}