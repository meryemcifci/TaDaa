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

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]  //Sayfanýn önbelleðe alýnmasýný engelliyor. Hata oluþan isteðe özel benzersiz bir kimlik (RequestId) üretiyor. Bu kimlik, hata takip ve log analizinde kolaylýk saðlýyor.
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
