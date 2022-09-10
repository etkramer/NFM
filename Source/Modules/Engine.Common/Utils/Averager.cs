using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common
{
	public class Averager
	{
		[Notify] public double Result { get; private set; }

		private int threshold = 0;
		private int count = 0;
		private double[] values;

		/// <summary>
		/// Constructs a new Averager
		/// </summary>
		/// <param name="threshold">The number of values that can be added before the average is updated</param>
		public Averager(int threshold)
		{
			this.threshold = threshold;
			values = new double[threshold];
		}

		/// <summary>
		/// Adds a value to the average, then updates it if it has reached the threshold given at construction.
		/// </summary>
		public void AddValue(double value)
		{
			values[count] = value;
			if (++count == threshold)
			{
				double result = 0d;
				for (int i = 0; i < threshold; i++)
				{
					result += values[i];
				}
				Result = result / threshold;

				count = 0;
			}
		}
	}
}
