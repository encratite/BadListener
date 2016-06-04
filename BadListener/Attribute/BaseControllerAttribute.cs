using System.Net;

namespace BadListener.Attribute
{
	public abstract class BaseControllerAttribute : System.Attribute
	{
		public abstract void Render(object model, HttpListenerResponse response, HttpServer server);
	}
}
