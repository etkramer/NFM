using System;
using Avalonia.Controls;
using Engine.Resources;	

namespace Engine.Frontend
{
	class LibraryFolder
	{
		public string Name { get; set; }
		public bool IsRoot { get; set; }
		public List<LibraryFolder> Subfolders { get; set; }
	}

	public partial class LibraryPanel : ToolPanel
	{
		List<LibraryFolder> Folders { get; set; } = new();

		public LibraryPanel()
		{
			InitializeComponent();
			DataContext = this;

			BuildFolderTree();
		}

		private void BuildFolderTree()
		{
			foreach (AssetPrefix prefix in AssetPrefix.All)
			{
				LibraryFolder folder = new LibraryFolder()
				{
					Name = prefix.Name,
					IsRoot = true,
					Subfolders = new()
				};

				BuildFolderTreeRecurse($"{prefix.ID}:", folder);
				Folders.Add(folder);
			}
		}

		private void BuildFolderTreeRecurse(string folderRoot, LibraryFolder parent)
		{
			foreach (string subfolder in GetSubfolders(folderRoot))
			{
				string fullPath = folderRoot + subfolder + '/';
				LibraryFolder folder = new LibraryFolder()
				{
					Name = subfolder,
					IsRoot = false,
					Subfolders = new()
				};
				
				BuildFolderTreeRecurse(fullPath, folder);
				parent.Subfolders.Add(folder);
			}
		}

		private string[] GetSubfolders(string folderPath)
		{
			var folderPathComponents = folderPath.Split(new[] { ':', '/' }, StringSplitOptions.RemoveEmptyEntries);
			List<string> subfolders = new();

			foreach (var assetPath in Asset.Assets.Keys)
			{
				// Cannot be a folder, so skip.
				if (!assetPath.Contains('/'))
				{
					continue;
				}

				var assetPathComponents = assetPath.Split(':', '/');

				if (assetPathComponents.Take(folderPathComponents.Length).SequenceEqual(folderPathComponents, StringComparer.OrdinalIgnoreCase))
				{
					string subfolderPath = assetPath.Substring(0, assetPath.LastIndexOf('/'));
					string subfolderName = subfolderPath.Split(':', '/').Take(folderPathComponents.Length + 1).Last();
					
					// This is just the parent folder, skip it.
					if (subfolderPath.Trim('/') == folderPath.Trim('/'))
					{
						continue;
					}

					if (!subfolders.Contains(subfolderName))
					{
						subfolders.Add(subfolderName);
					}
				}
			}

			return subfolders.ToArray();
		}
	}
}