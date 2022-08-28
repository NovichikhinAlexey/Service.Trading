using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Service.Trading.Domain;
using Service.Trading.Huobi;

namespace Service.Trading.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AssetMapperService>()
                .As<IAssetMapperService>()
                .AsSelf()
                .AutoActivate()
                .SingleInstance();
            
            builder.RegisterType<ExternalPriceService>()
                .As<IExternalPriceService>()
                .AutoActivate()
                .SingleInstance();
            
            builder.RegisterType<HuobiPriceSource>()
                .AsSelf()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<TradeService>()
                .As<ITradeService>()
                .WithParameter("minMarkupPercentage", Program.Settings.MinMarkupPercentage)
                .WithParameter("maxMarkupPercentage", Program.Settings.MaxMarkupPercentage)
                .AutoActivate()
                .SingleInstance();
        }
    }
}