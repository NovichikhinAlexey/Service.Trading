using System.Threading.Tasks;
using Service.Trading.Domain.Models;

namespace Service.Trading.Domain;

public interface ITradeService
{
    Task<TradeRequest> ExecuteTrade(TradeRequest incomeTrade);
}