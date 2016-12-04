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
        const string URL = "http://www.offleaseonly.com/used-cars/type_used/page_{0}/";
        public Dictionary<string, int> Get()
        {
            var cars = new Dictionary<string, int>();
            var web = new HtmlWeb();
            for (int i = 1; i < 10; i++)
            {
                var doc = web.Load(string.Format(URL, i));
                var vehicles = doc.DocumentNode.DescendantsWithClass("div", "vehicle-listing");
                foreach (var veh in vehicles)
                {
                    var car = GetCar(veh);
                    if (car.vin != null)
                        cars.Add(car.vin, 0);
                }
            }
            return cars;
        }

        private Car GetCar(HtmlNode vehNode)
        {
            var car = new Car();
            var vinObj = vehNode.DescendantsWithClass("div", "second-half").FirstOrDefault();
            if (vinObj != null)
            {
                var vinContainer = vinObj.DescendantsWithClass("div", "container").Skip(2).FirstOrDefault();
                if (vinContainer != null)
                {
                    car.vin = vinContainer.DescendantsWithClass("span", "spec-data").FirstOrDefault().InnerText;

                }
            }
            return car;
        }
    }
}
