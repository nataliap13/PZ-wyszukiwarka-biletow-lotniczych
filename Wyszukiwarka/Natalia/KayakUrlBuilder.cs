using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wyszukiwarka {
    public class KayakUrlBuilder {
        public static string Bulid(DaneWyszukiwania data) {
            var html = @"https://www.kayak.pl/flights/" + data.Zrodlo + "-" + data.Cel + "/"
                + data.Data.Year + "-";
            if(data.Data.Month < 10) {
                html += "0";
            }
            html += data.Data.Month + "-";

            if(data.Data.Day < 10) {
                html += "0";
            }
            html += data.Data.Day + "/";
            //var html = @"https://www.kayak.pl/flights/WRO-STN/2018-12-13/1adults/children-17-17?sort=price_a&fs=airlines=-AF;stops=0";
            if(data.Osoby.Dorosli > 0) { html += data.Osoby.Dorosli + "adults"; }

            if(data.Osoby.Mlodziez > 0 || data.Osoby.Niemowleta > 0 ||
                data.Osoby.Dzieci > 0) {
                html += "/children";
                if(data.Osoby.Niemowleta > 0) {
                    for(int i = 0; i < data.Osoby.Niemowleta; i++) { html += "-1S"; }
                }
                if(data.Osoby.Dzieci > 0) {
                    for(int i = 0; i < data.Osoby.Dzieci; i++) { html += "-11"; }
                }
                if(data.Osoby.Mlodziez > 0) {
                    for(int i = 0; i < data.Osoby.Mlodziez; i++) { html += "-17"; }
                }
            }
            html += "?sort=price_a";//sortowanie rosnaco po cenie
            bool is_special_features_added = false;
            var shortcuts = Konwerter.KonwertujPrzewoznikowDoSkrotow(data.PrzewNie);
            for(int i = 0; i < shortcuts.Count; i++) {
                if(is_special_features_added == false) {
                    html += "&fs=airlines=-";
                    is_special_features_added = true;
                }
                if(i > 0) { html += ","; }

                html += shortcuts[i];
            }

            if(data.Bezposrednie) {
                if(is_special_features_added) {
                    html += ";";
                } else {
                    html += "&fs=";
                    is_special_features_added = true;
                }

                html += "stops=0";
            }
            return html;
        }
    }
}
