using System.Threading.Tasks;
using Service.Trading.Domain.Models;

namespace Service.Trading.Domain.Trade;

public interface ITradeService
{
    Task<TradeRequest> ExecuteTrade(TradeRequest incomeTrade);
}