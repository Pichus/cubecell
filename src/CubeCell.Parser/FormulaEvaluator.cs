using Antlr4.Runtime;

namespace CubeCell.Parser;

public class FormulaEvaluator
{
    private readonly IReadonlyCellStorage _cellStorage;

    public FormulaEvaluator(IReadonlyCellStorage cellStorage)
    {
        _cellStorage = cellStorage;
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

        var visitor = new FormulaVisitor(_cellStorage);
        return visitor.Visit(context);
    }
}
