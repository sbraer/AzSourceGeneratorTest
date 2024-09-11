using Models;
using Models.Generated;
using System.Diagnostics;

MyObject obj = new();
Helper.SetPropertyMyObject(obj, "Id", "12");
Helper.SetPropertyMyObject(obj, "Name", "AZ");

Console.WriteLine($"Id = {obj.Id} Name = '{obj.Name}'");

Console.WriteLine();
Console.WriteLine("Try to insert wrong type...");
if (!Helper.SetPropertyMyObject(obj, "Id", "aaa"))
{
    Console.WriteLine("'aaa' cannot be converted to int");
}

Console.WriteLine();
Console.WriteLine("Try to insert value in wrong property...");
if (!Helper.SetPropertyMyObject(obj, "Description", "aaa"))
{
    Console.WriteLine("'Description' property does not exist in MyObject class");
}

Console.WriteLine();
Console.WriteLine("Exception for wrong property type");

try
{
    Helper.SetPropertyMyObject(obj, "Value", "aaa");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Excepion message: {ex.Message}");
}

Console.WriteLine(new string('=', 100));

OtherObject obj2 = new();
Helper.SetPropertyOtherObject(obj2, "Comment", null);
Helper.SetPropertyOtherObject(obj2, "DateTime", null);

Debug.Assert(obj2.Comment is null);
Debug.Assert(obj2.DateTime is null);

Helper.SetPropertyOtherObject(obj2, "Comment", "This is a comment");
Helper.SetPropertyOtherObject(obj2, "DateTime", "2024-09-09");

Console.WriteLine($"Comment = '{obj2.Comment}', DateTime = '{obj2.DateTime}'");

Console.WriteLine();
Console.WriteLine("Try to insert wrong datetime in property DateTime...");
if (!Helper.SetPropertyOtherObject(obj2, "DateTime", "abcde"))
{
    Console.WriteLine("'abcde' cannot be converted to DateTime");
}

Console.WriteLine(new string('=', 100));

Console.WriteLine("Test class generation from csv file");
var book = new Book { Author = "aa", Title = "bb", Description = "This is the description", Pages = 356, Price = 35.99 };
Console.WriteLine(book);