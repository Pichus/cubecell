using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;

using CubeCell.App.Models;
using CubeCell.App.Services;
using CubeCell.App.ViewModels.Abstractions;

using ReactiveUI;

using File = Google.Apis.Drive.v3.Data.File;

namespace CubeCell.App.ViewModels.Pages;

public class WelcomePageViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly ICloudSpreadsheetPersistenceService _driveService;
    private readonly ISpreadsheetImportService _spreadsheetImportService;

    private int _colCount = 26;
    private int _rowCount = 100;

    private bool _userAuthorized;

    public WelcomePageViewModel(IScreen screen)
    {
        HostScreen = screen;
        _driveService = new GoogleDriveSpreadsheetPersistenceService();
        _spreadsheetImportService = new SpreadsheetImportService();

        GoToAboutPageCommand = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new AboutPageViewModel(HostScreen))
        );

        GoToSpreadSheetEditorPageCommand =
            ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(
                    new SpreadsheetEditorPageViewModel(HostScreen, _rowCount, _colCount)));

        OpenLocalFileCommand = ReactiveCommand.Create<string>(OpenFileCommandHandler);
        SignInWithGoogleCommand = ReactiveCommand.CreateFromTask(SignInWithGoogleCommandHandler);
        OpenCloudFileCommand = ReactiveCommand.CreateFromTask<FileViewModel>(OpenCloudFileCommandHandler);
    }

    public bool UserAuthorized
    {
        get => _userAuthorized;
        set => this.RaiseAndSetIfChanged(ref _userAuthorized, value);
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
    public ReactiveCommand<Unit, Unit> SignInWithGoogleCommand { get; }

    public ObservableCollection<FileViewModel> FileViewModels { get; } = [];

    public ReactiveCommand<string, Unit> OpenLocalFileCommand { get; }
    public ReactiveCommand<FileViewModel, Unit> OpenCloudFileCommand { get; }


    public string? UrlPathSegment { get; } = "welcome";
    public IScreen HostScreen { get; }

    private async Task OpenCloudFileCommandHandler(FileViewModel fileViewModel)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), fileViewModel.Name);
        await _driveService.DownloadSpreadsheetFromCloud(fileViewModel.Id, tempPath);
        OpenFileCommandHandler(tempPath);
    }

    private void OpenFileCommandHandler(string filePath)
    {
        var spreadsheet = new Spreadsheet();
        SpreadsheetSize spreadsheetSize = _spreadsheetImportService.LoadSpreadsheetFromFile(spreadsheet, filePath);
        HostScreen.Router.Navigate.Execute(
            new SpreadsheetEditorPageViewModel(HostScreen, spreadsheetSize.RowCount, spreadsheetSize.ColCount,
                spreadsheet, filePath));
    }

    private async Task SignInWithGoogleCommandHandler()
    {
        IEnumerable<File> files = await _driveService.GetSpreadsheetsStoredInCloudByUserAsync();
        foreach (File file in files)
        {
            FileViewModels.Add(new FileViewModel { Id = file.Id, Name = file.Name });
        }
    }
}
