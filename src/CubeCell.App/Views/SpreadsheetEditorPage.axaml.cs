using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CubeCell.App.Models;
using CubeCell.App.ViewModels;
using ReactiveUI;

namespace CubeCell.App.Views;

public partial class SpreadsheetEditorPage : ReactiveUserControl<SpreadsheetEditorPageViewModel>
{
    public SpreadsheetEditorPage()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    private void CellInput_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SpreadsheetEditorPageViewModel vm)
            if (sender is TextBox button && button.DataContext is CellModel cell)
                vm.CalculateCellFormulaCommand.Execute(cell).Subscribe();
    }

    private void CellInput_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (DataContext is SpreadsheetEditorPageViewModel vm)
            if (sender is TextBox button && button.DataContext is CellModel cell)
                vm.SelectedCell = cell;
    }

    private void FormulaInput_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SpreadsheetEditorPageViewModel vm)
            vm.CalculateSelectedCellFormulaCommand.Execute().Subscribe();
    }
}