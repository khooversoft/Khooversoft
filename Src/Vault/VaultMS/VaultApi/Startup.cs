using Autofac;
using Autofac.Extensions.DependencyInjection;
using Khooversoft.Actor;
using Khooversoft.AspMvc;
using Khooversoft.EventFlow;
using Khooversoft.Net;
using Khooversoft.Observers;
using Khooversoft.Security;
using Khooversoft.Services;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultApi
{
    public class Startup : IStartup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);

            Configuration = builder.Build();

            EventSourceSubject = new EventListenerSubject()
                .EnableEvents(ToolboxEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(ActorEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(NetEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(ObserversEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(SecurityEventSource.Log, EventLevel.LogAlways)
                .EnableEvents(ServicesEventSource.Log, EventLevel.LogAlways);

            // Link event buffer
            EventSourceSubject
                .Subscribe(EventDataBuffer);

            ServiceConfiguration = new ServiceConfiguration()
                .Set(EventDataBuffer);

        }

        public ILifetimeScope ApplicationContainer { get; private set; }

        public IConfigurationRoot Configuration { get; }

        public IServiceConfiguration ServiceConfiguration { get; }

        public EventListenerSubject EventSourceSubject { get; }

        public IEventDataBuffer EventDataBuffer { get; } = new EventDataBufferObserver();

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(x => x.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true }))
                .AddMvcOptions(x => x.Filters.Add(new HmacAuthenticateAttribute()));



            //.AddMvcOptions(x => { x.Filters.Add(new GlobalExceptionFilter()); });

            //services.AddSwaggerGen(x => x.SwaggerDoc("v1", new Info { Title = "Lease Object Service", Version = "v1" }));

            // Build AutoFac container
            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterInstance(ApiApplication.Configuration);
            builder.RegisterInstance(ApiApplication.Configuration.ServerTokenManagerConfiguration);
            builder.RegisterInstance(ApiApplication.Configuration.ServerTokenManagerConfiguration.HmacConfiguration);
            builder.RegisterInstance(new HeaderFactory()).As<IHeaderFactory>();
            builder.Register(ctx => new MiddlewareContext(this.ApplicationContainer)).As<IMiddlewareContext>().SingleInstance();

            builder.AddCertificateModule();
            builder.AddIdentityModule();
            builder.AddTokenServerModule();
            builder.AddVaultActor();
            builder.AddInMemoryVaultStore();

            //builder.RegisterModule(new IdentityActorAutoFacModule());
            //builder.RegisterModule(new CertificateAutoFacModule());
            //builder.RegisterModule(new IdentityActorAutoFacModule());
            //builder.RegisterModule(new VaultActorAutoFacModule());
            //builder.RegisterModule(new InMemoryVaultStoreAutoFacModule());
            //builder.RegisterModule(new TokenManagerAutoFacModule(client: false, server: true));

            builder.RegisterType<VaultGroupManager>().As<IVaultGroupManager>();
            builder.RegisterType<VaultSecretManager>().As<IVaultSecretManager>();
            builder.RegisterType<HmacMiddleware>();
            //builder.RegisterType<HmacAuthenticateAttribute>();

            builder.Register(c => new ActorManagerBuilder().Set(ApplicationContainer).Build()).SingleInstance();
            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddSingleton(ApiApplication.Configuration);
        //    services.AddSingleton(ApiApplication.HeaderFactory);

        //    // Repositories
        //    services.AddSingleton<IVaultRepositoryConfiguration>(s => ApiApplication.Configuration.GetRepositoryConfiguration(ApiApplication.Context));
        //    services.AddSingleton<IVaultSecretRepository, VaultSecretRepository>();
        //    services.AddSingleton<IVaultGroupRepository, VaultGroupRepository>();
        //    services.AddSingleton<IVaultAdministratorRepository, VaultAdministratorRepository>();

        //    // Managers
        //    services.AddSingleton<IVaultSecretManager, VaultSecretManager>();
        //    services.AddSingleton<IVaultGroupManager, VaultGroupManager>();

        //    // Add framework services.
        //    services.AddMvc()
        //        .AddJsonOptions(x => x.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true }));
        //    //.AddMvcOptions(x => { x.Filters.Add(new GlobalExceptionFilter()); });

        //    services.AddSwaggerGen(x => x.SwaggerDoc("v1", new Info { Title = "Lease Object Service", Version = "v1" }));
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        //{
        //    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
        //    loggerFactory.AddDebug();

        //    app.UseMiddleware<SetupMiddleware>();
        //    app.UseMiddleware<ErrorHandlingMiddleware>();
        //    app.UseMvc();

        //    //app.UseSwagger();
        //    //app.UseSwaggerUI(x =>
        //    //{
        //    //    x.SwaggerEndpoint("/swagger/v1/swagger.json", "Version 1 of Lease Service Web API");
        //    //});
        //}

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<SetupMiddleware>();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<HmacMiddleware>();
            app.UseMvc();
        }
    }
}
