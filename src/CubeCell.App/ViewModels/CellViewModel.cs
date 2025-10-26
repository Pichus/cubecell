using CubeCell.App.Models;

using ReactiveUI;

namespace CubeCell.App.ViewModels;

public class CellViewModel : ReactiveObject
{
    private Cell? _cellModel;

    public CellViewModel()
    {
    }

    public CellViewModel(Cell cellModel)
    {
        _cellModel = cellModel;
    }

    public string Value { get; private set; } = "";

    public string DisplayText
    {
        get => _cellModel?.Value ?? Value;
        set
        {
            Value = value;

            if (_cellModel is not null)
            {
                _cellModel.Value = value;
                _cellModel.Formula = value;
            }

            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(Formula));
        }
    }

    public string? Formula
    {
        get => _cellModel?.Formula ?? Value;
        set
        {
            Value = value ?? "";

            if (_cellModel is not null)
            {
                _cellModel.Value = value;
                _cellModel.Formula = value;
            }

            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(DisplayText));
        }
    }

    public void SetCellModel(Cell cellModel)
    {
        _cellModel = cellModel;
    }

    public void Refresh()
    {
        if (_cellModel is null)
        {
            return;
        }

        this.RaisePropertyChanged(nameof(DisplayText));
        this.RaisePropertyChanged(nameof(Formula));
    }
}
