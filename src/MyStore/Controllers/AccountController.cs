using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyStore.Framework;
using MyStore.Models;
using MyStore.Services;

namespace MyStore.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthenticator _authenticator;
        private readonly IUserService _userService;

        public AccountController(IAuthenticator authenticator,
            IUserService userService)
        {
            _authenticator = authenticator;
            _userService = userService;
        }
        
        [HttpGet("login")]
        public IActionResult Login()
            => View();
        
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

            return View(viewModel);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            await _userService.RegisterAsync(viewModel.Email,
                viewModel.Password, viewModel.Role);
            TempData["message"] = "Account created";

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("logoff")]
        public async Task<IActionResult> LogOff()
        {
            await _authenticator.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}