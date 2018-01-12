using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Client
{
    public interface IVaultClient
    {
        ManagementClient Management { get; }

        GroupClient Group { get; }

        SecretClient Secret { get; }
    }
}
