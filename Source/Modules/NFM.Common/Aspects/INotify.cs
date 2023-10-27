using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace NFM.Common;

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
		FieldInfo? eventField = GetType().GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.Public  | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (eventField == null)
		{
			return;
		}

		PropertyChangedEventHandler? eventHandler = eventField.GetValue(this) as PropertyChangedEventHandler;
		Guard.NotNull(eventHandler).Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

public static class NotifyExtensions
{
	public static void SubscribeFast<TSubject>(this TSubject subject, string propertyName, Action callback)
	{
		if (subject is INotifyPropertyChanged changable)
		{
			changable.PropertyChanged += (o, e) =>
			{
				if (e.PropertyName == propertyName || propertyName == null)
				{
					callback?.Invoke();
				}
			};
		}
	}

	public static void SubscribeFast<TSubject>(this TSubject subject, string propertyName1, string propertyName2, Action callback)
	{
		SubscribeFast(subject, propertyName1, callback);
		SubscribeFast(subject, propertyName2, callback);
	}

	public static void SubscribeFast<TSubject>(this TSubject subject, string propertyName1, string propertyName2, string propertyName3, Action callback)
	{
		SubscribeFast(subject, propertyName1, callback);
		SubscribeFast(subject, propertyName2, callback);
		SubscribeFast(subject, propertyName3, callback);
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