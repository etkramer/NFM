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
		public string Category { get; set; }

		public InspectAttribute() {}

		public InspectAttribute(string category)
		{
			Category = category;
		}
	}
}