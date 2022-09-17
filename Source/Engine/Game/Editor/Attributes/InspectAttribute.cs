using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AspectInjector.Broker;

namespace Engine
{
	/// <summary>
	/// Marks an object as both saved and visible in the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	[Injection(typeof(NotifyAspect))]
	public class InspectAttribute : SaveAttribute
	{
		public string Tooltip { get; set; }

		public InspectAttribute() {}

		public InspectAttribute(string tooltip)
		{
			Tooltip = tooltip;
		}
	}
}