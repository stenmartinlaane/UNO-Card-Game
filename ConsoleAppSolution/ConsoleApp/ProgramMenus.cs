using ConsoleUI;
using DAL;
using Domain;
using GameEngine;
using MenuSystem;

namespace ConsoleAppProject;

public static class ProgramMenus
{
    public static Menu GetMainMenu(IGameRepository gameRepository)
    {
        return new Menu("<< UNO >>", new List<MenuItem>()
            {
                new MenuItem()
                {
                    Shortcut = "s",
                    MenuLabel = "Start a new game",
                    Action = () => GameSetup(gameRepository)
                },
                new MenuItem()
                {
                    Shortcut = "l",
                    MenuLabel = "Load game",
                    SubMenu = () => GetLoadMenu(gameRepository).Run(EMenuLevel.Second)
                },
                new MenuItem() {
                    Shortcut = "o",
                    MenuLabel = "Options",
                    SubMenu = () => GetOptionsMenu().Run(EMenuLevel.Second)
                }
            }
        );
    }

    public static void GameSetup(IGameRepository gameRepository)
    {
        int playerCount;
        do
        {
            Console.Write("How many players (2-7)[2]:");
            var input = Console.ReadLine()?.Trim();
            if (input == "")
            {
                input = "2";
            }
            if (!int.TryParse(input, out playerCount)) continue;
            if (playerCount is >= 2 and <= 7)
            {
                break;
            }
        } while (true);

        List<Player> players = new();
        
        for (int i = 1; i < playerCount + 1; i++)
        {
            string playerType = ConsoleVisualizations.PromptUserForInput(
                (input) =>
                {
                    if (input != "a" && input != "h" && input != "")
                    {
                        return "Input must a - ai or h - human.";
                    }
                    return null;
                },
                () => Console.Write($"Player {i} type (A - ai / H - human)[h]:"),
                "h",
                true
            );
            string playerName = ConsoleVisualizations.PromptUserForInput(
                null,
                () => Console.Write($"Player {i} Name[{playerType + i}]:"),
                "",
                true
            );
            if (playerName == "")
            {
                playerName = playerType + i;
            }
            Player player = new Player(playerType, playerName);
            
            players.Add(player);
        }
        
        new GameController(
            new UnoEngine(GameOptions.PlayerEditedGAmeOptions, players),
            gameRepository
        ).Run();
    }

    public static Menu GetOptionsMenu()
    {
        return new Menu("Options", new List<MenuItem>()
            {
                new MenuItem()
                {
                    Shortcut = "s",
                    MenuLabelFunction = () => $"Score to win - {GameOptions.PlayerEditedGAmeOptions.ScoreToWin}",
                    Action = PromptToChangeScore
                },
                new MenuItem()
                {
                    Shortcut = "h",
                    MenuLabelFunction = () => $"Hand size - {GameOptions.PlayerEditedGAmeOptions.StartingHandSize}",
                    Action = PromptToChangeHandSize
                }
            }
        );
    }

    private static void PromptToChangeScore()
    {
        String playerChoice = ConsoleVisualizations.PromptUserForInput(
            (input) =>
            {
                if (input == "")
                {
                    return null;
                }
                if (!int.TryParse(input, out int result))
                {
                    return "Input must be numeric";
                }

                if (!(1 <= result && result <= 2000))
                {
                    return "Input must be between numbers 1 and 2000.";
                }

                return null;
            },
            () => Console.WriteLine("How many points to win the game? (1 - 2000)[500]: "),
            "500",
            true
        );
        GameOptions.PlayerEditedGAmeOptions.ScoreToWin = int.Parse(playerChoice);
    }
    
    private static void PromptToChangeHandSize()
    {
        Console.Clear();
        String playerChoice = ConsoleVisualizations.PromptUserForInput(
            (input) =>
            {
                if (input == "")
                {
                    return null;
                }
                if (!int.TryParse(input, out int result))
                {
                    return "Input must be numeric";
                }

                if (!(2 <= result && result <= 20))
                {
                    return "Input must be between numbers 2 and 20.";
                }

                return null;
            },
            () => Console.WriteLine("How many cards should each player have it starting hand? (2 - 20)[7]: "),
            "7",
            true
        );
        GameOptions.PlayerEditedGAmeOptions.StartingHandSize = int.Parse(playerChoice);
    }

    public static Menu GetLoadMenu(IGameRepository gameRepository)
    {
        var gamesData = gameRepository.GetSaveGamesData();
        List<MenuItem> menuItems = [];

        for (int i = 0; i < gamesData.Count; i++)
        {
            var gameData = gamesData[i];
            MenuItem menuItem = new MenuItem()
            {
                Shortcut = (i + 1).ToString() ,
                MenuLabel = $"Game Saved At - {gameData.dt}",
                Action = new GameController(
                    new UnoEngine(gameRepository.LoadGame(gameData.gameState.Id)!),
                    new GameRepositoryFileSystem()
                ).Run
            };
            menuItems.Add(menuItem);
        }

        
        
        Menu loadMenu = new Menu("Load game", menuItems);
        return loadMenu;
    }
}