using Irony.Parsing;

namespace Example;

public class ODataFilterParser
{
    private readonly Parser _parser;

    public ODataFilterParser(ODataGrammar grammar)
    {
        var l = new LanguageData(grammar);
        _parser = new Parser(l);
    }

    public ParseTree? ParseFilter(string oDataFilter)
    {
        return _parser.Parse(oDataFilter);
    }
}
