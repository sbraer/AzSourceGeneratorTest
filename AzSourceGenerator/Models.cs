using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace AzSourceGeneratorTest;

public enum GeneratorNotTypeRecognized : int { Skip = 0, ThrowException = 1, CompilationError = 2 }

public class Config(string nameSpace, string extension)
{
    public string Namespace { get; set; } = nameSpace;
    public string Extension { get; set; } = extension;
}

public class CsvFile
{
    public string ClassName { get; set; } = null!;
    public string[] Rows { get; set; } = [];
}

public class MasterType(string className, bool isPartial, bool isStatic)
{
    public string ClassName { get; set; } = className;
    public bool IsPartial { get; set; } = isPartial;
    public bool IsStatic { get; set; } = isStatic;
    public List<SubTypeClass> SubTypes { get; set; } = [];
}

public class SubTypeClass(string className, string? nameSpace, GeneratorNotTypeRecognized generatorNotTypeRecognized, List<SubProperty> properties)
{
    public string Classname { get; set; } = className;
    public string? Namespace { get; set; } = nameSpace;
    public GeneratorNotTypeRecognized GeneratorNotTypeRecognized { get; set; } = generatorNotTypeRecognized;
    public List<SubProperty> Properties { get; set; } = properties;
}

public class SubProperty(string name, ITypeSymbol typeSimbol)
{
    public string Name { get; set; } = name;
    public ITypeSymbol Type { get; set; } = typeSimbol;
}
