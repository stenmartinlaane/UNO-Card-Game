using System.Text.Json;
using Domain;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    private const string SaveLocation = "./";
    JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public void Save(Guid id, GameState state)
    {
        var content = JsonSerializer.Serialize(state, JsonSerializerOptions);
    }

    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        throw new NotImplementedException();
    }

    public GameState LoadGame(Guid id)
    {
        throw new NotImplementedException();
    }
}