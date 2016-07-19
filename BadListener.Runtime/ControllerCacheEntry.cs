using System.Reflection;

namespace BadListener.Runtime
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
