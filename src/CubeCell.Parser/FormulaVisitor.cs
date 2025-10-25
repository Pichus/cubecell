using System;
using System.Collections.Generic;
using System.Globalization;
using Antlr4.Runtime.Misc;

public class FormulaVisitor : CubeCellBaseVisitor<object>
{
    private readonly Func<string, object?> _getCellValue;

    public FormulaVisitor(Func<string, object?>? getCellValue = null)
    {
        _getCellValue = getCellValue ?? (_ => 0);
    }

    public object Evaluate(CubeCellParser.FormulaContext ctx)
        => Visit(ctx);

    // ---------------- Literals ----------------

    public override object VisitNumberExpr(CubeCellParser.NumberExprContext ctx)
    {
        var text = ctx.number().GetText();
        return double.Parse(text, CultureInfo.InvariantCulture);
    }

    public override object VisitStringExpr(CubeCellParser.StringExprContext ctx)
    {
        var str = ctx.@string().GetText();
        return str.Substring(1, str.Length - 2);
    }

    public override object VisitCellRefExpr(CubeCellParser.CellRefExprContext ctx)
    {
        var name = ctx.cellReference().GetText();
        return _getCellValue(name) ?? 0;
    }

    // ---------------- Arithmetic ----------------

    public override object VisitAddSubExpr(CubeCellParser.AddSubExprContext ctx)
    {
        var left = ToNumber(Visit(ctx.expression(0)));
        var right = ToNumber(Visit(ctx.expression(1)));
        var op = ctx.GetChild(1).GetText();

        return op switch
        {
            "+" => left + right,
            "-" => left - right,
            _ => throw new Exception($"Unknown operator {op}")
        };
    }

    public override object VisitMulDivExpr(CubeCellParser.MulDivExprContext ctx)
    {
        var left = ToNumber(Visit(ctx.expression(0)));
        var right = ToNumber(Visit(ctx.expression(1)));
        var op = ctx.GetChild(1).GetText();

        return op switch
        {
            "*" => left * right,
            "/" => left / right,
            _ => throw new Exception($"Unknown operator {op}")
        };
    }

    public override object VisitPowerExpr(CubeCellParser.PowerExprContext ctx)
    {
        var left = ToNumber(Visit(ctx.expression(0)));
        var right = ToNumber(Visit(ctx.expression(1)));
        return Math.Pow(left, right);
    }

    // ---------------- Comparisons ----------------

    public override object VisitComparisonExpr(CubeCellParser.ComparisonExprContext ctx)
    {
        var left = Visit(ctx.expression(0));
        var right = Visit(ctx.expression(1));
        var op = ctx.GetChild(1).GetText();

        double ln = ToNumber(left);
        double rn = ToNumber(right);

        return op switch
        {
            "=" => ln == rn,
            "<" => ln < rn,
            ">" => ln > rn,
            _ => throw new Exception($"Unknown comparison operator {op}")
        };
    }

    public override object VisitComparisonExpr2(CubeCellParser.ComparisonExpr2Context ctx)
    {
        var left = Visit(ctx.expression(0));
        var right = Visit(ctx.expression(1));
        var op = ctx.GetChild(1).GetText();

        double ln = ToNumber(left);
        double rn = ToNumber(right);

        return op switch
        {
            "<=" => ln <= rn,
            ">=" => ln >= rn,
            "<>" => ln != rn,
            _ => throw new Exception($"Unknown comparison operator {op}")
        };
    }

    // ---------------- Logical ----------------

    public override object VisitNotExpr(CubeCellParser.NotExprContext ctx)
    {
        var val = Visit(ctx.expression());
        return !ToBool(val);
    }

    public override object VisitLogicalExpr(CubeCellParser.LogicalExprContext ctx)
    {
        var left = Visit(ctx.expression(0));
        var right = Visit(ctx.expression(1));
        var op = ctx.GetChild(1).GetText().ToUpperInvariant();

        return op switch
        {
            "AND" => ToBool(left) && ToBool(right),
            "OR"  => ToBool(left) || ToBool(right),
            _ => throw new Exception($"Unknown logical operator {op}")
        };
    }

    // ---------------- Functions ----------------

    public override object VisitFunctionExpr(CubeCellParser.FunctionExprContext ctx)
    {
        string name = ctx.functionCall().IDENTIFIER().GetText().ToUpperInvariant();
        var args = new List<double>();

        foreach (var e in ctx.functionCall().expression())
            args.Add(ToNumber(Visit(e)));

        return name switch
        {
            "MAX"  => args.Count == 0 ? 0 : args.Max(),
            "MIN"  => args.Count == 0 ? 0 : args.Min(),
            "MMAX" => args.Count == 0 ? 0 : args.Max(),
            "MMIN" => args.Count == 0 ? 0 : args.Min(),
            _ => throw new Exception($"Unknown function {name}")
        };
    }

    // ---------------- Parentheses ----------------

    public override object VisitParenExpr(CubeCellParser.ParenExprContext ctx)
        => Visit(ctx.expression());

    // ---------------- Helpers ----------------

    private static double ToNumber(object value)
    {
        return value switch
        {
            double d => d,
            int i => i,
            bool b => b ? 1 : 0,
            string s when double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var n) => n,
            _ => 0
        };
    }

    private static bool ToBool(object value)
    {
        return value switch
        {
            bool b => b,
            double d => d != 0,
            int i => i != 0,
            string s => !string.IsNullOrEmpty(s),
            _ => false
        };
    }
}
