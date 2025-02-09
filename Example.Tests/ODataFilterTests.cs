namespace Example.Tests;

public record Person(string Name, int Value, int[] Values);

public class ODataFilterTests
{
    private readonly Person _jeff = new("Jeff", 100, []);
    private readonly Person _john = new("John", 5, []);
    private readonly Person _jane = new("Jane", 10, [1, 2, 3]);
    private Person[] Persons => [_john, _jane, _jeff];

    [Fact]
    public void Should_handle_equal_operator()
    {
        var query = "Name eq 'John'";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), [_john]);
    }

    [Fact]
    public void Should_handle_equal_operator_int()
    {
        var query = "Value eq 100";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), [_jeff]);
    }

    [Fact]
    public void Should_handle_not_equal_operator()
    {
        var query = "Name ne 'John'";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), [_jane, _jeff]);
    }

    [Fact]
    public void Should_handle_less_than_operator()
    {
        var query = "Value lt 10";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), [_john]);
    }

    [Fact]
    public void Should_handle_greater_than_operator()
    {
        var query = "Value gt 10";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), [_jeff]);
    }

    [Fact]
    public void Should_handle_less_or_equal()
    {
        var query = "Value le 10";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), [_john, _jane]);
    }

    [Fact]
    public void Should_handle_greater_or_equal()
    {
        var query = "Value ge 10";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), [_jane, _jeff]);
    }

    [Fact]
    public void Should_handle_or()
    {
        var query = "Value eq 5 or Name eq 'Jeff'";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), [_john, _jeff]);
    }

    [Fact]
    public void Should_handle_and()
    {
        var query = "Value eq 5 and Name eq 'Jeff'";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), []);
    }

    [Fact]
    public void Should_handle_collection_filter_expression_any()
    {
        var query = "Values/any()";

        var func = ODataFilter.CreateLambda<Person>(query);

        Assert.IsType<Func<Person, bool>>(func);
        Assert.Equal(Persons.Where(func), [_jane]);
    }
}
