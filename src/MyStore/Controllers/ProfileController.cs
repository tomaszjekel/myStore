using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyStore.Domain;
using MyStore.Services;

namespace MyStore.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileServices _profileServices ;
        public ProfileController(IProfileServices profileServices)
        {
            _profileServices = profileServices;
        }

        public IActionResult Index()
        {
            
            var product = new Product();
            var lista = new List<Product>();
            lista.Add(product);
            var profile = new Profile(new Guid() ,new Guid(),lista ) ;
           // var save = _profileServices.CreateAsync(profile);

            //var viewModels = products.Select(p =>
            //   new ProductViewModel
            //   {
            //       Id = p.Id,
            //       Name = p.Name,
            //       Category = p.Category,
            //       Price = p.Price
            //   });
            return View(profile);
        }
    }
}