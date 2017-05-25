using System.Collections.Generic;

namespace OffLeaseOnly
{
    public class Prices : Base<PriceHistory>
    {
        public static string FilesPath = "prices.json";
        public static string JsonPath = BaseDir + "\\" + FilesPath;

        public static List<PriceHistory> Data
        {
            get
            {
                return JsonData(FilesPath);
            }
        }
    }
}
