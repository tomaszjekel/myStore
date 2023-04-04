using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using MyStore.Infrastructure.EF;
using MyStore.Models;
using MyStore.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UnityEngine;

namespace MyStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyStoreContext _context;
        private readonly IProductService _productService;
        public AdminController( MyStoreContext context, IProductService productService) 
        { 
            _context= context;
            _productService= productService;
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

        [Authorize]
        public async Task<IActionResult> Category()
        {

            var categories = _context.Categories.ToList();
            CategoriesViemModel model = new CategoriesViemModel
            {
                Categories= categories
            };

            return View(model);
        }

        [Authorize]
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

            return RedirectToAction("Category", "Admin");
        }

        [Authorize]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {

            var category = _context.Categories.Where(x=>x.Id == id).FirstOrDefault();
            
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Category", "Admin");
        }


        //[HttpGet("editProduct")]
        public async Task<IActionResult> EditProduct(string keyword, int? pageIndex, Guid userId)
        {
            ViewBag.Shop = "Shop";
            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);
            var products = await _productService.BrowseByUserId(keyword, pageIndex, userId, null);


            var viewModels = products.Select(p =>
                new ProductViewModel
                {
                    Id = p.Id,
                    UserId = userGuid,
                    ProductUserId = p.UserId,
                    Name = p.Name,
                    Category = _context.Categories.Where(x=>x.Id == new Guid(p.Category)).Select(x=>x.Name).FirstOrDefault(),
                    Price = p.Price,
                    Description = p.Description,
                    Files = p.Files
                });
            //if (userGuid != Guid.Empty && name !="all")
            //    viewModels = viewModels.Where(c => c.ProductUserId == userGuid);

            ProductNewViewModel newModel = new ProductNewViewModel();
            newModel.Products = viewModels.ToList();
            newModel.HasNextPage = products.HasNextPage;
            newModel.HasPreviousPage = products.HasPreviousPage;
            newModel.PageIndex = products.PageIndex;
            newModel.TotalPages = products.TotalPages;

            return View(newModel);
        }
    }

}
