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
                throw new ApplicationException($"Menu shortcut'{menuItem.Shortcut.ToLower()}' is reserved.");
            } 
            
            if (MenuItems.ContainsKey(menuItem.Shortcut.ToLower()))
            {
                throw new ApplicationException($"Menu shortcut'{menuItem.Shortcut.ToLower()}' is reserved.");
            }
            
            MenuItems[menuItem.Shortcut.ToLower()] = menuItem;
        }
    }


    private void Draw(EMenuLevel menuLevel)
    {
        Console.WriteLine(Title);
        Console.WriteLine(MenuSeparator);
        foreach (var menuItem in MenuItems)
        {
            Console.Write(menuItem.Key);
            Console.Write(") ");
            Console.WriteLine(menuItem.Value.MenuLabel);
        }

        if (menuLevel != EMenuLevel.First)
        {
            Console.WriteLine("b) Back");
        }

        if (menuLevel == EMenuLevel.Other)
        {
            Console.WriteLine("r) Return to main");
        }
        
        Console.WriteLine("e) Exit program");


        Console.WriteLine(MenuSeparator);
        Console.Write("You choice: ");
    }

    public string? Run(EMenuLevel menuLevel = EMenuLevel.First)
    {
        Console.Clear();
        var userChoice = "";

        do
        {
            Draw(menuLevel);
            userChoice = Console.ReadLine()?.ToLower().Trim();
            if (userChoice == null)
            {
                continue;
            }

            if (MenuItems.ContainsKey(userChoice))
            {
                //METHODS, SUBMENU
                if (MenuItems[userChoice].MethodToRun != null)
                {
                    userChoice =  MenuItems[userChoice].MethodToRun!();
                    if (userChoice != "e")
                    {
                        continue;
                    }
                }
                if (MenuItems[userChoice].Action != null)
                {
                    MenuItems[userChoice].Action!();
                    Console.Clear();
                    continue;
                }
            }
            //NAVIGATION
           if (userChoice == "e")
            {
                return "e";
            }
            if (userChoice == "b")
            {
                return null;
            }
            else if (!ReservedShortcuts.Contains(userChoice) && userChoice is not null)
            {
                Console.WriteLine("Undefined Shortcut");
            }
            
            Console.WriteLine();
        } while (!ReservedShortcuts.Contains(userChoice));

        return userChoice;
    }
}