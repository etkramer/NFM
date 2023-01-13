using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using NFM.Common;
using NFM.Resources;
using static NFM.Frontend.LibraryPanel;

namespace NFM.Frontend
{
	public partial class LibraryPanel : ToolPanel
	{
		public ObservableCollection<MountFolder> FolderTree { get; set; } = new();

		public ObservableCollection<object> SearchResults { get; set; } = new();

		public LibraryPanel()
		{
			DataContext = this;
			InitializeComponent();

			foreach (var asset in Asset.Assets.Values)
			{
				OnAssetAdded(asset);
			}

			// Leave this here until we get some assets with more interesting paths...
			OnAssetAdded("USER:/models/humans/group01/male_01.mdl");

			// Update results with folder selection.
			folderTreeView.SelectionChanged += (o, e) =>
			{
				var newSelection = e.AddedItems == null ? null : e.AddedItems[e.AddedItems.Count - 1];
				RefreshSearch(searchBox.Text, newSelection as Folder);
			};

			// Update results with search filter.
			searchBox.GetObservable(TextBox.TextProperty).Subscribe((s) =>
			{
				RefreshSearch(s);
			});
		}

		private void RefreshSearch(string search, Folder folderOverride = null)
		{
			SearchResults.Clear();

			var folder = folderOverride ?? (folderTreeView.SelectedItem ??= FolderTree.FirstOrDefault()) as Folder;
			const StringComparison comparisonMode = StringComparison.OrdinalIgnoreCase;

			if (search == null || search?.Length == 0)
			{
				if (folder == null)
				{
					return;
				}

				// Add all assets in folder.
				foreach (var asset in Asset.Assets.Where(o => o.Key.StartsWith(folder.Path, comparisonMode) && o.Key.Count(o2 => o2 == '/') == folder.Path.Count(o2 => o2 == '/')))
				{
					SearchResults.Add(asset.Value);
				}

				// Add all subfolders.
				foreach (var subFolder in GetFolderFromPath(folder.Path).Folders)
				{
					SearchResults.Add(subFolder);
				}
			}
			else
			{
				// Add all assets matching search query.
				foreach (var asset in Asset.Assets.Where(o => o.Key.Contains(search, comparisonMode) && o.Key.StartsWith(folder.Path, comparisonMode)).Select(o => o.Value))
				{
					SearchResults.Add(asset);
				}
			}
		}

		#region Folder Tree

		private Folder GetFolderFromPath(string path)
		{
			string[] pathComponents = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
			var mountFolder = FolderTree.FirstOrDefault(o => o.Name == pathComponents[0].Substring(0, pathComponents[0].Length - 1));

			if (pathComponents.Length == 1)
			{
				return mountFolder;
			}

			return GetFolderFromPathRecurse(pathComponents, 0, mountFolder);
		}

		private Folder GetFolderFromPathRecurse(string[] pathComponents, int lastComponent, Folder lastFolder)
		{
			var nextFolder = lastFolder.Folders.FirstOrDefault(o => o.Name == pathComponents[lastComponent + 1]);

			if (nextFolder.Name == pathComponents.Last())
			{
				return nextFolder;
			}
			else
			{
				return GetFolderFromPathRecurse(pathComponents, lastComponent + 1, nextFolder);
			}
		}

		protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
		{
			Asset.OnAssetAdded += OnAssetAdded;
		}

		protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
		{
			Asset.OnAssetAdded -= OnAssetAdded;
		}

		private void OnAssetAdded(Asset asset) => OnAssetAdded(asset.Path);
		private void OnAssetAdded(string path)
		{
			RefreshSearch(searchBox.Text);

			// Get folder names from "/"-separated path
			string[] pathComponents = path.Split("/");

			// Find mount.
			string mountName = pathComponents[0].Substring(0, pathComponents[0].Length - 1);
			MountPoint mount = MountPoint.All.First(o => o.ID == mountName);

			// Create the folder if needed
			if (!FolderTree.Any(o => o.Mount == mount))
			{
				FolderTree.Add(new MountFolder()
				{
					Path = pathComponents[0] + "/",
					Name = mountName,
					Mount = mount
				});
			}

			AddFromPathRecurse(pathComponents, 0, FolderTree.First(o => o.Mount == mount));
		}

		private void AddFromPathRecurse(string[] pathComponents, int lastComponent, Folder lastFolder)
		{
			if (lastComponent == pathComponents.Length - 2) // -2 instead of -1 because we don't want to include the file
			{
				return;
			}

			string folderName = pathComponents[lastComponent + 1];

			// Create folder if needed.
			if (!lastFolder.Folders.Any(o => o.Name == folderName))
			{
				lastFolder.Folders.Add(new Folder()
				{
					Path = string.Join('/', pathComponents[0..(lastComponent + 2)]) + "/",
					Name = folderName
				});
			}

			AddFromPathRecurse(pathComponents, lastComponent + 1, lastFolder.Folders.First(o => o.Name == folderName));
		}

		public class Folder
		{
			public string Path { get; set; }
			public string Name { get; set; }
			public ObservableCollection<Folder> Folders { get; set; } = new();
		}

		public class MountFolder : Folder
		{
			public MountPoint Mount { get; set; }
		}

		#endregion
	}
}
