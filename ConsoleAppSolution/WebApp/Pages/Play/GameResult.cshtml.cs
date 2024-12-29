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

public class GameResult : PageModel
{
    
    private readonly IGameRepository _gameRepository;
    
    private readonly DAL.AppDbContext _context;
    
    public GameResult(IGameRepository gameRepository, DAL.AppDbContext context)
    {
        _gameRepository = gameRepository;
        _context = context;
    }
    
    [BindProperty(SupportsGet = true)]
    public Guid PlayerId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }
    
    [BindProperty]
    public GameState? GameState { get; set; }
    
    public new Player? User { get; set; }
    
    public IActionResult OnGetAsync()
    {
        GameState = _gameRepository.LoadGame(GameId)!;
        if (GameState.TurnState != ETurnState.ScoreBoard)
        {
            return RedirectToPage("/Play/PlayGame", new { PlayerId, GameId });
        }
        User = GameState.Players.First(p => PlayerId == p.PlayerId);
        return Page();
    }

    public ActionResult OnPostAsync()
    {
        GameState = _gameRepository.LoadGame(GameId)!;
        UnoEngine engine = new UnoEngine(GameState);
        engine.StartNewGame(GameState.GameOptions, GameState.Players);
        _gameRepository.Save(GameId, GameState);
        return RedirectToPage("/Play/PlayGame", new { PlayerId, GameId });
    }
}