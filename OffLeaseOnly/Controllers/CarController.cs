using System.Linq;
using System.Linq.Dynamic;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    public class CarController : ApiController
    {
        // GET: api/Cars?vin=1235
        public Car Get(string vin)
        {
            return Cars.Data.Where(x => x.vin == vin).FirstOrDefault();
        }
    }
}
