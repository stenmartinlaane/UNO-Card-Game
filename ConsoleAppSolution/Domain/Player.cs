namespace Domain;

public class Player
{
    public Player(string playerType, string nickName)
    {
        if (playerType == "a")
        {
            PlayerType = EPlayerType.AI;
        }
        else
        {
            PlayerType = EPlayerType.Human;
        }

        NickName = nickName;
    }
    
    public Player() {}
    
    public string NickName { get; set; } = default!;

    public int Points { set; get; }
    
    public Guid PlayerId { set; get; } = Guid.NewGuid();
    
    public EPlayerType PlayerType { get; set; }

    public List<GameCard> PlayerHand { get; set; } = new();
    
    public Player Clone()
    {
        return new Player
        {
            NickName = this.NickName,
            PlayerId = this.PlayerId,
            PlayerType = this.PlayerType,
            PlayerHand = this.PlayerHand.Select(card => card.Clone()).ToList(),
            Points = this.Points
            
        };
    }
}