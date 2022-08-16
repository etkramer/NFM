using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Logging;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;

namespace Engine.Platform
{
	public class FastRenderLoop : IRenderLoop
	{
        private List<IRenderLoopTask> tasks = new List<IRenderLoopTask>();

        public FastRenderLoop()
        {
			AvaloniaLocator.Current.GetService<IRenderTimer>().Tick += TimerTick;
        }

		public static event Action OnUpdate = delegate {};

        public void Add(IRenderLoopTask task) => tasks.Add(task);
        public void Remove(IRenderLoopTask task) => tasks.Remove(task);

        private void TimerTick(TimeSpan time)
        {
			// Update tasks.
			foreach (var task in tasks)
			{
				if (task.NeedsUpdate)
				{
					task.Update(time);
				}
			}

			// Render tasks.
			foreach (var task in tasks)
			{
				task.Render();
			}
        }
	}
}