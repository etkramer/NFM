using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NFM;

public class InspectorModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	[Reactive]
	public Type ObjectType { get; set; }

	[Reactive]
	public Control PropertyContent { get; set; } = null;

	[Reactive]
	public string ObjectName { get; set; } = "None";

	[ObservableAsProperty]
	public string TypeName { get; } = "None";

	[ObservableAsProperty]
	public string TypeIcon { get; } = "question_mark";

	public InspectorModel()
	{
		this.WhenActivated(disposables =>
		{
			// Type icon behavior
			this.WhenAnyValue(o => o.ObjectType)
				.Select(o => o is null ? "question_mark" : ObjectType.TryGetAttribute(out IconAttribute icon) ? icon.IconGlyph : "question_mark")
				.ToPropertyEx(this, o => o.TypeIcon)
				.DisposeWith(disposables);

			// Type name behavior
			this.WhenAnyValue(o => o.ObjectType)
				.Select(o => o is null ? "None" : ObjectType.Name)
				.ToPropertyEx(this, o => o.TypeName)
				.DisposeWith(disposables);

			Selection.Selected
				.ToObservableChangeSet()
				.Subscribe(o =>
				{
					var source = Selection.Selected;
					ObjectType = (source.Count == 0) ? null : ReflectionHelper.FindCommonAncestor(source.Select(o => o.GetType()));

					if (source.Count == 0)
					{
						ObjectType = null;
						ObjectName = "None";
						PropertyContent = null;
					}
					else
					{
						ObjectType = ReflectionHelper.FindCommonAncestor(source.Select(o => o.GetType()));
						ObjectName = source.Count > 1 ? $"({source.Count} objects)" : source.First().Name;
						PropertyContent = GetPropertiesContent(ObjectType);
					}
				})
				.DisposeWith(disposables);
		});
	}

	public Control GetPropertiesContent(Type type)
	{
		// Filter and bucket properties by category
		var buckets = type.GetProperties()
			.Where(o => o.HasAttribute<InspectAttribute>())
			.GroupBy(o => o.DeclaringType);

		var contents = new List<Control>();

		// Loop over buckets
		foreach (var bucket in buckets.Reverse())
		{
			// Add a separator for all but the first bucket
			if (bucket != buckets.Last())
			{
				var separator = new Separator();
				separator.Background(separator.GetResourceBrush("ThemeBackgroundColor"));
				separator.Margin = new(0);
				contents.Add(separator);
			}

			// Get the type that properties in this bucket belong to.
			Type bucketType = bucket.First().DeclaringType;

			// Get that type's display name.
			string bucketName = bucketType.Name.PascalToDisplay();
			if (bucketName.EndsWith(" Node"))
			{
				bucketName = bucketName.Remove(bucketName.Length - " Node".Length);
			}

			// Loop over properties.
			foreach (var property in bucket)
			{
				contents.Add(new InspectorPropertyItem(Selection.Selected.ToArray(), property));
			}
		}

		// Submit the property grid.
		return new StackPanel()
			.Orientation(Orientation.Vertical)
			.Spacing(8)
			.With(o => o.Margin = new(0, 8))
			.Children(contents);
	}

	public sealed class InspectorPropertyItem : UserControl
	{
		[Notify] public PropertyInfo Property { get; set; }
		[Notify] public IEnumerable<object> Subjects { get; set; }

		[Notify] public Control FieldContent { get; set; }

		public InspectorPropertyItem(IEnumerable<object> subjects, PropertyInfo property)
		{
			Subjects = subjects;
			Property = property;
			DataContext = this;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			Content = new Grid()
				.Columns("0.5*, *")
				.Children(
					// Name
					new TextBlock()
						.Column(0)
						.Margin(14, 2, 0, 0)
						.HorizontalAlignment(HorizontalAlignment.Left)
						.VerticalAlignment(VerticalAlignment.Top)
						.Text(Property.Name.PascalToDisplay())
						.Foreground(this.GetResourceBrush("ThemeForegroundMidBrush"))
						.Size(11),
					// Field
					new ContentControl()
						.Column(1)
						.Margin(0, 0, 10, 0)		
						.VerticalAlignment(VerticalAlignment.Center)
						.Content(nameof(FieldContent), BindingMode.Default)
				);

			FieldContent = InspectHelper.Create(subjects, Property);
		}
	}
}