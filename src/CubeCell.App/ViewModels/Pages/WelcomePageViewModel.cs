using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;

using CubeCell.App.ViewModels.Abstractions;

using ReactiveUI;

namespace CubeCell.App.ViewModels.Pages;

public class WelcomePageViewModel : ViewModelBase, IRoutableViewModel
{
    private int _colCount = 26;

    private int _rowCount = 100;

    public WelcomePageViewModel(IScreen screen)
    {
        HostScreen = screen;

        GoToAboutPageCommand = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new AboutPageViewModel(HostScreen))
        );

        GoToSpreadSheetEditorPageCommand =
            ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(
                    new SpreadsheetEditorPageViewModel(HostScreen, _rowCount, _colCount)));
    }

    public int RowCount
    {
        get => _rowCount;
        set => this.RaiseAndSetIfChanged(ref _rowCount, value);
    }

    public int ColCount
    {
        get => _colCount;
        set => this.RaiseAndSetIfChanged(ref _colCount, value);
    }

    public ReactiveCommand<Unit, IRoutableViewModel> GoToAboutPageCommand { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoToSpreadSheetEditorPageCommand { get; }

    public bool IsGoogleAuthorized { get; set; }
    public string GoogleUserName { get; set; }
    public string GoogleUserEmail { get; set; }
    public string GoogleUserInitial { get; set; }
    public ObservableCollection<string> RecentGoogleFiles { get; set; }
    public ICommand GoogleSignInCommand { get; }
    public ICommand GoogleSignOutCommand { get; }
    public ICommand OpenGoogleFileCommand { get; }
    public ICommand RefreshGoogleFilesCommand { get; }

    public string? UrlPathSegment { get; } = "welcome";
    public IScreen HostScreen { get; }
}
