using Antlr4.Runtime;

namespace CubeCell.Parser;

public class FormulaEvaluator
{
    private readonly ICellValueProvider _cellValueProvider;

    public FormulaEvaluator(ICellValueProvider cellValueProvider)
    {
        _cellValueProvider = cellValueProvider;
    }

    public object Evaluate(string formula)
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

        var visitor = new FormulaVisitor(_cellValueProvider);
        return visitor.Visit(context);
    }
}
