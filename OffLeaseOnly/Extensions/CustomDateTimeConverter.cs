using Newtonsoft.Json.Converters;

namespace OffLeaseOnly
{
    class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
