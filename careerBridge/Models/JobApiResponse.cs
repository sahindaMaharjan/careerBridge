using Newtonsoft.Json;
using System.Collections.Generic;

namespace careerBridge.Models
{
    public class JobApiResponse
    {
        [JsonProperty("data")]
        public List<ExternalJobViewModel> Data { get; set; }
    }
}
