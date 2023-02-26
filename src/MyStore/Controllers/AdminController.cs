using Microsoft.AspNetCore.Mvc;

namespace MyStore.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
