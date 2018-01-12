using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Contract
{
    public static partial class ConvertExtensions
    {
        public static VaultSecretContractV1 Convert(this InternalVaultSecret self)
        {
            if (self == null)
            {
                return null;
            }

            return new VaultSecretContractV1
            {
                ObjectId = self.ObjectId,
                ObjectData = self.ObjectData?.Decrypt(),
                LeasedDate = self.LeasedDate,
                GoodToDate = self.GoodToDate,
                LessorId = self?.LessorId,
            };
        }

        public static GroupContractV1 Convert(this InternalGroupMaster self)
        {
            if (self == null)
            {
                return null;
            }

            return new GroupContractV1
            {
                GroupName = self.GroupName,
                Description = self.Description,
                CanLease = self.CanLease,
                CreatedDate = self._CreatedDate,
            };
        }
    }
}
