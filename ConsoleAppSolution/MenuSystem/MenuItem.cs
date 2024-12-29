using DAL;

namespace MenuSystem;

public class MenuItem
{
    public string MenuLabel { get; set; } = "";

    public Func<string>? MenuLabelFunction { get; set; } = null;
    public string Shortcut { get; set; } = default!;
    public Func<string?>? SubMenu { get; set; } = null;
    

    public Action? Action { get; set; } = null;
}