using NFM.World;
using ReactiveUI;

namespace NFM;

public class MainWindowModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	public MainWindowModel()
	{

	}

	public async void OpenPressed(object sender)
	{
		var openPath = (await Dialog.ShowOpenDialog(MainWindow.Instance, new Dialog.FileFilter("NFM Project", "json"))).FirstOrDefault();

		if (openPath is not null)
		{
			Project.Load(openPath);
		}
	}

	public void SavePressed(object sender)
	{
		if (Project.Path is null)
		{
			SaveAsPressed(sender);
			return;
		}

		Project.Save(null);
	}

	public async void SaveAsPressed(object sender)
	{
		var savePath = await Dialog.ShowSaveDialog(MainWindow.Instance, new Dialog.FileFilter("NFM Project", "json"));

		if (savePath is not null)
		{
			Project.Save(savePath);
		}
	}

	public void NewPressed() => Project.Reset();
	public void QuitPressed(object sender) => MainWindow.Instance.Close();
	public void DeletePressed() => Selection.Selected.OfType<Node>().ToArray().ForEach(o => o.Dispose());
}
