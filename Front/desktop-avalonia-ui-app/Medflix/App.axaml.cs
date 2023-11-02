using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaWebView;
using Medflix.ViewModels;
using Medflix.Views;
using System.Threading.Tasks;

namespace Medflix;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            mainWindow.Closed += (s, e) => desktop.Shutdown();
            desktop.MainWindow = mainWindow;
        }


        base.OnFrameworkInitializationCompleted();
    }

    public override void RegisterServices()
    {
        base.RegisterServices();

        // if you use only WebView  
        AvaloniaWebViewBuilder.Initialize(default);
    }
}
