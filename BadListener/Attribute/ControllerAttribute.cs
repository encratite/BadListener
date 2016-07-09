using System;
using System.Linq;
using System.Net;

namespace BadListener.Attribute
{
	public class ControllerAttribute : BaseControllerAttribute
	{
		public override void Render(string name, object model, HttpListenerContext context, HttpServer server)
		{
			var handlerType = server.RequestHandler.GetType();
			var assemblyTypes = handlerType.Assembly.GetTypes();
			var viewType = assemblyTypes.Where(type => type.BaseType == typeof(View<>) && type.Name == name).FirstOrDefault();
			if (viewType == null)
				throw new ServerError("Unable to find a corresponding view.");
			var method = viewType.GetMethod("Render");
			var instance = Activator.CreateInstance(viewType);
			var arguments = new object[] { model };
			string content = (string)method.Invoke(instance, arguments);
			context.Response.SetStringResponse(content, "text/html");
		}
	}
}
