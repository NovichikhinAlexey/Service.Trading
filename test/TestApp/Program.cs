

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Service.Trading.Domain;
using Service.Trading.Domain.Models;
using Service.Trading.Huobi;

var mapper = new AssetMapperService();

#region markets
mapper
    .Add(new AssetMap()
        {
            Source = ExternalMarketConst.HuobiSource,
            Market = "btcusdt",
            ExternalBaseAsset = "btc",
            ExternalQuoteAsset = "usdt",
            BaseAsset = "BTC",
            QuoteAsset = "USDT",
            PriceAccuracy = 2,
            AmountAccuracy = 6,
            VolumeAccuracy = 8,
            MinAmount = 0.0001m,
            MaxAmount = 100,
            MinVolume = 1,
            MaxVolume = 1000000m
        })
    .Add(new AssetMap()
    {
        Source = ExternalMarketConst.HuobiSource,
        Market = "ethusdt",
        ExternalBaseAsset = "eth",
        ExternalQuoteAsset = "usdt",
        BaseAsset = "ETH",
        QuoteAsset = "USDT",
        PriceAccuracy = 2,
        AmountAccuracy = 4,
        VolumeAccuracy = 8,
        MinAmount = 0.001m,
        MaxAmount = 1000,
        MinVolume = 1,
        MaxVolume = 1000000.0m
    })
    .Add(new AssetMap()
    {
        Source = ExternalMarketConst.HuobiSource,
        Market = "xlmusdt",
        ExternalBaseAsset = "xlm",
        ExternalQuoteAsset = "usdt",
        BaseAsset = "XLM",
        QuoteAsset = "USDT",
        PriceAccuracy = 6,
        AmountAccuracy = 4,
        VolumeAccuracy = 8,
        MinAmount = 0.1m,
        MaxAmount = 1000000,
        MinVolume = 1,
        MaxVolume = 50000m
    })
    .Add(new AssetMap()
    {
        Source = ExternalMarketConst.HuobiSource,
        Market = "xrpusdt",
        ExternalBaseAsset = "xrp",
        ExternalQuoteAsset = "usdt",
        BaseAsset = "XRP",
        QuoteAsset = "USDT",
        PriceAccuracy = 5,
        AmountAccuracy = 2,
        VolumeAccuracy = 8,
        MinAmount = 1m,
        MaxAmount = 500000,
        MinVolume = 1,
        MaxVolume = 100000
    });
#endregion

var externalPrices = new ExternalPriceService();

var builder = WebApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(b =>
{
    b.AddSimpleConsole();
});

var huobi = new HuobiPriceSource(loggerFactory.CreateLogger<HuobiPriceSource>(), mapper, externalPrices);

huobi.Start().GetAwaiter().GetResult();

var cmd = Console.ReadKey();

while (cmd.Key != ConsoleKey.Q)
{
    foreach (var price in externalPrices.GetBySource(ExternalMarketConst.HuobiSource))
    {
        Console.WriteLine($"{price.Timestamp:HH:mm:ss} {price.Market} {price.Ask} {price.Bid}");
    }
    Console.WriteLine();
    cmd = Console.ReadKey();
}