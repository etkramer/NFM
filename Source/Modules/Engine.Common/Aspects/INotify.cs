using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Engine.Common
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
			FieldInfo eventField = GetType().GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.Public  | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (eventField == null)
			{
				return;
			}

			PropertyChangedEventHandler eventHandler = (PropertyChangedEventHandler)eventField.GetValue(this);
			eventHandler.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

		public static void Subscribe(this INotifyCollectionChanged subject, NotifyCollectionChangedEventHandler handler)
		{
			subject.CollectionChanged += handler;
		}

		public static void Unsubscribe(this INotifyCollectionChanged subject, NotifyCollectionChangedEventHandler handler)
		{
			subject.CollectionChanged -= handler;
		}
	}
}