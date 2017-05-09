using System;
using System.Collections.Generic;

namespace OffLeaseOnly
{
    public class PriceHistory
    {
        public PriceHistory(string vin, int price )
        {
            this.vin = vin;
            this.price = new Dictionary<int, DateTime> { [price] = DateTime.Now };
        }

        public string vin;
        public Dictionary<int, DateTime> price;
    }
}