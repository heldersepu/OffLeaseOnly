using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;

namespace OffLeaseOnly
{
    public static class Cars
    {
        private static string CAR_DATA = "CarData";
        private static string BaseDir { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        public static string JsonPath = BaseDir + @"\cars{0}.json";
        public static string CsvPath = BaseDir + @"\cars.csv";

        public static List<Car> Data
        {
            get
            {
                var data = new List<Car>();
                var memCache = MemoryCache.Default.Get(CAR_DATA);
                if (memCache == null)
                    return ReadJson;
                else
                    return (List<Car>)memCache;
            }
        }

        public static List<Car> ReadJson
        {
            get
            {
                var data = new List<Car>();
                var allfiles = Directory.GetFiles(BaseDir, "cars?.json");
                foreach (var path in allfiles)
                    data.AddRange(JsonConvert.DeserializeObject<List<Car>>(File.ReadAllText(path)));
                UpdateMemCache(data);
                return data;
            }
        }

        public static void UpdateMemCache(List<Car> data)
        {
            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(8) };
            MemoryCache.Default.Add(CAR_DATA, data, policy);
        }
    }
}