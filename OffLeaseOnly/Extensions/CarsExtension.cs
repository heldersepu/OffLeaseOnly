using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OffLeaseOnly
{
    public static class CarsExtension
    {
        public static CarStats Statistics(this List<Car> cars)
        {
            var obj = new CarStats();

            obj.year = cars.GroupBy(x => x.year).ToDict();
            obj.eng = cars.GroupBy(x => x.eng).ToDict();
            obj.make = cars.GroupBy(x => x.make).ToDict();
            obj.location = cars.GroupBy(x => x.location).ToDict();

            return obj;
        }

        public static void Save(this List<Car> cars, int max)
        {
            Cars.UpdateMemCache(cars);
            using (var file = File.CreateText(Cars.CsvPath))
            {
                file.WriteLine(Car.CsvHeaders());
                foreach (var car in cars)
                {
                    file.WriteLine(car.ToString());
                }
            }

            int delta = (cars.Count / max) + 1;
            for (int i = 0; i < max; i++)
            {
                using (var file = File.CreateText(String.Format(Cars.JsonPath, i)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, cars.Skip(i * delta).Take(delta));
                }
            }
            AddPrices(cars);
        }

        public static Car Match(this List<Car> cars, string vin)
        {
            return cars.Where(x => x.vin == vin).FirstOrDefault();
        }

        private static void AddPrices(List<Car> cars)
        {
            var prices = Prices.Data;
            bool changed = false;
            foreach (var car in cars)
            {
                var p = prices.Where(x => x.vin == car.vin).FirstOrDefault();
                if (p == null)
                {
                    prices.Add(new PriceHistory(car.vin, car.price));
                    changed = true;
                }
                else if (!p.prices.Any(x => x.price == car.price))
                {
                    p.prices.Add(new PriceHistory.Price(car.price));
                    changed = true;
                }
            }
            if (changed) prices.Save();
        }
    }
}
