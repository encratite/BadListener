using System;
using System.Net;
using System.Text;

namespace BadListener
{
	public class Context
	{
		[ThreadStatic]
		public static HttpListenerContext ListenerContext = null;

		[ThreadStatic]
		public static StringBuilder ResponseBuilder = new StringBuilder();
	}
}
