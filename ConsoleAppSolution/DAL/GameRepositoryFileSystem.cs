using System.Runtime.Serialization;
using System.Text.Json;
using Domain;
using Helpers;

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
        using StreamWriter writer = System.IO.File.AppendText(Path.Combine(SaveLocation, fileName));
        writer.WriteLine(jsonContent);
    }
    public List<(Guid id, DateTime dt)> GetSaveGamesData()
    {
        var data = Directory.EnumerateFiles(SaveLocation)
            .Where(path =>
                    Path.GetExtension(path) == ".json"
            );
        List<(Guid id, DateTime dt)> res = data
            .Select(
                path => (
                    Guid.Parse(Path.GetFileNameWithoutExtension(path)),
                    File.GetLastWriteTime(path)
                )
            ).ToList();
        
        res = res
        .OrderByDescending(game => game.dt)
        .ToList();
        
        return res;
    }

    public GameState? LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");
        var filePath = Path.Combine(SaveLocation, fileName);
        if (File.Exists(filePath))
        {
            using StreamReader reader = new StreamReader(filePath);
            string jsonContent = reader.ReadToEnd();

            var result = JsonSerializer.Deserialize<GameState>(jsonContent, _jsonSerializerOptions);

            return result;
        }
        else
        {
            return null;
        }
    }
}