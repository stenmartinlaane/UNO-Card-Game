namespace Domain;

public class GameState
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public List<GameCard> DeckOfGameCardsInPlay { get; set; }
    public List<GameCard> DeckOfCardsGraveyard { get; set; } = new List<GameCard>();
    public int ActivePlayerNr { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public GameOptions GameOptions { get; set; }
}