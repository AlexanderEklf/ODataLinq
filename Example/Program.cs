// See https://aka.ms/new-console-template for more information
using Example;

var person = new Person("John");
List<Person> personList = [person, new("Lennart")];

var y = ODataFilter.CreateLambda<Person>("Name eq 'John'");

var result = personList.Where(y);

foreach (var item in result)
{
    Console.WriteLine(item.Name);
}

public record Person(string Name);
