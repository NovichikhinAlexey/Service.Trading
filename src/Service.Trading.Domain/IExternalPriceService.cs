using System;
using System.Collections.Generic;
using Service.Trading.Domain.Models;

namespace Service.Trading.Domain;

public interface IExternalPriceService
{
    List<ExternalPrice> GetBySource(string source);
    public void UpdatePrice(DateTime timestamp, string source, string market, decimal ask, decimal bid);
}