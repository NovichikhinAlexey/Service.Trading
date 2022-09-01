using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Service.Trading.Modules;
using Service.Trading.Postgres;
using Service.Trading.Services;

namespace Service.Trading
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureJetWallet<ApplicationLifetimeManager>(Program.Settings.ZipkinUrl);
            
            MyDbContext.LoggerFactory = Program.LogFactory;
            services.AddDatabase(MyContext.Schema, Program.Settings.PostgresConnectionString, o => new MyContext(o));  
            MyDbContext.LoggerFactory = null;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureJetWallet(env, endpoints =>
            {
                endpoints.MapGrpcService<CubiTradingGrpc>();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureJetWallet();
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
        }
    }
}
