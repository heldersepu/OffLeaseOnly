using System.Collections.Generic;

namespace OffLeaseOnly
{
    public class Cars: Base<Car>
    {
        public static string FilesPath = "cars?.json";
        public static string JsonPath = BaseDir + @"\cars{0}.json";
        public static string CsvPath = BaseDir + @"\cars.csv";

        public static List<Car> Data
        {
            get
            {
                return JsonData(FilesPath);
            }
        }
    }
}
