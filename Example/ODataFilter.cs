namespace Example;

public class ODataFilter
{
    public static Func<T, bool> CreateLambda<T>(string oDataFilter)
    {
        var parseTree = new ODataFilterParser(new ODataGrammar()).ParseFilter(oDataFilter);
        if (parseTree == null)
        {
            throw new Exception($"Could not parse query, error");
        }

        if (parseTree.HasErrors())
        {
            throw new Exception(
                $"Parsed with errors: {string.Join(", ", parseTree.ParserMessages.Select(x => x.Message))}"
            );
        }

        return ExpressionFactory<T>.CreateLambdaExpression(parseTree);
    }
}
