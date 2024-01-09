using System.Text.Json;
using Domain;
using Domain.Database;
using Helpers;

namespace DAL;

public class GameRepositoryEF(AppDbContext ctx) : IGameRepository
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = JsonConfig.JsonSerializerOptions;

    public void Save(Guid id, GameState state)
    {
        var game = ctx.Games.FirstOrDefault(g => g.Id == state.Id);
        if (game == null)
        {
            game = new Domain.Database.Game()
            {
                Id = id,
                State = JsonSerializer.Serialize(state, _jsonSerializerOptions),
                CreatedAtDt = DateTime.Now
            };
            ctx.Games.Add(game);
        }
        else
        {
            game.UpdatedAtDt = DateTime.Now;
            game.State = JsonSerializer.Serialize(state, _jsonSerializerOptions);
        }
        var changeCount = ctx.SaveChanges();
    }

    public List<(Guid id, DateTime dt)> GetSaveGamesData()
    {
        return ctx.Games
            .OrderByDescending(g => g.UpdatedAtDt)
            .ToList()
            .Select(g => (g.Id, g.UpdatedAtDt))
            .ToList();
    }


    public GameState LoadGame(Guid id)
    {
        var game = ctx.Games.First(g => g.Id == id);
        return JsonSerializer.Deserialize<GameState>(game.State, _jsonSerializerOptions)!;
    }
}