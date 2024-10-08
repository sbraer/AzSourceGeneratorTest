﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace AzSourceGeneratorTest;

[Generator(LanguageNames.CSharp)]
public class AzGeneratorFromAttribute : IIncrementalGenerator
{
    private static readonly string generatedCodeAttribute = $@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{typeof(AzGeneratorFromAttribute).Assembly.GetName().Name}"", ""{typeof(AzGeneratorFromAttribute).Assembly.GetName().Version}"")]";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached) Debugger.Launch();
        context.RegisterPostInitializationOutput(static void (IncrementalGeneratorPostInitializationContext context) =>
        {
            {
                const string hintName = "AzGeneratorFromAttribute.GeneratedAttribute.g.cs";
                //language=c#
                string source = $$"""
                // <auto-generated/>
                #nullable enable
                using System;
                namespace AzGeneratorFromAttribute;

                {{generatedCodeAttribute}}
                [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
                public sealed class AzGeneratedAttribute : global::System.Attribute
                {
                    public Type Type { get; }
                    public GeneratorNotTypeRecognized GeneratorNotTypeRecognized { get; }

                    public AzGeneratedAttribute(Type type, GeneratorNotTypeRecognized generatorNotTypeRecognized = GeneratorNotTypeRecognized.CompilationError)
                    {
                        Type = type;
                        GeneratorNotTypeRecognized = generatorNotTypeRecognized;
                    }
                }

                public enum GeneratorNotTypeRecognized : int { Skip = 0, ThrowException = 1, CompilationError = 2 }
                """;

                context.AddSource(hintName, source);
            }

            {
                const string hintName = "AzGeneratorFromAttribute.HelperFunction.g.cs";
                //language=c#
                string source = $$"""
                // <auto-generated/>
                #nullable enable
                using System.Globalization;
                using System.Numerics;
                using System.Runtime.CompilerServices;

                namespace AzGeneratorFromAttribute;

                {{generatedCodeAttribute}}
                public static class ConversionHelper
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static bool ParseDateTime(string s, Func<DateTime, bool> setValue) =>
                        (DateTimeNullableConvert(s) is not { } value) ? false : setValue(value);

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static bool ParseDateTimeNullable(string? s, Func<DateTime?,bool> setValue) =>
                        (s is null) ? setValue(null) : ParseDateTime(s, (x) => setValue(x!));

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static bool ParseNumber<T>(string s, Func<T, bool> setValue) where T : struct, INumberBase<T> =>
                        (NumberNullableConvert<T>(s) is not { } value) ? false : setValue(value);

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static bool ParseNumberNullable<T>(string? s, Func<T?, bool> setValue) where T : struct, INumberBase<T> =>
                        (s is null) ? setValue(null) : ParseNumber<T>(s, (x) => setValue(x!));

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static bool ParseString(string s, Func<string, bool> setValue) =>
                        setValue(s);

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static bool ParseStringNullable(string? s, Func<string?, bool> setValue) =>
                        (s is null) ? setValue(null) : setValue(s);

                    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
                    public static DateTime? DateTimeNullableConvert(string? date) => DateTime.TryParse(date, out var dt) ? dt : null;

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static T? NumberNullableConvert<T>(string? number) where T : struct, INumberBase<T> => T.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out var t) ? t : null;
                }
                """;

                context.AddSource(hintName, source);
            }
        });

        IncrementalValuesProvider<(string TypeName, Accessibility ClassAccessibility, string? Namespaces, MasterType masterType)> provider =
            context.SyntaxProvider.ForAttributeWithMetadataName("AzGeneratorFromAttribute.AzGeneratedAttribute",
                predicate: static bool (SyntaxNode node, CancellationToken cancellationToken) => node is ClassDeclarationSyntax,
                transform: static (string TypeName, Accessibility ClassAccessibility, string? Namespaces, MasterType masterType) (GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken) =>
                {
                    //if (!Debugger.IsAttached) Debugger.Launch();
                    var symbol = (INamedTypeSymbol)context.TargetSymbol;
                    var className = symbol.Name;

                    var classDeclarationSyntax = (ClassDeclarationSyntax)context.TargetNode;
                    var isPartial = classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
                    var isStatic = classDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);

                    var namespacesClass = string.IsNullOrEmpty(symbol.ContainingNamespace.Name) ? null : symbol.ContainingNamespace.ToDisplayString();
                    MasterType masterType = new(className, isPartial, isStatic);
                    List<string> classNames = [];

                    foreach (var attributeData in context.Attributes)
                    {
                        var typeArgument0 = attributeData.ConstructorArguments[0];
                        if (typeArgument0.Value is not ITypeSymbol typeSymbol) continue;
                        if (classNames.Contains(typeSymbol.Name)) continue;
                        classNames.Add(typeSymbol.Name);

                        var generatorNotTypeRecognized = GeneratorNotTypeRecognized.CompilationError;
                        if (attributeData.ConstructorArguments.Length == 2)
                        {
                            var typeArgument1 = attributeData.ConstructorArguments[1];
                            if (typeArgument1.Kind == TypedConstantKind.Enum)
                            {
                                generatorNotTypeRecognized = (GeneratorNotTypeRecognized)int.Parse(typeArgument1.Value!.ToString());
                            }
                        }

                        var ns = string.IsNullOrEmpty(typeSymbol.ContainingNamespace.Name) ? null : typeSymbol.ContainingNamespace.ToDisplayString();
                        var properties = typeSymbol.GetMembers()
                            .OfType<IPropertySymbol>()
                            .Select(t => new SubProperty(t.Name, t.Type)).ToList();

                        masterType.SubTypes.Add(new SubTypeClass(
                            typeSymbol.Name,
                            ns,
                            generatorNotTypeRecognized,
                            properties)
                        );
                    }

                    return (className, symbol.DeclaredAccessibility, namespacesClass, masterType);
                });

        context.RegisterSourceOutput(provider, static void (SourceProductionContext context, (string TypeName, Accessibility ClassAccessibility, string? Namespaces, MasterType MasterType) info) =>
        {
            //if (!Debugger.IsAttached) Debugger.Launch();
            if (!info.MasterType.IsPartial || !info.MasterType.IsStatic)
            {
                Helper.ReportClassNotSupportedDiagnostic(context, info.MasterType.ClassName);
                return;
            }

            using StringWriter writer = new(CultureInfo.InvariantCulture);
            using IndentedTextWriter source = new(writer);

            source.WriteLine("// <auto-generated/>");
            source.WriteLine("#nullable enable");
            source.WriteLine();
            source.WriteLine("using System.Globalization;");
            source.WriteLine("using AzGeneratorFromAttribute;");

            if (!string.IsNullOrEmpty(info.Namespaces))
            {
                source.WriteLine($"namespace {info.Namespaces}");
                source.WriteLine("{");
                source.Indent++;
            }

            source.WriteLine($"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{typeof(AzGeneratorFromAttribute).Assembly.GetName().Name}\", \"{typeof(AzGeneratorFromAttribute).Assembly.GetName().Version}\")]");
            source.WriteLine($"{SyntaxFacts.GetText(info.ClassAccessibility)} static partial class {info.TypeName}");
            source.WriteLine("{");
            source.Indent++;

            foreach (var subType in info.MasterType.SubTypes)
            {
                var ns = string.IsNullOrEmpty(subType.Namespace) ? string.Empty : subType.Namespace! + ".";
                source.WriteLine($"{SyntaxFacts.GetText(info.ClassAccessibility)} static bool SetProperty{subType.Classname}({ns}{subType.Classname} obj, ReadOnlySpan<char> propertyName, string? value) =>");
                source.Indent++;
                source.WriteLine($"propertyName switch");
                source.WriteLine("{");
                source.Indent++;
                InsertPropertiesInSwitch(context, source, subType);
                source.WriteLine("_ => false");
                source.Indent--;
                source.WriteLine("};");
                source.Indent--;
                source.WriteLine();
            }

            source.Indent--;
            source.WriteLine("}");
            if (!string.IsNullOrEmpty(info.Namespaces))
            {
                source.Indent--;
                source.WriteLine("}");
            }

            Debug.Assert(source.Indent == 0);
            context.AddSource($"{info.TypeName}.g.cs", writer.ToString());
        });
    }

    private static void InsertPropertiesInSwitch(SourceProductionContext context, IndentedTextWriter source, SubTypeClass subType)
    {
        foreach (var property in subType.Properties)
        {
            source.Write($"\"{property.Name}\" => ");

            var clrType = Helper.GetClrTypeName(property.Type);
            if (clrType.TypeClr == Helper.TypeClr.Unknown)
            {
                switch (subType.GeneratorNotTypeRecognized)
                {
                    case GeneratorNotTypeRecognized.Skip: break;
                    case GeneratorNotTypeRecognized.ThrowException: source.WriteLine($"throw new ArgumentException(\"The type '{property.Type.ToDisplayString()}' in property '{property.Name}' is not supported\"),"); return;
                    case GeneratorNotTypeRecognized.CompilationError: { Helper.ReportTypeNotSupportedDiagnostic(context, property.Name, property.Type.ToDisplayString()); break; }
                }
            }

            source.Write((clrType.TypeClr, clrType.IsNullable) switch
            {
                (Helper.TypeClr.DateTime, false) => $"ConversionHelper.ParseDateTime(value!, (x => {{ obj.{property.Name} = x; return true; }}))",
                (Helper.TypeClr.DateTime, true) => $"ConversionHelper.ParseDateTimeNullable(value, (x => {{ obj.{property.Name} = x; return true; }}))",
                (Helper.TypeClr.Number, false) => $"ConversionHelper.ParseNumber<{clrType.TypeString}>(value!, (x => {{ obj.{property.Name} = x; return true; }}))",
                (Helper.TypeClr.Number, true) => $"ConversionHelper.ParseNumberNullable<{clrType.TypeString}>(value, (x => {{ obj.{property.Name} = x; return true; }}))",
                (Helper.TypeClr.String, false) => $"ConversionHelper.ParseString(value!, (x => {{ obj.{property.Name} = x; return true; }}))",
                (Helper.TypeClr.String, true) => $"ConversionHelper.ParseStringNullable(value, (x => {{ obj.{property.Name} = x; return true; }}))",
                _ => "false"
            });

            source.WriteLine(',');
        }
    }
}
