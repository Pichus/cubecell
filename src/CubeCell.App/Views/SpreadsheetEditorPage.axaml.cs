using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using CubeCell.App.Models;
using CubeCell.App.ViewModels;
using ReactiveUI;

namespace CubeCell.App.Views;

public partial class SpreadsheetEditorPage : ReactiveUserControl<SpreadsheetEditorPageViewModel>
{
    private const int DefaultCellWidth = 80;
    private const int DefaultCellHeight = 28;

    public SpreadsheetEditorPage()
    {
        InitializeComponent(); // or AvaloniaXamlLoader.Load(this);

        this.WhenActivated(_ => InitializeGrid(ViewModel.ColCount, ViewModel.RowCount));
    }

    private void InitializeGrid(int rows, int cols)
    {
        CellsGrid.RowDefinitions.Clear();
        CellsGrid.ColumnDefinitions.Clear();
        RowHeaderGrid.RowDefinitions.Clear();
        ColumnHeaderGrid.ColumnDefinitions.Clear();
        CellsGrid.Children.Clear();
        RowHeaderGrid.Children.Clear();
        ColumnHeaderGrid.Children.Clear();

        // Define grid structure — make new instances for each grid
        for (var i = 0; i < rows; i++)
        {
            CellsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(DefaultCellHeight)));
            RowHeaderGrid.RowDefinitions.Add(new RowDefinition(new GridLength(DefaultCellHeight)));
        }

        for (var j = 0; j < cols; j++)
        {
            CellsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(DefaultCellWidth)));
            ColumnHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(DefaultCellWidth)));
        }

        // === Column Headers ===
        for (var col = 0; col < cols; col++)
        {
            var header = new Border
            {
                Background = Brush.Parse("#F3F3F3"),
                BorderBrush = Brush.Parse("#D0D0D0"),
                BorderThickness = new Thickness(0, 0, 1, 1),
                Padding = new Thickness(2, 1, 1, 1),
                Child = new TextBlock
                {
                    Text = GetColumnName(col),
                    FontWeight = FontWeight.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brush.Parse("#333333")
                }
            };

            ColumnHeaderGrid.Children.Add(header);
            Grid.SetColumn(header, col);
        }

        // === Row Headers ===
        for (var row = 0; row < rows; row++)
        {
            var header = new Border
            {
                Background = Brush.Parse("#F3F3F3"),
                BorderBrush = Brush.Parse("#D0D0D0"),
                BorderThickness = new Thickness(0, 0, 1, 1),
                Padding = new Thickness(2, 0, 2, 0),
                Child = new TextBlock
                {
                    Text = (row + 1).ToString(),
                    FontWeight = FontWeight.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brush.Parse("#333333")
                }
            };

            RowHeaderGrid.Children.Add(header);
            Grid.SetRow(header, row);
        }

        // Fill cells manually
        for (var row = 0; row < rows; row++)
        for (var col = 0; col < cols; col++)
        {
            var cell = new Border
            {
                Padding = new Thickness(2),
                BorderBrush = Brush.Parse("#D0D0D0"),
                BorderThickness = new Thickness(0, 0, 1, 1),
                Background = Brushes.White,
                UseLayoutRounding = true,
                Child = new TextBox
                {
                    Text = $"{(char)('A' + col)}{row + 1}",
                    CornerRadius = new CornerRadius(0),
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(0),
                    Padding = new Thickness(4, 0, 4, 0),
                    Background = Brushes.White,
                    Foreground = Brushes.Black,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    FontSize = 14,
                    MinHeight = 0,
                    MinWidth = 0
                }
            };

            Grid.SetRow(cell, row);
            Grid.SetColumn(cell, col);

            CellsGrid.Children.Add(cell);
        }
    }

    private static string GetColumnName(int index)
    {
        var name = "";
        while (index >= 0)
        {
            name = (char)('A' + index % 26) + name;
            index = index / 26 - 1;
        }

        return name;
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