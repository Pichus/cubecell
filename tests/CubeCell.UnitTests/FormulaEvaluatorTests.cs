using System.Numerics;

using CubeCell.Parser;

using FluentAssertions;

using Moq;

namespace CubeCell.UnitTests;

public class FormulaEvaluatorTests
{
    private readonly FormulaEvaluator _evaluator;
    private readonly Mock<ICellValueProvider> _providerMock;

    public FormulaEvaluatorTests()
    {
        _providerMock = new Mock<ICellValueProvider>(MockBehavior.Strict);
        _evaluator = new FormulaEvaluator(_providerMock.Object);
    }

    private object Eval(string formula)
    {
        return _evaluator.Evaluate(formula);
    }


    [Fact]
    public void Should_Add_Subtract_Multiply_And_Divide()
    {
        Eval("=1+2").Should().Be(new BigInteger(3));
        Eval("=5-2").Should().Be(new BigInteger(3));
        Eval("=3*2").Should().Be(new BigInteger(6));
        Eval("=8/2").Should().Be(new BigInteger(4));
    }

    [Fact]
    public void Should_Throw_On_DivideByZero()
    {
        Action act = () => Eval("=5/0");
        act.Should().Throw<DivideByZeroException>();
    }


    [Fact]
    public void Should_Respect_Precedence_And_Parentheses()
    {
        Eval("=1+2*3").Should().Be(new BigInteger(7));
        Eval("=(1+2)*3").Should().Be(new BigInteger(9));
    }


    [Fact]
    public void Should_Handle_Unary_And_Power()
    {
        Eval("=-5").Should().Be(new BigInteger(-5));
        Eval("=+5").Should().Be(new BigInteger(5));
        Eval("=2^3").Should().Be(new BigInteger(8));
    }

    [Fact]
    public void Should_Throw_On_Negative_Exponent()
    {
        Action act = () => Eval("=2^-1");
        act.Should().Throw<ArithmeticException>()
            .WithMessage("*Negative exponents*");
    }


    [Fact]
    public void Should_Handle_Logical_Expressions()
    {
        Eval("=1 and 0").Should().Be(false);
        Eval("=1 or 0").Should().Be(true);
        Eval("=not(1)").Should().Be(false);
        Eval("=not(0)").Should().Be(true);
    }


    [Fact]
    public void Should_Handle_Comparisons()
    {
        Eval("=5>3").Should().Be(true);
        Eval("=5=5").Should().Be(true);
        Eval("=5<>4").Should().Be(true);
        Eval("=2>=3").Should().Be(false);
    }


    [Fact]
    public void Should_Handle_Max_And_Min_Functions()
    {
        Eval("=max(2,5)").Should().Be(new BigInteger(5));
        Eval("=min(2,5)").Should().Be(new BigInteger(2));
        Eval("=mmax(1,3,2)").Should().Be(new BigInteger(3));
        Eval("=mmin(1,3,2)").Should().Be(new BigInteger(1));
    }


    [Fact]
    public void Should_Use_Cell_Reference_Value()
    {
        _providerMock.Setup(p => p.GetCellValueByAddress("A1"))
            .Returns("10");

        object result = Eval("=A1+5");

        result.Should().Be(new BigInteger(15));
        _providerMock.VerifyAll();
    }

    [Fact]
    public void Should_Return_Empty_String_For_Null_Cell()
    {
        _providerMock.Setup(p => p.GetCellValueByAddress("B2"))
            .Returns((string?)null);

        object result = Eval("=B2");

        result.Should().Be("");
    }
}
