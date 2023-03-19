using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using MyStore.Infrastructure.EF;
using MyStore.Models;
using MyStore.Services;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace MyStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyStoreContext _context;
        public AdminController( MyStoreContext context) 
        { 
            _context= context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Orders(int pageIndex)
        {
            AdminOrdersViewModel adminOrdersViewModel = null;
         if(pageIndex == 0) 
            { 
                pageIndex= 1;
            }  
                var orders = _context.Orders.Include(x => x.Address).Include(x => x.Items);
                var orders1 = await PaginatedList<Order>.CreateAsync(orders, pageIndex, 10);

                decimal count = _context.Orders.Count() / 10;
                var countM = _context.Orders.Count() % 10;
                if (countM > 0)
                {
                    count++;
                }

                adminOrdersViewModel = new AdminOrdersViewModel
                {
                    orders = orders1.ToList(),
                    count = Convert.ToInt32(count),
                    pageIndex = pageIndex
                };
            

            return View(adminOrdersViewModel);
        }

        public async Task<IActionResult> Order(Guid order)
        {

            var orderMain = _context.Orders.Where(x => x.Id == order).FirstOrDefault();
            AdminOrderViewModelcs model = new AdminOrderViewModelcs
            {
                order = orderMain
            };

            return View(model);
        }

        public async Task<IActionResult> Category()
        {

            var categories = _context.Categories.ToList();
            CategoriesViemModel model = new CategoriesViemModel
            {
                Categories= categories
            };

            return View(model);
        }
        
        [HttpPost("RegisterCategory")]
         public async Task<IActionResult> RegisterCategory(CategoriesViemModel addModel)
        {
            Category cat = new Category { Name= addModel.Name };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            var categories = _context.Categories.ToList();
            CategoriesViemModel model = new CategoriesViemModel
            {
                Categories = categories
            };

            return View(model);
        }
    }
}
