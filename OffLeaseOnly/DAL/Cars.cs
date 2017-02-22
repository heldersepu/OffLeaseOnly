using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace OffLeaseOnly
{
    public static class Cars
    {
        private static string CAR_DATA = "CarData";
        public static string BaseDir { get { return AppDomain.CurrentDomain.BaseDirectory; } }

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

        public static dynamic Statistics(List<Car> cars)
        {
            dynamic obj = new ExpandoObject();

            obj.cleanCarFax = cars.GroupBy(x => x.cleanCarFax).ToDictionary(p => p.Key, p =>p.Count());
            obj.year = cars.GroupBy(x => x.year).ToDictionary(p => p.Key, p => p.Count());
            obj.eng = cars.GroupBy(x => x.eng).ToDictionary(p => p.Key, p => p.Count());
            obj.make = cars.GroupBy(x => x.make).ToDictionary(p => p.Key, p => p.Count());
            obj.location = cars.GroupBy(x => x.location).ToDictionary(p => p.Key, p => p.Count());

            return obj;
        }
    }
}