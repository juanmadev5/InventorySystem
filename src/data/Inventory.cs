using System.Collections.Concurrent;
using System.Text.Json;
using InventorySystem.src.interfaces;
using InventorySystem.src.model;

namespace InventorySystem.src.data
{
    public class Inventory : IRepository
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Product>> inventories = new();
        private static readonly Inventory instance = new();

        // JsonSerializerOptions cacheado para rendimiento
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true
        };

        // Ruta predefinida para guardar el JSON
        private static readonly string defaultFilePath;

        static Inventory()
        {
            // Asegurarse de que AppData existe
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrWhiteSpace(appData))
                appData = Directory.GetCurrentDirectory();

            string folderPath = Path.Combine(appData, "InventorySystem");
            Directory.CreateDirectory(folderPath);

            defaultFilePath = Path.Combine(folderPath, "inventories.json");
        }

        private Inventory() { }

        public static Inventory Instance => instance;

        // ---------------------- CRUD ----------------------
        public Result CreateInventory(string name)
        {
            var key = name.ToLower();
            return inventories.TryAdd(key, new ConcurrentDictionary<string, Product>())
                ? Result.Success
                : Result.InventoryAlreadyExists;
        }

        public Result AddProduct(string inventoryName, Product product)
        {
            var key = inventoryName.ToLower();
            if (!inventories.TryGetValue(key, out var products))
                return Result.InventoryNotFound;

            return products.TryAdd(product.Code.ToUpper(), product)
                ? Result.Success
                : Result.ProductAlreadyExists;
        }

        public Result UpdateProduct(string inventoryName, string productCode, Product updatedProduct)
        {
            var key = inventoryName.ToLower();
            if (!inventories.TryGetValue(key, out var products))
                return Result.InventoryNotFound;

            var code = productCode.ToUpper();
            if (!products.ContainsKey(code))
                return Result.ProductNotFound;

            products[code] = updatedProduct;
            return Result.Success;
        }

        public (Result, IReadOnlyCollection<Product>) GetProducts(string inventoryName)
        {
            var key = inventoryName.ToLower();
            if (!inventories.TryGetValue(key, out var products))
                return (Result.InventoryNotFound, Array.Empty<Product>());

            return (Result.Success, products.Values.ToList().AsReadOnly());
        }

        public Result RemoveProduct(string inventoryName, string productCode)
        {
            var key = inventoryName.ToLower();
            if (!inventories.TryGetValue(key, out var products))
                return Result.InventoryNotFound;

            return products.TryRemove(productCode.ToUpper(), out _)
                ? Result.Success
                : Result.ProductNotFound;
        }

        public IReadOnlyCollection<string> GetInventories()
        {
            return inventories.Keys.ToList().AsReadOnly();
        }

        // ---------------------- PERSISTENCIA ----------------------
        public Result SaveAllInventories(string filePath)
        {
            try
            {
                var dictToSave = inventories.ToDictionary(
                    inv => inv.Key,
                    inv => inv.Value.Values.ToList()
                );

                string json = JsonSerializer.Serialize(dictToSave, jsonOptions);
                File.WriteAllText(filePath, json);

                return Result.Success;
            }
            catch
            {
                return Result.UnknownError;
            }
        }

        public Result LoadAllInventories(string filePath)
        {
            if (!File.Exists(filePath))
                return Result.InventoryNotFound;

            try
            {
                string json = File.ReadAllText(filePath);

                var dictFromFile = JsonSerializer.Deserialize<Dictionary<string, List<Product>>>(json, jsonOptions);
                if (dictFromFile == null || dictFromFile.Count == 0)
                    return Result.ProductNotFound;

                foreach (var inv in dictFromFile)
                {
                    var dictProducts = new ConcurrentDictionary<string, Product>(
                        inv.Value.ToDictionary(p => p.Code.ToUpper(), p => p)
                    );

                    inventories.AddOrUpdate(inv.Key.ToLower(), dictProducts, (k, old) =>
                    {
                        foreach (var p in dictProducts)
                            old[p.Key] = p.Value; // merge/update
                        return old;
                    });
                }

                return Result.Success;
            }
            catch
            {
                return Result.UnknownError;
            }
        }

        // ---------------------- MÉTODOS CON RUTA PREDEFINIDA ----------------------
        public Result SaveDefault() => SaveAllInventories(defaultFilePath);
        public Result LoadDefault() => LoadAllInventories(defaultFilePath);
        public static string GetDefaultFilePath() => defaultFilePath;
    }
}
