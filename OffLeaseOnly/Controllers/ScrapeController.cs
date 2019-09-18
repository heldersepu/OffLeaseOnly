using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OffLeaseOnly.Controllers
{
    public class ScrapeController : ApiController
    {
        const string DOMAIN = "http://www.offleaseonly.com";
        const string URL = DOMAIN + "/used-cars/type_used/page_{0}/";

        public async Task<CarStats> Get()
        {
            var tasks = new List<Task<HtmlDocument>>();
            var cars = new List<Car>();
            var web = new HtmlWeb();
            int i = 1;

            var doc = web.Load(string.Format(URL, 1));
            var makes = doc.DocumentNode.ChildNode("div", "search-by-make").GetOptionValues(1);
            while (i < 200)
            {
                for (int j = 1; j < 8; j++)
                {
                    tasks.Add(web.LoadFromWebAsync(string.Format(URL, i++)));
                }
                foreach (var task in tasks)
                {
                    doc = await task;
                    var vehicles = doc.DocumentNode.ChildNodes("div", "vehicle-listing");
                    foreach (var veh in vehicles)
                    {
                        try
                        {
                            var car = GetCar(veh, makes);
                            if (!cars.Any(c => c.vin == car.vin))
                                cars.Add(car);
                        }
                        catch { }
                    }
                }
            }

            if (cars.Count > 0)
                cars.Save(2);
            return cars.Statistics();
        }

        private Car GetCar(HtmlNode vehNode, List<string> makes)
        {
            var car = new Car();
            var header = vehNode.ChildNode("div", "header");
            var bodyContainer = vehNode.ChildNode("div", "container", 0);
            if (header != null && bodyContainer != null)
            {
                car.vin = header.ChildNode("span", null, 1)?.InnerText.Replace("VIN", "").Replace("#", "").Trim();
                car.stock = header.ChildNode("span", "stock")?.InnerText.Replace("Stock", "").Replace("#", "").Trim();

                car.price = Int32.Parse(bodyContainer.ChildNode("div", "value")?.InnerText?.Replace(",", "")?.Replace("$", ""));
                car.trans = bodyContainer.ChildNode("tr", "transmission")?.ChildNode("td")?.InnerText;
                car.mileage = Int32.Parse(bodyContainer.ChildNode("tr", "mileage")?.ChildNode("td")?.InnerText?.Replace(",",""));
                car.eng = bodyContainer.ChildNode("tr", "engine")?.ChildNode("td")?.InnerText;
                car.color = bodyContainer.ChildNode("tr", "exterior-color")?.ChildNode("td")?.InnerText;

                car.location = vehNode.ChildNode("div", "location")?.ChildNode("span")?.GetAttributeValue("class", "");
                // var vPhoto = vehNode.ChildNode("div", "vehicle-photo");
                // if (vPhoto != null)
                // {
                //     car.image = vPhoto.ChildNode("img")?.GetAttributeValue("rel", "");
                //     car.link = DOMAIN + vPhoto.ChildNode("a")?.GetAttributeValue("href", "");
                // }

                var title = header.ChildNode("div", "title")?.ChildNode("a")?.InnerText;
                var objT = title.Split(' ');
                car.year = Int32.Parse(objT[0]);
                string txt = title.Substring(4).Trim();
                car.make = makes.Where(make => txt.StartsWith(make)).FirstOrDefault();
                car.model = txt.Replace(car.make, "").Trim();

                string comments = vehNode.ChildNode("div", "vehicle-comments")?.InnerText;
                if (!string.IsNullOrEmpty(comments))
                    car.cleanCarFax = comments.ToUpper().Contains("CLEAN CARFAX") ? 1: 0;
            }
            return car;
        }
    }
}
