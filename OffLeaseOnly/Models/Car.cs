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
        public string stockNum;
        public bool? cleanCarFax;

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
                stockNum.Replace(",", ";") + "," +
                cleanCarFax.ToString() + "," +
                mileage.ToString() + "," +
                price.ToString() + "," +
                year.ToString();
        }
    }
}