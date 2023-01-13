using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace NFM.Common
{
	public class ObservableRangeCollection<T> : ObservableCollection<T>
	{
		public void AddRange(T[] items)
		{
			foreach (T item in items)
			{
				Items.Add(item);
			}

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		}
	}
}
