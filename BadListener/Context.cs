using System;
using System.Net;

namespace BadListener
{
	public class Context
	{
		[ThreadStatic]
		public static HttpListenerContext Current = null;
	}
}
