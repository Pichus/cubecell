using Antlr4.Runtime.Misc;

namespace CubeCell.Parser;

public class FormulaVisitor : CubeCellBaseVisitor<object>
{
    public override object VisitNumberExpr([NotNull] CubeCellParser.NumberExprContext context)
    {
        return double.Parse(context.GetText());
    }

    public override object VisitAddSubExpr([NotNull] CubeCellParser.AddSubExprContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        if (left is double l && right is double r)
        {
            if (context.GetChild(1).GetText() == "+")
                return l + r;
            return l - r;
        }

        throw new InvalidOperationException("Invalid operands for addition/subtraction");
    }

    public override object VisitMulDivExpr([NotNull] CubeCellParser.MulDivExprContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        if (left is double l && right is double r)
        {
            if (context.GetChild(1).GetText() == "*")
                return l * r;
            return l / r;
        }

        throw new InvalidOperationException("Invalid operands for multiplication/division");
    }

    public override object VisitPowerExpr([NotNull] CubeCellParser.PowerExprContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));

        if (left is double l && right is double r) return Math.Pow(l, r);

        throw new InvalidOperationException("Invalid operands for power");
    }

    public override object VisitParenExpr([NotNull] CubeCellParser.ParenExprContext context)
    {
        return Visit(context.expression());
    }

    // Add more visitor methods for other expression types as needed
}