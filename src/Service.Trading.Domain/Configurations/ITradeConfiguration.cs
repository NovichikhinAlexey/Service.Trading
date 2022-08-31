using Service.Trading.Domain.Models;

namespace Service.Trading.Domain.Configurations;

public interface ITradeConfiguration
{
    TradeConfig GetTradeConfig();
}