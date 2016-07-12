using BadListener.Attribute;
using BadListener.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace BadListener
{
    public class HttpServer : IDisposable
	{
        public event Action OnBeginRequest;
        public event Action OnEndRequest;

        private HttpListener _Listener;
		private List<Thread> _RequestThreads = new List<Thread>();

		private Dictionary<string, ControllerCacheEntry> _ControllerCache = new Dictionary<string, ControllerCacheEntry>();

		public object RequestHandler { get; private set; }

		public HttpServer(string prefix, object requestHandler)
		{
			_Listener = new HttpListener();
			_Listener.Prefixes.Add(prefix);
			RequestHandler = requestHandler;
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
                lock (this)
                {
				    var requestThread = new Thread(() => OnRequest(context));
				    requestThread.Start();
                    _RequestThreads = _RequestThreads.Where(t => t.ThreadState == ThreadState.Running).ToList();
				    _RequestThreads.Add(requestThread);
                }
			}
		}

		public void Stop()
		{
            lock (this)
            {
			    _Listener.Stop();
			    foreach (var thread in _RequestThreads)
                {
                    try
                    {
				        thread.Abort();
                    }
                    catch
                    {
                    }
                }
			    _RequestThreads.Clear();
            }
		}

		private void OnRequest(HttpListenerContext context)
		{
            Context.OnBeginRequest(context);
            OnBeginRequest?.Invoke();
			try
			{
				ProcessRequest(context);
			}
			catch (Exception exception)
			{
				string message;
				if (exception is ServerError)
					message = exception.Message;
				else
					message = "An internal server error occurred.";
				context.Response.SetStringResponse(message, MimeType.Plain);
			}
            Context.OnEndRequest();
            OnEndRequest?.Invoke();
            Context.Dispose();
		}

		private void ProcessRequest(HttpListenerContext context)
		{
			var request = context.Request;
			var pattern = new Regex("^/([A-Za-z0-9]*)");
			var match = pattern.Match(request.RawUrl);
			if (match == null)
				throw new ServerError("Invalid path.");
			string name = match.Groups[1].Value;
			if (name == string.Empty)
				name = "Index";
			ControllerCacheEntry entry;
			if (!_ControllerCache.TryGetValue(name, out entry))
				throw new ServerError("No such controller.");
			Type modelType;
			var model = Invoke(entry.Method, request, out modelType);
			entry.Attribute.Render(name, model, context, this);
		}

		private void SetControllerCache()
		{
			var type = RequestHandler.GetType();
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

		private object Invoke(MethodInfo method, HttpListenerRequest request, out Type modelType)
		{
			var parameters = method.GetParameters();
			var invokeParameters = new List<object>();
			foreach (var parameter in parameters)
				SetParameter(request, invokeParameters, parameter);
			var output = method.Invoke(RequestHandler, invokeParameters.ToArray());
			modelType = method.ReturnType;
			return output;
		}

		private void SetParameter(HttpListenerRequest request, List<object> invokeParameters, ParameterInfo parameter)
		{
			object convertedParameter;
			var type = parameter.ParameterType;
			bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
			string argument = request.QueryString[parameter.Name];
			if (argument != null)
			{
				if (isNullable)
					type = type.GenericTypeArguments.First();
				convertedParameter = Convert.ChangeType(argument, type);
			}
			else
			{
				if (type.IsClass || isNullable)
				{
					convertedParameter = null;
				}
				else
				{
					string message = string.Format("Parameter \"{0}\" has not been specified.", parameter.Name);
					throw new ServerError(message);
				}
			}
			invokeParameters.Add(convertedParameter);
		}
	}
}