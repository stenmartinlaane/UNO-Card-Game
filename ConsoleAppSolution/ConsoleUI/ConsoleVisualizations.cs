using System.Data;
using System.Runtime.InteropServices.JavaScript;
using Domain;

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
        /*
        Console.WriteLine("Your current hand is: " +
                          string.Join(
                              "  ",
                              player.PlayerHand.Select((c, i) => (i+1) + ": " + c)
                          )
        );
        */

        Console.Write("Your current hand is: ");
        for (int i = 1; i < player.PlayerHand.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(i.ToString() + ":");
            Console.ResetColor();
            Console.Write(player.PlayerHand[i - 1] + " ");
        }
    }

    public static void AskPlayerMoveMessage(GameState state)
    {
        String message = "";
        switch (state.TurnState)
        {
            case ETurnState.PlayCard:
            {
                if (state.GameOptions.MultibleCardsPlayedPerTurn == false)
                {
                    message =  $"Choose card to play 1 - {state.Players[state.ActivePlayerNr].PlayerHand.Count}, p to pick up card(s): ";
                }

                break;
            }
            case ETurnState.PlayCardAfterPickingUp:
            {
                message = $"You drew card {state.currentPlayerHand()[^1]}. Do you want to play the card (y/n)";
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
                DrawPlayerHand(state.GetLastPlayer());
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
}