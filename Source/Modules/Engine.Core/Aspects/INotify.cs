using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Engine.Core
{
	public interface INotify : INotifyPropertyChanged
	{
		public void Subscribe(string propertyName, Action callback)
		{
			PropertyChanged += (o, e) =>
			{
				if (e.PropertyName == propertyName || propertyName == null)
				{
					callback?.Invoke();
				}
			};
		}

		public void Raise(string propertyName)
		{
			((Delegate)GetType().GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this)).DynamicInvoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public static class NotifyExtensions
	{
		public static void Subscribe(this INotifyPropertyChanged subject, string propertyName, Action callback)
		{
			subject.PropertyChanged += (o, e) =>
			{
				if (e.PropertyName == propertyName || propertyName == null)
				{
					callback?.Invoke();
				}
			};
		}

		public static void Subscribe(this INotifyCollectionChanged subject, Action callback)
		{
			subject.CollectionChanged += (o, e) => callback();
		}
	}
}