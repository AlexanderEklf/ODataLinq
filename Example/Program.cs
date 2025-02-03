// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using Example;
using Irony.Parsing;

var g = new ODataGrammar();
var l = new LanguageData(g);
var r = new Parser(l);

var person = new Person("John");
List<Person> personList = [person, new("Lennart")];

var parseTree = r.Parse("Name eq 'John'");

var y = ODataFilter.CreateLambda<Person>("Name eq 'John'");

var result = personList.Where(y);

Console.WriteLine(result);

public record Person(string Name);
