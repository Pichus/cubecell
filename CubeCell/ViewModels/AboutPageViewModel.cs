using System.Reactive;
using ReactiveUI;

namespace CubeCell.ViewModels;

public class AboutPageViewModel : ViewModelBase, IRoutableViewModel
{
    public AboutPageViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

    public ReactiveCommand<Unit, IRoutableViewModel> GoBackCommand => HostScreen.Router.NavigateBack;

    public string? UrlPathSegment { get; } = "about";
    public IScreen HostScreen { get; }
}