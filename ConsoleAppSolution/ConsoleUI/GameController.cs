using System.Runtime.InteropServices.JavaScript;
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
            String playerChoice = PlayMatch();
            if (playerChoice == "e")
            {
                return;
            }
            ConsoleVisualizations.DrawScoreBoard(_engine.State);
            _engine.StartNewGame(_engine.State.GameOptions, _engine.State.Players);
        }
        Console.Clear();
        Console.WriteLine($"{_engine.GetGameWinner().NickName} has won the UNO game with {_engine.GetGameWinner()!.Points} points.");
        Console.Read();
    }

    public String PlayMatch()
    {
        _reopsitory.Save(_engine.State.Id, _engine.State);
        while (_engine.State.TurnState != ETurnState.ScoreBoard)
        {
            if (_engine.State.CurrentPlayer().PlayerType == EPlayerType.AI)
            {
                _engine.MakeMove(UnoAI.MakeMove(_engine));
                continue;
            }
            String? playerChoice = ConsoleVisualizations.PromptUserForInput(
                null,
                () =>
                {
                    Console.WriteLine(
                        $"Player {_engine.State.ActivePlayerNr + 1} - {_engine.State.CurrentPlayer().NickName}");
                    Console.Write("Your turn, make sure you are alone looking at screen! Press enter to continue...");
                },
                "",
                true
            );
            if (playerChoice == "e")
            {
                return playerChoice;
            }
            while (true)
            {
                
                ConsoleVisualizations.PromptUserForInput(
                    (input) =>
                    {
                        if (input == "e")
                        {
                            return null;
                        }
                        _engine.MakeMove(input);
                        if (_engine.ErrorMessages.Count > 0)
                        {
                            return _engine.ErrorMessages.First();
                        }

                        return null;
                    },
                    () => ConsoleVisualizations.DrawInfoForPlayerMove(_engine),
                    "",
                    true
                );
                _reopsitory.Save(_engine.State.Id, _engine.State);
                if (_engine.IsTurnOver())
                {
                    break;
                }
            }
        }
        return "";
    }
}