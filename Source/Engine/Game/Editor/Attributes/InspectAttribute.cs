using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AspectInjector.Broker;

namespace Engine
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Constructor)]
	[Injection(typeof(NotifyAspect))]
	public class InspectAttribute : Attribute
	{
		public string Tooltip { get; set; }

		public InspectAttribute() {}

		public InspectAttribute(string tooltip)
		{
			Tooltip = tooltip;
		}
	}
}