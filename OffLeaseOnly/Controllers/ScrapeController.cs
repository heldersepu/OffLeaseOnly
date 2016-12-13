using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json;

namespace OffLeaseOnly.Controllers
{
    public class ScrapeController : ApiController
    {
        const string DOMAIN = "http://www.offleaseonly.com";
        const string URL = DOMAIN + "/used-cars/type_used/page_{0}/";
        public List<Car> Get(int count = -1)
        {
            var cars = new List<Car>();
            var web = new HtmlWeb();
            List<string> makes = null;

            int max = 5;
            for (int i = 1; i < max; i++)
            {
                var doc = web.Load(string.Format(URL, i));
                if (i == 1)
                {
                    makes = doc.DocumentNode.ChildNode("div", "search-by-make").GetOptionValues(1);
                    string maxpage = doc.DocumentNode.ChildNode("div", "current-page").InnerText;
                    max = Int32.Parse(maxpage.Split(';').Last()) + 1;
                }
                var vehicles = doc.DocumentNode.ChildNodes("div", "vehicle-listing");
                foreach (var veh in vehicles)
                {
                    try
                    {
                        var car = GetCar(veh, makes);
                        cars.Add(car);
                        count--;
                        if (count == 0) break;
                    }
                    catch { }
                }
                if (count == 0) break;
            }

            SaveCars(cars);
            return cars;
        }

        private void SaveCars(List<Car> cars)
        {
            string filePathJson = BaseDir + @"\cars{0}.json";
            string filePathCsv = BaseDir + @"\cars.csv";

            using (var file = File.CreateText(filePathCsv))
            {
                file.WriteLine(Car.CsvHeaders());
                foreach (var car in cars)
                {
                    file.WriteLine(car.ToString());
                }
            }

            int max = 2;
            int delta = (cars.Count / max) + 1;
            for (int i = 0; i < max; i++)
            {
                using (var file = File.CreateText(String.Format(filePathJson, i)))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, cars.Skip(i*delta).Take(delta));
                }
            }
        }

        private string BaseDir { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        private Car GetCar(HtmlNode vehNode, List<string> makes)
        {
            var car = new Car();
            var firObj = vehNode.ChildNode("div", "first-half");
            var secObj = vehNode.ChildNode("div", "second-half");
            if (firObj != null && secObj != null)
            {
                var vinContainer = secObj.ChildNode("div", "container", 2);
                if (vinContainer != null)
                {
                    car.vin = vinContainer.ChildNode("span", "spec-data")?.InnerText;
                    var title = vehNode.ChildNode("div", "vehicle-title-wrap")?.ChildNode("h6")?.InnerText;
                    car.price = Int32.Parse(vehNode.ChildNode("span", "pricing-ourprice", 2)?.InnerText?.Replace(",", "")?.Replace("$", ""));

                    car.trans = firObj.ChildNode("div", "container", 0)?.ChildNode("span", "spec-data")?.InnerText;
                    car.mileage = Int32.Parse(firObj.ChildNode("div", "container", 1)?.ChildNode("span", "spec-data")?.InnerText?.Replace(",",""));
                    car.eng = firObj.ChildNode("div", "container", 2)?.ChildNode("span", "spec-data")?.InnerText;
                    car.color = secObj.ChildNode("div", "container", 0)?.ChildNode("span", "spec-data")?.InnerText;
                    car.stock = secObj.ChildNode("div", "container", 1)?.ChildNode("span", "spec-data")?.InnerText;
                    var vPhoto = vehNode.ChildNode("div", "vehicle-photo");
                    if (vPhoto != null)
                    {
                        car.image = vPhoto.ChildNode("img")?.GetAttributeValue("rel", "");
                        car.link = DOMAIN + vPhoto.ChildNode("a")?.GetAttributeValue("href", "");
                    }

                    var objT = title.Split(' ');
                    car.year = Int32.Parse(objT[0]);
                    string txt = title.Substring(4).Trim();
                    car.make = makes.Where(make => txt.StartsWith(make)).FirstOrDefault();
                    car.model = txt.Replace(car.make, "").Trim();

                    string comments = vehNode.ChildNode("div", "vehicle-comments")?.InnerText;
                    if (!string.IsNullOrEmpty(comments))
                        car.cleanCarFax = comments.ToUpper().Contains("CLEAN CARFAX") ? 1: 0;
                }
            }
            return car;
        }
    }
}
