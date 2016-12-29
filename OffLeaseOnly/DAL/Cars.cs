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
            var cleanCarFax = cars.GroupBy(x => x.cleanCarFax).ToArray();
            var d1 = new Dictionary<int, int>();
            foreach (var item in cleanCarFax)
                d1.Add(item.Key, item.Count());
            obj.cleanCarFax = d1;

            var year = cars.GroupBy(x => x.year).ToArray();
            var d2 = new Dictionary<int, int>();
            foreach (var item in year)
                d2.Add(item.Key, item.Count());
            obj.year = d2;

            var eng = cars.GroupBy(x => x.eng).ToArray();
            var d3 = new Dictionary<string, int>();
            foreach (var item in eng)
                if (item.Key != null)
                    d3.Add(item.Key, item.Count());
            obj.eng = d3;

            var make = cars.GroupBy(x => x.make).ToArray();
            var d4 = new Dictionary<string, int>();
            foreach (var item in make)
                if (item.Key != null)
                    d4.Add(item.Key, item.Count());
            obj.make = d4;

            var location = cars.GroupBy(x => x.location).ToArray();
            var d5 = new Dictionary<string, int>();
            foreach (var item in location)
                if (item.Key != null)
                    d5.Add(item.Key, item.Count());
            obj.location = d5;
            return obj;
        }
    }
}