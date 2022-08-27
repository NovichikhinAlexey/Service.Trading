using System;

namespace Service.Trading.Domain.Models
{
    public class ExternalPrice : ICloneable
    {
        public string Source { get; set; }
        public string Market { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public DateTime Timestamp { get; set; }
        
        public ExternalPrice Clone()
        {
            return new ExternalPrice()
            {
                Source = Source,
                Market = Market,
                Ask = Ask,
                Bid = Bid,
                Timestamp = Timestamp
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}