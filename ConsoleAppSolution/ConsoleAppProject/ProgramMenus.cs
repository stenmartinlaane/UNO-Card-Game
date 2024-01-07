using ConsoleUI;
using DAL;
using Domain;
using GameEngine;
using MenuSystem;

namespace ConsoleAppProject;

public static class ProgramMenus
{
    public static Menu GetMainMenu(Menu optionsMenu, IGameRepository gameRepository)
    {
        return new Menu("<< UNO >>", new List<MenuItem>()
            {
                new MenuItem()
                {
                    Shortcut = "s",
                    MenuLabel = "Start a new game",
                    Action = GameSetup
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

    public static void GameSetup()
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
            new GameRepositoryFileSystem()
        ).Run();
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
                    Shortcut = "h",
                    MenuLabel = "Hand size - 6"
                }
            }
        );
    }

    public static Menu GetLoadMenu(IGameRepository gameRepository)
    {
        //List<GameState> gameStates = gameRepository.GetSaveGames();

        return GetOptionsMenu();
    }
}