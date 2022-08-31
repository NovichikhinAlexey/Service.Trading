using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Trading.Domain.Configurations;
using Service.Trading.Domain.Models;
using Service.Trading.Domain.Prices;

namespace Service.Trading.Domain.Trade;

public class TradePriceCalculator
{
    private readonly ILogger<TradePriceCalculator> _logger;
    private readonly IAssetMapperService _assetMapperService;
    private readonly IExternalPriceService _externalPriceService;
    private readonly ITradeConfiguration _tradeConfiguration;

    public TradePriceCalculator(
        ILogger<TradePriceCalculator> logger,
        IAssetMapperService assetMapperService,
        IExternalPriceService externalPriceService,
        ITradeConfiguration tradeConfiguration)
    {
        _logger = logger;
        _assetMapperService = assetMapperService;
        _externalPriceService = externalPriceService;
        _tradeConfiguration = tradeConfiguration;
    }

    public List<QuotePrice> CalculateQuotes()
    {
        var markets = _assetMapperService.GetAllBySource(ExternalMarketConst.HuobiSource);

        var tradeConfig = _tradeConfiguration.GetTradeConfig();
        
        var result = new List<QuotePrice>();
        
        foreach (var market in markets)
        {
            try
            {
                var priceItem = _externalPriceService.GetBySourceAndMarket(market.Source, market.Market);

                var price = priceItem.Bid * (1 - tradeConfig.MaxMarkupPercentage / 100m);
                var quote = new QuotePrice(
                    market.BaseAsset,
                    market.QuoteAsset,
                    Math.Round(price, market.VolumeAccuracy),
                    market.AmountAccuracy,
                    market.VolumeAccuracy,
                    DateTime.UtcNow
                );
                result.Add(quote);
                
                price = priceItem.Ask * (1 + tradeConfig.MaxMarkupPercentage / 100m);
                quote = new QuotePrice(
                    market.QuoteAsset,
                    market.BaseAsset,
                    Math.Round(price, market.AmountAccuracy),
                    market.VolumeAccuracy,
                    market.AmountAccuracy,
                    DateTime.UtcNow
                );
                result.Add(quote);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot handle market {source}:{market}", market.Source, market.Market);
            }
        }

        return result;
    }
}