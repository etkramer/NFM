using System;

namespace Engine.Resources
{
	public class Shader : Resource
	{
		public string ShaderSource { get; set; }

		public override void OnLoad()
		{
			base.OnLoad();
		}
	}
}