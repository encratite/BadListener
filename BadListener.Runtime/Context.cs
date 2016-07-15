using System;
using System.Diagnostics;
using System.Net;

namespace BadListener.Runtime
{
    public class Context
	{
		[ThreadStatic]
		public static HttpListenerRequest Request = null;

        [ThreadStatic]
        public static HttpListenerResponse Response = null;

        [ThreadStatic]
        public static Stopwatch Stopwatch = null;

		public static void OnBeginRequest(HttpListenerContext context)
		{
			Request = context.Request;
            Response = context.Response;
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
		}

        public static void OnEndRequest()
        {
            Stopwatch.Stop();
        }

		public static void Dispose()
		{
            Stopwatch = null;
            Response = null;
            Request = null;
		}
	}
}
