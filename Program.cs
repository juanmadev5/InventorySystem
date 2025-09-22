using InventorySystem.src.data;
using InventorySystem.src.ui;
using InventorySystem.src.utils;

class Program
{
    static void Main()
    {
        InventoryLoader.Loader();

        string[] menuItems = SystemUI.menuItems;

        bool exit = false;

        int selectedIndex = 0;

        Console.Title = "📦 Sistema de Gestión de Inventarios";

        while (!exit)
        {
            Console.Clear();
            Screens.MainMenu(menuItems, selectedIndex);

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? menuItems.Length - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % menuItems.Length;
                    break;
                case ConsoleKey.Enter:
                    Console.Clear();
                    switch (selectedIndex)
                    {
                        case 0: Screens.CreateInventory(); break;
                        case 1: Screens.ListInventories(); break;
                        case 2: Screens.AddProduct(); break;
                        case 3: Screens.ListProducts(); break;
                        case 4: Screens.RemoveProduct(); break;
                        case 5: Screens.EditProduct(); break;
                        case 6: Screens.SaveAll(); break;
                        case 7: Screens.LoadAll(); break;
                        case 8: exit = true; break;
                    }

                    if (!exit)
                    {
                        Messages.ShowInfo("\nPresione ENTER para volver al menú...");
                        Console.ReadLine();
                    }
                    break;
            }
        }

        InventorySaver.Saver();
    }
}
