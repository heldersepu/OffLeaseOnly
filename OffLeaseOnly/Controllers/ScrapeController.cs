using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    public class ScrapeController : ApiController
    {
        public Dictionary<string, int> Get()
        {
            var cars = new Dictionary<string, int>();

            var web = new HtmlWeb();
            var doc = web.Load("http://www.offleaseonly.com/used-cars/type_used/page_20/");
            var vehicles = doc.DocumentNode.DescendantsWithClass("div", "vehicle-listing");
            foreach (var veh in vehicles)
            {
                string vin = null;
                var vinObj = veh.DescendantsWithClass("div", "second-half").FirstOrDefault();
                if (vinObj != null)
                {
                    var vinContainer = vinObj.DescendantsWithClass("div", "container").Skip(2).FirstOrDefault();
                    if (vinContainer != null)
                        vin = vinContainer.InnerText;
                }
                if (vin != null)
                    cars.Add(vin, 0);
            }

            return cars;
        }
    }
}
