using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyszukiwarka {
    public class KonwerterPrzewoznikow {
        public static List<string> KonwertujDoSkrotow(string[] przewoznicy) {
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
    }
}