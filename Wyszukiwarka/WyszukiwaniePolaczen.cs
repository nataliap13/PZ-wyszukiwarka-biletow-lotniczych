using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace Wyszukiwarka {

    public class Wyszukiwanie {
        private static Queue<Process> kolejka = new Queue<Process>();
        private static List<Process> wszystkie_proc = new List<Process>();
        private static string[] miesiace = new string[] { "", "Styczeń", "Luty", "Marzec", "Kwiecień", "Maj", "Czerwiec", "Lipiec", "Sierpień", "Wrzesień", "Październik", "Listopad", "Grudzień" };
        private static object sl = new object();

        private static int pobierzLiczbe(string tekst, int poz) {
            int liczba;
            if(int.TryParse(tekst.Substring(0, poz), out liczba)) return liczba; else return 0;
        }

        private static Process UtworzProces() {
            Process p = new Process();
            p.StartInfo.FileName = @"C:\Pyhon\python.exe";  //@"C:\Users\Marcin\AppData\Local\Programs\Python\Python36\python.exe";
            p.StartInfo.Arguments = @"C:\Users\tuPłotek\Desktop\Wyszukiwarka\Python\tt.py";    //@"C:\Users\Marcin\Downloads\a\tt.py";

            //przekierowanie wejścia/wyjścia
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;

            //bez wyświetlania okna
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            wszystkie_proc.Add(p);
            return p;
        }

        public static void ZakonczProcesy() {
            Process[] p;

            lock(sl) {
                p = wszystkie_proc.ToArray();
            }

            for(int i = 0; i < p.Length; i++) {
                try {
                    p[i].Kill();
                } catch { }
            }

            Process[] pr = (from pp in Process.GetProcesses() where pp.ProcessName == "chromedriver" select pp).ToArray();
            for(int i = 0; i < pr.Length; i++) {
                try {
                    pr[i].Kill();
                } catch { }
            }
        }

        private static List<PolaczenieLotnicze> SzukajPiotr(DaneWyszukiwania dane) {
            Process p;

            lock(sl) {
                if(kolejka.Count > 0) p = kolejka.Dequeue(); else p = UtworzProces();
            }

            try {
                dane.Zrodlo = char.ToUpper(dane.Zrodlo[0]) + dane.Zrodlo.Substring(1);
                dane.Cel = char.ToUpper(dane.Cel[0]) + dane.Cel.Substring(1);
            } catch { }

            p.StandardInput.WriteLine(dane.Zrodlo);
            p.StandardInput.WriteLine(dane.Cel);
            p.StandardInput.WriteLine(miesiace[dane.Data.Month]);
            p.StandardInput.WriteLine(dane.Data.Year);
            p.StandardInput.WriteLine(dane.Data.Day);

            p.StandardInput.WriteLine(dane.Osoby.Dorosli);
            p.StandardInput.WriteLine(dane.Osoby.Mlodziez);
            p.StandardInput.WriteLine(dane.Osoby.Dzieci);
            p.StandardInput.WriteLine(dane.Osoby.Niemowleta);


            List<PolaczenieLotnicze> pol = new List<PolaczenieLotnicze>();

            int ilosc = int.Parse(p.StandardOutput.ReadLine());

            for(int i = 0; i < ilosc; i++) {
                string[] czas;
                PolaczenieLotnicze pp = new PolaczenieLotnicze();
                pp.Przewoznik = p.StandardOutput.ReadLine();
                pp.Cena = double.Parse(p.StandardOutput.ReadLine());
                pp.Zrodlo = Lotniska.NazwaLotniska(p.StandardOutput.ReadLine());
                pp.Cel = Lotniska.NazwaLotniska(p.StandardOutput.ReadLine());
                pp.DataWylotu = dane.Data.Add(TimeSpan.Parse(p.StandardOutput.ReadLine()));
                pp.DataPrzylotu = dane.Data.Add(TimeSpan.Parse(p.StandardOutput.ReadLine()));
                czas = p.StandardOutput.ReadLine().Split(' ');
                pp.Link = p.StandardOutput.ReadLine();
                p.StandardOutput.ReadLine();    //kreski

                int poz;
                int godz = 0, min = 0;

                for(int j = 0; j < czas.Length; j++) {
                    poz = czas[j].IndexOf('h');
                    if(poz > 0) {
                        godz = pobierzLiczbe(czas[j], poz);
                    } else {
                        poz = czas[j].IndexOf("min");
                        if(poz > 0) min = pobierzLiczbe(czas[j], poz);
                    }
                }

                pp.Czas = new TimeSpan(godz, min, 0);

                if(pp.Zrodlo != "" && pp.Cel != "") pol.Add(pp);
            }

            lock(sl) {
                kolejka.Enqueue(p);
            }

            return pol;
        }

        private static List<PolaczenieLotnicze> SzukajNatalia(DaneWyszukiwania dane) {
            var HAPparser = new WebsiteDataService();

            List<PolaczenieLotnicze> pol = new List<PolaczenieLotnicze>();
            List<Flight> lista;

            try {
                lista = HAPparser.Kayak_go(dane);
            } catch(Exception) {
                return pol;
            }

            var en = lista.GetEnumerator();
            while(en.MoveNext()) {
                try {
                    PolaczenieLotnicze pp = Konwerter.KonwertujFlight(en.Current, dane);
                    pp.Zrodlo = Lotniska.NazwaLotniska(pp.Zrodlo);
                    pp.Cel = Lotniska.NazwaLotniska(pp.Cel);
                    pol.Add(pp);
                } catch { }
            }

            return pol;
        }

        private static List<PolaczenieLotnicze> Szukaj2(DaneWyszukiwania dane) {
            List<List<PolaczenieLotnicze>> pp = new List<List<PolaczenieLotnicze>>();
            int max = -1;
            int ix = -1;

            for(int i = 0; i < 5; i++) {
                var a = SzukajNatalia(dane.Klonuj());
                pp.Add(a);
            }

            for(int i = 0; i < pp.Count; i++) {
                if(pp[i].Count > max) {
                    max = pp[i].Count;
                    ix = i;
                }
            }

            if(ix != -1) {
                return pp[ix];
            }

            return null;
        }

        public static WyszukanePolaczenia WyszukajPolaczenia(DaneWyszukiwania dane) {
            if(dane == null) return null;
            if(!Lotniska.CzyIstnieje(dane.Zrodlo) || !Lotniska.CzyIstnieje(dane.Cel)) return null;
            if(dane.Zrodlo == null || dane.Cel == null || dane.Data <= DateTime.Now || dane.PrzewTak == null || dane.PrzewNie == null || dane.Osoby == null) return null;
            if(dane.PrzewTak.Length == 0) return null;

            if(dane.Osoby.Dorosli <= 0 || dane.Osoby.Mlodziez < 0 || dane.Osoby.Dzieci < 0 || dane.Osoby.Niemowleta < 0) return null;
            int niepelnoletni = dane.Osoby.Dzieci + dane.Osoby.Mlodziez + dane.Osoby.Niemowleta;
            if(niepelnoletni + dane.Osoby.Dorosli > 9) return null;
            if(dane.Osoby.Dorosli < niepelnoletni) return null;

            List<PolaczenieLotnicze> pol = new List<PolaczenieLotnicze>();


            var Piotr = Task.Run(() => SzukajPiotr(dane.Klonuj()));
            var Natalia = Task.Run(() => Szukaj2(dane.Klonuj()));

            Task.WaitAll(Piotr, Natalia);

            pol.AddRange(Piotr.Result);
            if(Natalia.Result != null) pol.AddRange(Natalia.Result);

            if(dane.PrzewTak.Contains("inni")) {
                dane.PrzewNie = (from d in dane.PrzewNie select d.ToLower()).ToArray();
                pol = pol.Where(p => !dane.PrzewNie.Contains(p.Przewoznik.ToLower())).ToList();
            } else {
                dane.PrzewTak = (from d in dane.PrzewTak select d.ToLower()).ToArray();
                pol = pol.Where(p => dane.PrzewTak.Contains(p.Przewoznik.ToLower())).ToList();
            }

            WyszukanePolaczenia wysz = new WyszukanePolaczenia();
            wysz.Polaczenia = pol.OrderBy(l => l.DataWylotu).ToArray();
            wysz.Zasoby = Wydajnosc.PobierzZasoby();
            return wysz;
        }
    }
}