using System;
using System.Collections.Generic;
using Service.Trading.Domain.Models;

namespace Service.Trading.Domain.Prices;

public interface IExternalPriceService
{
    List<ExternalPrice> GetBySource(string source);
    ExternalPrice GetBySourceAndMarket(string source, string market);
    public void UpdatePrice(DateTime timestamp, string source, string market, decimal ask, decimal bid);
}