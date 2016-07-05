using System;
using System.IO;
using System.Net;

namespace BadListener
{
    public class Context
	{
		[ThreadStatic]
		public static HttpListenerContext ListenerContext = null;

		[ThreadStatic]
		public static StreamWriter Writer = null;

        public static void Initialize(HttpListenerContext listenerContext)
        {
            ListenerContext = listenerContext;
            var memoryStream = new MemoryStream();
            Writer = new StreamWriter(memoryStream);
        }

        public static void Dispose()
        {
            try
            {
                if (Writer != null)
                    Writer.Dispose();
                Writer = null;
                ListenerContext = null;
            }
            catch
            {
            }
        }
	}
}
