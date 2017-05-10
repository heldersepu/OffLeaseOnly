﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OffLeaseOnly
{
    public static class PricesExtension
    {
        public static Dictionary<int, int> Statistics(this List<PriceHistory> prices)
        {
            var p = prices.Select(x => x.price.OrderBy(y => y.Value).FirstOrDefault());
            return p.GroupBy(x => x.Key).ToDict(); ;
        }

        public static void Save(this List<PriceHistory> prices)
        {
            Prices.UpdateMemCache(prices);

            using (var file = File.CreateText(Prices.JsonPath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, prices);
            }
        }
    }
}
