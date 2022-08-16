using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Engine.Roslyn;

namespace Engine.Aspects
{
	/// <summary>
	/// On finalization, adds this object to the disposal queue.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class AutoDisposeAttribute : Attribute
	{
		public static List<object> PendingObjects = new();
	}

	[Generator]
	public class AutoDisposeGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{

		}

		public void Execute(GeneratorExecutionContext context)
		{
			foreach (SyntaxTree syntaxTree in context.Compilation.SyntaxTrees)
			{
				SemanticModel model = context.Compilation.GetSemanticModel(syntaxTree);

				foreach (AttributeSyntax attributeSyntax in syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<AttributeSyntax>())
				{
					ITypeSymbol attributeType = model.GetTypeInfo(attributeSyntax).Type;

					if (attributeType.GetFullName() == "Engine.AutoDisposeAttribute")
					{
						ITypeSymbol targetType = model.GetDeclaredSymbol(attributeSyntax.Parent.Parent) as ITypeSymbol;

						bool hasFinalizer = targetType.GetMembers().OfType<IMethodSymbol>().Any((o) => o.Name == "Finalize" && o.Parameters.Length == 0);
						bool hasDispose = targetType.GetMembers().OfType<IMethodSymbol>().Any((o) => o.Name == "Dispose" && o.Parameters.Length == 0);

						if (!hasDispose)
						{
							DiagnosticDescriptor diagnostic = new("SD0001", null, "Types with [AutoDispose] must contain a parameterless Dispose() method.", "AutoDispose", DiagnosticSeverity.Warning, true);
							context.ReportDiagnostic(Diagnostic.Create(diagnostic, attributeSyntax.Parent.Parent.GetLocation(), (object[])null));
							continue;
						}
						else if (hasFinalizer)
						{
							DiagnosticDescriptor diagnostic = new("SD0002", null, "Types with [AutoDispose] must not contain a finalizer.", "AutoDispose", DiagnosticSeverity.Warning, true);
							context.ReportDiagnostic(Diagnostic.Create(diagnostic, attributeSyntax.Parent.Parent.GetLocation(), (object[])null));
							continue;
						}

						// Implement the finalizer.
						ImplementAutoDispose(model.GetDeclaredSymbol(attributeSyntax.Parent.Parent) as ITypeSymbol, context);
						break;
					}
				}
			}
		}

		private void ImplementAutoDispose(ITypeSymbol targetType, GeneratorExecutionContext context)
		{
			string partialSource = $@"
			namespace {targetType.ContainingNamespace.ToDisplayString()}
			{{
				partial class {targetType.Name}
				{{
					~{targetType.Name}()
					{{
						// This is the *only* time we ever want the finalizer to be called.
						System.GC.SuppressFinalize(this);

						// Schedules a call to Dispose(), referencing the object and effectively resurrecting it.
						Engine.Core.Queue.Schedule(() => this.Dispose(), -1);
					}}
				}}
			}}
			";

			context.AddSource($"{targetType.Name}.g.cs", partialSource);
		}
	}
}
