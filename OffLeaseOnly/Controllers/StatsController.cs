using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace OffLeaseOnly.Controllers
{
    [RoutePrefix("api/Stats")]
    [CacheOutput(ClientTimeSpan = 3600, ServerTimeSpan = 3600)]
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

        [HttpGet]
        [Route("Additions")]
        public Dictionary<string, int> AddedStats(string dateFormat="yyyy-MM-dd")
        {
            var prices = Prices.Data.Select(x => new { date = x.prices.OrderBy(y => y.date).FirstOrDefault().date.ToString(dateFormat) });
            return prices.OrderByDescending(y => y.date).GroupBy(x => x.date).ToDict();
        }

        [HttpGet]
        [Route("Data")]
        public IHttpActionResult DataStats()
        {
            var di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            return Ok(di.GetFiles("*.json")
                        .Select((file, index) =>
                            new { file.Name, file.Length, file.LastWriteTime }));
        }
    }
}
