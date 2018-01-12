using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Contract
{
    public class InternalGroupMaster
    {
        public GroupName GroupName { get; set; }

        public Description Description { get; set; }

        public bool CanLease { get; set; }

        public DateTimeOffset _CreatedDate { get; set; }

        public InternalGroupMaster Clone()
        {
            return new InternalGroupMaster
            {
                GroupName = this.GroupName,
                Description = this.Description,
                CanLease = this.CanLease,
                _CreatedDate = this._CreatedDate,
            };
        }
    }
}
