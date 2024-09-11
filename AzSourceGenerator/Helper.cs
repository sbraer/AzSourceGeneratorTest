using Microsoft.CodeAnalysis;

namespace AzSourceGeneratorTest;

public static class Helper
{
    public enum TypeClr { String, Number, DateTime, Unknown}

    public static (TypeClr TypeClr, bool IsNullable, string TypeString) GetClrTypeName(ITypeSymbol typeSymbol)
    {
        (var nullable, typeSymbol) = CheckIfTypeIsNullable(typeSymbol);

        if (typeSymbol.ToDisplayString() == "System.DateTime")
        {
            return (TypeClr.DateTime, nullable, "DateTime");
        }
        else if (typeSymbol.SpecialType == SpecialType.System_String)
        {
            return (TypeClr.String, nullable, "string");
        }

        var typeString = (typeSymbol.SpecialType) switch
        {
            SpecialType.System_Int32 => "int",
            SpecialType.System_Double => "double",
            SpecialType.System_Decimal => "decimal",
            SpecialType.System_Single => "float",
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Int64 => "long",
            SpecialType.System_Int16 => "short",
            SpecialType.System_Byte => "byte",
            SpecialType.System_Char => "char",
            _ => null
        };

        if (typeString is null)
        {
            return (TypeClr.Unknown, nullable, string.Empty);
        }
        else
        {
            return (TypeClr.Number, nullable, typeString);
        }
    }

    public static (bool, ITypeSymbol) CheckIfTypeIsNullable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol &&
        namedTypeSymbol.IsGenericType &&
        namedTypeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            var underlyingType = namedTypeSymbol.TypeArguments[0];
            return (true, underlyingType);
        }

        if (typeSymbol.NullableAnnotation == NullableAnnotation.Annotated)
        {
            return (true, typeSymbol);
        }

        return (false, typeSymbol);
    }

    public static void ReportTypeNotSupportedDiagnostic(SourceProductionContext context, string message, string type)
    {
        var descriptor = new DiagnosticDescriptor(
            id: "FSG001",
            title: "Type not supported",
            messageFormat: "The type '{1}' in property '{0}' is not supported",
            category: "RegisterSourceOutput",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None, message, type));
    }

    public static void ReportClassNotSupportedDiagnostic(SourceProductionContext context, string className)
    {
        var descriptor = new DiagnosticDescriptor(
            id: "FSG002",
            title: "Class not supported",
            messageFormat: "Class '{0}' must be static and partial",
            category: "RegisterSourceOutput",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None, className));
    }
}