using Config;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppProject
{
    static class Program
    {
        static void Main(string[] args)
        {
            
            //TODO:hide db init in method
            var connectionString = DataBaseConfig.GetDataBasePath();
            var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connectionString)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;
            using var db = new AppDbContext(contextOptions);
            db.Database.Migrate();
            IGameRepository gameRepository = new GameRepositoryEF(db);
            //IGameRepository gameRepository = new GameRepositoryFileSystem();

            var id = gameRepository.LoadGame(Guid.Parse("1e48ff4e-5f44-4e18-92c0-0711e9d52dc0")).Id;
            Console.WriteLine(id);
            var mainMenu = ProgramMenus.GetMainMenu(gameRepository);
            //mainMenu.Run();
        }
    }
}