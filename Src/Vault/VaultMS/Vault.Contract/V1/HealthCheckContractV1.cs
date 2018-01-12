using Newtonsoft.Json;

namespace Vault.Contract
{
    [JsonObject]
    public class HealthCheckContractV1
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
