using Khooversoft.Services;
using System;

namespace Vault.Client
{
    public interface IVaultClientConfiguration : IClientTokenManagerConfiguration
    {
        Uri ClientBaseUrl { get; }
    }
}
