using DAL;
using Domain;
using Domain.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Play;

public class JoinGame : PageModel
{
    private readonly IGameRepository _gameRepository;

    public JoinGame(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public List<(GameState gameState, DateTime dt)> GamesData { get;set; } = default!;

    public async Task OnGetAsync()
    {
        GamesData = _gameRepository.GetSaveGamesData();
    }
    
    [BindProperty]
    public String GameId { get; set; }
    
    public IActionResult OnPost()
    {
        Console.WriteLine(GameId);
        GameState? state = _gameRepository.LoadGame(Guid.Parse(GameId));
        Console.WriteLine(GameId);
        if (!ModelState.IsValid || state == null || state.Players.Count() > 6)
        {
            return Page();
        }
        Guid PlayerId = JoinTheGame(state);
        return RedirectToPage("/Play/CreateGame", new {GameId, PlayerId});
    }

    private Guid JoinTheGame(GameState state)
    {
        Guid id = Guid.NewGuid();
        state.Players.Add(new Player()
            {
                PlayerId = id
            }
        );
        _gameRepository.Save(state.Id, state);
        return id;
    }
}