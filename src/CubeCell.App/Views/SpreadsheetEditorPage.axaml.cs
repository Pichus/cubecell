using System;
using System.Reactive.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;

using CubeCell.App.Models;
using CubeCell.App.Utils;
using CubeCell.App.ViewModels;
using CubeCell.App.ViewModels.Pages;

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
        ResetGrids();

        DefineGridStructure(rowCount, colCount);

        FillRowHeadersGrid(rowCount);

        FillColHeadersGrid(colCount);

        FillCellsGrid(rowCount, colCount);
    }

    private void ResetGrids()
    {
        CellsGrid.RowDefinitions.Clear();
        CellsGrid.ColumnDefinitions.Clear();
        RowHeaderGrid.RowDefinitions.Clear();
        ColumnHeaderGrid.ColumnDefinitions.Clear();
        CellsGrid.Children.Clear();
        RowHeaderGrid.Children.Clear();
        ColumnHeaderGrid.Children.Clear();
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
            Border header = CreateRowHeaderElement((row + 1).ToString());

            RowHeaderGrid.Children.Add(header);
            Grid.SetRow(header, row);
        }
    }

    private void FillColHeadersGrid(int colCount)
    {
        for (var col = 0; col < colCount; col++)
        {
            Border header = CreateColHeaderElement(CellAddressUtils.ColumnIndexToLetters(col));

            ColumnHeaderGrid.Children.Add(header);
            Grid.SetColumn(header, col);
        }
    }

    private void FillCellsGrid(int rowCount, int colCount)
    {
        for (var row = 0; row < rowCount; row++)
        {
            for (var col = 0; col < colCount; col++)
            {
                Border cellElement = CreateCellElement();

                CellViewModel? cellViewModelForBinding =
                    ViewModel?.GetCellViewModelForBinding(new CellCoordinates(col, row));

                var textBox = (TextBox)cellElement.Child!;

                textBox.Bind(TextBox.TextProperty,
                    new Binding { Path = "DisplayText", Mode = BindingMode.TwoWay, Source = cellViewModelForBinding });

                textBox.LostFocus += CellInput_OnLostFocus;
                textBox.GotFocus += CellInput_OnGotFocus;

                Grid.SetRow(cellElement, row);
                Grid.SetColumn(cellElement, col);

                CellsGrid.Children.Add(cellElement);
            }
        }
    }

    private static Border CreateRowHeaderElement(string text)
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

    private static Border CreateColHeaderElement(string text)
    {
        return new Border
        {
            Background = Brush.Parse("#F3F3F3"),
            BorderBrush = Brush.Parse("#D0D0D0"),
            BorderThickness = new Thickness(0, 0, 1, 1),
            Padding = new Thickness(2, 1, 1, 1),
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

    private Border CreateCellElement(string text = "")
    {
        var textBox = new TextBox
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
        };

        var border = new Border
        {
            Padding = new Thickness(2),
            BorderBrush = Brush.Parse("#D0D0D0"),
            BorderThickness = new Thickness(0, 0, 1, 1),
            Background = Brushes.White,
            UseLayoutRounding = true,
            Child = textBox
        };

        return border;
    }

    private async void CellInput_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SpreadsheetEditorPageViewModel vm)
        {
            if (sender is TextBox cellInputTextBox && cellInputTextBox.Parent is Border cellInput)
            {
                if (!string.IsNullOrEmpty(cellInputTextBox.Text))
                {
                    var column = cellInput.GetValue(Grid.ColumnProperty);
                    var row = cellInput.GetValue(Grid.RowProperty);
                    var cellCoordinates = new CellCoordinates(column, row);
                    await vm.AttachCellModelToCellViewModelCommand.Execute(cellCoordinates);

                    await vm.CalculateCellFormulaCommand.Execute(cellCoordinates);
                }
            }
        }
    }

    private async void CellInput_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (DataContext is SpreadsheetEditorPageViewModel vm)
        {
            if (sender is TextBox cellInputTextBox && cellInputTextBox.Parent is Border cellInput)
            {
                var column = cellInput.GetValue(Grid.ColumnProperty);
                var row = cellInput.GetValue(Grid.RowProperty);
                var cellCoordinates = new CellCoordinates(column, row);
                await vm.UpdateLastSelectedCellCommand.Execute(cellCoordinates);
            }
        }
    }

    private async void FormulaInput_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SpreadsheetEditorPageViewModel vm)
        {
            if (sender is TextBox formulaInputTextBox)
            {
                if (!string.IsNullOrEmpty(formulaInputTextBox.Text))
                {
                    await vm.AttachCellModelToLastSelectedCellViewModelCommand.Execute();

                    await vm.CalculateSelectedCellFormulaCommand.Execute();
                }
            }
        }
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is null || ViewModel.CurrentSpreadSheetFileLocation is null)
        {
            ExportAsButton_OnClick(sender, e);
            return;
        }

        await ViewModel.SaveCommand.Execute();
    }

    private async void ExportAsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
        {
            return;
        }

        IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save cubecell spreadsheet",
            SuggestedFileName = ViewModel?.SpreadSheetName,
            DefaultExtension = "cubecell.xlsx",
            ShowOverwritePrompt = true,
            FileTypeChoices =
            [
                new FilePickerFileType("Cubecell Spreadsheet")
                {
                    Patterns = ["*.cubecell.xlsx"],
                    AppleUniformTypeIdentifiers = ["org.openxmlformats.spreadsheetml.sheet"],
                    MimeTypes = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"]
                }
            ]
        });

        if (file is not null && ViewModel is not null)
        {
            await ViewModel.ExportAsCommand.Execute(new ExportAsRequest(file.Path.LocalPath, file.Name));
        }
    }
}
