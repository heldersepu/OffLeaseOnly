using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OffLeaseOnly
{
    public static class CarsExtension
    {
        public static Stats Statistics(this List<Car> cars)
        {
            var obj = new Stats();

            obj.cleanCarFax = cars.GroupBy(x => x.cleanCarFax).ToDict();
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
        }
    }
}
