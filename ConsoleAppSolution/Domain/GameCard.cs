namespace Domain;

public class GameCard
{
    public GameCard(ECardText cardText, ECardColor cardColor)
    {
        CardColor = cardColor;
        CardText = cardText;
    }
    
    private ECardColor CardColor { get; set; }
    private ECardText CardText { get; set; }

    public override string ToString()
    {
        return CardColor.ToString() + CardText.ToString();
    }
}