using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace OffLeaseOnly
{
    public static class PricesExtension
    {
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
