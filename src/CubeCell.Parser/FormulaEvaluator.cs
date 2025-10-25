using Antlr4.Runtime;

namespace CubeCell.Parser;

public class FormulaEvaluator
{
    private readonly Func<string, object?> _getCellValue;

    public FormulaEvaluator(Func<string, object?> getCellValue)
    {
        _getCellValue = getCellValue;
    }

    public object Evaluate(string formula)
    {
        if (formula.StartsWith("=")) formula = formula.Substring(1);

        var inputStream = new AntlrInputStream(formula);

        var lexer = new CubeCellLexer(inputStream);

        var tokenStream = new CommonTokenStream(lexer);

        var parser = new CubeCellParser(tokenStream);

        var context = parser.expression();

        var visitor = new FormulaVisitor(_getCellValue);
        return visitor.Visit(context);
    }
}