using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace OffLeaseOnly
{
    public class Cars: Base<Car>
    {
        public static string JsonPath = BaseDir + @"\cars{0}.json";
        public static string CsvPath = BaseDir + @"\cars.csv";

        public static dynamic Statistics(List<Car> cars)
        {
            dynamic obj = new ExpandoObject();

            obj.cleanCarFax = cars.GroupBy(x => x.cleanCarFax).ToDict();
            obj.year = cars.GroupBy(x => x.year).ToDict();
            obj.eng = cars.GroupBy(x => x.eng).ToDict();
            obj.make = cars.GroupBy(x => x.make).ToDict();
            obj.location = cars.GroupBy(x => x.location).ToDict();

            return obj;
        }
    }
}