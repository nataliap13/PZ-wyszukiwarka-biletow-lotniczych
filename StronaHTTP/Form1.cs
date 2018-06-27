using System;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Wyszukiwarka;
using System.Linq;

namespace StronaHTTP {

    public partial class Form1 : Form {
        private const string FOLDER_SERWISU = @"C:\Users\tuPłotek\Desktop\Wyszukiwarka\Strona"; //@"C:\Users\Marcin\Documents\Visual Studio 2015\Projects\Samoloty\StronaWPF\";
        private HTTP.Serwer serwer;

        private int pobierzLiczbe(string tekst, int poz) {
            int liczba;
            if(int.TryParse(tekst.Substring(0, poz), out liczba)) return liczba; else return 0;
        }

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Wyszukiwanie.ZakonczProcesy();
            HTTP.UstawieniaSerwera ust = new HTTP.UstawieniaSerwera();
            ust.FolderSerwisu = FOLDER_SERWISU;
            ust.FolderSerwera = ust.FolderSerwisu + "HTTPDane\\";
            ust.ZapiszBledy = false;
            ust.ZapiszOdwiedziny = false;
            ust.Funkcje = new HTTP.IDaneFunkcji[] {
                new HTTP.DaneFunkcji<object>("/Przewoznicy", HTTP.MetodaHTTP.GET, null, Przewoznicy),
                new HTTP.DaneFunkcji<object>("/Polaczenia", HTTP.MetodaHTTP.POST, null, Polaczenia),
                new HTTP.DaneFunkcji<NazwaLotn>("/Lotniska", HTTP.MetodaHTTP.GET, null, NazwyLotnisk)
            };
            ust.FunkcjaZwrotna = (HTTP.Polaczenie pol) => pol.WyslijPlik(true);
            serwer = new HTTP.Serwer(ust);
            serwer.Uruchom();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            serwer.Zatrzymaj();
            Wyszukiwanie.ZakonczProcesy();
        }

        private void Przewoznicy(HTTP.Polaczenie pol, object dane) {
            string[] przew = Wyszukiwarka.Przewoznicy.PobierzPrzewoznikow();

            if(przew == null) {
                pol.Wyslij500_InternalServerError();
            } else {
                pol.Odpowiedz.ContentType = "application/json";
                pol.ZawartoscOdpowiedzi = new UTF8Encoding().GetBytes(JsonConvert.SerializeObject(przew));
                pol.Wyslij200_OK();
            }
        }

        private void Polaczenia(HTTP.Polaczenie pol, object dane) {
            string odp = new UTF8Encoding().GetString(pol.ZawartoscZapytania);
            DaneWyszukiwania danewysz = null;
            try {
                danewysz = JsonConvert.DeserializeObject<DaneWyszukiwania>(odp);
            } catch {
                pol.Wyslij400_BadRequest();
                return;
            }

            WyszukanePolaczenia p = Wyszukiwanie.WyszukajPolaczenia(danewysz);

            if(p == null) {
                pol.Wyslij500_InternalServerError();
            } else {
                pol.Odpowiedz.ContentType = "application/json";
                pol.ZawartoscOdpowiedzi = new UTF8Encoding().GetBytes(JsonConvert.SerializeObject(p));
                pol.Wyslij200_OK();
            }
        }

        private void NazwyLotnisk(HTTP.Polaczenie pol, NazwaLotn dane) {
            if(dane.nazwa == "") {
                pol.Wyslij400_BadRequest();
                return;
            }

            string[] Miasta = (from l in Lotniska.PobierzLotniska(dane.nazwa) select l.Nazwa + " (" + l.Kod + ")").ToArray();

            pol.Odpowiedz.ContentType = "application/json";
            pol.ZawartoscOdpowiedzi = new UTF8Encoding().GetBytes(JsonConvert.SerializeObject(Miasta));
            pol.Wyslij200_OK();
        }

        private class NazwaLotn {
            [HTTP.Serializacja.ParametrGET]
            public string nazwa { get; set; } = "";
        }
    }
}