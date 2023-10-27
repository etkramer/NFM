using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AspectInjector.Broker;

namespace NFM.Common;

public static class StaticNotify
{
	private static event Action<Type, string> staticPropertyChanged = delegate {};

	public static void Raise(Type type, string propertyName)
	{
		staticPropertyChanged.Invoke(type, propertyName);
	}

	public static void Subscribe(Type type, string propertyName, Action callback)
	{
		staticPropertyChanged += (o, p) =>
		{
			if (o == type && p == propertyName)
			{
				callback();
			}
		};
	}
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Constructor)]
[Injection(typeof(NotifyAspect))]
public class NotifyAttribute : Attribute
{
	public NotifyAttribute() {}
}

[Mixin(typeof(INotify))]
[Aspect(Scope.PerInstance)]
public sealed class NotifyAspect : INotify
{
	public event PropertyChangedEventHandler? PropertyChanged = (o, e) => {};

	[Advice(Kind.After, Targets = Target.Setter | Target.AnyAccess | Target.AnyScope)]
	public void AfterSetter([Argument(Source.Instance)] object source, [Argument(Source.Name)] string propertyName, [Argument(Source.Type)] Type type)
	{
		// Invoke PropertyChanged.
		PropertyChanged?.Invoke(source, new PropertyChangedEventArgs(propertyName));

		if (source == null)
		{
			StaticNotify.Raise(type, propertyName);
		}
	}
}