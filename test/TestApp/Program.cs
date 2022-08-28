

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Service.Trading.Domain;
using Service.Trading.Domain.Models;
using Service.Trading.Huobi;

var mapper = new AssetMapperService();
mapper.Start();

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