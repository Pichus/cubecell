using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace CubeCell.Parser;

public class DependencyExtractor : CubeCellBaseVisitor<object>
{
    private readonly HashSet<string> _dependencies = new();

    public HashSet<string> ExtractDependencies(string formula)
    {
        if (formula.StartsWith("="))
        {
            formula = formula.Substring(1);
        }

        AntlrInputStream inputStream = new(formula);

        CubeCellLexer lexer = new(inputStream);

        CommonTokenStream tokenStream = new(lexer);

        CubeCellParser parser = new(tokenStream);

        CubeCellParser.ExpressionContext? context = parser.expression();

        Visit(context);

        return _dependencies;
    }

    public override object VisitCellRefExpr([NotNull] CubeCellParser.CellRefExprContext context)
    {
        string cellAddress = context.CELL_REF().GetText().ToUpperInvariant();
        _dependencies.Add(cellAddress);
        return null;
    }
}
