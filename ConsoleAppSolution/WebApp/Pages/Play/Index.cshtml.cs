
using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;

public class Index : PageModel
{
    private readonly IGameRepository _gameRepository;

    public Index(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    public void OnGet()
    {
        
    }

    public IActionResult OnPost(string button)
    {
        Console.WriteLine(button);
        if (!ModelState.IsValid)
        {
            return Page();
        }
        switch (button)
        {
            case "CreateNewGame":
                GameState state = StartNewGame();
                Guid GameId = state.Id;
                Guid PlayerId = state.Players.First().PlayerId;

                return RedirectToPage("/Play/CreateGame", new { PlayerId, GameId });
            case "LoadGame":
                Console.WriteLine("loadgame pickleman");
                return RedirectToPage("/Play/LoadGame");
            case "JoinGame":
                return RedirectToPage("/Play/JoinGame");
        }
        return Page();
    }


    private GameState StartNewGame()
    {
        UnoEngine engine = new UnoEngine(GameOptions.DefaultOptions(), []);
        GameState state = engine.State;
        state.SearchingForPlayers = true;
        state.Players.Add(new Player()
            {
                PlayerId = Guid.NewGuid(),
                NickName = "h1"
            }
        );
        _gameRepository.Save(state.Id, state);
        return state;
    }
}