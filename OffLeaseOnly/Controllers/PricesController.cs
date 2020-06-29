using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    [RoutePrefix("api/Prices")]
    public class PricesController : ApiController
    {
        // GET: api/Prices/1235
        [Route("{vin}")]
        public IEnumerable<PriceHistory> GetByVin(string vin)
        {
            return Prices.Data.Where(x => x.vin.Contains(vin));
        }
    }
}
