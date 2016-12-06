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
        public string image;

        public int cleanCarFax = 0;
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
                cleanCarFax.ToString() + "," +
                mileage.ToString() + "," +
                price.ToString() + "," +
                year.ToString() + "," +
                image;
        }

        public static string CsvHeaders()
        {
            return "vin,eng,make,model,color,trans,stock,cleanCarFax,mileage,price,year,image";
        }
    }
}