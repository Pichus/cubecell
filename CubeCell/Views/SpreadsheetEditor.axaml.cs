using Avalonia.ReactiveUI;
using CubeCell.ViewModels;

namespace CubeCell.Views;

public partial class SpreadsheetEditor : ReactiveUserControl<SpreadsheetEditorPageViewModel>
{
    public SpreadsheetEditor()
    {
        InitializeComponent();
    }
}