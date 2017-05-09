using System;
using System.Collections.Generic;

namespace OffLeaseOnly
{
    public class PriceHistory
    {
        public string vin;
        public Dictionary<int, DateTime> price;
    }
}