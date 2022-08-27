using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.Trading.Grpc;

namespace Service.Trading.Client
{
    [UsedImplicitly]
    public class TradingClientFactory: MyGrpcClientFactory
    {
        public TradingClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}
