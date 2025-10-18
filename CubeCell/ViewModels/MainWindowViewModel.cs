using ReactiveUI;

namespace CubeCell.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public MainWindowViewModel()
    {
        Router.Navigate.Execute(new WelcomePageViewModel(this));
    }

    public RoutingState Router { get; } = new();
}