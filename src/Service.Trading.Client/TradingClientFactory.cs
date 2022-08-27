using System;
using CubiTradingService;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using MyJetWallet.Sdk.GrpcMetrics;

namespace Service.Trading.Client
{
    [UsedImplicitly]
    public class TradingClientFactory
    {
        private readonly CallInvoker _channel;
        
        public TradingClientFactory(string grpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            _channel = GrpcChannel.ForAddress(grpcServiceUrl).Intercept((Interceptor) new PrometheusMetricsInterceptor());
        }

        public CubiTrading.CubiTradingClient GetCubiTradingClient() => new(_channel);
    }
}
