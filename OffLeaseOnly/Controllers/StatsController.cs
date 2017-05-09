using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    public class StatsController : ApiController
    {
        // GET: api/Cars
        public dynamic Get()
        {
            return Cars.Data.Statistics();
        }
    }
}
