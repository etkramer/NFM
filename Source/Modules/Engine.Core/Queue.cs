using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Core
{
	public static class Queue
	{
		private static ConcurrentDictionary<int, List<Action>> queues = new();

		public static void Schedule(Action action, int queueID)
		{
			var list = queues.GetOrAdd(queueID, new List<Action>());

			lock (list)
			{
				list.Add(action);
			}
		}

		public static void Run(int queueID)
		{
			if (queues.TryGetValue(queueID, out List<Action> list))
			{
				lock (list)
				{
					foreach (var action in list)
					{
						action.Invoke();
					}

					list.Clear();
				}
			}
		}
	}
}
