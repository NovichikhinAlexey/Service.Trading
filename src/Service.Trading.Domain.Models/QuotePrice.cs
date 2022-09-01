using System;
using MyNoSqlServer.Abstractions;

namespace Service.Trading.Domain.Models
{
    public class QuotePrice : MyNoSqlDbEntity
    {
        public const string TableName = "cb-trading-quote-price";
        public static string GeneratePartitionKey(string sellAsset) => sellAsset;
        public static string GenerateRowKey(string buyAsset) => buyAsset;
        
        public QuotePrice(
            string sellAsset, string buyAsset, decimal price, int sellAssetAccuracy, int buyAssetAccuracy, 
            decimal minSellAmount, decimal minBuyAmount, decimal maxSellAmount, decimal maxBuyAmount,
            DateTime timestamp)
        {
            SellAsset = sellAsset;
            BuyAsset = buyAsset;
            Price = price;
            SellAssetAccuracy = sellAssetAccuracy;
            BuyAssetAccuracy = buyAssetAccuracy;
            Timestamp = timestamp;
            
            MinSellAmount = minSellAmount;
            MinBuyAmount = minBuyAmount;
            MaxSellAmount = maxSellAmount;
            MaxBuyAmount = maxBuyAmount;

            PartitionKey = GeneratePartitionKey(sellAsset);
            RowKey = GenerateRowKey(buyAsset);
        }

        public QuotePrice()
        {
        }

        public string SellAsset { get; set; }
        public string BuyAsset { get; set; }
        public decimal Price { get; set; }
        
        public int SellAssetAccuracy { get; set; }
        public int BuyAssetAccuracy { get; set; }
        
        public decimal MinSellAmount { get; set; }
        public decimal MinBuyAmount { get; set; }
        public decimal MaxSellAmount { get; set; }
        public decimal MaxBuyAmount { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}