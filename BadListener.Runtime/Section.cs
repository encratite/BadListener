using System;

namespace BadListener.Runtime
{
	public class Section
	{
		public string Name { get; private set; }

		public Action Render { get; private set; }

		public Section(string name, Action render)
		{
			Name = name;
			Render = render;
		}
	}
}

