using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MyStore.Framework;
using MyStore.Models;
using MyStore.Services;

namespace MyStore.Controllers
{
    public class AccountController : BaseController
    {
        private IConfiguration _configuration;
        private readonly IAuthenticator _authenticator;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<AccountController> _localizer;
        private string SecretKeyHtml = Environment.GetEnvironmentVariable("SecretKeyHtml");
        private string SecretKey = Environment.GetEnvironmentVariable("SecretKey");

        public AccountController(IConfiguration iconfig,IAuthenticator authenticator,
            IUserService userService, IStringLocalizer<AccountController> localize) 
        {
            _configuration = iconfig;
            _authenticator = authenticator;
            _userService = userService;
            _localizer = localize;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var secret1 = _configuration.GetValue<string>("appSettings:secretKey");
            //var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            //var culture = rqf.RequestCulture.Culture;
            //System.Console.WriteLine($"Culture: {culture}");
            //Test
           
            ViewBag.SecretKeyHtml = SecretKeyHtml;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SecretKeyHtml = SecretKeyHtml;
                return View(viewModel);
            }
            var request = Request.Form["g-recaptcha-response"];
            if (!ReCaptchaPassed(request, SecretKey))
            {
                ModelState.AddModelError(string.Empty, "You failed the CAPTCHA, stupid robot. Go play some 1x1 on SFs instead.");
                ViewBag.SecretKeyHtml = SecretKeyHtml;
                return View(viewModel);
            }

            var success = await _userService.LoginAsync(viewModel.Email, viewModel.Password);
            if (success)
            {
                var user = await _userService.GetAsync(viewModel.Email);
                await _authenticator.SignInAsync(user.Id, user.Email, user.Role);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.SecretKeyHtml = SecretKeyHtml;
                ViewBag.Message = "Invalid username or password";
                return View();
            }
            
        }

        [HttpGet("reset")]
        public async Task<IActionResult> Reset()
        {
            var viewModel = new Reset();
            ViewBag.SecretKeyHtml = SecretKeyHtml;
            return View(viewModel);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset(Reset reset)
            {
            if (!ModelState.IsValid)
            {
                ViewBag.SecretKeyHtml = SecretKeyHtml;
                return await Reset();
            }

            var request = Request.Form["g-recaptcha-response"];
            if (!ReCaptchaPassed(request, SecretKey))
            {
                ViewBag.SecretKeyHtml = SecretKeyHtml;
                ModelState.AddModelError(string.Empty, "You failed the CAPTCHA, stupid robot. Go play some 1x1 on SFs instead.");
                return View(reset);
            }

            await _userService.ResetPassword(reset.Email);
            ViewBag.SecretKeyHtml = SecretKeyHtml;
            ViewBag.Message = "Link to reset your account was sent to your e-mial";
            return View(reset);
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            var viewModel = new UserViewModel();
            ViewBag.SecretKeyHtml = SecretKeyHtml;
            return View(viewModel);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SecretKeyHtml = SecretKeyHtml;
                return View(viewModel);
            }

            var request = Request.Form["g-recaptcha-response"];
            if (!ReCaptchaPassed(request, SecretKey))
            {
                ViewBag.SecretKeyHtml = SecretKeyHtml;
                ModelState.AddModelError(string.Empty, "You failed the CAPTCHA, stupid robot. Go play some 1x1 on SFs instead.");
                return View(viewModel);
            }

            await _userService.RegisterAsync(viewModel.Email,
                viewModel.Password, null);

            TempData["message"] = "Account created";
            ViewBag.SecretKeyHtml = SecretKeyHtml;
            ViewBag.Message = "Account created";

            return View(viewModel);
            //return RedirectToAction("Index", "Home");
        }

        [HttpGet("confirmation")]
        public async Task<IActionResult> Confirmation(string userId, string confirmationId)
        {
            ConfirmationViewModel model = new ConfirmationViewModel();
            model.Message = await _userService.Confirmation(userId,confirmationId);
            return View(model);
        }

        [HttpGet("reset_password")]
        public async Task<IActionResult> ResetPassword(string guid)
        {
            ViewBag.Guid = guid;
            return View();
        }

        [HttpPost("reset_password")]
        public async Task<IActionResult> ResetPassword(PasswordResetViewModel model, string guid)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(guid))
            {
                return await ResetPassword(guid);
            }
            var success = await _userService.RegisterNewPassword(model.Password, guid);
            if (success)
            {
                ViewBag.Message = "Password changed";
            }else
            {
                ViewBag.Message = "Password Changed Error";
            }
            return await ResetPassword(guid);
        }

        [HttpGet("logoff")]
        public async Task<IActionResult> LogOff()
        {
            await _authenticator.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}