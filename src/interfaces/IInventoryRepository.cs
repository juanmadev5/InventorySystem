using InventorySystem.src.model;

namespace InventorySystem.src.interfaces
{
    public interface IRepository
    {
        Result CreateInventory(string name);

        Result AddProduct(string inventoryName, Product product);

        Result UpdateProduct(string inventoryName, string productCode, Product updatedProduct);

        (Result, IReadOnlyCollection<Product>) GetProducts(string inventoryName);

        Result RemoveProduct(string inventoryName, string productCode);

        IReadOnlyCollection<string> GetInventories();

        Result SaveAllInventories(string filePath);
        Result LoadAllInventories(string filePath);

    }
}
