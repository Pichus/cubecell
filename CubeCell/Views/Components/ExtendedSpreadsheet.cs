using System;
using Avalonia;
using Spreadalonia;

namespace CubeCell.Views.Components;

public class ExtendedSpreadsheet : Spreadsheet
{
    private (int Column, int Row)? _lastEditingCell;
    private string? _lastEditingValue;

    public event EventHandler<CellValueChangedEventArgs>? CellValueChanged;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property != IsEditingProperty) return;
        
        var isEditing = change.GetNewValue<bool>();

        if (isEditing && Selection.Count > 0)
        {
            var lastSelectedCell = Selection[^1];
            _lastEditingCell = (lastSelectedCell.Left, lastSelectedCell.Top);
            Data.TryGetValue(_lastEditingCell.Value, out _lastEditingValue);
        }
        else if (_lastEditingCell is not null)
        {
            string? newValue;
            Data.TryGetValue(_lastEditingCell.Value, out newValue);

            if (_lastEditingValue != newValue)
                CellValueChanged?.Invoke(this, new CellValueChangedEventArgs(
                    _lastEditingCell.Value.Column,
                    _lastEditingCell.Value.Row,
                    _lastEditingValue,
                    newValue));

            _lastEditingCell = null;
            _lastEditingValue = null;
        }
    }
}

public class CellValueChangedEventArgs : EventArgs
{
    public CellValueChangedEventArgs(int col, int row, string? oldValue, string? newValue)
    {
        Column = col;
        Row = row;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public int Column { get; }
    public int Row { get; }
    public string? OldValue { get; }
    public string? NewValue { get; }
}