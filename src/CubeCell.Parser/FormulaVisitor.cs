using System.Numerics;

using Antlr4.Runtime.Misc;

using CubeCell.Parser.Grammar;

namespace CubeCell.Parser;

public class FormulaVisitor : CubeCellBaseVisitor<object>
{
    private readonly ICellValueProvider _cellValueProvider;

    public FormulaVisitor(ICellValueProvider cellValueProvider)
    {
        _cellValueProvider = cellValueProvider;
    }

    public object Evaluate(CubeCellParser.FormulaContext ctx)
    {
        return Visit(ctx);
    }

    public override object VisitNumberExpr([NotNull] CubeCellParser.NumberExprContext context)
    {
        string? text = context.NUMBER().GetText();
        return BigInteger.Parse(text);
    }

    public override object VisitCellRefExpr([NotNull] CubeCellParser.CellRefExprContext context)
    {
        string name = context.CELL_REF().GetText().ToUpperInvariant();
        return _cellValueProvider.GetCellValueByAddress(name) ?? "";
    }

    public override object VisitAddSubExpr([NotNull] CubeCellParser.AddSubExprContext context)
    {
        BigInteger left = ToNumber(Visit(context.expression(0)));
        BigInteger right = ToNumber(Visit(context.expression(1)));
        string? op = context.GetChild(1).GetText();

        return op switch
        {
            "+" => left + right,
            "-" => left - right,
            _ => throw new Exception($"Unknown operator {op}")
        };
    }

    public override object VisitMulDivModExpr([NotNull] CubeCellParser.MulDivModExprContext context)
    {
        BigInteger left = ToNumber(Visit(context.expression(0)));
        BigInteger right = ToNumber(Visit(context.expression(1)));
        string op = context.GetChild(1).GetText().ToLowerInvariant();

        return op switch
        {
            "*" => left * right,
            "/" => right == 0 ? throw new DivideByZeroException() : left / right,
            "div" => right == 0 ? throw new DivideByZeroException() : left / right,
            "mod" => right == 0 ? throw new DivideByZeroException() : left % BigInteger.Abs(right),
            _ => throw new Exception($"Unknown operator {op}")
        };
    }

    public override object VisitPowerExpr([NotNull] CubeCellParser.PowerExprContext context)
    {
        BigInteger baseValue = ToNumber(Visit(context.expression(0)));
        BigInteger exponent = ToNumber(Visit(context.expression(1)));

        if (exponent < 0)
        {
            throw new ArithmeticException("Negative exponents not supported for integers");
        }

        if (exponent > int.MaxValue)
        {
            throw new ArithmeticException("Exponent too large");
        }

        return BigInteger.Pow(baseValue, (int)exponent);
    }


    public override object VisitUnaryPlusMinusExpr([NotNull] CubeCellParser.UnaryPlusMinusExprContext context)
    {
        BigInteger value = ToNumber(Visit(context.expression()));
        string? op = context.GetChild(0).GetText();

        return op == "-" ? -value : value;
    }

    public override object VisitIncExpr([NotNull] CubeCellParser.IncExprContext context)
    {
        BigInteger value = ToNumber(Visit(context.expression()));
        return value + 1;
    }

    public override object VisitDecExpr([NotNull] CubeCellParser.DecExprContext context)
    {
        BigInteger value = ToNumber(Visit(context.expression()));
        return value - 1;
    }

    public override object VisitNotExpr([NotNull] CubeCellParser.NotExprContext context)
    {
        object? value = Visit(context.expression());
        return !ToBool(value);
    }


    public override object VisitComparisonExpr([NotNull] CubeCellParser.ComparisonExprContext context)
    {
        BigInteger left = ToNumber(Visit(context.expression(0)));
        BigInteger right = ToNumber(Visit(context.expression(1)));
        string? op = context.GetChild(1).GetText();

        bool result = op switch
        {
            "=" => left == right,
            "<>" => left != right,
            "<" => left < right,
            ">" => left > right,
            "<=" => left <= right,
            ">=" => left >= right,
            _ => throw new Exception($"Unknown comparison operator {op}")
        };

        return result;
    }


    public override object VisitAndExpr([NotNull] CubeCellParser.AndExprContext context)
    {
        object? left = Visit(context.expression(0));
        object? right = Visit(context.expression(1));
        return ToBool(left) && ToBool(right);
    }

    public override object VisitOrExpr([NotNull] CubeCellParser.OrExprContext context)
    {
        object? left = Visit(context.expression(0));
        object? right = Visit(context.expression(1));
        return ToBool(left) || ToBool(right);
    }

    public override object VisitEqvExpr([NotNull] CubeCellParser.EqvExprContext context)
    {
        object? left = Visit(context.expression(0));
        object? right = Visit(context.expression(1));
        bool leftBool = ToBool(left);
        bool rightBool = ToBool(right);
        return leftBool == rightBool;
    }


    public override object VisitMaxExpr([NotNull] CubeCellParser.MaxExprContext context)
    {
        BigInteger left = ToNumber(Visit(context.expression(0)));
        BigInteger right = ToNumber(Visit(context.expression(1)));
        return BigInteger.Max(left, right);
    }

    public override object VisitMinExpr([NotNull] CubeCellParser.MinExprContext context)
    {
        BigInteger left = ToNumber(Visit(context.expression(0)));
        BigInteger right = ToNumber(Visit(context.expression(1)));
        return BigInteger.Min(left, right);
    }

    public override object VisitMmaxExpr([NotNull] CubeCellParser.MmaxExprContext context)
    {
        BigInteger? max = null;
        foreach (CubeCellParser.ExpressionContext? expr in context.expressionList().expression())
        {
            BigInteger value = ToNumber(Visit(expr));
            if (max == null || value > max)
            {
                max = value;
            }
        }

        return max ?? BigInteger.Zero;
    }

    public override object VisitMminExpr([NotNull] CubeCellParser.MminExprContext context)
    {
        BigInteger? min = null;
        foreach (CubeCellParser.ExpressionContext? expr in context.expressionList().expression())
        {
            BigInteger value = ToNumber(Visit(expr));
            if (min == null || value < min)
            {
                min = value;
            }
        }

        return min ?? BigInteger.Zero;
    }


    public override object VisitParenExpr([NotNull] CubeCellParser.ParenExprContext context)
    {
        return Visit(context.expression());
    }


    private static BigInteger ToNumber(object value)
    {
        return value switch
        {
            BigInteger b => b,
            int i => new BigInteger(i),
            long l => new BigInteger(l),
            bool b => b ? BigInteger.One : BigInteger.Zero,
            string s when BigInteger.TryParse(s, out BigInteger n) => n,
            _ => BigInteger.Zero
        };
    }

    private static bool ToBool(object value)
    {
        return value switch
        {
            bool b => b,
            BigInteger bi => bi != BigInteger.Zero,
            int i => i != 0,
            long l => l != 0,
            string s => !string.IsNullOrEmpty(s),
            _ => false
        };
    }
}
