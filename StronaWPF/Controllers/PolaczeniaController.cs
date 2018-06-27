using System.Web.Http;
using Wyszukiwarka;

namespace StronaWPF.Controllers {
    public class PolaczeniaController : ApiController {

        [HttpPost]
        public WyszukanePolaczenia PrzetworzZapytanie(DaneWyszukiwania Dane) {
            return Wyszukiwanie.WyszukajPolaczenia(Dane);
        }
    }
}