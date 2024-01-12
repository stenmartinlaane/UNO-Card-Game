using Domain;
using Domain.Database;

namespace DAL;

public interface IGameRepository
{
    void Save(Guid id, GameState state);
    List<(GameState gameState, DateTime dt)> GetSaveGamesData();

    GameState? LoadGame(Guid id);
}