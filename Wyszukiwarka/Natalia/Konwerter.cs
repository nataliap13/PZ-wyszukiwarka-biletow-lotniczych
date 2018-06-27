using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyszukiwarka {
    public class Konwerter {
        public static PolaczenieLotnicze KonwertujFlight(Flight flight, DaneWyszukiwania data) {
            PolaczenieLotnicze p = new PolaczenieLotnicze();
            var time = Konwerter.KonwertujStringGodzMinNaTimeSpan(flight.Departure_time);
            p.Cel = flight.Arrival_city;
            p.Cena = Konwerter.KonwertujCeneNaDouble(flight.Cena);
            p.Czas = Konwerter.KonwertujStringCzasLotuNaTimeSpan(flight.Czas);
            p.DataWylotu = new DateTime(data.Data.Year, data.Data.Month, data.Data.Day,
                time.Hours, time.Minutes, time.Seconds);
            p.Link = flight.Link;
            p.Przewoznik = flight.Carrier;
            p.Zrodlo = flight.Departure_city;
            return p;
        }
        public static List<string> KonwertujPrzewoznikowDoSkrotow(string[] przewoznicy) {
            List<string> skroty = new List<string>();

            for(int i = 0; i < przewoznicy.Length; i++) {
                switch(przewoznicy[i].ToLower()) {
                    case "aer lingus": {
                            skroty.Add("EI");
                            break;
                        }
                    case "airbaltic": {
                            skroty.Add("BT");
                            break;
                        }
                    case "air france": {
                            skroty.Add("AF");
                            break;
                        }
                    case "alitalia": {
                            skroty.Add("AZ");
                            break;
                        }
                    case "austrian airlines": {
                            skroty.Add("OS");
                            break;
                        }
                    case "british airways": {
                            skroty.Add("BA");
                            break;
                        }
                    case "czech airlines": {
                            skroty.Add("OK");
                            break;
                        }
                    case "lot": {
                            skroty.Add("LO");
                            break;
                        }
                    case "lufthansa": {
                            skroty.Add("LH");
                            break;
                        }
                    case "norwegian": {
                            skroty.Add("DY");
                            break;
                        }
                    case "ryanair": {
                            skroty.Add("FR");
                            break;
                        }
                    case "wizz air": {
                            skroty.Add("W6");
                            break;
                        }
                }
            }
            return skroty;
        }

        //public static TimeSpan KonwertujStringCzasGodzMinNaDate(string time_hour_and_minutes)
        //{
        //    string time = string.Empty;
        //    foreach (var number in time_hour_and_minutes)
        //    {
        //        if ('0' <= number && number <= '9')
        //        { time += number; }
        //    }
        //    string format = "g";//"h:m"
        //    TimeSpan x = TimeSpan.ParseExact(time_hour_and_minutes, format, null);
        //    return x;
        //}
        private static TimeSpan KonwertujStringGodzMinNaTimeSpan(string hour_and_minutes) {
            string time = string.Empty;
            foreach(var number in hour_and_minutes) {
                if((number == ':') || ('0' <= number && number <= '9')) { time += number; }
            }
            string format = "g";//"h:m"
            TimeSpan x = TimeSpan.ParseExact(hour_and_minutes, format, null);
            return x;
        }
        private static TimeSpan KonwertujStringCzasLotuNaTimeSpan(string czas) {
            string time = string.Empty;
            foreach(var number in czas) {
                if(number == 'h') { time += ':'; } else if('0' <= number && number <= '9') { time += number; }
            }
            string format = "g";//"h:m"
            TimeSpan x = TimeSpan.ParseExact(time, format, null);
            return x;
        }
        private static double KonwertujCeneNaDouble(string cena) {
            string price = string.Empty;
            foreach(var number in cena) {
                if('0' <= number && number <= '9') { price += number; }
            }
            return Convert.ToDouble(price);
        }
    }
}
