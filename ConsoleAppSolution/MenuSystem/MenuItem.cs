namespace MenuSystem;

public class MenuItem
{
    public string MenuLabel { get; set; }
    public Func<string>? MenuLabelFunction { get; set; }
    public string Shortcut { get; set; } = default!;
    public Func<string?>? MethodToRun { get; set; } = null;
    public Func<EMenuLevel>? SubMenuToRun { get; set; }

    public Action? Action;
}