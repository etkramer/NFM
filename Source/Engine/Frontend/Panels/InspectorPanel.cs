using System;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using Engine.Editor;

namespace Engine.Frontend
{
	public class InspectorPanel : ToolPanel
	{
		[Notify] public Control InspectorContent { get; private set; } = null;

		[Notify] private string objectName { get; set; }
		[Notify] private string objectTypeName { get; set; }
		[Notify] private char objectIcon { get; set; }

		[Notify] private string currentFilter { get; set; } = "";

		public InspectorPanel()
		{
			DataContext = this;

			Title = "Inspector";
			Content = new Grid()
				.Rows("32, 50, *")
				.Children(
					new Grid()
						.Columns("auto, *")
						.Row(0)
						.Margin(4)
						.Children(
							// Forward/back buttons
							new StackPanel()
								.Column(0)
								.Orientation(Orientation.Horizontal)
								.Spacing(4)
								.Children(
									// Back button
									new Button()
										.Style("window")
										.Tooltip("Previous")
										.Content(
											new TextBlock()
												.Text("\uE5C4")
												.Size(16)
												.Font(this.GetResource<FontFamily>("IconsFont"))
										),
									// Forward button
									new Button()
										.Style("window")
										.Tooltip("Next")
										.Margin(0, 0, 8, 0)
										.OnClick(null)
										.Content(
											new TextBlock()
												.Text("\uE5C8")
												.Size(16)
												.Font(this.GetResource<FontFamily>("IconsFont"))
										)
								),
							// Search bar
							new TextBox()
								.Column(1)
								.Hint("Filter..")
								.Text(nameof(currentFilter), BindingMode.Default)
						),
					// Object description
					new Grid()
						.Background(this.GetResourceBrush("ControlBackgroundColor"))
						.Row(1)
						.Children(
							new StackPanel()
								.Orientation(Orientation.Horizontal)
								.Children(
									new TextBlock()
										.Text(nameof(objectIcon), BindingMode.Default)
										.VerticalAlignment(VerticalAlignment.Center)
										.Size(26)
										.Margin(7, 0, 7, 0)
										.Foreground("#B0E24D")
										.Font(this.GetResource<FontFamily>("IconsFont2")),
									new StackPanel()
										.Orientation(Orientation.Vertical)
										.VerticalAlignment(VerticalAlignment.Center)
										.Children(
											new TextBlock()
												.Text(nameof(objectName), BindingMode.Default)
												.Size(14)
												.Weight(FontWeight.Bold)
												.Foreground("#B0E24D"),
											new TextBlock()
												.Text(nameof(objectTypeName), BindingMode.Default)
												.Foreground("#BEB0E24D")
										)
								)
						),
					// Property grid
					new ContentControl()
						.Row(2)
						.Content(nameof(InspectorContent), BindingMode.Default)
				);

			Selection.Selected.Subscribe(() => Refresh());
			(this as INotify).Subscribe(nameof(currentFilter), () => Refresh());
			Refresh();
		}

		public void Refresh()
		{
			// Clear out values if we've got nothing selected.
			if (Selection.Selected.Count == 0)
			{
				objectName = "None";
				objectTypeName = "None";
				objectIcon = '\uEB8B';

				InspectorContent = null;
				return;
			}

			// Find the most specific base type for these objects.
			Type selectedType = ReflectionHelper.FindCommonAncestor(Selection.Selected.Select(o => o.GetType()));

			// Update the name display.
			objectName = (Selection.Selected.Count > 1) ? $"({Selection.Selected.Count} objects)" : Selection.Selected[0].Name;
			objectTypeName = selectedType.Name;

			// Get per-type icon.
			if (selectedType.TryGetAttribute(out IconAttribute icon))
			{
				objectIcon = icon.IconGlyph;
			}
			else
			{
				objectIcon = '\uEB8B';
			}

			// Filter and bucket properties by category.
			var buckets = selectedType.GetProperties()
				.Where(o => o.HasAttribute<InspectAttribute>())
				.Where(o => o.Name.Contains(currentFilter, StringComparison.OrdinalIgnoreCase) || o.DeclaringType.Name.Contains(currentFilter, StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(currentFilter))
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
			InspectorContent = new StackPanel()
				.Orientation(Orientation.Vertical)
				.Children(expanders.ToArray());
		}
	}
}