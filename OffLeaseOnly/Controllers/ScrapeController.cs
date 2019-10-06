using HtmlAgilityPack;
using Swagger.Net.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace OffLeaseOnly.Controllers
{
    [CacheOutput(ClientTimeSpan = 10, ServerTimeSpan = 10)]
    public class ScrapeController : ApiController
    {
        const string DOMAIN = "http://www.offleaseonly.com";
        const string URL = DOMAIN + "/used-cars/type_used/page_{0}/";
        private static DateTime? startTime = null;
        private static CarStats data;

        private HttpResponseMessage runningResponse
        {
            get {
                var response = Request.CreateResponse(HttpStatusCode.Accepted);
                var uri = Request.RequestUri;
                response.Headers.Add("location", $"{uri.Scheme}://{uri.Host}/api/Scrape/");
                response.Headers.Add("retry-after", "30");
                return response;
            }
        }

        [SwaggerResponse(HttpStatusCode.Accepted, "Starting job")]
        public HttpResponseMessage Post()
        {
            if (startTime == null)
            {
                data = null;
                startTime = DateTime.Now;
                Task.Factory.StartNew(() => doWork());
            }
            return runningResponse;
        }

        [SwaggerResponse(HttpStatusCode.OK, "The job has completed")]
        [SwaggerResponse(HttpStatusCode.Accepted, "The job is still running")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "No job exists with the specified id")]
        public HttpResponseMessage Get()
        {
            if (data != null && startTime != null)
            {
                var time = startTime.Diff();
                startTime = null;
                return Request.CreateResponse(HttpStatusCode.OK, new { time, data });
            }
            else if (startTime != null)
                return runningResponse;
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No job running");
        }

        private async void doWork()
        {
            var tasks = new List<Task<HtmlDocument>>();
            var cars = new List<Car>();
            var web = new HtmlWeb();
            int i = 1;

            var doc = web.Load(string.Format(URL, 1));
            var makes = doc.DocumentNode.ChildNode("div", "search-by-make").GetOptionValues(1);
            while (i < 200)
            {
                for (int j = 0; j < 9; j++)
                {
                    tasks.Add(web.LoadFromWebAsync(string.Format(URL, i++)));
                }
                foreach (var task in tasks)
                {
                    doc = await task;
                    var vehicles = doc.DocumentNode.ChildNodes("div", "vehicle-listing");
                    foreach (var veh in vehicles)
                    {
                        var car = GetCar(veh, makes);
                        if (!cars.Any(c => c.vin == car.vin))
                            cars.Add(car);
                    }
                }
                tasks.Clear();
            }

            if (cars.Count > 0)
                cars.Save(2);
            data = cars.Statistics();
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
                car.mileage = Int32.Parse(bodyContainer.ChildNode("tr", "mileage")?.ChildNode("td")?.InnerText?.Replace(",", ""));
                car.eng = bodyContainer.ChildNode("tr", "engine")?.ChildNode("td")?.InnerText;
                car.color = bodyContainer.ChildNode("tr", "exterior-color")?.ChildNode("td")?.InnerText;

                car.location = vehNode.ChildNode("div", "location")?.ChildNode("span")?.GetAttributeValue("class", "");

                var title = header.ChildNode("div", "title")?.ChildNode("a")?.InnerText;
                var objT = title.Split(' ');
                car.year = Int32.Parse(objT[0]);
                string txt = title.Substring(4).Trim();
                car.make = makes.Where(make => txt.StartsWith(make)).FirstOrDefault();
                car.model = txt.Replace(car.make, "").Trim();

                string comments = vehNode.ChildNode("div", "vehicle-comments")?.InnerText;
            }
            return car;
        }
    }
}
