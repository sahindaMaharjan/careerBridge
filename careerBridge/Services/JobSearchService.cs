using System.Net.Http;
using System.Threading.Tasks;

namespace careerBridge.Services
{
    public class JobSearchService
    {
        private readonly HttpClient _httpClient;
        private const string apiKey = "YgkX3Tk422X5h1aIuN3pSgQ==TxlrEIj2r344qpIY"; // Replace with your actual key
        private const string baseUrl = "https://api.api-ninjas.com/v1/jobs?query=";

        public JobSearchService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        }

        public async Task<string> SearchJobsAsync(string query)
        {
            var response = await _httpClient.GetAsync(baseUrl + query);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
