using DAL;
using Domain;
using GameEngine;

namespace ConsoleUI;

public class GameController
{
    private readonly UnoEngine _engine;
    private readonly IGameRepository _reopsitory;
    private readonly ConsoleVisualizations _consoleVisualizations;

    public GameController(UnoEngine engine, IGameRepository reopsitory)
    {
        _engine = engine;
        _reopsitory = reopsitory;
    }

    public void Run()
    {
        Console.Clear();
        while (!_engine.IsGameOver())
        {
            Console.WriteLine($"Player {_engine.State.ActivePlayerNr + 1} - {_engine.State.CurrentPlayer().NickName}");
            Console.Write("Your turn, make sure you are alone looking at screen! Press enter to continue...");
            
            Console.ReadLine();
            Console.Clear();
            
            while (true)
            {
                Console.WriteLine($"Player {_engine.State.ActivePlayerNr + 1} - {_engine.State.CurrentPlayer().NickName}");
                ConsoleVisualizations.DrawBoard(_engine.State);
                ConsoleVisualizations.DrawPlayerHand(_engine.State.CurrentPlayer());
                Console.WriteLine(_engine.State.TurnState);
                ConsoleVisualizations.AskPlayerMoveMessage(_engine.State);
                
                var playerChoice = Console.ReadLine()?.Trim();
                _engine.TryToMakePlayerMove(playerChoice);
                if (_engine.ErrorMessage != null)
                {
                    Console.WriteLine(_engine.ErrorMessage);
                    _engine.ErrorMessage = null;
                    Console.WriteLine("====================");
                    continue;
                }
                else if (_engine.IsTurnOver())
                {
                    _engine.NextPlayerMove();
                    Console.Clear();
                    break;
                }
            }


            
        }
    }
}