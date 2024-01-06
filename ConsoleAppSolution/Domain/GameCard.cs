namespace Domain;

public class GameCard
{
    public GameCard(ECardText cardText, ECardColor cardColor)
    {
        CardColor = cardColor;
        CardText = cardText;
    }
    
    public ECardColor CardColor { get; set; }
    public ECardText CardText { get; set; }
    
    public override string ToString()
    {
        return CardColor.ToDescriptionString().Trim() + CardText.ToDescriptionString().Trim();
    }
    
    public GameCard Clone()
    {
        return new GameCard(this.CardText, this.CardColor);
    }

}