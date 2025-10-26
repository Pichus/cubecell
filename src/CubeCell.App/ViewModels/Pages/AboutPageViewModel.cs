using System.Reactive;

using CubeCell.App.ViewModels.Abstractions;

using ReactiveUI;

namespace CubeCell.App.ViewModels.Pages;

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