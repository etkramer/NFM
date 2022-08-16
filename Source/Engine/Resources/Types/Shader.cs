using System;

namespace Engine.Resources
{
	public class Shader : Resource
	{
		public string Source { get; set; }

		public override void OnLoad()
		{
			base.OnLoad();
		}
	}
}