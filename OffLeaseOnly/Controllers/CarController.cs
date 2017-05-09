using System.Linq;
using System.Linq.Dynamic;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    public class CarController : ApiController
    {
        // GET: api/Cars?vin=1235
        public CarData Get(string vin)
        {
            var c = new CarData();
            c.car = Cars.Data.Where(x => x.vin == vin).FirstOrDefault();
            c.value = Prices.Data.Where(x => x.vin == vin).FirstOrDefault();
            return c;
        }
    }
}
