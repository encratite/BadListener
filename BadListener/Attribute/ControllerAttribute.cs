using System;
using System.Net;

namespace BadListener.Attribute
{
	public class ControllerAttribute : BaseControllerAttribute
	{
		public override void Render(object model, HttpListenerResponse response, HttpServer server)
		{
			// SetStringResponse(html, "text/html", response);
			throw new NotImplementedException();
		}
	}
}
