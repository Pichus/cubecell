using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CubeCell.ViewModels;
using ReactiveUI;

namespace CubeCell.Views;

public partial class Welcome : ReactiveUserControl<WelcomePageViewModel>
{
    public Welcome()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}