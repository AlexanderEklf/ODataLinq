using System.Diagnostics;
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

        if (node.Term.Name == "logical_expression")
        {
            return VisitLogicalExpression(node);
        }

        if (node.Term.Name == "collection_filter_expression")
        {
            return VisitCollectionFilterExpression(node);
        }

        throw new Exception();
    }

    private static BinaryExpression VisitCollectionFilterExpression(ParseTreeNode node)
    {
        var parameter = VisitFieldPath(node.ChildNodes[0]);

        var lamda = node.ChildNodes[1];

        return lamda.Term.Name switch
        {
            "/any()" => Expression.NotEqual(
                Expression.ArrayLength(parameter),
                Expression.Constant(0)
            ),
            _ => throw new UnreachableException(
                $"{nameof(VisitCollectionFilterExpression)} is not exhaustive, ${lamda.Term.Name}"
            ),
        };
    }

    private static Expression VisitLogicalExpression(ParseTreeNode node)
    {
        var lhs = node.ChildNodes[0];
        var expression = node.ChildNodes[1].FindTokenAndGetText();
        var rhs = node.ChildNodes[2];

        return expression switch
        {
            "or" => Expression.Or(Visit(lhs), Visit(rhs)),
            "and" => Expression.And(Visit(lhs), Visit(rhs)),
            _ => throw new UnreachableException($"invalid token ${expression}"),
        };
    }

    private static Expression VisitConstant(ParseTreeNode node)
    {
        var literal = node.ChildNodes[0];
        return literal.Term.Name switch
        {
            "string_literal" => Expression.Constant(literal.Token.Value, typeof(string)),
            "float_literal" => Expression.Constant(
                literal.Token.Value,
                literal.Token.Value.GetType()
            ),
            "integer_literal" => Expression.Constant(
                literal.Token.Value,
                literal.Token.Value.GetType()
            ),
            _ => throw new UnreachableException($"{nameof(VisitConstant)} is not exhaustive"),
        };
    }

    private static Expression VisitComparisonExpression(ParseTreeNode node)
    {
        var lhs = node.ChildNodes[0];
        var op = node.ChildNodes[1];
        var rhs = node.ChildNodes[2];

        return op.FindTokenAndGetText() switch
        {
            "eq" => Expression.Equal(Visit(lhs), Visit(rhs)),
            "ne" => Expression.NotEqual(Visit(lhs), Visit(rhs)),
            "lt" => Expression.LessThan(Visit(lhs), Visit(rhs)),
            "gt" => Expression.GreaterThan(Visit(lhs), Visit(rhs)),
            "le" => Expression.LessThanOrEqual(Visit(lhs), Visit(rhs)),
            "ge" => Expression.GreaterThanOrEqual(Visit(lhs), Visit(rhs)),
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
            typeof(T).GetProperty(node.FindTokenAndGetText())
                ?? throw new InvalidOperationException(
                    $"{nameof(T)} does not contain property {node.FindTokenAndGetText()}"
                )
        );
    }

    private static Expression VisitFieldPath(ParseTreeNode node)
    {
        return Expression.Property(
            Param,
            typeof(T).GetProperty(node.FindTokenAndGetText())
                ?? throw new InvalidOperationException(
                    $"{nameof(T)} does not contain property {node.FindTokenAndGetText()}"
                )
        );
    }

    private static Expression VisitBooleanExpression(ParseTreeNode node)
    {
        return Visit(node.ChildNodes[0]);
    }
}
