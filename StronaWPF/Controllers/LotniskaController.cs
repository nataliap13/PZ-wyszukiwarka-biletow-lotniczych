using System.Web.Http;
using System.Linq;

namespace StronaWPF.Controllers {
    public class LotniskaController : ApiController {

        [HttpGet]
        public string[] PobierzLotniska(string nazwa) {
            return (from l in Wyszukiwarka.Lotniska.PobierzLotniska(nazwa) select l.Nazwa + " (" + l.Kod + ")").ToArray();
        }
    }
}