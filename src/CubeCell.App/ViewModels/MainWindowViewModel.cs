using CubeCell.App.ViewModels.Abstractions;
using CubeCell.App.ViewModels.Pages;

using ReactiveUI;

namespace CubeCell.App.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public MainWindowViewModel()
    {
        Router.Navigate.Execute(new WelcomePageViewModel(this));
    }

    public RoutingState Router { get; } = new();
}