using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    public class CarsController : ApiController
    {
        // GET: api/Cars
        public IEnumerable<Car> Get()
        {
            return Cars.Statistics(Cars.Data);
        }

        // GET: api/Cars?vin=1235
        public Car Get(string vin)
        {
            return Cars.Data.Where(x => x.vin == vin).FirstOrDefault();
        }

        // GET: api/Cars?take=10&skip=0&query=
        // GET: api/cars?take=100&skip=0&query=price<19000+AND+mileage<12000+AND+cleanCarFax=1+AND+eng="4"+AND+make!="Kia"
        public IEnumerable<Car> Get(int take, int skip, string query)
        {
            return Cars.Data.Where(query).Skip(skip).Take(take);

        }
    }
}
