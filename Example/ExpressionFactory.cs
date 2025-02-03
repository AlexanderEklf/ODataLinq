using System;
using System.Linq.Expressions;
using Irony.Parsing;

namespace Example;

public static class ExpressionFactory<T>
{
    private static readonly ParameterExpression Param = Expression.Parameter(typeof(T), "arg");

    public static Func<T, bool> CreateLambdaExpression(ParseTree parseTree)
    {
        return Expression.Lambda<Func<T, bool>>(Visit(parseTree.Root), Param).Compile();
    }

    private static Expression Visit(ParseTreeNode node)
    {
        if (node.Term.Name == "filter_expression")
        {
            return VisitFilterExpression(node);
        }

        if (node.Term.Name == "boolean_expression")
        {
            return VisitBooleanExpression(node);
        }

        if (node.Term.Name == "comparison_expression")
        {
            return VisitComparisonExpression(node);
        }

        if (node.Term.Name == "constant")
        {
            return VisitConstant(node);
        }

        if (node.Term.Name == "variable")
        {
            return VisitVariable(node);
        }

        throw new Exception();
    }

    private static Expression VisitConstant(ParseTreeNode node)
    {
        return Expression.Constant(node.FindTokenAndGetText().Replace("'", ""), typeof(string));
    }

    private static Expression VisitComparisonExpression(ParseTreeNode node)
    {
        var lhs = node.ChildNodes[0];
        var op = node.ChildNodes[1];
        var rhs = node.ChildNodes[2];

        return op.FindTokenAndGetText() switch
        {
            "eq" => Expression.Equal(Visit(lhs), Visit(rhs)),
            _ => throw new Exception($"Unrecognized token {op.FindTokenAndGetText()}"),
        };
    }

    private static Expression VisitFilterExpression(ParseTreeNode parseTreeNode)
    {
        return Visit(parseTreeNode.ChildNodes[0]);
    }

    private static Expression VisitVariable(ParseTreeNode node)
    {
        return Expression.Property(
            Param,
            typeof(Person).GetProperty(node.FindTokenAndGetText())
                ?? throw new InvalidOperationException(
                    $"{nameof(Person)} does not contain property {node.FindTokenAndGetText()}"
                )
        );
    }

    private static Expression VisitBooleanExpression(ParseTreeNode node)
    {
        return Visit(node.ChildNodes[0]);
    }
}
