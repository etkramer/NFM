using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NFM.Generators;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DeclarativePropertyAttribute : Attribute
{
	public string SetterName { get; set; }
	public string PropertyName { get; set; }
	public string BindingPropertyName { get; set; }
	public Type PropertyType { get; set; }
	public Type TargetType { get; set; }

	public DeclarativePropertyAttribute()
	{

	}
}

[Generator]
public class DeclarativeGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{

	}

	public void Execute(GeneratorExecutionContext context)
	{
		Dictionary<ITypeSymbol, AttributeData[]> types = new(comparer: SymbolEqualityComparer.Default);

		foreach (SyntaxTree syntaxTree in context.Compilation.SyntaxTrees)
		{
			SemanticModel model = context.Compilation.GetSemanticModel(syntaxTree);

			foreach (AttributeSyntax attributeSyntax in syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<AttributeSyntax>())
			{
				ITypeSymbol attributeType = model.GetTypeInfo(attributeSyntax).Type;
				ITypeSymbol declaringType = model.GetDeclaredSymbol(attributeSyntax.Parent.Parent) as ITypeSymbol;

				if (attributeType.GetFullName() == "NFM.Generators.DeclarativePropertyAttribute")
				{
					if (!types.ContainsKey(declaringType))
					{
						AttributeData[] data = declaringType.GetAttributes().Where(o => o.AttributeClass.GetFullName() == "NFM.Generators.DeclarativePropertyAttribute")?.ToArray();
						if (data is not null)
						{
							types.Add(declaringType, data);
						}
					}
				}
			}
		}

		foreach (ITypeSymbol type in types.Keys)
		{
			string source = GenerateSource(type, types[type]);
			context.AddSource($"{type.GetFullName()}Declarative.g.cs", source);
		}
	}

	private string GenerateSource(ITypeSymbol type, AttributeData[] attributes)
	{
		List<string> methods = new();
		foreach (AttributeData attribute in attributes)
		{
			string setterName = attribute.NamedArguments.Where(o => o.Key == "SetterName").First().Value.Value as string;
			string propertyName = attribute.NamedArguments.Where(o => o.Key == "PropertyName").First().Value.Value as string;
			string bindingPropertyName = attribute.NamedArguments.Where(o => o.Key == "BindingPropertyName").FirstOrDefault().Value.Value as string;
			INamedTypeSymbol propertyType = attribute.NamedArguments.Where(o => o.Key == "PropertyType").First().Value.Value as INamedTypeSymbol;
			INamedTypeSymbol targetType = attribute.NamedArguments.Where(o => o.Key == "TargetType").First().Value.Value as INamedTypeSymbol;
			INamedTypeSymbol[] inputTypes = attribute.NamedArguments.Where(o => o.Key == "InputTypes").SelectMany(o => o.Value.Values.Select(o => (INamedTypeSymbol)o.Value))?.ToArray();

			// Parse set.
			var parserResults = propertyType.GetMembers().Where(o => o.IsStatic && o.Name == "Parse" && o is IMethodSymbol && (o as IMethodSymbol).Parameters.Length == 1 && (o as IMethodSymbol).Parameters[0].Type.GetFullName() == "System.String");
			if (parserResults.Any())
			{
				methods.Add($@"
					public static T {setterName}<T>(this T subject, string value) where T : {targetType.GetFullName()}
					{{
						subject.{propertyName} = {propertyType.GetFullName()}.Parse(value);
						return subject;
					}}");
			}

			// Constructor set.
			methods.Add($@"
				public static T {setterName}<T>(this T subject, params object[] parameters) where T : {targetType.GetFullName()}
				{{
					subject.{propertyName} = ({propertyType.GetFullName()})Activator.CreateInstance(typeof({propertyType.GetFullName()}), parameters);

					return subject;
				}}");

			// Direct set (supports binding).
			if (bindingPropertyName is not null)
			{
				methods.Add($@"
					public static T {setterName}<T>(this T subject, {propertyType.GetFullName()} value) where T : {targetType.GetFullName()}
					{{
						subject.ClearValue({targetType.GetFullName()}.{bindingPropertyName});
						subject.{propertyName} = value;
						return subject;
					}}");
			}
			else
			{
				methods.Add($@"
					public static T {setterName}<T>(this T subject, {propertyType.GetFullName()} value) where T : {targetType.GetFullName()}
					{{
						subject.{propertyName} = value;
						return subject;
					}}");
			}

			// Binding set.
			if (bindingPropertyName is not null)
			{
				methods.Add($@"
					public static T {setterName}<T>(this T subject, string bindingPath, Avalonia.Data.BindingMode mode) where T : {targetType.GetFullName()}
					{{
						Avalonia.AvaloniaObjectExtensions.Bind(subject, {targetType.GetFullName()}.{bindingPropertyName}, new Avalonia.Data.Binding(bindingPath, mode));
						return subject;
					}}");

				methods.Add($@"
					public static T {setterName}<T>(this T subject, string bindingPath, object source) where T : {targetType.GetFullName()}
					{{
						Avalonia.Data.Binding binding = new(bindingPath);
						binding.Source = source;
			
						Avalonia.AvaloniaObjectExtensions.Bind(subject, {targetType.GetFullName()}.{bindingPropertyName}, binding);
						return subject;
					}}");
			}

			// Binding set (manual).
			if (bindingPropertyName is not null)
			{
				methods.Add($@"
					public static T {setterName}<T>(this T subject, Avalonia.Data.Binding binding) where T : {targetType.GetFullName()}
					{{
						Avalonia.AvaloniaObjectExtensions.Bind(subject, {targetType.GetFullName()}.{bindingPropertyName}, binding);
						return subject;
					}}");
			}
		}

		string source = $@"
			namespace {type.ContainingNamespace.ToDisplayString()}
			{{
				public static class {type.Name}Generated
				{{
					{string.Join("\n", methods)}
				}}
			}}";

		return source;
	}
}
