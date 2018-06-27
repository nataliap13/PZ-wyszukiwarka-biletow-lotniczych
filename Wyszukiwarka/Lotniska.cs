using System.Linq;
using CsvHelper;
using System.IO;
using System.Collections.Generic;

namespace Wyszukiwarka {
    public class DaneLotniska {
        public string Nazwa { get; set; }
        public string Kod { get; set; }
    }

    public static class Lotniska {
        private class Lotnisko {
            public string ident { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string coordinates { get; set; }
            public string elevation_ft { get; set; }
            public string continent { get; set; }
            public string iso_country { get; set; }
            public string iso_region { get; set; }
            public string municipality { get; set; }
            public string gps_code { get; set; }
            public string iata_code { get; set; }
            public string local_code { get; set; }
        }

        private static DaneLotniska[] lotniska = null;

        private class PorownywaczLotnisk : IEqualityComparer<DaneLotniska> {
            public bool Equals(DaneLotniska x, DaneLotniska y) => x.Kod == y.Kod;
            public int GetHashCode(DaneLotniska obj) => obj.Kod.GetHashCode();
        }

        private static void WczytajLotniska() {
            try {
                StreamReader s = new StreamReader(@"C:\Users\tuPłotek\Desktop\Wyszukiwarka\Strona\Pliki\airport-codes.csv");// @"C:\Users\Marcin\Documents\Visual Studio 2015\Projects\Samoloty\StronaWPF\Pliki\airport-codes.csv");
                CsvReader csv = new CsvReader(s);
                Lotnisko[] lotn = csv.GetRecords<Lotnisko>().ToArray();
                string[] typy = new string[] { "balloonport", "closed", "heliport", "seaplane_base" };
                lotniska = (from l in lotn where !typy.Contains(l.type) && !(l.iata_code == "" && l.local_code == "") select new DaneLotniska { Kod = l.iata_code == "" ? l.local_code : l.iata_code, Nazwa = l.name }).Distinct(new PorownywaczLotnisk()).ToArray();
                s.Close();
            } catch { }
        }

        public static DaneLotniska[] PobierzLotniska(string FragmentNazwy) {
            if(lotniska == null) WczytajLotniska();
            FragmentNazwy = FragmentNazwy.ToLower();
            return (from m in lotniska where m.Nazwa.ToLower().Contains(FragmentNazwy) || m.Kod.ToLower().Contains(FragmentNazwy) select m).Take(10).OrderBy(l => l.Nazwa).ToArray();
        }

        public static string NazwaLotniska(string Kod) {
            if(lotniska == null) WczytajLotniska();
            Kod = Kod.ToLower();
            string nazwa = (from l in lotniska where l.Kod.ToLower() == Kod select l.Nazwa).FirstOrDefault();
            return nazwa == null ? "" : nazwa;
        }

        public static bool CzyIstnieje(string Kod) {
            if(lotniska == null) WczytajLotniska();
            Kod = Kod.ToLower();
            DaneLotniska lotn = (from l in lotniska where l.Kod.ToLower() == Kod select l).FirstOrDefault();
            return lotn != null;
        }
    }
}