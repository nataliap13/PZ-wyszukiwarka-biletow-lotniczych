using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Wyszukiwarka {
    public class WebsiteDataService {
        public List<Flight> Kayak_go(DaneWyszukiwania data) {
            //try
            {
                var html = KayakUrlBuilder.Bulid(data);
                Console.WriteLine(html);

                HtmlWeb web = new HtmlWeb();
                web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
                var doc = web.Load(html);

                var nodes_carriers = doc.DocumentNode.SelectNodes("//*[@class='section times']//*[@class='bottom']");
                var task_carriers = Task.Run(() => get_list_of_strings_from_HtmlNodeCollection(nodes_carriers));

                var nodes_departure_times = doc.DocumentNode.SelectNodes("//*[@class='depart-time base-time']");
                var task_departure_times = Task.Run(() => get_list_of_strings_from_HtmlNodeCollection(nodes_departure_times));

                var nodes_departure_cities = doc.DocumentNode.SelectNodes("//*[@class='section duration']//*[@class='bottom']/span[1]");
                var task_departure_cities = Task.Run(() => get_list_of_strings_from_HtmlNodeCollection(nodes_departure_cities));

                var nodes_stops = doc.DocumentNode.SelectNodes("//*[@class='section stops']/div[1]");
                var task_stops = Task.Run(() => get_list_of_strings_from_HtmlNodeCollection(nodes_stops));

                var nodes_arrival_times = doc.DocumentNode.SelectNodes("//*[@class='arrival-time base-time']");
                var task_arrival_times = Task.Run(() => get_list_of_strings_from_HtmlNodeCollection(nodes_arrival_times));

                var nodes_arrival_cities = doc.DocumentNode.SelectNodes("//*[@class='section duration']//*[@class='bottom']/span[3]");
                var task_arrival_cities = Task.Run(() => get_list_of_strings_from_HtmlNodeCollection(nodes_arrival_cities));

                var nodes_flight_times = doc.DocumentNode.SelectNodes("//*[@class='section duration']//*[@class='top']");
                var task_flight_times = Task.Run(() => get_list_of_strings_from_HtmlNodeCollection(nodes_flight_times));

                var nodes_prices = doc.DocumentNode.SelectNodes("//*[@class='multibook-dropdown']//*[@class='price option-text']");
                var task_prices = Task.Run(() => get_list_of_strings_from_HtmlNodeCollection(nodes_prices));

                var nodes_links = doc.DocumentNode.SelectNodes("//*[@class='col col-best']/div/div/a");
                var task_links = Task.Run(() => get_list_of_strings_from_HtmlNodeCollection_kayak_url(nodes_links));

                var task_array = new Task<List<string>>[9]
                { task_carriers,
                    task_departure_times, task_departure_cities,
                    task_stops,
                    task_arrival_times, task_arrival_cities,
                    task_flight_times, task_prices,
                    task_links
                };
                Task.WaitAll(task_array);

                var carriers = task_carriers.Result;
                var departure_times = task_departure_times.Result;
                var departure_cities = task_departure_cities.Result;
                var stops = task_stops.Result;
                var arrival_times = task_arrival_times.Result;
                var arrival_cities = task_arrival_cities.Result;
                var flight_times = task_flight_times.Result;
                var prices = task_prices.Result;
                var links = task_links.Result;

                List<Flight> lst = new List<Flight>();
                List<PolaczenieLotnicze> polaczenia = new List<PolaczenieLotnicze>();
                for(int i = 0; i < departure_times.Count; i++) {
                    var flight = new Flight();
                    flight.Carrier = carriers[i];
                    flight.Departure_time = departure_times[i];
                    flight.Departure_city = departure_cities[i];
                    flight.Stops = stops[i];
                    flight.Arrival_time = arrival_times[i];
                    flight.Arrival_city = arrival_cities[i];
                    flight.Czas = flight_times[i];
                    flight.Cena = prices[i];
                    flight.Link = links[i];
                    lst.Add(flight);
                }
                return lst;
            }/*
            catch (Exception e)
            {
                Console.WriteLine("ERROR!");
                Console.WriteLine(e.Message);
                Console.WriteLine("END OF ERROR!");
                return new List<Flight>();
            }*/
        }
        private List<string> get_list_of_strings_from_HtmlNodeCollection(HtmlNodeCollection nodes) {
            try {
                List<string> list_of_strings = new List<string>();
                foreach(var x in nodes) {
                    list_of_strings.Add(x.InnerText);
                }
                return list_of_strings;
            } catch(Exception e) {
                Console.WriteLine("ERROR HERE!");
                Console.WriteLine(e.Message);
                Console.WriteLine("END OF ERROR HERE!");
                return new List<string>();
            }
        }
        private List<string> get_list_of_strings_from_HtmlNodeCollection_kayak_url(HtmlNodeCollection nodes) {
            List<string> list_of_strings = new List<string>();
            foreach(var x in nodes) {
                string value_link = "http://www.kayak.pl" + x.GetAttributeValue("href", string.Empty).ToString();
                list_of_strings.Add(value_link);
            }
            return list_of_strings;
        }
    }
}
