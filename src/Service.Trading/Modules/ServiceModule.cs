using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.Trading.Domain;
using Service.Trading.Domain.Configurations;
using Service.Trading.Domain.Models;
using Service.Trading.Domain.Prices;
using Service.Trading.Domain.Trade;
using Service.Trading.Huobi;
using Service.Trading.Jobs;

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
                .AutoActivate()
                .SingleInstance();

            builder.RegisterInstance(new TradeConfiguration(
                    decimal.Parse(Program.Settings.MinMarkupPercentage),
                    decimal.Parse(Program.Settings.MaxMarkupPercentage),
                    Program.Settings.QuotePriceRecalculateIntervalInSec))
                .As<ITradeConfiguration>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<TradePriceCalculator>()
                .AsSelf()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterMyNoSqlWriter<QuotePrice>(() => Program.Settings.MyNoSqlWriterUrl, QuotePrice.TableName);

            builder.RegisterType<QuoteUpdateJob>()
                .AsSelf()
                .AutoActivate()
                .SingleInstance();
        }
    }
}