using System.Web.Http;

namespace StronaWPF.Controllers {
    public class PrzewoznicyController : ApiController {
        
        [HttpGet]
        public string[] PobierzPrzewoznikow() {
            return Wyszukiwarka.Przewoznicy.PobierzPrzewoznikow();
        }
    }
}