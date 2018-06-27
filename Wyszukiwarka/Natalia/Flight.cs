using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wyszukiwarka {
    public class Flight {
        private string carrier;
        private string departure_time;
        private string departure_city;
        private string stops;
        private string arrival_time;
        private string arrival_city;
        private string _czas;
        private string _cena;
        private string _link;

        public string Carrier { get => carrier; set => carrier = value; }
        public string Departure_time { get => departure_time; set => departure_time = value; }
        public string Departure_city { get => departure_city; set => departure_city = value; }
        public string Stops { get => stops; set => stops = value; }
        public string Arrival_time { get => arrival_time; set => arrival_time = value; }
        public string Arrival_city { get => arrival_city; set => arrival_city = value; }
        public string Czas { get => _czas; set => _czas = value; }
        public string Cena { get => _cena; set => _cena = value; }
        public string Link { get => _link; set => _link = value; }
    }
}