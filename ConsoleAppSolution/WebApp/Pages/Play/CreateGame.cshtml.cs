using Config;
using DAL;
using Domain;
using Domain.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using GameEngine;

namespace WebApp.Pages.Play;

public class CreateGame : PageModel
{
    
    private readonly IGameRepository _gameRepository;
    
    private readonly DAL.AppDbContext _context;
    
    public CreateGame(IGameRepository gameRepository, DAL.AppDbContext context)
    {
        _gameRepository = gameRepository;
        _context = context;
    }
    
    [BindProperty(SupportsGet = true)]
    
    public Guid PlayerId { get; set; }

    //[BindProperty] public String NickName { get; set; } = "enter Nickname";
    
    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }

    [BindProperty] public new Player User { get; set; } = new();
    [BindProperty] public GameState GameState { get; set; }
    [BindProperty] public Guid? AdminId { get; set; } = new Guid();

    [BindProperty] public Game Game { get; set; } = default!;
    [BindProperty] public int HandSize { get; set; }
    [BindProperty] public int ScoreToWin { get; set; }
    [BindProperty] public string BotName { get; set; } = "mightyBot";
    
    public async Task<IActionResult> OnGetAsync()
    {
        Game = await _context.Games.FirstOrDefaultAsync(m => m.Id == GameId);
        GameState = JsonSerializer.Deserialize<GameState>(Game.State, JsonConfig.JsonSerializerOptions)!;
        HandSize = GameState.GameOptions.StartingHandSize;
        ScoreToWin = GameState.GameOptions.ScoreToWin;
        AdminId = GameState.Players.First().PlayerId;
        Console.WriteLine(GameState.Players.First().PlayerId);
        User = GameState.Players.First(p => PlayerId == p.PlayerId);
        if (!GameState.SearchingForPlayers)
        {
            return RedirectToPage("/Play/PlayGame", new { PlayerId, GameId });
        }
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(string submitType)
    {
        if (submitType == "ChangeNickname")
        {
            GameState = JsonSerializer.Deserialize<GameState>(Game.State, JsonConfig.JsonSerializerOptions)!;
            GameState.Players.First(p => PlayerId == p.PlayerId).NickName = User.NickName;
            _gameRepository.Save(GameState.Id, GameState);
            return RedirectToPage("/Play/CreateGame", new { PlayerId, GameId });
        }
        else if (submitType == "CreateNewGame")
        {
            GameState = JsonSerializer.Deserialize<GameState>(Game.State, JsonConfig.JsonSerializerOptions)!;
            GameState.GameOptions.StartingHandSize = HandSize;
            GameState.GameOptions.ScoreToWin = ScoreToWin;
            GameState.SearchingForPlayers = false;
            new UnoEngine(GameState).StartNewGame(GameState.GameOptions, GameState.Players);
            _gameRepository.Save(GameState.Id, GameState);
            return RedirectToPage("/Play/PlayGame", new { PlayerId, GameId });
        }
        else if (submitType == "AddAI")
        {
            GameState = JsonSerializer.Deserialize<GameState>(Game.State, JsonConfig.JsonSerializerOptions)!;
            GameState.Players.Add(new Player()
            {
                NickName = BotName,
                PlayerId = Guid.NewGuid(),
                PlayerType = EPlayerType.AI
            });
            _gameRepository.Save(GameState.Id, GameState);
            return RedirectToPage("/Play/CreateGame", new { PlayerId, GameId });
        }
        return Page();
    }
}