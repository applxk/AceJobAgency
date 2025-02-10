using Newtonsoft.Json;

namespace AceJobAgency.Model
{
    public class ReCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }

}
