using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Threading;
using Engine.Editor;
using Engine.World;
using ReactiveUI;

namespace Engine.Frontend
{
	public class MainWindowModel : ReactiveObject, IActivatableViewModel
	{
		public ViewModelActivator Activator { get; } = new();

		public MainWindowModel()
		{

		}

		public async void OpenPressed(object sender)
		{
			string openPath = await Dialog.ShowOpenDialog((sender as Window), new FileFilter("Project", "json"));

			if (openPath != null)
			{
				Project.Load(openPath);
			}
		}

		public void SavePressed(object sender)
		{
			if (Project.Path == null)
			{
				SaveAsPressed(sender);
				return;
			}

			Project.Save(null);
		}

		public async void SaveAsPressed(object sender)
		{
			string savePath = await Dialog.ShowSaveDialog((sender as Window), new FileFilter("Project", "json"));

			if (savePath != null)
			{
				Project.Save(savePath);
			}
		}

		public void NewPressed() => Project.Reset();
		public void QuitPressed(object sender) => (sender as Window).Close();
		public void UndoPressed() => Command.Undo();
		public void RedoPressed() => Command.Redo();
		public void DeletePressed() => Selection.Selected.OfType<Node>().ToArray().ForEach(o => o.Dispose());
	}
}
