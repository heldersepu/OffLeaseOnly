using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static string MemKey
        {
            get
            {
                return typeof(T).ToString() + "Data";
            }
        }

        public static List<T> Data
        {
            get
            {
                var data = new List<T>();
                var memCache = MemoryCache.Default.Get(MemKey);
                if (memCache == null)
                    return ReadJson;
                else
                    return (List<T>)memCache;
            }
        }

        public static List<T> ReadJson
        {
            get
            {
                var data = new List<T>();
                var allfiles = Directory.GetFiles(BaseDir, "cars?.json");
                foreach (var path in allfiles)
                    data.AddRange(JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(path)));
                UpdateMemCache(data);
                return data;
            }
        }

        public static void UpdateMemCache(List<T> data)
        {
            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(8) };
            MemoryCache.Default.Add(MemKey, data, policy);
        }

    }
}