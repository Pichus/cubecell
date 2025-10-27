using System.Reactive;

using CubeCell.App.Models;
using CubeCell.App.Service;
using CubeCell.App.ViewModels.Abstractions;

using ReactiveUI;

namespace CubeCell.App.ViewModels.Pages;

public class WelcomePageViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly ISpreadsheetImportService _spreadsheetImportService;

    private int _colCount = 26;
    private int _rowCount = 100;

    public WelcomePageViewModel(IScreen screen)
    {
        HostScreen = screen;
        _spreadsheetImportService = new SpreadsheetImportService();

        GoToAboutPageCommand = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new AboutPageViewModel(HostScreen))
        );

        GoToSpreadSheetEditorPageCommand =
            ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(
                    new SpreadsheetEditorPageViewModel(HostScreen, _rowCount, _colCount)));

        OpenFileCommand = ReactiveCommand.Create<string>(OpenFileCommandHandler);
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

    public ReactiveCommand<string, Unit> OpenFileCommand { get; }


    public string? UrlPathSegment { get; } = "welcome";
    public IScreen HostScreen { get; }

    private void OpenFileCommandHandler(string filePath)
    {
        var spreadsheet = new Spreadsheet();
        SpreadsheetSize spreadsheetSize = _spreadsheetImportService.LoadSpreadsheetFromFile(spreadsheet, filePath);
        HostScreen.Router.Navigate.Execute(
            new SpreadsheetEditorPageViewModel(HostScreen, spreadsheetSize.RowCount, spreadsheetSize.ColCount,
                spreadsheet, filePath));
    }
}
