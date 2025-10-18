using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CubeCell.App.ViewModels;
using ReactiveUI;

namespace CubeCell.App.Views;

public partial class Welcome : ReactiveUserControl<WelcomePageViewModel>
{
    public Welcome()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}