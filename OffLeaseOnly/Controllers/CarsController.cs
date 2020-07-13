using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    [RoutePrefix("api/Cars")]
    public class CarsController : ApiController
    {
        /// <remarks>
        /// <h2>All electric cars less than 15K </h2>
        /// <pre>eng.Contains("Electric") AND price &lt; 15000</pre>
        ///
        /// <h2>Cars under 15K and less than 12K miles</h2>
        /// <pre>price &lt; 15000 AND mileage &lt; 12000</pre>
        ///
        /// </remarks>
        public IEnumerable<Car> Get(string query, int skip = 0, int take = 10)
        {
            return Cars.Data.Where(query).Skip(skip).Take(take);
        }

        // GET: api/Cars/1235
        [Route("{vin}")]
        public IEnumerable<CarData> GetByVin(string vin)
        {
            var cars = Cars.Data.Where(x => x.vin.Contains(vin));
            var prices = Prices.Data.Where(x => x.vin.Contains(vin));
            return cars.Select(c => new CarData(c, prices.First(x => x.vin == c.vin)));
        }

        // GET: api/Cars/new
        [Route("new")]
        public IEnumerable<CarData> GetNew(string query = "car.year>0", int skip = 0, int take = 10)
        {
            var obj = new List<CarData>();
            var prices = Prices.Data
                .Where(x => x.prices.Count == 1)
                .Select(x => new { x.vin, x.prices.OrderByDescending(y => y.date).FirstOrDefault().date });
            var recent10 = prices.OrderByDescending(y => y.date).Take(400);

            foreach (var p in recent10)
            {
                var c = Cars.Data.Match(p.vin);
                if (c != null)
                    obj.Add(new CarData(c, Prices.Data.Match(p.vin)));
            }
            return obj.Where(query).Skip(skip).Take(take);
        }

        // GET: api/Cars/old
        [Route("old")]
        public IEnumerable<CarData> GetOld(string query = "car.year>0", int skip = 0, int take = 10)
        {
            var obj = new List<CarData>();
            var prices = Prices.Data.Select(x => new { x.vin, x.prices.OrderBy(y => y.date).FirstOrDefault().date });
            var oldest = prices.Where(x => Cars.Data.Any(c => c.vin == x.vin)).OrderBy(y => y.date).Take(400);

            foreach (var p in oldest)
            {
                var c = Cars.Data.Match(p.vin);
                if (c != null)
                    obj.Add(new CarData(c, Prices.Data.Match(p.vin)));
            }
            return obj.Where(query).Skip(skip).Take(take);
        }

        // GET: api/Cars/hot
        [Route("hot")]
        public IEnumerable<CarData> GetHot(string query = "car.year>0", int skip = 0, int take = 10)
        {
            var obj = new List<CarData>();
            var hot = Prices.Data.Where(x => x.prices.Count > 1 && Cars.Data.Any(c => c.vin == x.vin))
                                 .OrderBy(y => (x.prices.First().price - x.prices.Last().price));
                                 .Take(400);

            foreach (var p in hot)
            {
                var c = Cars.Data.Match(p.vin);
                if (c != null)
                    obj.Add(new CarData(c, Prices.Data.Match(p.vin)));
            }
            return obj.Where(query).Skip(skip).Take(take);
        }
    }
}
