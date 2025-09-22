using InventorySystem.src.model;
using InventorySystem.src.utils;

namespace InventorySystem.src.data
{
    public class InventorySaver
    {
        // Save automatically on exit
        public static void Saver()
        {
            var saveResult = Inventory.Instance.SaveDefault();
            if (saveResult == Result.Success)
                Messages.ShowInfo($"Inventarios guardados en: {Inventory.GetDefaultFilePath()}");
            else
                Messages.ShowError("Error al guardar inventarios al salir.");
        }
    }
}