using Newtonsoft.Json;
using System;

namespace Vault.Contract
{
    [JsonObject]
    public class GroupContractV1
    {
        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("canLease")]
        public bool CanLease { get; set; }

        [JsonProperty("createdDate")]
        public DateTimeOffset CreatedDate { get; set; }
    }
}
