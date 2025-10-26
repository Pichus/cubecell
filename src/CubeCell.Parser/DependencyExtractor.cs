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

        var inputStream = new AntlrInputStream(formula);

        var lexer = new CubeCellLexer(inputStream);

        var tokenStream = new CommonTokenStream(lexer);

        var parser = new CubeCellParser(tokenStream);

        CubeCellParser.ExpressionContext? context = parser.expression();

        Visit(context);

        return _dependencies;
    }

    public override object VisitCellRefExpr([NotNull] CubeCellParser.CellRefExprContext context)
    {
        var cellAddress = context.CELL_REF().GetText().ToUpperInvariant();
        _dependencies.Add(cellAddress);
        return null;
    }
}
