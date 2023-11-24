using System.Collections;

namespace MenuSystem;

public class Menu
{
    public string? Title { get; set; }
    public Dictionary<string, MenuItem> MenuItems { get; set; } = new();

    private const string MenuSeparator = "=======================";
    private static readonly HashSet<string> ReservedShortcuts = new() {"x", "b", "r"};

    public Menu(string? title, List<MenuItem> menuItems)
    {
        Title = title;
        foreach (var menuItem in menuItems)
        {
            if (ReservedShortcuts.Contains(menuItem.Shortcut.ToLower()))
            {
                throw new ApplicationException($"Menu shortcur'{menuItem.Shortcut.ToLower()}' is not reserved.");
            }


            MenuItems[menuItem.Shortcut.ToLower()] = menuItem;
        }
    }


    private void Draw(EMenuLevel menuLevel)
    {
        Console.WriteLine();
    }
}