global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;
global using System.IO;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Windows.Forms;
global using NFM.Common;
global using NFM.Mathematics;
global using NFM.World;
using NFM.Hosting;

namespace NFM;

static class Program
{
	public static MainForm MainForm { get; private set; } = null!;

	[STAThread]
	static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

		Engine.Init();
		_ = Project.LoadStartupAsync();

		MainForm = new MainForm();

		// The window stays hidden until the page has rendered, so bring it up by hand.
		_ = MainForm.Handle;

		Application.Run(MainForm);

		Engine.Cleanup();
	}
}
