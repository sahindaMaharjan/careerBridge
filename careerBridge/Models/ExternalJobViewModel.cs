using Newtonsoft.Json;

namespace careerBridge.Models
{
    public class ExternalJobViewModel
    {
        [JsonProperty("job_title")]
        public string Title { get; set; }

        [JsonProperty("employer_name")]
        public string CompanyName { get; set; }

        [JsonProperty("job_city")]
        public string Location { get; set; }

        [JsonProperty("job_category")]
        public string Category { get; set; }

        [JsonProperty("job_description")]
        public string Description { get; set; }
    }
}
