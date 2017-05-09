using System.Collections.Generic;
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
    }
}
