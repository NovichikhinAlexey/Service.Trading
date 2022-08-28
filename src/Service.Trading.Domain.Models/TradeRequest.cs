using System;

namespace Service.Trading.Domain.Models
{
    public class TradeRequest
    {
        public string RequestId { get; set; }
        
        public string SellAssetSymbol { get; set; }
        public string BuyAssetSymbol { get; set; }
        public decimal SellAmount { get; set; }
        public decimal BuyAmount { get; set; }
        public string FeeAsset { get; set; }
        public decimal FeeAmount { get; set; }
        
        public DateTime Timestamp { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        
        public decimal ExpectedSellAmount { get; set; }
        public decimal ExpectedBuyAmount { get; set; }
        
        public string ClientId { get; set; }
    }
}