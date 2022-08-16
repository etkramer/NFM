using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;

namespace Engine.Platform
{
	public class FastRenderTimer : IRenderTimer
	{
		public event Action<TimeSpan> Tick = delegate {};

        public FastRenderTimer()
        {
			Start();
        }

        private void Start()
        {
			// Run render on UI thread.
			DispatcherTimer.Run(RaiseTick, TimeSpan.Zero, DispatcherPriority.Render);
        }

        private bool RaiseTick()
        {
            Tick(TimeSpan.FromMilliseconds(Environment.TickCount));
			return true;
        }
	}
}
