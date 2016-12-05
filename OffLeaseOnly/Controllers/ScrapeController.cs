﻿using HtmlAgilityPack;
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
        const string URL = "http://www.offleaseonly.com/used-cars/type_used/page_{0}/";
        public List<Car> Get()
        {
            var cars = new List<Car>();
            var web = new HtmlWeb();
            List<string> makes = null;
            string filePath = BaseDir + @"\cars.json";
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
                    }
                    catch { }
                }
            }

            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, cars);
            }

            return cars;
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
                    car.title = vehNode.ChildNode("div", "vehicle-title-wrap")?.ChildNode("h6")?.InnerText;
                    car.price = Int32.Parse(vehNode.ChildNode("span", "pricing-ourprice", 2)?.InnerText?.Replace(",", "")?.Replace("$", ""));

                    car.trans = firObj.ChildNode("div", "container", 0)?.ChildNode("span", "spec-data")?.InnerText;
                    car.mileage = Int32.Parse(firObj.ChildNode("div", "container", 1)?.ChildNode("span", "spec-data")?.InnerText?.Replace(",",""));
                    car.eng = firObj.ChildNode("div", "container", 2)?.ChildNode("span", "spec-data")?.InnerText;
                    car.color = secObj.ChildNode("div", "container", 0)?.ChildNode("span", "spec-data")?.InnerText;
                    car.stockNum = secObj.ChildNode("div", "container", 1)?.ChildNode("span", "spec-data")?.InnerText;

                    var objT = car.title.Split(' ');
                    car.year = Int32.Parse(objT[0]);
                    string txt = car.title.Substring(4).Trim();
                    car.make = makes.Where(make => txt.StartsWith(make)).FirstOrDefault();
                    car.model = txt.Replace(car.make, "").Trim();

                    car.cleanCarFax = vehNode.ChildNode("div", "vehicle-comments")?.InnerText?.Contains("Clean Carfax");
                }
            }
            return car;
        }
    }
}
