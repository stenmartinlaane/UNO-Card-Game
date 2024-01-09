using Domain;

namespace DAL;

public interface IGameRepository
{
    void Save(Guid id, GameState state);
    List<(Guid id, DateTime dt)> GetSaveGamesData();

    GameState? LoadGame(Guid id);
}