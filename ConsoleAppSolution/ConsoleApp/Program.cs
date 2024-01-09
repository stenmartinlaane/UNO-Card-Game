using DAL;
using Domain;
using GameEngine;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppProject
{
    static class Program
    {
        static void Main(string[] args)
        {
            
            //TODO:hide db init in method
            var connectionString = "DataSource=<%temppath%>uno.db;Cache=Shared";
            connectionString = connectionString.Replace("<%temppath%>", Path.GetTempPath());
            Console.WriteLine(connectionString);
            var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connectionString)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;
            using var db = new AppDbContext(contextOptions);
            db.Database.Migrate();
            IGameRepository gameRepository = new GameRepositoryEF(db);
            //IGameRepository gameRepository = new GameRepositoryFileSystem();
            
            
            var mainMenu = ProgramMenus.GetMainMenu(gameRepository);
            mainMenu.Run();
        }
    }
}