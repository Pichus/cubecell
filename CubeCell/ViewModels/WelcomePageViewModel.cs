using System.Reactive;
using ReactiveUI;

namespace CubeCell.ViewModels;

public class WelcomePageViewModel : ViewModelBase, IRoutableViewModel
{
    public WelcomePageViewModel(IScreen screen)
    {
        HostScreen = screen;

        GoToAboutPageCommand =
            ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new AboutPageViewModel(HostScreen)));
        
        GoToSpreadSheetEditorPageCommand =
            ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new SpreadsheetEditorPageViewModel(HostScreen)));
    }

    public ReactiveCommand<Unit, IRoutableViewModel> GoToAboutPageCommand { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoToSpreadSheetEditorPageCommand { get; }

    public string? UrlPathSegment { get; } = "welcome";
    public IScreen HostScreen { get; }
}