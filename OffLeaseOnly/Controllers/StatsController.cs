﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    [RoutePrefix("api/Stats")]
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
        public Dictionary<string, int> AddedStats()
        {
            var prices = Prices.Data.Select(x => new { date = x.prices.OrderBy(y => y.date).FirstOrDefault().date.ToString("yyyy-MM-dd") });
            return prices.GroupBy(x => x.date).ToDict();
        }
    }
}
