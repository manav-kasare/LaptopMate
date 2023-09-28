using System.Collections.Generic;
using LaptopMate.Models;
using Microsoft.AspNetCore.Identity; // Import IdentityUser if not already imported


namespace LaptopMate.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; }

        public IdentityUser User { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    public class CartItem
    {
        public int CartItemId { get; set; }

        public int LaptopId { get; set; }

        public int Quantity { get; set; }

        // Navigation property to link a cart item to a laptop
        public Laptop Laptop { get; set; }

        // Reference to the cart that contains this item
        public int CartId { get; set; }

        // Navigation property to link a cart item to its parent cart
        public Cart Cart { get; set; }
    }
}
