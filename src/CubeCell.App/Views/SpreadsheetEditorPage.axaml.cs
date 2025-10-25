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
        InitializeComponent();

        this.WhenActivated(_ =>
        {
            if (ViewModel is null)
            {
                throw new NullReferenceException("View model in unexpectedly null");
            }

            InitializeGrid(ViewModel.ColCount, ViewModel.RowCount);
        });
    }

    private void InitializeGrid(int rowCount, int colCount)
    {
        CellsGrid.RowDefinitions.Clear();
        CellsGrid.ColumnDefinitions.Clear();
        RowHeaderGrid.RowDefinitions.Clear();
        ColumnHeaderGrid.ColumnDefinitions.Clear();
        CellsGrid.Children.Clear();
        RowHeaderGrid.Children.Clear();
        ColumnHeaderGrid.Children.Clear();

        DefineGridStructure(rowCount, colCount);

        FillRowHeadersGrid(rowCount);

        FillColHeadersGrid(colCount);

        FillCellsGrid(rowCount, colCount);
    }

    private void FillCellsGrid(int rowCount, int colCount)
    {
        for (var row = 0; row < rowCount; row++)
        {
            for (var col = 0; col < colCount; col++)
            {
                Border cell = CreateCellElement(col, row, $"{(char)('A' + col)}{row + 1}");

                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);

                CellsGrid.Children.Add(cell);
            }
        }
    }

    private static Border CreateCellElement(int col, int row, string text)
    {
        return new Border
        {
            Padding = new Thickness(2),
            BorderBrush = Brush.Parse("#D0D0D0"),
            BorderThickness = new Thickness(0, 0, 1, 1),
            Background = Brushes.White,
            UseLayoutRounding = true,
            Child = new TextBox
            {
                Text = text,
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
    }

    private void DefineGridStructure(int rowCount, int colCount)
    {
        for (var i = 0; i < rowCount; i++)
        {
            CellsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(DefaultCellHeight)));
            RowHeaderGrid.RowDefinitions.Add(new RowDefinition(new GridLength(DefaultCellHeight)));
        }

        for (var j = 0; j < colCount; j++)
        {
            CellsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(DefaultCellWidth)));
            ColumnHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(DefaultCellWidth)));
        }
    }


    private void FillRowHeadersGrid(int rowCount)
    {
        for (var row = 0; row < rowCount; row++)
        {
            Border header = CreateRowHeaderElement(row, (row + 1).ToString());

            RowHeaderGrid.Children.Add(header);
            Grid.SetRow(header, row);
        }
    }

    private static Border CreateRowHeaderElement(int row, string text)
    {
        return new Border
        {
            Background = Brush.Parse("#F3F3F3"),
            BorderBrush = Brush.Parse("#D0D0D0"),
            BorderThickness = new Thickness(0, 0, 1, 1),
            Padding = new Thickness(2, 0, 2, 0),
            Child = new TextBlock
            {
                Text = text,
                FontWeight = FontWeight.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brush.Parse("#333333")
            }
        };
    }

    private void FillColHeadersGrid(int colCount)
    {
        for (var col = 0; col < colCount; col++)
        {
            Border header = CreateColHeaderElement(col);

            ColumnHeaderGrid.Children.Add(header);
            Grid.SetColumn(header, col);
        }
    }

    private static Border CreateColHeaderElement(int col)
    {
        return new Border
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
    }

    private static string GetColumnName(int index)
    {
        var name = "";
        while (index >= 0)
        {
            name = (char)('A' + (index % 26)) + name;
            index = (index / 26) - 1;
        }

        return name;
    }

    private void CellInput_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SpreadsheetEditorPageViewModel vm)
        {
            if (sender is TextBox button && button.DataContext is Cell cell)
            {
                vm.CalculateCellFormulaCommand.Execute(cell).Subscribe();
            }
        }
    }

    private void CellInput_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (DataContext is SpreadsheetEditorPageViewModel vm)
        {
            if (sender is TextBox button && button.DataContext is Cell cell)
            {
                vm.SelectedCell = cell;
            }
        }
    }

    private void FormulaInput_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SpreadsheetEditorPageViewModel vm)
        {
            vm.CalculateSelectedCellFormulaCommand.Execute().Subscribe();
        }
    }
}
