namespace OffLeaseOnly
{
    public class Car
    {
        public string vin;
        public string eng;
        public string make;
        public string model;
        public string color;
        public string trans;
        public string stock;
        public string location;

        public int mileage;
        public int price;
        public int year;

        public new string ToString()
        {
            return vin.Replace(",", ";") + "," +
                eng.Replace(",", ";") + "," +
                make.Replace(",", ";") + "," +
                model.Replace(",", ";") + "," +
                color.Replace(",", ";") + "," +
                trans.Replace(",", ";") + "," +
                stock.Replace(",", ";") + "," +
                mileage.ToString() + "," +
                price.ToString() + "," +
                year.ToString();
        }

        public static string CsvHeaders()
        {
            return "vin,eng,make,model,color,trans,stock,mileage,price,year";
        }
    }
}
