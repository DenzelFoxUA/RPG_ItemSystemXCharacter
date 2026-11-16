using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;

namespace InventoryDemo.UI;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.SetOut(new MirroringTextWriter(Console.Out));

        var lifetime = new ClassicDesktopStyleApplicationLifetime
        {
            Args = args,
            ShutdownMode = ShutdownMode.OnMainWindowClose
        };

        AppBuilder.Configure<Application>()
            .UsePlatformDetect()
            .AfterSetup(builder => builder.Instance?.Styles.Add(new FluentTheme()))
            .SetupWithLifetime(lifetime);

        lifetime.MainWindow = new MainWindow();
        Console.WriteLine("RPG Inventory Avalonia UI started.");
        lifetime.Start(args);
    }
}
