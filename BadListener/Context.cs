using System;
using System.IO;
using System.Net;

namespace BadListener
{
    public class Context
	{
		[ThreadStatic]
		public static HttpListenerContext Current = null;

        public static void Initialize(HttpListenerContext context)
        {
            Current = context;
        }

        public static void Dispose()
        {
            Current = null;
        }
	}
}
