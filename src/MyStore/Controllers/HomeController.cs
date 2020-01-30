using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyStore.Framework;
using MyStore.Models;

namespace MyStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppOptions _appOptions;
        private readonly IMemoryCache _cache;

        public HomeController(AppOptions appOptions, IMemoryCache cache)
        {
            _appOptions = appOptions;
            _cache = cache;
        }

        public IActionResult Index()
        {
            var message = TempData["message"];

            //return View();
            return RedirectToAction("browse", "Products");
        }

        public IActionResult About()
        {
            var message = _cache.Get<string>("message");
            if (string.IsNullOrWhiteSpace(message))
            {
                message = $"Welcome to {_appOptions.StoreName}.";
                _cache.Set("message", message);
            }
            ViewData["Message"] = message;

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpGet("date")]
        [ResponseCache(Duration = 10, 
            VaryByHeader = "x-custom", 
            VaryByQueryKeys = new []{"q1"})]
        public IActionResult GetDate()
            => Content(DateTime.Now.ToString(CultureInfo.InvariantCulture));

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
