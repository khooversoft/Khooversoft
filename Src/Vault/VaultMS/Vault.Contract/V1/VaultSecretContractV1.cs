using Newtonsoft.Json;
using System;

namespace Vault.Contract
{
    [JsonObject]
    public class VaultSecretContractV1
    {
        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("objectData")]
        public string ObjectData { get; set; }

        [JsonProperty("leasedDate")]
        public DateTimeOffset? LeasedDate { get; set; }

        [JsonProperty("goodToDate")]
        public DateTimeOffset? GoodToDate { get; set; }

        [JsonProperty("kessorId")]
        public string LessorId { get; set; }
    }
}
