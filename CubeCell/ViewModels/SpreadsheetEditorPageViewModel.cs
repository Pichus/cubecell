using ReactiveUI;

namespace CubeCell.ViewModels;

public class SpreadsheetEditorPageViewModel : ViewModelBase, IRoutableViewModel
{
    public SpreadsheetEditorPageViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

    public string? UrlPathSegment { get; } = "spreadsheetEditor";
    public IScreen HostScreen { get; }
}