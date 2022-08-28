using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.Trading.Domain;
using Service.Trading.Huobi;

namespace Service.Trading
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly AssetMapperService _assetMapperService;
        private readonly HuobiPriceSource _huobiPriceSource;

        public ApplicationLifetimeManager(
            IHostApplicationLifetime appLifetime, 
            ILogger<ApplicationLifetimeManager> logger,
            AssetMapperService assetMapperService,
            HuobiPriceSource huobiPriceSource)
            : base(appLifetime)
        {
            _logger = logger;
            _assetMapperService = assetMapperService;
            _huobiPriceSource = huobiPriceSource;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _assetMapperService.Start();
            _huobiPriceSource.Start().GetAwaiter().GetResult();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
