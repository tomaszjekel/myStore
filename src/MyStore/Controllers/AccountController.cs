using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            var success = await _userService.LoginAsync(viewModel.Email, viewModel.Password);
            if (success)
            {
                var user = await _userService.GetAsync(viewModel.Email);
                await _authenticator.SignInAsync(user.Id, user.Email, user.Role);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Invalid username or password";
                return View();
            }
            
        }

        [HttpGet("reset")]
        public async Task<IActionResult> Reset()
        {
            var viewModel = new Reset();

            return View(viewModel);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset(Reset reset)
            {
            if (!ModelState.IsValid)
            {
                return await Reset();
            }
            await _userService.ResetPassword(reset.Email);
            ViewBag.Message = "Link to reset your account was sent to your e-mial";
            return View(reset);
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
                viewModel.Password, null);

            TempData["message"] = "Account created";

            return RedirectToAction("Index", "Home");
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