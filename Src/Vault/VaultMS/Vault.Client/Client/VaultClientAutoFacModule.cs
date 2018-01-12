using Autofac;
using Khooversoft.Net;
using Khooversoft.Services;
using Khooversoft.Toolbox;

namespace Vault.Client
{
    public class VaultClientAutoFacModule : Module
    {
        private readonly IRestClientConfiguration _restClientConfiguration;

        public VaultClientAutoFacModule()
        {
        }

        public VaultClientAutoFacModule(IRestClientConfiguration restClientConfiguration)
        {
            Verify.IsNotNull(nameof(restClientConfiguration), restClientConfiguration);

            _restClientConfiguration = restClientConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.AddCertificateModule();
            builder.AddTokenModule(client: true, server: false, restClientConfiguration: _restClientConfiguration);

            if (_restClientConfiguration != null)
            {
                builder.RegisterType<VaultClient>()
                    .As<IVaultClient>()
                    .WithParameter(new TypedParameter(typeof(IRestClientConfiguration), _restClientConfiguration));
            }
            else
            {
                builder.RegisterType<VaultClient>().As<IVaultClient>();
            }
        }
    }

    public static class VaultClientAutoFacModuleExtension
    {
        public static ContainerBuilder AddVaultClientModule(this ContainerBuilder self)
        {
            self.RegisterModule(new VaultClientAutoFacModule());
            return self;
        }

        public static ContainerBuilder AddVaultClientModule(this ContainerBuilder self, IRestClientConfiguration restClientConfiguration)
        {
            self.RegisterModule(new VaultClientAutoFacModule(restClientConfiguration));
            return self;
        }
    }
}
