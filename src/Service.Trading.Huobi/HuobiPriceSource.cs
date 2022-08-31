using System.Diagnostics;
using Huobi.Net.Clients;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Trading.Domain;
using Service.Trading.Domain.Configurations;
using Service.Trading.Domain.Models;
using Service.Trading.Domain.Prices;

namespace Service.Trading.Huobi;

public class HuobiPriceSource
{
    private readonly ILogger<HuobiPriceSource> _logger;
    private readonly IAssetMapperService _assetMapperService;
    private readonly IExternalPriceService _externalPriceService;
    private HuobiSocketClient _client;

    public HuobiPriceSource(
        ILogger<HuobiPriceSource> logger,
        IAssetMapperService assetMapperService,
        IExternalPriceService externalPriceService)
    {
        _logger = logger;
        _assetMapperService = assetMapperService;
        _externalPriceService = externalPriceService;
        _client = new HuobiSocketClient();
    }

    public async Task Start()
    {
        _client = new HuobiSocketClient();
        var markets = _assetMapperService.GetAllBySource(ExternalMarketConst.HuobiSource);

        _logger.LogInformation($"Start subscribe to TickerUpdates [{ExternalMarketConst.HuobiSource}]");
        var sw = Stopwatch.StartNew();
        foreach (var market in markets)
        {
            var res = await _client.SpotStreams.SubscribeToTickerUpdatesAsync(market.Market, e =>
            {
                _externalPriceService.UpdatePrice(e.Timestamp, ExternalMarketConst.HuobiSource, market.Market,
                    e.Data.BestAskPrice, e.Data.BestBidPrice);
            });

            if (res.Error != null)
            {
                _logger.LogError($"Cannot subscribe to TickerUpdates [{ExternalMarketConst.HuobiSource}][{market.Market}]: {JsonConvert.SerializeObject(res)}");
            }
            else
            {
                _logger.LogInformation($"Subscribed to TickerUpdates [{ExternalMarketConst.HuobiSource}][{market.Market}]");
            }
        }
        sw.Stop();
        _logger.LogInformation($"Start subscribe to TickerUpdates [{ExternalMarketConst.HuobiSource}]. Time: {sw.Elapsed.ToString()}");
    }
}