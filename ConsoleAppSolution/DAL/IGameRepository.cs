using Domain;

namespace DAL;

public interface IGameRepository
{
    void Save(Guid? id, GameState state);
    List<GameState> GetSaveGames();

    GameState? LoadGame(Guid id);
}