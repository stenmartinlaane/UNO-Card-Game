using MenuSystem;

namespace ConsoleAppProject
{

    class Program
    {
        static void Main(string[] args)
        {
            var mainMenu = ProgramMenus.GetMainMenu(ProgramMenus.GetOptionsMenu());
            mainMenu.Run();

            // var optionsMenu = ProgramMenus.GetOptionsMenu();
            // optionsMenu.Run();


        }
    }
}