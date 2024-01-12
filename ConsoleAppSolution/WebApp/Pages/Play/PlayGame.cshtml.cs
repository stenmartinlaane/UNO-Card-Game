using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using Config;
using DAL;
using Domain;
using Domain.Database;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Play;

public class PlayGame(IGameRepository gameRepository, DAL.AppDbContext context) : PageModel
{
    private readonly IGameRepository _gameRepository = gameRepository;

    
    [BindProperty(SupportsGet = true)]
    public Guid PlayerId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }
    [BindProperty] public new Player User { get; set; } = new();
    [BindProperty] public GameState GameState { get; set; }
    [BindProperty] public Guid? AdminId { get; set; }
    [BindProperty] public bool IsMyTurn { get; set; }
    [BindProperty] public Game Game { get; set; } = default!;
    [BindProperty] public UnoEngine Engine { get; set; } = default!;
    [BindProperty] public List<EPlayerAction> ValidActions { get; set; } = new List<EPlayerAction>();
    [BindProperty(SupportsGet = true)] public string? ErrorMessage { get; set; } = null;
    
    public async Task<IActionResult> OnGetAsync()
    {
        var game =  await context.Games.FirstOrDefaultAsync(m => m.Id == GameId);
        if (game == null)
        {
            return NotFound();
        }
        Game = game;
        GameState = JsonSerializer.Deserialize<GameState>(game.State, JsonConfig.JsonSerializerOptions)!;
        Console.WriteLine(GameState.TurnState);
        UnoEngine engine = new UnoEngine(GameState);
        if (GameState.Winner != null)
        {
            RedirectToPage("/Play/GameResult", new { PlayerId, GameId });
        }
        AdminId = GameState.Players.First().PlayerId;
        MakeAiMoves(engine);
        User = GameState.Players.First(p => PlayerId == p.PlayerId);
        IsMyTurn = engine.State.CurrentPlayer().PlayerId == User.PlayerId;
        ValidActions = engine.GetValidActions();
        return Page();
    }
    
    public IActionResult OnPost(string playerChoice, string playerAction)
    {
        string? ErrorMessage = null;
        if (playerAction == "playMove")
        {
            GameState = JsonSerializer.Deserialize<GameState>(Game.State, JsonConfig.JsonSerializerOptions)!;
            UnoEngine engine = new UnoEngine(GameState);
            engine.MakeMove(playerChoice);
            MakeAiMoves(engine);
            ErrorMessage = engine.ErrorMessages.FirstOrDefault();
            _gameRepository.Save(GameId, engine.State);
            if (engine.State.Winner != null)
            {
                RedirectToPage("/Play/GameResult", new { PlayerId, GameId });
            }
        }
        return RedirectToPage("/Play/CreateGame", new { PlayerId, GameId, ErrorMessage });
    }

    public void MakeAiMoves(UnoEngine engine)
    {
        while (engine.State.CurrentPlayer().PlayerType == EPlayerType.AI)
        {
            engine.MakeMove(UnoAI.MakeMove(engine));
            _gameRepository.Save(GameId, GameState);
        }
        if (engine.State.Winner != null)
        {
            RedirectToPage("/Play/GameResult", new { PlayerId, GameId });
        }
    }
}