using System.Reflection;
using BadListener.Attribute;

namespace BadListener
{
	class ControllerCacheEntry
	{
		public MethodInfo Method { get; private set; }

		public BaseControllerAttribute Attribute { get; private set; }

		public ControllerCacheEntry(MethodInfo method, BaseControllerAttribute attribute)
		{
			Method = method;
			Attribute = attribute;
		}
	}
}
