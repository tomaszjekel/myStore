using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using MyStore.Framework;
using MyStore.Models;
using MyStore.Services;

namespace MyStore.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthenticator _authenticator;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<AccountController> _localizer;


        public AccountController(IAuthenticator authenticator,
            IUserService userService, IStringLocalizer<AccountController> localize) 
        {
            _authenticator = authenticator;
            _userService = userService;
            _localizer = localize;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            //var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            //var culture = rqf.RequestCulture.Culture;
            //System.Console.WriteLine($"Culture: {culture}");
            //Test
            var lang = _localizer["signIn"];
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            await _userService.LoginAsync(viewModel.Email, viewModel.Password);
            var user = await _userService.GetAsync(viewModel.Email);
            await _authenticator.SignInAsync(user.Id, user.Email, user.Role);
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet("register")]
        public IActionResult Register()
        {
            var viewModel = new UserViewModel();
            //viewModel.Roles = new List<SelectListItem>
            //{
            //    new SelectListItem {Value = "user", Text =  _localizer["renter"]},
            //    new SelectListItem {Value = "admin", Text = _localizer["owner"]}
            //};

            return View(viewModel);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
            //    viewModel.Roles = new List<SelectListItem>
            //{
            //    new SelectListItem {Value = "user", Text =  _localizer["renter"]},
            //    new SelectListItem {Value = "admin", Text = _localizer["owner"]}
            //};
                return View(viewModel);
            }

            await _userService.RegisterAsync(viewModel.Email,
                viewModel.Password, null);

            TempData["message"] = "Account created";

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("confirmation")]
        public async Task<IActionResult> Confirmation(string userId, string confirmationId)
        {
            ConfirmationViewModel model = new ConfirmationViewModel();
           model.Message = await _userService.Confirmation(userId,confirmationId);
            //return Ok("User is activate");
            return View(model);
        }

        [HttpGet("logoff")]
        public async Task<IActionResult> LogOff()
        {
            await _authenticator.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}