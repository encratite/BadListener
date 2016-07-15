using System.Net;

namespace BadListener.Runtime.Attribute
{
	public abstract class BaseControllerAttribute : System.Attribute
	{
		public abstract void Render(string name, object model, HttpListenerContext context, HttpServer server);
	}
}
