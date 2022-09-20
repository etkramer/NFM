using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Engine.Common
{
	public static class Queue
	{
		private static ConcurrentDictionary<int, List<Action>> queues = new();

		public static void Add(Action action, int queueID)
		{
			if (queues.TryGetValue(queueID, out List<Action> list))
			{
				list.Add(action);
			}
			else
			{
				list = new List<Action>();
				list.Add(action);
				queues.TryAdd(queueID, list);
			}
		}

		public static void Invoke(int queueID)
		{
			if (queues.TryGetValue(queueID, out List<Action> list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Invoke();
				}

				list.Clear();
			}
		}
	}
}
