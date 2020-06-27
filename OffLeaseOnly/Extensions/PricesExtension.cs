using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OffLeaseOnly
{
    public static class PricesExtension
    {
        public static Dictionary<int, int> Statistics(this List<PriceHistory> prices)
        {
            var p = prices.Select(x => x.prices.OrderByDescending(y => y.date).FirstOrDefault());
            return p.GroupBy(x => x.price).ToDict();
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

        public static PriceHistory Match(this List<PriceHistory> prices, string vin)
        {
            return prices.Where(x => x.vin == vin).FirstOrDefault();
        }
    }
}
