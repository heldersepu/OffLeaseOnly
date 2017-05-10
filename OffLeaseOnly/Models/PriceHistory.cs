using System;
using System.Collections.Generic;

namespace OffLeaseOnly
{
    public class PriceHistory
    {
        public PriceHistory(string vin, int price )
        {
            this.vin = vin;
            prices = new List<Price> { new Price(price) };
        }

        public string vin;
        public List<Price> prices;

        public class Price
        {
            public Price(int price )
            {
                this.price = price;
                date = DateTime.Now;
            }

            public int price;
            public DateTime date;
        }
    }
}