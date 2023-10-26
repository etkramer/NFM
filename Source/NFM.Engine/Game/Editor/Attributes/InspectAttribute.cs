using AspectInjector.Broker;

namespace NFM;

/// <summary>
/// Marks an object as both saved and visible in the inspector.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
[Injection(typeof(NotifyAspect))]
public class InspectAttribute : Attribute
{
	public InspectAttribute()
	{

	}
}