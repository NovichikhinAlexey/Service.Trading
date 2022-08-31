namespace Service.Trading.Domain.Models
{
    public class TradeConfig
    {
        public TradeConfig()
        {
        }

        public TradeConfig(decimal minMarkupPercentage, decimal maxMarkupPercentage, int quotePriceRecalculateIntervalInSec)
        {
            MinMarkupPercentage = minMarkupPercentage;
            MaxMarkupPercentage = maxMarkupPercentage;
            QuotePriceRecalculateIntervalInSec = quotePriceRecalculateIntervalInSec;
        }

        public decimal MinMarkupPercentage { get; set; }
        public decimal MaxMarkupPercentage { get; set; }
        public int QuotePriceRecalculateIntervalInSec { get; set; }
    }
}