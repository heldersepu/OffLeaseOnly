using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace OffLeaseOnly
{
    public abstract class Base<T>
    {
        public static string BaseDir
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        private static string MemKey
        {
            get
            {
                return typeof(T).ToString() + "Data";
            }
        }

        private static List<T> ReadJson(string files)
        {
            var data = new List<T>();
            var allfiles = Directory.GetFiles(BaseDir, files);
            foreach (var path in allfiles)
                data.AddRange(JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(path)));
            UpdateMemCache(data);
            return data;
        }

        private static void Save(List<T> data, int max)
        {
            using (var file = File.CreateText(Cars.CsvPath))
            {
                file.WriteLine(Car.CsvHeaders());
                foreach (var car in data)
                {
                    file.WriteLine(car.ToString());
                }
            }

            int delta = (data.Count / max) + 1;
            for (int i = 0; i < max; i++)
            {
                using (var file = File.CreateText(String.Format(Cars.JsonPath, i)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, data.Skip(i * delta).Take(delta));
                }
            }
        }

        protected static List<T> JsonData(string files)
        {
            var data = new List<T>();
            var memCache = MemoryCache.Default.Get(MemKey);
            if (memCache == null)
                return ReadJson(files);
            else
                return (List<T>)memCache;
        }

        public static void UpdateMemCache(List<T> data, int max = 0)
        {
            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(8) };
            MemoryCache.Default.Add(MemKey, data, policy);
            if (max > 0) Save(data, max);
        }
    }
}