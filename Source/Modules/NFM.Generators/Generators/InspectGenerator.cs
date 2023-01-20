using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NFM.Generators;

[Generator]
public class InspectGenerator : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context) {}

	public void Execute(GeneratorExecutionContext context)
	{
		Dictionary<ITypeSymbol, AttributeData[]> types = new(comparer: SymbolEqualityComparer.Default);

		foreach (SyntaxTree syntaxTree in context.Compilation.SyntaxTrees)
		{
			SemanticModel model = context.Compilation.GetSemanticModel(syntaxTree);

			foreach (AttributeSyntax attributeSyntax in syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<AttributeSyntax>())
			{
				ITypeSymbol attributeType = model.GetTypeInfo(attributeSyntax).Type;

				if (attributeType.GetFullName() == "NFM.Frontend.CustomInspectorAttribute")
				{
					// Get inspector info
					ITypeSymbol inspectorType = model.GetDeclaredSymbol(attributeSyntax.Parent.Parent) as ITypeSymbol;

					// Generate source
					context.AddSource($"{inspectorType.GetFullName()}.g.cs", GenerateSource(inspectorType));
				}
			}
		}
	}

	private string GenerateSource(ITypeSymbol inspectorType)
	{
		string source = $@"
			namespace {inspectorType.ContainingNamespace.ToDisplayString()}
			{{
				partial class {inspectorType.Name} : System.IDisposable, System.ComponentModel.INotifyPropertyChanged
				{{
					protected System.Reflection.PropertyInfo Property {{ get; set; }}
					protected System.Collections.Generic.IEnumerable<object> Subjects {{ get; set; }}
					
					private System.ComponentModel.PropertyChangedEventHandler propertyChanged = delegate {{}};
					event System.ComponentModel.PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged
					{{
						add => propertyChanged += value;
						remove => propertyChanged -= value;
					}}

					public object Value
					{{
						get => Property.GetValue(Subjects.First());
						set
						{{
							foreach (var subject in Subjects)
							{{
								Property.SetValue(subject, value);
							}}
						}}
					}}

					private System.ComponentModel.PropertyChangedEventHandler notifyHandler = null;

					protected override void OnAttachedToLogicalTree(Avalonia.LogicalTree.LogicalTreeAttachmentEventArgs e)
					{{
						// Respond to changes in source property.
						if (Subjects.First() is System.ComponentModel.INotifyPropertyChanged notify)
						{{
							notifyHandler = (o, e) =>
							{{
								if (e.PropertyName == Property.Name)
								{{
									this.RaisePropertyChanged(nameof(Value));
								}}
							}};

							notify.PropertyChanged += notifyHandler;
						}}

						base.OnAttachedToLogicalTree(e);
					}}

					protected override void OnDetachedFromLogicalTree(Avalonia.LogicalTree.LogicalTreeAttachmentEventArgs e)
					{{
						Dispose();

						// Respond to changes in source property.
						if (Subjects.First() is System.ComponentModel.INotifyPropertyChanged notify)
						{{
							notify.PropertyChanged -= notifyHandler;
						}}

						notifyHandler = null;
						base.OnDetachedFromLogicalTree(e);
					}}

					private void RaisePropertyChanged(string propertyName)
					{{
						propertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
					}}
				}}
			}}";

		return source;
	}
}
