namespace Domain;

public sealed class GameOptions
{
    private static GameOptions? _playerEditedGAmeOptions;

    public int StartingHandSize { get; set; } = 6;

    public bool MultibleCardsPlayedPerTurn { get; set; } = false;
    
    public GameOptions() { }
    
    public static GameOptions PlayerEditedGAmeOptions => _playerEditedGAmeOptions ??= new GameOptions();

    public GameOptions Clone()
    {
        return new GameOptions
        {
            StartingHandSize = this.StartingHandSize,
            MultibleCardsPlayedPerTurn = this.MultibleCardsPlayedPerTurn
        };
    }
}