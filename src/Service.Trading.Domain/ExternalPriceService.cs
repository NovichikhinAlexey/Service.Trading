using System;
using System.Collections.Generic;
using System.Linq;
using Service.Trading.Domain.Models;

namespace Service.Trading.Domain;

public class ExternalPriceService : IExternalPriceService
{
    private readonly Dictionary<string, Dictionary<string, ExternalPrice>> _prices = new();
    private readonly object _gate = new();

    public List<ExternalPrice> GetBySource(string source)
    {
        lock (_gate)
        {
            if (!_prices.TryGetValue(source, out var sourceItem))
                return new List<ExternalPrice>();

            return sourceItem.Values.Select(e => e.Clone()).ToList();
        }
    }

    public ExternalPrice GetBySourceAndMarket(string source, string market)
    {
        lock (_gate)
        {
            if (!_prices.TryGetValue(source, out var sourceItem))
                throw new PriceNotFoundException($"Price not found for: {source}; {market}");
            
            if (!sourceItem.TryGetValue(market, out var marketItem))
                throw new PriceNotFoundException($"Price not found for: {source}; {market}");

            return marketItem;
        }
    }

    public void UpdatePrice(DateTime timestamp, string source, string market, decimal ask, decimal bid)
    {
        lock (_gate)
        {
            if (!_prices.TryGetValue(source, out var sourceItem))
            {
                sourceItem = new Dictionary<string, ExternalPrice>();
                _prices[source] = sourceItem;
            }

            if (!sourceItem.TryGetValue(market, out var marketItem))
            {
                marketItem = new ExternalPrice()
                {
                    Source = source,
                    Market = market,
                    Ask = ask,
                    Bid = bid,
                    Timestamp = timestamp
                };
                sourceItem[market] = marketItem;
            }

            marketItem.Ask = ask;
            marketItem.Bid = bid;
            marketItem.Timestamp = timestamp;
        }
    }
}