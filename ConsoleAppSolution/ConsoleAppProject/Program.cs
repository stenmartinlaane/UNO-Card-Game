using ConsoleUI;
using DAL;
using Domain;
using GameEngine;
using MenuSystem;

namespace ConsoleAppProject
{
    static class Program
    {
        static void Main(string[] args)
        {
            var mainMenu = ProgramMenus.GetMainMenu(ProgramMenus.GetOptionsMenu());
            // mainMenu.Run();
            
            
            
            GameOptions gameOptions = new GameOptions();
            UnoEngine unoEngine = new UnoEngine(gameOptions);

            
            //if (unoEngine.ValidateCardPlayed(new GameCard(ECardText.Eight, ECardColor.Wild))) { Console.Write("hdfsa"); }
            
            IGameRepository gameRepository = new GameRepositoryFileSystem();
            var gameController = new GameController(
                unoEngine,
                gameRepository
                );
            
            gameController.Run();

            // var optionsMenu = ProgramMenus.GetOptionsMenu();
            // optionsMenu.Run();


        }
    }
}