using InventorySystem.src.data;
using InventorySystem.src.model;
using InventorySystem.src.utils;

namespace InventorySystem.src.ui
{
    public static class Screens
    {
        public static void MainMenu(string[] items, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("   SISTEMA DE GESTIÓN DE INVENTARIO");
            Console.WriteLine("═══════════════════════════════════════");
            Console.ResetColor();

            for (int i = 0; i < items.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine($"> {items[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {items[i]}");
                }
            }
        }

        public static void CreateInventory()
        {
            Console.Write("Nombre del inventario: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(name))
            {
                Messages.ShowError("Nombre inválido.");
                return;
            }

            var result = Inventory.Instance.CreateInventory(name);
            if (result == Result.Success)
                Messages.ShowSuccess($"Inventario '{name}' creado.");
            else
                Messages.ShowError("Ese inventario ya existe.");
        }

        public static void ListInventories()
        {
            var inventories = Inventory.Instance.GetInventories();
            if (inventories.Count == 0)
            {
                Messages.ShowInfo("No hay inventarios creados.");
                return;
            }

            Messages.ShowInfo("Inventarios existentes:");
            foreach (var inv in inventories)
                Console.WriteLine($"- {inv}");
        }

        public static void AddProduct()
        {
            var inventories = Inventory.Instance.GetInventories();
            if (inventories.Count == 0)
            {
                Messages.ShowError("No hay inventarios creados.");
                return;
            }

            string inventoryName = SelectInventory(inventories);
            if (string.IsNullOrEmpty(inventoryName)) return;

            Console.Write("Código del producto (alfanumérico): ");
            string code = Console.ReadLine()?.Trim() ?? "";
            if (!System.Text.RegularExpressions.Regex.IsMatch(code, "^[a-zA-Z0-9]+$"))
            {
                Messages.ShowError("Código inválido.");
                return;
            }

            Console.Write("Nombre del producto: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Categoría: ");
            string category = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Cantidad: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 0)
            {
                Messages.ShowError("Cantidad inválida.");
                return;
            }

            Console.Write("Precio unitario: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                Messages.ShowError("Precio inválido.");
                return;
            }

            var product = new Product(code.ToUpper(), name, category, quantity, price);
            var result = Inventory.Instance.AddProduct(inventoryName, product);

            if (result == Result.Success)
                Messages.ShowSuccess("Producto agregado.");
            else if (result == Result.ProductAlreadyExists)
                Messages.ShowError("El producto ya existe en este inventario.");
            else
                Messages.ShowError("Inventario no encontrado.");
        }

        public static void ListProducts()
        {
            var inventories = Inventory.Instance.GetInventories();
            if (inventories.Count == 0)
            {
                Messages.ShowError("No hay inventarios.");
                return;
            }

            string inventoryName = SelectInventory(inventories);
            if (string.IsNullOrEmpty(inventoryName)) return;

            var (result, products) = Inventory.Instance.GetProducts(inventoryName);

            if (result == Result.InventoryNotFound)
            {
                Messages.ShowError("Inventario no encontrado.");
                return;
            }

            if (products.Count == 0)
            {
                Messages.ShowInfo("No hay productos en este inventario.");
                return;
            }

            Messages.ShowInfo($"\nProductos en '{inventoryName}':");
            ShowProductsWithIndices(products);
        }

        public static void RemoveProduct()
        {
            var inventories = Inventory.Instance.GetInventories();
            if (inventories.Count == 0)
            {
                Messages.ShowError("No hay inventarios.");
                return;
            }

            string inventoryName = SelectInventory(inventories);
            if (string.IsNullOrEmpty(inventoryName)) return;

            var (result, products) = Inventory.Instance.GetProducts(inventoryName);
            if (result != Result.Success || products.Count == 0)
            {
                Messages.ShowInfo("No hay productos en este inventario.");
                return;
            }

            Messages.ShowInfo($"Seleccione un producto para eliminar de '{inventoryName}':");
            int index = SelectProduct(products);
            if (index == -1) return;

            var product = products.ElementAt(index);

            Console.Write($"¿Seguro que desea eliminar '{product.Code} - {product.Name}'? (s/n): ");
            if (Console.ReadLine()?.Trim().ToLower() != "s")
            {
                Messages.ShowInfo("Operación cancelada.");
                return;
            }

            var res = Inventory.Instance.RemoveProduct(inventoryName, product.Code);
            if (res == Result.Success)
                Messages.ShowSuccess("Producto eliminado.");
            else
                Messages.ShowError("Error al eliminar producto.");
        }

