using System.ComponentModel.DataAnnotations;
using Config;
using DAL;
using Domain;
using Domain.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.Text.Json;

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
    
    public async Task<IActionResult> OnGetAsync()
    {
        Console.WriteLine(PlayerId);
        var game =  await _context.Games.FirstOrDefaultAsync(m => m.Id == GameId);
        if (game == null)
        {
            return NotFound();
        }
        Game = game;
        GameState = JsonSerializer.Deserialize<GameState>(game.State, JsonConfig.JsonSerializerOptions)!;
        AdminId = GameState.Players.First().PlayerId;
        Console.WriteLine(GameState.Players.First().PlayerId);
        User = GameState.Players.First(p => PlayerId == p.PlayerId);
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        //Console.WriteLine(Game.State);
        GameState = JsonSerializer.Deserialize<GameState>(Game.State, JsonConfig.JsonSerializerOptions)!;
        Console.WriteLine("secretmagic");
        GameState.Players.First(p => PlayerId == p.PlayerId).NickName = User.NickName;
        Game.State = JsonSerializer.Serialize(GameState, JsonConfig.JsonSerializerOptions);
        _gameRepository.Save(GameState.Id, GameState);
        //_context.Games.Update(Game);
        //await _context.SaveChangesAsync();

        return RedirectToPage("/Play/CreateGame", new { PlayerId, GameId });
    }
}