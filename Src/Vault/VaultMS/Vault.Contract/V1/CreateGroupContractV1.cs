using Khooversoft.Toolbox;
using Newtonsoft.Json;

namespace Vault.Contract
{
    [JsonObject]
    public class CreateGroupContractV1
    {
        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("canLease")]
        public bool CanLease { get; set; }
    }

    public static class CreateGroupContractV1Extensions
    {
        public static bool IsValid(this CreateGroupContractV1 contract)
        {
            return contract.IsNotNull() &&
                contract.GroupName.IsNotEmpty();
        }
    }
}
