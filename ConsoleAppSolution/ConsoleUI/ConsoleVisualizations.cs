using System.Data;
using System.Runtime.InteropServices.JavaScript;
using Domain;
using GameEngine;

namespace ConsoleUI;

public class ConsoleVisualizations
{
    public static void DrawBoard(GameState state)
    {
        Console.WriteLine($"Cards in deck: {state.DeckOfGameCardsInPlay.Count}");
        Console.WriteLine($"Last card played: {state.LastCardPlayed}");

        for (var i = 0; i < state.Players.Count; i++)
        {
            Console.WriteLine(
                $"Player {i + 1} - {state.Players[i].NickName} has {state.Players[i].PlayerHand.Count} cards");
        }
        
    }

    public static void DrawPlayerHand(Player player)
    {
        Console.Write("Your current hand is: ");
        VisualizeHand(player);
        Console.WriteLine();
    }

    private static void VisualizeHand(Player player)
    {
        for (int i = 1; i < player.PlayerHand.Count + 1; i++)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(i.ToString() + ":");
            Console.ResetColor();
            Console.Write(player.PlayerHand[i - 1] + " ");
        }
    }

    public static void AskPlayerMoveMessage(UnoEngine engine)
    {
        String message = "";
        switch (engine.State.TurnState)
        {
            case ETurnState.PlayCard:
            {
                if (engine.State.GameOptions.MultibleCardsPlayedPerTurn == false)
                {
                    message =  $"Choose card to play 1 - {engine.State.Players[engine.State.ActivePlayerNr].PlayerHand.Count}, p to pick up card(s): ";
                }

                break;
            }
            case ETurnState.PlayCardAfterPickingUp:
            {
                GameCard cardDrawn = engine.State.currentPlayerHand()[^1];
                if (engine.ValidateCardPlayed(engine.State.currentPlayerHand()[^1]))
                {
                    message = $"You drew card {cardDrawn}. Do you want to play the card (y - yes / n -no)[no]: ";
                }
                else
                {
                    message = $"You drew card {cardDrawn}.";
                }
                break;
            }
            case ETurnState.ChooseColor:
            {
                message = "Choose next color (r)ed, (b)lue, (y)ellow, (g)reen: ";
                break;
            }
            case ETurnState.PlusTwo:
            {
                message = "You pick up two cards and miss your turn.";
                break;
            }
            case ETurnState.WildPlusFour:
            {
                message = "You can pick up 4 cards or check or check if player did not have previous color. (p)ick or (c)heck?";
                break;
            }
            case ETurnState.RevealLastPlayerCards:
            {
                Console.Write("The player who played plus four has had: ");
                VisualizeHand(engine.State.GetLastPlayer());
                Console.WriteLine();
                break;
            }
            case ETurnState.Skip:
            {
                message = "Your turn has been skipped. Press enter to end your turn.";
                break;
            }
        }

        if (message != "")
        {
            Console.Write(message);
        }
    }

    public static void DrawScoreBoard(GameState state)
    {
        Console.WriteLine($"{state.Winner} won the uno match!");
        
        Console.WriteLine("SCOREBOARD");
        Console.WriteLine($"Play is until {state.GameOptions.ScoreToWin} points.");
        Console.WriteLine("Current standings:");
        foreach (Player player in state.Players)
        {
            Console.WriteLine($"{player.NickName} - {player.Points}");
        }

        Console.ReadLine();
    }
}