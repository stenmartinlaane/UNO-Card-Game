using System.Runtime.Serialization;
using System.Text.Json;
using Config;
using Domain;
using Domain.Database;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    private static readonly string SaveLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "UNO_CardGame_stenmartin");
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = JsonConfig.JsonSerializerOptions;

    public void Save(Guid id, GameState state)
    {
        state.Id = id;
        var jsonContent = JsonSerializer.Serialize(state, _jsonSerializerOptions);

        if (!Path.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }
        var fileName = Path.ChangeExtension(state.Id.ToString(), ".json");
        var filePath = Path.Combine(SaveLocation, fileName);
        File.WriteAllText(filePath, jsonContent);
        //using StreamWriter writer = System.IO.File.AppendText(Path.Combine(SaveLocation, fileName));
        //writer.WriteLine(jsonContent);
    }
    public List<(GameState gameState, DateTime dt)> GetSaveGamesData()
    {
        var data = Directory.EnumerateFiles(SaveLocation)
            .Where(path =>
                    Path.GetExtension(path) == ".json"
            ).OrderByDescending(path => File.GetLastWriteTime(path));

        var res = data.Select(
            path => (
                    GetGameFromFile(path), 
                    File.GetLastWriteTime(path)
                )
        ).ToList();
        return res!;
    }

    public GameState? LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");
        var filePath = Path.Combine(SaveLocation, fileName);
        if (File.Exists(filePath))
        {
            return GetGameFromFile(filePath);
        }
        else
        {
            return null;
        }
    }

    private GameState? GetGameFromFile(String filePath)
    {
        using StreamReader reader = new StreamReader(filePath);
        string jsonContent = reader.ReadToEnd();

        var result = JsonSerializer.Deserialize<GameState>(jsonContent, _jsonSerializerOptions);

        return result;
    }
}