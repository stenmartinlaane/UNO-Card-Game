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
        while (_engine.GetGameWinner() == null)
        {
            PlayMatch();
            _engine.StartNewGame(_engine.State.GameOptions, _engine.State.Players);
            ConsoleVisualizations.DrawScoreBoard(_engine.State);
        }
        Console.Clear();
        Console.WriteLine($"{_engine.GetGameWinner()} has won the UNO game with {_engine.GetGameWinner()!.Points} points.");
    }

    public void PlayMatch()
    {
        Console.Clear();
        while (_engine.State.TurnState != ETurnState.ScoreBoard)
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
                ConsoleVisualizations.AskPlayerMoveMessage(_engine);
                
                var playerChoice = Console.ReadLine().Trim().ToLower();
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