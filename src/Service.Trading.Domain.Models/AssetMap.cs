namespace Service.Trading.Domain.Models
{
    public class AssetMap
    {
        public string Source { get; set; }
        
        public string Market { get; set; }
        public string ExternalBaseAsset { get; set; }
        public string ExternalQuoteAsset { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public int AmountAccuracy { get; set; }
        public int PriceAccuracy { get; set; }
        public int VolumeAccuracy { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal MinVolume{ get; set; }
        public decimal MaxVolume { get; set; }
    }
}