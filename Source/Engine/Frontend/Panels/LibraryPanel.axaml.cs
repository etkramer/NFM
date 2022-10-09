using System;
using System.Collections;
using Avalonia.LogicalTree;
using Engine.Resources;

namespace Engine.Frontend
{
	public partial class LibraryPanel : ToolPanel
	{
		public ObservableCollection<MountFolder> FolderTree { get; set; } = new();

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
					Name = folderName
				});
			}

			AddFromPathRecurse(pathComponents, lastComponent + 1, lastFolder.Folders.First(o => o.Name == folderName));
		}

		public class Folder
		{
			public string Name { get; set; }
			public ObservableCollection<Folder> Folders { get; set; } = new();
		}

		public class MountFolder : Folder
		{
			public MountPoint Mount { get; set; }
		}
	}
}
