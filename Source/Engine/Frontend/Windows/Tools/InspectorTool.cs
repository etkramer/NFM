using System;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Layout;
using Engine.Editor;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using ISelectable = Engine.Editor.ISelectable;
using System.Text.RegularExpressions;

namespace Engine.Frontend
{
	public class InspectorTool : ToolWindow
	{
		[Notify] public Control InspectorContent { get; private set; } = null;

		[Notify] private string objectName { get; set; }
		[Notify] private string objectTypeName { get; set; }
		[Notify] private char objectIcon { get; set; }

		[Notify] private string currentFilter { get; set; } = "";

		public InspectorTool()
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
						.Background(this.GetResourceBrush("ToolForeground"))
						.Row(1)
						.Children(
							new StackPanel()
								.Orientation(Orientation.Horizontal)
								.Children(
									new TextBlock()
										//.Text("\uEB8B")
										.Text(nameof(objectIcon), BindingMode.Default)
										.VerticalAlignment(VerticalAlignment.Center)
										.Size(26)
										.Margin(7, 0, 7, 0)
										.Foreground("#B0E24D")
										.Font(this.GetResource<FontFamily>("IconsFont")),
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
			objectName = (Selection.Selected.Count > 1) ? $"({Selection.Selected.Count} objects)" : Selection.Selected[0].GetName();
			objectTypeName = selectedType.Name;
			objectIcon = '\uE3C2';

			// Filter and bucket properties by category.
			var buckets = selectedType.GetProperties()
				.Where(o => o.HasAttribute<InspectAttribute>())
				.Where(o => o.Name.Contains(currentFilter, StringComparison.OrdinalIgnoreCase) || o.DeclaringType.Name.Contains(currentFilter, StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(currentFilter))
				.Bucket(o => o.DeclaringType);

			// Create the property grid.
			List<Control> propertyGrid = new();
			foreach (var bucket in buckets.Reverse())
			{
				Type bucketType = bucket.First().DeclaringType;

				// Get the bucket's display name.
				string bucketName = bucketType.Name.PascalToDisplay();
				if (bucketName.EndsWith(" Actor"))
					bucketName = bucketName.Remove(bucketName.Length - 6);

				// Build the property grid.
				var propertyInputs = bucket.Select(p => new PropertyInput(Selection.Selected, p));
				propertyGrid.Add(
					new Expander()
						.IsExpanded(true)
						.Header(bucketName)
						.With(o => o.FontWeight = FontWeight.SemiBold)
						.Content(
							new StackPanel().Children(propertyInputs.ToArray())
						)
				);
			}

			// Submit the property grid.
			InspectorContent = new StackPanel()
				.Orientation(Orientation.Vertical)
				.Children(propertyGrid.ToArray());
		}
	}
}