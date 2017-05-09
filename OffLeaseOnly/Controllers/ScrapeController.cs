﻿using HtmlAgilityPack;
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

        public bool Post()
        {
            bool start = true;
            var allfiles = Directory.GetFiles(Cars.BaseDir, Cars.FilesPath);
            foreach (var path in allfiles)
            {
                if (File.GetLastWriteTime(path) > DateTime.Now.AddHours(-2))
                {
                    start = false;
                    break;
                }
            }
            if (start)
                Task.Factory.StartNew(() => Get());
            return start;
        }

        public Stats Get(int count = -1)
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
                    max = int.Parse(maxpage.Split(';').Last()) + 1;
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

            if (cars.Count > 0)
            {
                cars.Save(2);
                AddPrices(cars);
            }
            return cars.Statistics();
        }

        private void AddPrices(List<Car> cars)
        {
            var prices = Prices.Data;
            bool changed = false;
            foreach (var car in cars)
            {
                var p = prices.Where(x => x.vin == car.vin).FirstOrDefault();
                if (p == null)
                {
                    prices.Add(new PriceHistory(car.vin, car.price));
                    changed = true;
                }
                else if (!p.price.ContainsKey(car.price))
                {
                    p.price.Add(car.price, DateTime.Now);
                    changed = true;
                }
            }
            if (changed) prices.Save();
        }

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
                    car.location = vehNode.ChildNode("div", "location")?.ChildNode("span")?.GetAttributeValue("class", "");
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
