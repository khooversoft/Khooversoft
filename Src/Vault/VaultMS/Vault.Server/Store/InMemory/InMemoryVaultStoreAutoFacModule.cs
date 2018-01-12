using Autofac;

namespace Vault.Server
{
    public class InMemoryVaultStoreAutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryVaultStore>()
                .SingleInstance()
                .AsSelf()
                .As<IVaultAdministratorStore>();

            builder.RegisterType<InMemoryGroupStore>().As<IVaultGroupStore>();
            builder.RegisterType<InMemorySecretStore>().As<IVaultSecretStore>();
        }
    }

    public static class InMemoryVaultStoreAutoFacModuleExtension
    {
        public static ContainerBuilder AddInMemoryVaultStore(this ContainerBuilder self)
        {
            self.RegisterModule(new InMemoryVaultStoreAutoFacModule());
            return self;
        }
    }
}
