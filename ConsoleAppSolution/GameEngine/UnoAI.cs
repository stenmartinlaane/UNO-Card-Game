using Domain;

namespace GameEngine;

public class UnoAI
{
    public static String MakeMove(UnoEngine engine)
    {
        if (engine.GetValidActions().Contains(EPlayerAction.PlayCard))
        {
            int i = 1;
            foreach (var card in engine.State.CurrentPlayerHand())
            {
                if (engine.ValidateCardPlayed(card))
                {
                    return i.ToString();
                }
                i++;
            }
            return "p";
        }
        if (engine.GetValidActions().Contains(EPlayerAction.PlayDrawnCard))
        {
            return "y";
        }
        if (engine.GetValidActions().Contains(EPlayerAction.ChooseColor))
        {
            return "r";
        }
        return "p";
    }
}