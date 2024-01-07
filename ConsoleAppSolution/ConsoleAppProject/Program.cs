using DAL;
using Domain;
using GameEngine;

namespace ConsoleAppProject
{
    static class Program
    {
        static void Main(string[] args)
        {
            IGameRepository gameRepository = new GameRepositoryFileSystem();
            var mainMenu = ProgramMenus.GetMainMenu(ProgramMenus.GetOptionsMenu(), gameRepository);
            //mainMenu.Run();

            Guid guid = new Guid();
            
            gameRepository.Save(guid,
                new UnoEngine(
                    GameOptions.PlayerEditedGAmeOptions,
                    new List<Player>()).State
                );
            
            Console.WriteLine(gameRepository.LoadGame(guid));
            
        }
    }
}