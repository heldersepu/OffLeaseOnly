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

        protected static List<T> JsonData(string files)
        {
            var data = new List<T>();
            var memCache = MemoryCache.Default.Get(MemKey);
            if (memCache == null)
                return ReadJson(files);
            else
                return (List<T>)memCache;
        }

        public static void UpdateMemCache(List<T> data)
        {
            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(10) };
            MemoryCache.Default.Add(MemKey, data, policy);
        }
    }
}
