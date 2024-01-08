using Domain;

namespace DAL;

public class GameRepositoryEF : IGameRepository
{
    public void Save(Guid? id, GameState state)
    {

    }

    public List<GameState> GetSaveGames()
    {
        throw new NotImplementedException();
    }

    public GameState LoadGame(Guid id)
    {
        throw new NotImplementedException();
    }
}