using System.Runtime.Serialization;
using System.Text.Json;
using Domain;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    private static readonly string SaveLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "UNO_CardGame_stenmartin");


    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public void Save(Guid? id, GameState state)
    {
        GameState stateCopy = state.Clone();
        if (id != null)
        {
            stateCopy.Id = (Guid) id;
        }
        var jsonContent = JsonSerializer.Serialize(stateCopy, _jsonSerializerOptions);
        

        if (!Path.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }
        var fileName = Path.ChangeExtension(stateCopy.Id.ToString(), ".json");
        using StreamWriter writer = System.IO.File.AppendText(Path.Combine(SaveLocation, fileName));
        writer.WriteLine(jsonContent);
    }
    public List<GameState> GetSaveGames()
    {
        var data = Directory.EnumerateFiles(SaveLocation);
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