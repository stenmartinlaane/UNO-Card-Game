namespace Domain;

public class GameState
{
    public GameState(List<GameCard?> deckOfGameCardsInPlay, List<Player> players, GameOptions gameOptions, int activePlayerNr)
    {
        DeckOfGameCardsInPlay = deckOfGameCardsInPlay;
        Players = players;
        GameOptions = gameOptions;
        ActivePlayerNr = activePlayerNr;
    }

    public GameState(){}

    public Guid Id { get; set; } = Guid.NewGuid();

    public List<GameCard> DeckOfGameCardsInPlay { get; set; } = new List<GameCard>();
    public List<GameCard> DeckOfCardsGraveyard { get; set; } = new List<GameCard>();
    public int ActivePlayerNr { get; set; } = 0;
    public List<Player> Players { get; set; } = new List<Player>();
    public GameOptions GameOptions { get; set; } = new GameOptions();

    public ETurnState TurnState { get; set; } = ETurnState.PlayCard;

    public int CardsToPickUp { get; set; } = 0;

    public bool DoubleTurn { get; set; } = false;

    public bool IsReverseEffectActive { get; set; } = false;

    public bool SearchingForPlayers { get; set; } = false;

    public Player? Winner { get; set; } = null;


    public GameCard LastCardPlayed
    {
        get => _lastCardPlayed;
        set => _lastCardPlayed = value.Clone();
    }

    private GameCard _lastCardPlayed = new GameCard(ECardText.Eight, ECardColor.Blue);

    
    public GameState Clone()
    {
        GameState clonedState = new GameState
        {
            Id = this.Id,
            ActivePlayerNr = this.ActivePlayerNr,
            GameOptions = this.GameOptions.Clone(),
            DeckOfGameCardsInPlay = this.DeckOfGameCardsInPlay.Select(card => card?.Clone()).ToList(),
            DeckOfCardsGraveyard = this.DeckOfCardsGraveyard.Select(card => card?.Clone()).ToList(),
            Players = this.Players.Select(player => player.Clone()).ToList(),
            TurnState = this.TurnState,
            CardsToPickUp = this.CardsToPickUp,
            IsReverseEffectActive = this.IsReverseEffectActive,
            LastCardPlayed = this.LastCardPlayed,
            Winner = this.Winner,
            DoubleTurn = this.DoubleTurn,
            SearchingForPlayers = this.SearchingForPlayers,
            AdminId = this.AdminId
        };

        return clonedState;
    }
    
    public List<GameCard> CurrentPlayerHand()
    {
        return Players[ActivePlayerNr].PlayerHand;
    }

    public Player CurrentPlayer()
    {
        return Players[ActivePlayerNr];
    }

    public Guid AdminId { get; set; } = new Guid();
    
    public Player GetLastPlayer()
    {
        if (IsReverseEffectActive)
        {
            return Players[(ActivePlayerNr + 1) % Players.Count];
        }
        else
        {
            return Players[(ActivePlayerNr - 1 + Players.Count) % Players.Count];
        }
    }
}