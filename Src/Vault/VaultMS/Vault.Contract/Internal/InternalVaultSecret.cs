using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Contract
{
    public class InternalVaultSecret
    {
        public ObjectId ObjectId { get; set; }

        public ObjectData ObjectData { get; set; }

        public DateTimeOffset? LeasedDate { get; set; }

        public DateTimeOffset? GoodToDate { get; set; }

        public LessorId LessorId { get; set; }

        public DateTimeOffset _CreatedDate { get; private set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset _UpdatedDate { get; set; } = DateTimeOffset.UtcNow;

        public InternalVaultSecret Clone(bool includeObjectData)
        {
            return new InternalVaultSecret
            {
                ObjectId = this.ObjectId,
                ObjectData = includeObjectData ? this.ObjectData : null,
                LeasedDate = this.LeasedDate,
                GoodToDate = this.GoodToDate,
                _CreatedDate = this._CreatedDate,
                _UpdatedDate = _UpdatedDate,
            };
        }
    }
}
