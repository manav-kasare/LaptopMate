namespace LaptopMate.Models
{
    public class LaptopViewModel
    {
        public List<Laptop> Laptops { get; set; }
        public Dictionary<int, int> LaptopQuantities { get; set; } // LaptopId -> Quantity
    }

}
