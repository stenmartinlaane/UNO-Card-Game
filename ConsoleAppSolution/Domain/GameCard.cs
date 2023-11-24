namespace Domain;

public class GameCard
{
    private ECardColor CardColor { get; set; }
    private EUnoCardValue UnoCardValue { get; set; }

    public override string ToString()
    {
        return CardColor.ToString() + UnoCardValue.ToString();
    }
}