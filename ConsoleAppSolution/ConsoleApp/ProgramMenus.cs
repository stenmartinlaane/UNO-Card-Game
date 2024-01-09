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
            string playerType;
            while (true)
            {
                Console.Write($"Player {i} type (A - ai / H - human)[h]:");
                playerType = Console.ReadLine()?.Trim();
                if (playerType == "")
                {
                    playerType = "h";
                }
                if (playerType is "h" or "a")
                {
                    break;
                }
            }
            
            string playerName;
            while (true)
            {
                Console.Write($"Player {i} Name[{playerType + i}]:");
                playerName = Console.ReadLine()?.Trim();
                if (playerName == "")
                {
                    playerName = playerType + i;
                }
                if (playerName != null)
                {
                    break;
                }
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
        Console.Clear();
        int cardCount;
        while (true)
        { Console.Write("How many points does player need to win? (1 - 2000)[500]: ");
            var input = Console.ReadLine()?.Trim();
            if (input == "")
            {
                input = "500";
            }
            
            if (!int.TryParse(input, out cardCount)) continue;
            if (cardCount is >= 1 and <= 2000)
            {
                break;
            }
        }
        GameOptions.PlayerEditedGAmeOptions.ScoreToWin = cardCount;
    }
    
    private static void PromptToChangeHandSize()
    {
        Console.Clear();
        int cardCount;
        while (true)
        { Console.Write("How many cards should each player have it starting hand? (2 - 20)[7]: ");
            var input = Console.ReadLine()?.Trim();
            if (input == "")
            {
                input = "7";
            }
            
            if (!int.TryParse(input, out cardCount)) continue;
            if (cardCount is >= 2 and <= 20)
            {
                break;
            }
        }
        GameOptions.PlayerEditedGAmeOptions.StartingHandSize = cardCount;
    }

    public static Menu GetLoadMenu(IGameRepository gameRepository)
    {
        List<(Guid id, DateTime dt)> gamesData = gameRepository.GetSaveGamesData();
        List<MenuItem> menuItems = [];

        for (int i = 0; i < gamesData.Count; i++)
        {
            (Guid id, DateTime dt) gameData = gamesData[i];
            MenuItem menuItem = new MenuItem()
            {
                Shortcut = (i + 1).ToString() ,
                MenuLabel = $"Game Saved At - {gameData.dt}",
                Action = new GameController(
                    new UnoEngine(gameRepository.LoadGame(gameData.id)!),
                    new GameRepositoryFileSystem()
                ).Run
            };
            menuItems.Add(menuItem);
        }

        
        
        Menu loadMenu = new Menu("Load game", menuItems);
        return loadMenu;
    }
}