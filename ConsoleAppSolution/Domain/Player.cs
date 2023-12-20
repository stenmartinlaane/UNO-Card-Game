namespace Domain;

public class Player
{
    public string NickName { get; set; } = default!;
    
    Guid PlayerId { set; get; } = Guid.NewGuid();
    
    public EPlayerType PlayerType { get; set; }

    public List<GameCard> PlayerHand { get; set; } = new List<GameCard>();
}