using ReactiveUI;

namespace CubeCell.App.Models;

public class CellModel : ReactiveObject
{
    private string? _formula;
    private string? _lastCalculatedFormula;
    private string? _value;
    public string Address { get; set; } = "";

    public string? Value
    {
        get => _value;
        set
        {
            this.RaiseAndSetIfChanged(ref _value, value);
            this.RaisePropertyChanged(nameof(DisplayText));
        }
    }

    public string? Formula
    {
        get => _formula;
        set
        {
            this.RaiseAndSetIfChanged(ref _formula, value);
            this.RaisePropertyChanged(nameof(DisplayText));
        }
    }

    public string? DisplayText
    {
        get => Value;
        set => Formula = value;
    }
    
    public bool NeedsRecalculation => _formula != _lastCalculatedFormula;
    
    public void MarkAsCalculated() => _lastCalculatedFormula = _formula;
}