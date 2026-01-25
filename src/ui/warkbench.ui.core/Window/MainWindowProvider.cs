using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace warkbench.src.ui.core.Window;

public class MainWindowProvider
{
    public Avalonia.Controls.Window? Get =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
}