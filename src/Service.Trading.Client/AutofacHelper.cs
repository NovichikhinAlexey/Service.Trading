using Autofac;
using CubiTradingService;

// ReSharper disable UnusedMember.Global

namespace Service.Trading.Client
{
    public static class AutofacHelper
    {
        public static void RegisterTradingClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new TradingClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetCubiTradingClient()).As<CubiTrading.CubiTradingClient>().SingleInstance();
        }
    }
}
