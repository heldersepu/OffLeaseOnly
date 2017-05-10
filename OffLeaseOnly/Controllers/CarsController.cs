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
    }
}
