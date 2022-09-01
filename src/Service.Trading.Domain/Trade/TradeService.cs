using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Trading.Domain.Configurations;
using Service.Trading.Domain.Models;
using Service.Trading.Domain.Prices;
using Service.Trading.Postgres;

namespace Service.Trading.Domain.Trade;

public class TradeService : ITradeService
{
    private readonly IExternalPriceService _externalPriceService;
    private readonly IAssetMapperService _assetMapperService;
    private readonly ILogger<TradeService> _logger;
    private readonly ITradeConfiguration _tradeConfiguration;
    private readonly IDbFactory _dbFactory;
    private readonly ITradePriceCalculator _tradePriceCalculator;

    public TradeService(
        IExternalPriceService externalPriceService,
        IAssetMapperService assetMapperService,
        ILogger<TradeService> logger,
        ITradeConfiguration tradeConfiguration,
        IDbFactory dbFactory,
        ITradePriceCalculator tradePriceCalculator)
    {
        _externalPriceService = externalPriceService;
        _assetMapperService = assetMapperService;
        _logger = logger;
        _tradeConfiguration = tradeConfiguration;
        _dbFactory = dbFactory;
        _tradePriceCalculator = tradePriceCalculator;
    }

    public async Task<TradeRequest> ExecuteTrade(TradeRequest incomeTrade)
    {
        incomeTrade.FeeAsset = incomeTrade.SellAssetSymbol;
        incomeTrade.FeeAmount = 0;
        incomeTrade.Timestamp = DateTime.UtcNow;
        incomeTrade.ErrorCode = (int) ErrorCodeTrade.Ok;
        incomeTrade.ErrorMessage = ErrorCodeTrade.Ok.ToString();

        TradePrecheck(incomeTrade);
        if (incomeTrade.ErrorCode != (int) ErrorCodeTrade.Ok)
        {
            await SaveTrade(incomeTrade);
            return incomeTrade;
        }

        var marketList = _assetMapperService.GetAllBySource(ExternalMarketConst.HuobiSource);

        var tradeConfig = _tradeConfiguration.GetTradeConfig();
        
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
                    RequestId = incomeTrade.RequestId,
                    Timestamp = DateTime.UtcNow,
                    Status = ExternalTradeStatus.New
                }
            };
            
            incomeTrade.ExpectedBuyAmount = incomeTrade.BuyAmount;
            incomeTrade.ExpectedSellAmount = volume;

            if (incomeTrade.SellAmount < incomeTrade.ExpectedSellAmount * (1 + tradeConfig.MinMarkupPercentage/100m))
            {
                _logger.LogInformation($"Price is changed, clientSellAmount={incomeTrade.SellAmount}; expected= {volume}; Client: {incomeTrade.ClientId}");
                incomeTrade.ErrorCode = (int) ErrorCodeTrade.PriceIsChanged;
                incomeTrade.ErrorMessage = ErrorCodeTrade.PriceIsChanged.ToString();
                await SaveTrade(incomeTrade);
                return incomeTrade;
            }
            
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
                    RequestId = incomeTrade.RequestId,
                    Timestamp = DateTime.UtcNow,
                    Status = ExternalTradeStatus.New
                }
            };
            
            incomeTrade.ExpectedBuyAmount = volume;
            incomeTrade.ExpectedSellAmount = incomeTrade.SellAmount;
            
            if (incomeTrade.BuyAmount > incomeTrade.ExpectedBuyAmount * (1 - tradeConfig.MinMarkupPercentage/100m))
            {
                _logger.LogInformation($"Price is changed, clientBuyAmount={incomeTrade.BuyAmount}; expected= {volume}; Client: {incomeTrade.ClientId}");
                incomeTrade.ErrorCode = (int) ErrorCodeTrade.PriceIsChanged;
                incomeTrade.ErrorMessage = ErrorCodeTrade.PriceIsChanged.ToString();
                await SaveTrade(incomeTrade);
                return incomeTrade;
            }
            
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

    private void TradePrecheck(TradeRequest incomeTrade)
    {
        var tradePriceList = _tradePriceCalculator.GetQuotes();
        var tradePrice = tradePriceList.FirstOrDefault(e =>
            e.SellAsset == incomeTrade.SellAssetSymbol && e.BuyAsset == incomeTrade.BuyAssetSymbol);

        if (tradePrice == null)
        {
            _logger.LogError("Cannot found QuotePrice for trade {requestId}: {jsonString}",
                incomeTrade.RequestId, JsonSerializer.Serialize(incomeTrade));
            incomeTrade.ErrorCode = (int) ErrorCodeTrade.NotEnoughLiquidity;
            incomeTrade.ErrorMessage = ErrorCodeTrade.NotEnoughLiquidity.ToString();
        }
        else if (incomeTrade.SellAmount < tradePrice.MinSellAmount)
        {
            _logger.LogError("Cannot execute trade, sell amount to small {requestId}: {jsonString}",
                incomeTrade.RequestId, JsonSerializer.Serialize(incomeTrade));
            incomeTrade.ErrorCode = (int) ErrorCodeTrade.SellAmountToSmall;
            incomeTrade.ErrorMessage = ErrorCodeTrade.SellAmountToSmall.ToString();
        }
        else if (incomeTrade.BuyAmount < tradePrice.MinBuyAmount)
        {
            _logger.LogError("Cannot execute trade, buy amount to small {requestId}: {jsonString}",
                incomeTrade.RequestId, JsonSerializer.Serialize(incomeTrade));
            incomeTrade.ErrorCode = (int) ErrorCodeTrade.BuyAmountToSmall;
            incomeTrade.ErrorMessage = ErrorCodeTrade.BuyAmountToSmall.ToString();
        }
        else if (incomeTrade.SellAmount > tradePrice.MaxSellAmount)
        {
            _logger.LogError("Cannot execute trade, sell amount to large {requestId}: {jsonString}",
                incomeTrade.RequestId, JsonSerializer.Serialize(incomeTrade));
            incomeTrade.ErrorCode = (int) ErrorCodeTrade.SellAmountToLarge;
            incomeTrade.ErrorMessage = ErrorCodeTrade.SellAmountToLarge.ToString();
        }
        else if (incomeTrade.BuyAmount > tradePrice.MaxBuyAmount)
        {
            _logger.LogError("Cannot execute trade, buy amount to large {requestId}: {jsonString}",
                incomeTrade.RequestId, JsonSerializer.Serialize(incomeTrade));
            incomeTrade.ErrorCode = (int) ErrorCodeTrade.BuyAmountToLarge;
            incomeTrade.ErrorMessage = ErrorCodeTrade.BuyAmountToLarge.ToString();
        }
    }

    private async Task SaveExternalTrades(List<ExternalTrade> trades)
    {
        await using var ctx = _dbFactory.Context();
        await ctx.UpsertRange<ExternalTrade>(trades).On(e => e.TradeId).NoUpdate().RunAsync();
    }

    private async Task SaveTrade(TradeRequest trade)
    {
        await using var ctx = _dbFactory.Context();
        await ctx.Upsert(trade).On(e => e.RequestId).NoUpdate().RunAsync();
    }
}