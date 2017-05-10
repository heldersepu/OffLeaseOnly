using System.Collections.Generic;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    [RoutePrefix("Stats")]
    public class StatsController : ApiController
    {
        [HttpGet]
        [Route("Cars")]
        public CarStats CarStats()
        {
            return Cars.Data.Statistics();
        }

        [HttpGet]
        [Route("Prices")]
        public Dictionary<int, int> PriceStats()
        {
            return Prices.Data.Statistics();
        }
    }
}
