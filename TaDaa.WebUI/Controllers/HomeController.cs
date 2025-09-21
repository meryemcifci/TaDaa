using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaDaa.WebUI.Models;

namespace TaDaa.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]  //Sayfan�n �nbelle�e al�nmas�n� engelliyor. Hata olu�an iste�e �zel benzersiz bir kimlik (RequestId) �retiyor. Bu kimlik, hata takip ve log analizinde kolayl�k sa�l�yor.
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
