using InventorySystem.src.model;
using InventorySystem.src.utils;

namespace InventorySystem.src.data
{
    public class InventoryLoader
    {
        // Load inventories automatically on start
        public static void Loader()
        {
            var loadResult = Inventory.Instance.LoadDefault();
            if (loadResult == Result.Success)
                Messages.ShowInfo($"Inventarios cargados desde: {Inventory.GetDefaultFilePath()}");
            else
                Messages.ShowInfo("No se encontraron inventarios previos o error al cargar.");
        }
    }
}