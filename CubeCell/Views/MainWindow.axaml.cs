using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using CubeCell.Views.Components;

namespace CubeCell.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        MySpreadSheet.CellValueChanged += OnCellValueChanged;
    }
    
    private void OnCellValueChanged(object? sender, CellValueChangedEventArgs e)
    {
        Console.WriteLine($"Cell [{e.Column}, {e.Row}] changed from '{e.OldValue}' to '{e.NewValue}'");
    }
}