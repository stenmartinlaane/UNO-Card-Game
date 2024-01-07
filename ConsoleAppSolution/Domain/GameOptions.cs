namespace Domain;

public sealed class GameOptions
{
    private static GameOptions? _playerEditedGAmeOptions;

    public int StartingHandSize { get; set; } = 7;

    public bool MultibleCardsPlayedPerTurn { get; set; } = false;

    public int ScoreToWin { get; set; } = 500;
    
    public GameOptions() { }
    
    public static GameOptions PlayerEditedGAmeOptions => _playerEditedGAmeOptions ??= new GameOptions();

    public GameOptions Clone()
    {
        return new GameOptions
        {
            StartingHandSize = this.StartingHandSize,
            MultibleCardsPlayedPerTurn = this.MultibleCardsPlayedPerTurn,
            ScoreToWin = this.ScoreToWin
        };
    }
}