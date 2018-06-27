using System.Linq;
using System.IO;

namespace Wyszukiwarka {
    public static class Przewoznicy {
        private const string PLIK_PRZEWOZNICY = @"C:\Users\tuPłotek\Desktop\Wyszukiwarka\Strona\Pliki\przewoznicy.txt"; //@"C:\Users\Marcin\Documents\Visual Studio 2015\Projects\Samoloty\StronaWPF\przewoznicy.txt";

        public static string[] PobierzPrzewoznikow() {
            try {
                return File.ReadAllLines(PLIK_PRZEWOZNICY).OrderBy(s => s).ToArray();
            } catch {
                return null;
            }
        }
    }
}