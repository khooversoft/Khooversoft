using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vault.Client;
using Vault.Server;

namespace Vault.Configuration
{
    public class ConfigurationManager
    {
        private static readonly Dictionary<HostEnvironment, IVaultServerConfiguration> _serverEnvironments = new Dictionary<HostEnvironment, IVaultServerConfiguration>
        {
            [HostEnvironment.Test] = new TestServerConfiguration(),
            [HostEnvironment.Local] = new LocalServerConfiguration(),
        };

        private static readonly Dictionary<HostEnvironment, IVaultClientConfiguration> _clientEnvironments = new Dictionary<HostEnvironment, IVaultClientConfiguration>
        {
            [HostEnvironment.Test] = new TestClientConfiguration(),
            [HostEnvironment.Local] = new LocalClientConfiguration(),
        };

        /// <summary>
        /// Get environment
        /// </summary>
        /// <param name="environment"></param>
        /// <returns>vault configuration</returns>
        /// <exception cref="KeyNotFoundException">Environment is unknown</exception>
        public static IVaultServerConfiguration GetServer(HostEnvironment environment)
        {
            try
            {
                return _serverEnvironments[environment];
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Unknown environment {environment}", ex);
            }
        }

        /// <summary>
        /// Get client environment
        /// </summary>
        /// <param name="environment"></param>
        /// <returns>vault configuration</returns>
        /// <exception cref="KeyNotFoundException">Environment is unknown</exception>
        public static IVaultClientConfiguration GetClient(HostEnvironment environment)
        {
            try
            {
                return _clientEnvironments[environment];
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Unknown environment {environment}", ex);
            }
        }
    }
}
