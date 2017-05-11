using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    [RoutePrefix("api/Cars")]
    public class CarsController : ApiController
    {
        // GET: api/Cars?take=10&skip=0&query=
        // GET: api/cars?take=100&skip=0&query=price<19000+AND+mileage<12000+AND+cleanCarFax=1+AND+eng="4"+AND+make!="Kia"
        public IEnumerable<Car> Get(int take, int skip, string query)
        {
            return Cars.Data.Where(query).Skip(skip).Take(take);
        }

        // GET: api/Cars/1235
        [Route("{vin}")]
        public CarData GetByVin(string vin)
        {
            return new CarData()
            {
                car = Cars.Data.Where(x => x.vin == vin).FirstOrDefault(),
                value = Prices.Data.Where(x => x.vin == vin).FirstOrDefault()
            };
        }

        // GET: api/Cars/new
        [Route("new")]
        public List<CarData> GetNew()
        {
            var obj = new List<CarData>();
            var prices = Prices.Data
                .Where(x => x.prices.Count == 1)
                .Select(x => new { x.vin, x.prices.OrderByDescending(y => y.date).FirstOrDefault().date });
            var recent10 = prices.OrderByDescending(y => y.date).Take(10);

            foreach (var p in recent10)
            {
                obj.Add(new CarData()
                {
                    car = Cars.Data.Where(x => x.vin == p.vin).FirstOrDefault(),
                    value = Prices.Data.Where(x => x.vin == p.vin).FirstOrDefault()
                });
            }
            return obj;
        }

        // GET: api/Cars/old
        [Route("old")]
        public List<CarData> GetOld()
        {
            var obj = new List<CarData>();
            var prices = Prices.Data.Select(x => new { x.vin, x.prices.OrderBy(y => y.date).FirstOrDefault().date });
            var oldest10 = prices.Where(x => Cars.Data.Any(c => c.vin == x.vin)).OrderBy(y => y.date).Take(10);

            foreach (var p in oldest10)
            {
                obj.Add(new CarData()
                {
                    car = Cars.Data.Where(x => x.vin == p.vin).FirstOrDefault(),
                    value = Prices.Data.Where(x => x.vin == p.vin).FirstOrDefault()
                });
            }
            return obj;
        }

        // GET: api/Cars/hot
        [Route("hot")]
        public List<CarData> GetHot()
        {
            var obj = new List<CarData>();
            var hot = Prices.Data.Where(x => x.prices.Count > 1 && Cars.Data.Any(c => c.vin == x.vin)).Take(10);

            foreach (var p in hot)
            {
                obj.Add(new CarData()
                {
                    car = Cars.Data.Where(x => x.vin == p.vin).FirstOrDefault(),
                    value = Prices.Data.Where(x => x.vin == p.vin).FirstOrDefault()
                });
            }
            return obj;
        }
    }
}
