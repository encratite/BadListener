using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using BadListener.Attribute;

namespace BadListener
{
	public class HttpServer : IDisposable
    {
        private HttpListener _Listener;
        private List<Thread> _RequestThreads = new List<Thread>();

		private object _RequestHandler;
		private Dictionary<string, ControllerCacheEntry> _ControllerCache = new Dictionary<string, ControllerCacheEntry>();

        public HttpServer(string prefix, object requestHandler)
        {
            _Listener = new HttpListener();
            _Listener.Prefixes.Add(prefix);
			_RequestHandler = requestHandler;
			SetControllerCache();
        }

        void IDisposable.Dispose()
        {
            Stop();
        }

        public void Start()
        {
            _Listener.Start();
            while (true)
            {
                var context = _Listener.GetContext();
                var requestThread = new Thread(() => OnRequest(context));
                requestThread.Start();
                _RequestThreads.Add(requestThread);
            }
        }

        public void Stop()
        {
            _Listener.Stop();
            foreach (var thread in _RequestThreads)
                thread.Abort();
            _RequestThreads.Clear();
        }

        private void OnRequest(HttpListenerContext context)
		{
			try
			{
				Context.Current = context;
				ProcessRequest(context);
			}
			catch (Exception exception)
			{
				string message;
				if (exception is ServerError)
					message = exception.Message;
				else
					message = "An internal server error occurred.";
				context.Response.SetStringResponse(message, "text/plain");
			}
		}

		private static void ProcessRequest(HttpListenerContext context)
		{
			var request = context.Request;
			var pattern = new Regex("^/([A-Za-z0-9]+)");
			var match = pattern.Match(request.RawUrl);
			if (match == null)
				throw new ServerError("No such controller.");
			throw new NotImplementedException();
		}

		private void SetControllerCache()
		{
			var type = _RequestHandler.GetType();
			var methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
			foreach (var method in methodInfos)
			{
				var attribute = method.GetCustomAttribute<BaseControllerAttribute>();
				if (attribute == null)
					continue;
				var entry = new ControllerCacheEntry(method, attribute);
				_ControllerCache[method.Name] = entry;
			}
		}
    }
}
