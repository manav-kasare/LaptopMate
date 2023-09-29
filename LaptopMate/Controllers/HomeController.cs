using LaptopMate.Data;
using LaptopMate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace LaptopMate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var laptops = await _context.Laptops.ToListAsync(); // Get all laptops
            var laptopQuantities = new Dictionary<int, int>();

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userCart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCart != null)
                {
                    foreach (var cartItem in userCart.CartItems)
                    {
                        laptopQuantities[cartItem.LaptopId] = cartItem.Quantity;
                    }
                }
            }

            var viewModel = new LaptopViewModel
            {
                Laptops = laptops,
                LaptopQuantities = laptopQuantities
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Cart()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the user's cart and load related cart items
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Laptop)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int laptopId, int quantity)
        {
            try
            {
                // Get the current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Find the user's cart in the database
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                // Check if the cart doesn't exist for the user, create one
                if (cart == null)
                {
                    cart = new Cart { UserId = userId };
                    _context.Carts.Add(cart);
                }

                // Check if the laptop is already in the cart
                var existingCartItem = cart.CartItems.FirstOrDefault(item => item.LaptopId == laptopId);

                if (existingCartItem != null)
                {
                    // Update the quantity
                    existingCartItem.Quantity += quantity;
                }
                else
                {
                    // Create a new cart item and add it to the cart
                    var cartItem = new CartItem
                    {
                        LaptopId = laptopId,
                        Quantity = quantity
                    };
                    cart.CartItems.Add(cartItem);
                }

                // Save changes to persist the updated cart
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, and return an appropriate view or message
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                // Find the cart item by its ID
                var cartItem = await _context.CartItems.FindAsync(cartItemId);

                if (cartItem != null)
                {
                    // Remove the cart item from the context and save changes
                    _context.CartItems.Remove(cartItem);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, and return an appropriate view or message
                return RedirectToAction("Error");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}