        public static void EditProduct()
        {
            var inventories = Inventory.Instance.GetInventories();
            if (inventories.Count == 0)
            {
                Messages.ShowError("No hay inventarios.");
                return;
            }

            string inventoryName = SelectInventory(inventories);
            if (string.IsNullOrEmpty(inventoryName)) return;

            var (result, products) = Inventory.Instance.GetProducts(inventoryName);
            if (result == Result.InventoryNotFound)
            {
                Messages.ShowError("Inventario no encontrado.");
                return;
            }

            if (products.Count == 0)
            {
                Messages.ShowInfo("No hay productos en este inventario.");
                return;
            }

            Messages.ShowInfo($"Seleccione un producto para editar en '{inventoryName}':");
            int index = SelectProduct(products);
            if (index == -1) return;

            var product = products.ElementAt(index);

            Messages.ShowInfo($"Editando producto actual: {product}");

            Console.Write("Nuevo nombre (ENTER para no cambiar): ");
            var newName = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(newName))
                product.Name = newName;

            Console.Write("Nueva categoría (ENTER para no cambiar): ");
            var newCategory = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(newCategory))
                product.Category = newCategory;

            Console.Write("Nueva cantidad (ENTER para no cambiar): ");
            var quantityInput = Console.ReadLine();
            if (int.TryParse(quantityInput, out int newQuantity) && newQuantity >= 0)
                product.Quantity = newQuantity;

            Console.Write("Nuevo precio (ENTER para no cambiar): ");
            var priceInput = Console.ReadLine();
            if (decimal.TryParse(priceInput, out decimal newPrice) && newPrice >= 0)
                product.Price = newPrice;

            var res = Inventory.Instance.UpdateProduct(inventoryName, product.Code, product);
            if (res == Result.Success)
                Messages.ShowSuccess("Producto actualizado correctamente.");
            else
                Messages.ShowError("No se pudo actualizar el producto.");
        }

        public static void SaveAll()
        {
            var result = Inventory.Instance.SaveDefault();
            if (result == Result.Success)
                Messages.ShowSuccess($"Inventarios guardados en: {Inventory.GetDefaultFilePath()}");
            else
                Messages.ShowError("Error al guardar.");
        }

        public static void LoadAll()
        {
            var result = Inventory.Instance.LoadDefault();
            if (result == Result.Success)
                Messages.ShowSuccess($"Inventarios cargados desde: {Inventory.GetDefaultFilePath()}");
            else
                Messages.ShowError("Error al cargar.");
        }

        public static string SelectInventory(IReadOnlyCollection<string> inventories)
        {
            if (inventories.Count == 1)
                return inventories.First();

            Messages.ShowInfo("Seleccione un inventario:");
            int i = 1;
            foreach (var inv in inventories)
                Console.WriteLine($"{i++}. {inv}");

            Console.Write("Opción: ");
            if (int.TryParse(Console.ReadLine(), out int option) && option > 0 && option <= inventories.Count)
                return inventories.ElementAt(option - 1);

            Messages.ShowError("Selección inválida.");
            return "";
        }

        public static int SelectProduct(IReadOnlyCollection<Product> products)
        {
            ShowProductsWithIndices(products);

            Console.Write("Opción: ");
            if (int.TryParse(Console.ReadLine(), out int option) && option > 0 && option <= products.Count)
                return option - 1;

            Messages.ShowError("Selección inválida.");
            return -1;
        }

        public static void ShowProductsWithIndices(IReadOnlyCollection<Product> products)
        {
            int i = 1;
            foreach (var p in products)
                Console.WriteLine($"{i++}. {p.Code} - {p.Name} ({p.Category}) Cant: {p.Quantity}, Precio: {p.Price:C}");
        }
    }
}