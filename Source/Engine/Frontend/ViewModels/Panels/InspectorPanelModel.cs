using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using DynamicData;
using DynamicData.Binding;
using Engine.Editor;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Engine.Frontend
{
	public class InspectorModel : ReactiveObject, IActivatableViewModel
	{
		public ViewModelActivator Activator { get; } = new();

		[Reactive]
		public string ObjectDisplayName { get; set; } = "None";
		[Reactive]
		public string TypeDisplayName { get; set; } = "None";
		[Reactive]
		public char TypeIcon { get; set; } = '\uEB8B';

		[Reactive]
		public Control PropertyContent { get; set; } = null;

		public InspectorModel()
		{
			this.WhenActivated(d =>
			{
				Selection.Selected
					.ToObservableChangeSet()
					.Subscribe(o =>
					{
						var source = Selection.Selected;

						if (source.Count == 0)
						{
							ObjectDisplayName = "None";
							TypeDisplayName = "None";
							TypeIcon = '\uEB8B';

							PropertyContent = null;
						}
						else
						{
							Type selectedType = ReflectionHelper.FindCommonAncestor(source.Select(o => o.GetType()));

							TypeDisplayName = selectedType.Name;
							TypeIcon = selectedType.TryGetAttribute(out IconAttribute icon) ? icon.IconGlyph : '\uEB8B';

							if (source.Count > 1)
							{
								ObjectDisplayName = $"({source.Count} objects)";
							}
							else
							{
								ObjectDisplayName = source.FirstOrDefault()?.Name;
							}

							PropertyContent = GetPropertiesContent(selectedType);
						}
					})
					.DisposeWith(d);
			});
		}

		public Control GetPropertiesContent(Type type)
		{
			// Filter and bucket properties by category.
			var buckets = type.GetProperties()
				.Where(o => o.HasAttribute<InspectAttribute>())
				.Bucket(o => o.DeclaringType);

			// Loop over types.
			List<Control> expanders = new();
			foreach (var bucket in buckets.Reverse())
			{
				// Get the type that properties in this bucket belong to.
				Type bucketType = bucket.First().DeclaringType;

				// Get that type's display name.
				string bucketName = bucketType.Name.PascalToDisplay();
				if (bucketName.EndsWith(" Node"))
				{
					bucketName = bucketName.Remove(bucketName.Length - " Node".Length);
				}

				// Loop over properties.
				List<Control> inspectors = new();
				foreach (var property in bucket)
				{
					inspectors.Add(new PropertyInspector(Selection.Selected.ToArray(), property));
				}

				// Create expander
				expanders.Add(
					new Expander()
						.IsExpanded(true)
						.Header(bucketName)
						.With(o => o.FontWeight = FontWeight.SemiBold)
						.Content(new StackPanel()
							.Children(inspectors.ToArray())
						)
				);
			}

			// Submit the property grid.
			return new StackPanel()
				.Orientation(Orientation.Vertical)
				.Children(expanders.ToArray());
		}
	}
}