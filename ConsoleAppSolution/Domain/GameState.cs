namespace Domain;

public class GameState
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private List<GameCard> DeckOGameCardsInPlay { get; set; } = new List<GameCard>();
    private List<GameCard> DeckOfCardsGraveyard { get; set; } = new List<GameCard>();
    private int ActivePlayerNr { get; set; }
    private ECardColor currentColor { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
}