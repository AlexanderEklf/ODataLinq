namespace Example.Tests;

public class EqualityTests
{
    [Theory]
    [InlineData("Name eq 'John'")]
    public void Should_Compile_Query(string query) { }
}
