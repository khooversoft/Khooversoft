using Khooversoft.Toolbox;
using Newtonsoft.Json;

namespace Vault.Contract
{
    [JsonObject]
    public class AcquireLeaseContractV1
    {
        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("leaseInSeconds")]
        public int LeaseInSeconds { get; set; }

        [JsonProperty("lessorId")]
        public string LessorId { get; set; }
    }

    public static class AcquireLeaseContractV1Extensions
    {
        public static bool IsValid(this AcquireLeaseContractV1 contract)
        {
            return contract.IsNotNull() &&
                contract.GroupName.IsNotEmpty() &&
                contract.LeaseInSeconds > 0 &&
                contract.LessorId.IsNotEmpty();
        }
    }
}
