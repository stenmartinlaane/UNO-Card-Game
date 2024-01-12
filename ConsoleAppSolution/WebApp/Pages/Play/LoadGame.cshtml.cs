using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;

public class LoadGame : PageModel
{
    private readonly IGameRepository _gameRepository;

    public LoadGame(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public List<(GameState gameState, DateTime dt)> GamesData { get;set; } = default!;
    public List<GameState> GameStates { get; set; } = default!;
    
    public async Task OnGetAsync()
    {
        GamesData = _gameRepository.GetSaveGamesData();
        GameStates = GamesData.Select(data => data.gameState).ToList();
    }
    
    public IActionResult OnPost(string playerId, string gameId)
    {
        Guid PlayerId = Guid.Parse(playerId);
        Guid GameId = Guid.Parse(gameId);
        return RedirectToPage("/Play/CreateGame", new { PlayerId, GameId });
    }
}