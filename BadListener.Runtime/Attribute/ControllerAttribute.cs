using System;
using System.Linq;
using System.Net;

namespace BadListener.Runtime.Attribute
{
	public class ControllerAttribute : BaseControllerAttribute
	{
		public override void Render(string name, object model, HttpListenerContext context, HttpServer server)
		{
			var handlerType = server.RequestHandler.GetType();
			var assemblyTypes = handlerType.Assembly.GetExportedTypes();
			var viewTypes = assemblyTypes.Where(type =>
				type.BaseType.IsGenericType &&
				type.BaseType.GetGenericTypeDefinition() == typeof(View<>) &&
				type.Name == name
			);
			var viewType = viewTypes.FirstOrDefault();
			if (viewType == null)
				throw new ServerException("Unable to find a corresponding view.");
			var method = viewType.GetMethod("Render");
			var instance = Activator.CreateInstance(viewType);
			var arguments = new object[] { model };
			string content = (string)method.Invoke(instance, arguments);
			context.Response.SetStringResponse(content, MimeType.TextHtml);
		}
	}
}
