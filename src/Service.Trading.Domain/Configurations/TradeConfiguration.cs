using Service.Trading.Domain.Models;

namespace Service.Trading.Domain.Configurations;

public class TradeConfiguration : ITradeConfiguration
{
    private readonly TradeConfig _config;
    
    public TradeConfiguration(decimal minMarkupPercentage, decimal maxMarkupPercentage, int quotePriceRecalculateIntervalInSec)
    {
        _config = new TradeConfig(minMarkupPercentage, maxMarkupPercentage, quotePriceRecalculateIntervalInSec);
    }

    public TradeConfig GetTradeConfig()
    {
        return _config;
    }
}