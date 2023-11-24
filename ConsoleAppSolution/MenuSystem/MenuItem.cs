namespace MenuSystem;

public class MenuItem
{
    public string MenuLable { get; set; }
    public Func<string>? MenuLableFunction { get; set; }
    public string Shortcut { get; set; } = default!;
    public Func<string?>? MethodToRun { get; set; } = null;
    public Func<EMenuLevel, string?>? SubMenuToRun { get; set; }

}