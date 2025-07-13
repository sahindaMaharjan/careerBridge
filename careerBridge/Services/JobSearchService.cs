using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace careerBridge.Services
{
    public class JobSearchService
    {
        private readonly HttpClient _httpClient;
        private const string apiKey = "5368558acemshdffde0a1cf12249p1d0d9ejsn9fc47e8eb1c3";  // Replace with your actual key
        private const string apiHost = "jsearch.p.rapidapi.com";
        private const string baseUrl = "https://jsearch.p.rapidapi.com/search";

        public JobSearchService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", apiHost);
        }

        public async Task<string> SearchJobsAsync(string query, string location = "", int? posted = null, int? minSalary = null)
        {
            var url = $"{baseUrl}?query={Uri.EscapeDataString(query)}";

            if (!string.IsNullOrWhiteSpace(location))
                url += $"&location={Uri.EscapeDataString(location)}";

            if (posted.HasValue)
                url += $"&date_posted={posted.Value}";

            if (minSalary.HasValue)
                url += $"&min_salary={minSalary.Value}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
