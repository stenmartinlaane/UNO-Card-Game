using ConsoleUI;
using DAL;
using Domain;
using GameEngine;
using MenuSystem;

namespace ConsoleAppProject;

public static class ProgramMenus
{
    public static Menu GetMainMenu(Menu optionsMenu)
    {
        return new Menu("<< UNO >>", new List<MenuItem>()
            {
                new MenuItem()
                {
                    Shortcut = "s",
                    MenuLabel = "Start a new game",
                    Action = new GameController(
                        new UnoEngine(GameOptions.PlayerEditedGAmeOptions),
                        new GameRepositoryFileSystem()
                        ).Run
                },
                new MenuItem()
                {
                    Shortcut = "l",
                    MenuLabel = "Load game"
                },
                new MenuItem() {
                    Shortcut = "o",
                    MenuLabel = "Options",
                    MethodToRun = () => optionsMenu.Run(EMenuLevel.Second)
                }
            }
        );
    }

    public static Menu GetOptionsMenu()
    {
        return new Menu("Options", new List<MenuItem>()
            {
                new MenuItem()
                {
                    Shortcut = "m",
                    MenuLabel = "Multiple of same card can be played per turn - no"
                },
                new MenuItem()
                {
                    Shortcut = "Hand size",
                    MenuLabel = "Hand size - 6"
                }
            }
        );
    }
}