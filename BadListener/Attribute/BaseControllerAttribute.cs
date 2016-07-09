using System.Net;

namespace BadListener.Attribute
{
	public abstract class BaseControllerAttribute : System.Attribute
	{
		public abstract void Render(string name, object model, HttpListenerContext context, HttpServer server);
	}
}
