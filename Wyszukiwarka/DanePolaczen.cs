using System;

namespace Wyszukiwarka {
    public class IloscOsob {
        public int Dorosli { get; set; }
        public int Mlodziez { get; set; }
        public int Dzieci { get; set; }
        public int Niemowleta { get; set; }
        public IloscOsob Klonuj() {
            IloscOsob il = new IloscOsob();
            il.Dorosli = Dorosli;
            il.Mlodziez = Mlodziez;
            il.Dzieci = Dzieci;
            il.Niemowleta = Niemowleta;
            return il;
        }
    }

    public class DaneWyszukiwania {
        public string Zrodlo { get; set; }
        public string Cel { get; set; }
        public DateTime Data { get; set; }
        public string[] PrzewTak { get; set; }
        public string[] PrzewNie { get; set; }
        public IloscOsob Osoby { get; set; }
        public bool Bezposrednie { get; set; }

        public DaneWyszukiwania Klonuj() {
            DaneWyszukiwania dw = new DaneWyszukiwania();
            dw.Zrodlo = string.Copy(Zrodlo);
            dw.Cel = string.Copy(Cel);
            dw.Data = Data;

            dw.PrzewTak = new string[PrzewTak.Length];
            for(int i = 0; i < PrzewTak.Length; i++) dw.PrzewTak[i] = string.Copy(PrzewTak[i]);

            dw.PrzewNie = new string[PrzewNie.Length];
            for(int i = 0; i < PrzewNie.Length; i++) dw.PrzewNie[i] = string.Copy(PrzewNie[i]);

            dw.Osoby = Osoby.Klonuj();
            dw.Bezposrednie = Bezposrednie;
            return dw;
        }
    }

    public class Polaczenie {
        public string Zrodlo = "";
        public DateTime DataWylotu = DateTime.MinValue;
        public TimeSpan Czas = TimeSpan.MinValue;
        public string Cel = "";
        public DateTime DataPrzylotu = DateTime.MinValue;
        public string Przewoznik = "";
        public double Cena = 0.0;
        public string Link = "";
    }

    public class PolaczenieLotnicze : Polaczenie {
        public Polaczenie[] Przesiadki = null;
    }

    public class WyszukanePolaczenia {
        public ZasobySerwera Zasoby = new ZasobySerwera();
        public PolaczenieLotnicze[] Polaczenia;
    }

}