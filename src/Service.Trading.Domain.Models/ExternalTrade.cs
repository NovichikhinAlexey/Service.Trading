using System;

namespace Service.Trading.Domain.Models
{
    public class ExternalTrade
    {
        public string TradeId { get; set; }
        public string RequestId { get; set; }
        public string Source { get; set; }
        public string Market { get; set; }
        public decimal Amount { get; set; }
        public decimal Volume { get; set; }
        
        public DateTime Timestamp { get; set; }
        public bool IsBuySide { get; set; }
        
        public ExternalTradeStatus Status { get; set; }
        public string ExchangeTradeId { get; set; }
    }

    public enum ExternalTradeStatus
    {
        New = 0,
        Hedged = 1
    }
}