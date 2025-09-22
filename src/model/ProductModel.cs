namespace InventorySystem.src.model
{
    public class Product(string code, string name, string category, int quantity, decimal price)
    {
        public string Code { get; set; } = code;
        public string Name { get; set; } = name;
        public string Category { get; set; } = category;
        public int Quantity { get; set; } = quantity;
        public decimal Price { get; set; } = price;

        public override string ToString()
        {
            return $"[{Code}] {Name} | Cat: {Category} | Cant: {Quantity} | Precio: {Price:C}";
        }
    }
}
