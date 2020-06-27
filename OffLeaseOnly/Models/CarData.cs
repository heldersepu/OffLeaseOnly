namespace OffLeaseOnly
{
    public class CarData
    {
        public Car car;
        public PriceHistory value;

        public CarData(Car c, PriceHistory priceHistory)
        {
            this.car = c;
            this.value = priceHistory;
        }
    }
}
