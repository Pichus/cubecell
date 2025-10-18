using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CubeCell.App.ViewModels;
using ReactiveUI;

namespace CubeCell.App.Views;

public partial class About : ReactiveUserControl<AboutPageViewModel>
{
    public About()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}