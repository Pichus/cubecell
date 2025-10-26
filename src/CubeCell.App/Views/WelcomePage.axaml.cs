using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CubeCell.App.ViewModels;
using CubeCell.App.ViewModels.Pages;

using ReactiveUI;

namespace CubeCell.App.Views;

public partial class WelcomePage : ReactiveUserControl<WelcomePageViewModel>
{
    public WelcomePage()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}