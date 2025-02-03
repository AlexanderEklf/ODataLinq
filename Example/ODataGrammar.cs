// See https://aka.ms/new-console-template for more information
using Irony.Parsing;

public class ODataGrammar : Grammar
{
    public ODataGrammar()
    {
        var identifier = new RegexBasedTerminal("identifier", "[a-zA-Z_][a-zA-Z_0-9]*");
        var string_literal = new StringLiteral("string_literal", "'");
        var integer_literal = new NumberLiteral("integer_literal", NumberOptions.IntOnly);
        var float_literal =
            new NumberLiteral("float_literal", NumberOptions.AllowSign | NumberOptions.AllowSign)
            | new RegexBasedTerminal("float_literal", "(NaN)|-?(INF)");
        var boolean_literal = new RegexBasedTerminal("boolean_literal", "(true)|(false)");

        var filter_expression = new NonTerminal("filter_expression");
        var boolean_expression = new NonTerminal("boolean_expression");
        var collection_filter_expression = new NonTerminal("collection_filter_expression");
        var logical_expression = new NonTerminal("logical_expression");
        var comparison_expression = new NonTerminal("comparison_expression");
        var variable = new NonTerminal("variable");
        var field_path = new NonTerminal("field_path");
        var lambda_expression = new NonTerminal("lambda_expression");
        var comparison_operator = new NonTerminal("comparison_operator");
        var constant = new NonTerminal("constant");

        Root = filter_expression;

        filter_expression.Rule = boolean_expression;

        boolean_expression.Rule =
            collection_filter_expression
            | logical_expression
            | comparison_expression
            | boolean_literal
            | "(" + boolean_expression + ")"
            | variable;
        variable.Rule = identifier | field_path;

        field_path.Rule = MakeStarRule(field_path, ToTerm("/"), identifier);

        collection_filter_expression.Rule =
            field_path + "/all(" + lambda_expression + ")"
            | field_path + "/any(" + lambda_expression + ")"
            | field_path + "/any()";

        lambda_expression.Rule = identifier + ":" + boolean_expression;

        logical_expression.Rule =
            boolean_expression + (ToTerm("and", "and") | ToTerm("or", "or")) + boolean_expression
            | ToTerm("not", "not") + boolean_expression;

        comparison_expression.Rule =
            variable + comparison_operator + constant | constant + comparison_operator + variable;

        constant.Rule =
            string_literal | integer_literal | float_literal | boolean_literal | ToTerm("null");

        comparison_operator.Rule = ToTerm("gt") | "lt" | "ge" | "le" | "eq" | "ne";

        RegisterBracePair("(", ")");
    }
}
