namespace Domain;

public class Player
{
    public string NickName { get; set; } = default!;
    
    Guid PlayerId { set; get; } = Guid.NewGuid();
    
    public EPlayerType PlayerType { get; set; }

    public List<GameCard?> PlayerHand { get; set; } = new List<GameCard?>();
    
    public Player Clone()
    {
        return new Player
        {
            NickName = this.NickName,
            PlayerId = this.PlayerId,
            PlayerType = this.PlayerType,
            PlayerHand = this.PlayerHand?.Select(card => card.Clone()).ToList()
        };
    }
}