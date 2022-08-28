using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Trading.Domain.Models;

namespace Service.Trading.Domain;

public class TradeService : ITradeService
{
    private readonly IExternalPriceService _externalPriceService;
    private readonly IAssetMapperService _assetMapperService;
    private readonly ILogger<TradeService> _logger;
    private readonly decimal _minMarkupPercentage;
    private readonly decimal _maxMarkupPercentage;

    public TradeService(
        IExternalPriceService externalPriceService,
        IAssetMapperService assetMapperService,
        ILogger<TradeService> logger,
        decimal minMarkupPercentage, decimal maxMarkupPercentage)
    {
        _externalPriceService = externalPriceService;
        _assetMapperService = assetMapperService;
        _logger = logger;
        _minMarkupPercentage = minMarkupPercentage;
        _maxMarkupPercentage = maxMarkupPercentage;
    }

    public async Task<TradeRequest> ExecuteTrade(TradeRequest incomeTrade)
    {
        incomeTrade.FeeAsset = incomeTrade.SellAssetSymbol;
        incomeTrade.FeeAmount = 0;
        incomeTrade.Timestamp = DateTime.UtcNow;
        incomeTrade.ErrorCode = (int) ErrorCodeTrade.Ok;
        incomeTrade.ErrorMessage = ErrorCodeTrade.Ok.ToString();
        
        var marketList = _assetMapperService.GetAllBySource(ExternalMarketConst.HuobiSource);

        // try handle in buy trade in one market
        
        var market = marketList.FirstOrDefault(e =>
            e.BaseAsset == incomeTrade.BuyAssetSymbol &&
            e.QuoteAsset == incomeTrade.SellAssetSymbol);

        if (market != null)
        {
            var price = _externalPriceService.GetBySourceAndMarket(market.Source, market.Market);
            _logger.LogInformation("External market price for trade {requestId}: {jsonString}",
                incomeTrade.RequestId, JsonSerializer.Serialize(price));

            var volume = incomeTrade.BuyAmount * price.Ask;
            
            var list = new List<ExternalTrade>()
            {
                new ExternalTrade()
                {
                    TradeId = $"{incomeTrade.RequestId}:1",
                    Source = market.Source,
                    Market = market.Market,
                    Amount = incomeTrade.BuyAmount,
                    Volume = volume,
                    IsBuySide = true,
                    RequestId = incomeTrade.RequestId
                }
            };

            if (incomeTrade.SellAmount < volume * (1 + _minMarkupPercentage/100m))
            {
                _logger.LogInformation($"Price is changed, clientSellAmount={incomeTrade.SellAmount}; expected= {volume}; Client: {incomeTrade.ClientId}");
                incomeTrade.ErrorCode = (int) ErrorCodeTrade.PriceIsChanged;
                incomeTrade.ErrorMessage = ErrorCodeTrade.PriceIsChanged.ToString();
                await SaveTrade(incomeTrade);
                return incomeTrade;
            }

            incomeTrade.ExpectedBuyAmount = incomeTrade.BuyAmount;
            incomeTrade.ExpectedSellAmount = volume;
            await SaveExternalTrades(list);
            await SaveTrade(incomeTrade);
            return incomeTrade;
        }
        
        // try handle in sell trade in one market

        market = marketList.FirstOrDefault(e =>
            e.BaseAsset == incomeTrade.SellAssetSymbol &&
            e.QuoteAsset == incomeTrade.BuyAssetSymbol);

        if (market != null)
        {
            var price = _externalPriceService.GetBySourceAndMarket(market.Source, market.Market);
            
            var volume = incomeTrade.SellAmount * price.Bid;
            
            var list = new List<ExternalTrade>()
            {
                new ExternalTrade()
                {
                    TradeId = $"{incomeTrade.RequestId}:1",
                    Source = market.Source,
                    Market = market.Market,
                    Amount = incomeTrade.SellAmount,
                    Volume = volume,
                    IsBuySide = false,
                    RequestId = incomeTrade.RequestId
                }
            };
            
            if (incomeTrade.BuyAmount > volume * (1 + _minMarkupPercentage/100m))
            {
                _logger.LogInformation($"Price is changed, clientBuyAmount={incomeTrade.BuyAmount}; expected= {volume}; Client: {incomeTrade.ClientId}");
                incomeTrade.ErrorCode = (int) ErrorCodeTrade.PriceIsChanged;
                incomeTrade.ErrorMessage = ErrorCodeTrade.PriceIsChanged.ToString();
                await SaveTrade(incomeTrade);
                return incomeTrade;
            }
            
            incomeTrade.ExpectedBuyAmount = volume;
            incomeTrade.ExpectedSellAmount = incomeTrade.SellAmount;
            await SaveExternalTrades(list);
            await SaveTrade(incomeTrade);
            return incomeTrade;
        }
        
        _logger.LogInformation($"Trade way do not found, sellAsset={incomeTrade.SellAssetSymbol}; buyAsset={incomeTrade.BuyAssetSymbol}; Client: {incomeTrade.ClientId}");
        incomeTrade.ErrorCode = (int) ErrorCodeTrade.NotEnoughLiquidity;
        incomeTrade.ErrorMessage = ErrorCodeTrade.NotEnoughLiquidity.ToString();
        await SaveTrade(incomeTrade);
        return incomeTrade;
    }

    private async Task SaveExternalTrades(List<ExternalTrade> trades)
    {
        //todo: Save trade and execute after
        Console.WriteLine($"[ExternalTrade] {JsonSerializer.Serialize(trades)}");
    }

    private async Task SaveTrade(TradeRequest trade)
    {
        //todo: Save trade
        Console.WriteLine($"[Trade] {JsonSerializer.Serialize(trade)}");
    }
}