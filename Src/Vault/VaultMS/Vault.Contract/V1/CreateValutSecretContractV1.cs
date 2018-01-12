using Khooversoft.Toolbox;
using Newtonsoft.Json;

namespace Vault.Contract
{
    [JsonObject]
    public class CreateValutSecretContractV1
    {
        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("objectData")]
        public string ObjectData { get; set; }
    }

    public static class CreateValutSecretContractV1Extensions
    {
        public static bool IsValid(this CreateValutSecretContractV1 contract)
        {
            return contract.IsNotNull() &&
                contract.ObjectId.IsNotEmpty();
        }
    }
}
