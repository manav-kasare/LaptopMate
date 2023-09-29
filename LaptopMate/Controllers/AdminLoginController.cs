using LaptopMate.Data;
using LaptopMate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LaptopMate.Controllers
{
    public class AdminLoginController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AdminLoginModel> _logger;

        public AdminLoginController(
          UserManager<IdentityUser> userManager,
          SignInManager<IdentityUser> signInManager,
          ILogger<AdminLoginModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var adminLogin = new AdminLoginModel();
            return View(adminLogin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AdminLoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to sign in the admin user
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Admin login successful, redirect to the admin dashboard or wherever needed
                    return RedirectToPage("/Admin");
                }
                else
                {
                    // Login failed, add an error message to display to the user
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we reach here, the model is not valid or login failed, return the view with errors
            return View(model);
        }
    }
}
