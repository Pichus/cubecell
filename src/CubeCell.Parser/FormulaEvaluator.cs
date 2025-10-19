using Antlr4.Runtime;

namespace CubeCell.Parser;

public class FormulaEvaluator
{
    public object Evaluate(string formula)
    {
        // Remove the leading '=' if present
        if (formula.StartsWith("=")) formula = formula.Substring(1);

        // Create input stream
        var inputStream = new AntlrInputStream(formula);

        // Create lexer
        var lexer = new CubeCellLexer(inputStream);

        // Create token stream
        var tokenStream = new CommonTokenStream(lexer);

        // Create parser
        var parser = new CubeCellParser(tokenStream);

        // Parse the expression
        var context = parser.expression();

        // Create visitor and evaluate
        var visitor = new FormulaVisitor();
        return visitor.Visit(context);
    }
}