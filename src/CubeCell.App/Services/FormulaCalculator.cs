using System;

using CubeCell.Parser;

namespace CubeCell.App.Services;

public class FormulaCalculator
{
    private readonly FormulaEvaluator _formulaEvaluator;

    public FormulaCalculator(FormulaEvaluator formulaEvaluator)
    {
        _formulaEvaluator = formulaEvaluator;
    }

    public string Calculate(string formula)
    {
        if (!formula.StartsWith("="))
        {
            throw new ArgumentException("formula must start with a '='");
        }

        string calculatedValue;
        try
        {
            calculatedValue = _formulaEvaluator.Evaluate(formula).ToString() ?? "";
        }
        catch (Exception exception)
        {
            calculatedValue = "#ERROR";
        }

        return calculatedValue;
    }
}
