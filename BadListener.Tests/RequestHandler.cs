using System;
using BadListener.Attribute;

namespace BadListener.Tests
{
	class RequestHandler
	{
		[Controller]
		public void Index()
		{
			var context = Context.ListenerContext;
			var request = context.Request;
			string output = $"Time: {DateTimeOffset.Now}\n";
			output += $"RawUrl: {request.RawUrl}\n";
			output += $"Url: {request.Url}\n";
			output += "\nQueryString:\n";
			foreach (string key in request.QueryString)
				output += $"{key}: {request.QueryString[key]}\n";
			output += "\nHeaders:\n";
			foreach (string key in request.Headers)
				output += $"{key}: {request.Headers[key]}\n";
			context.Response.SetStringResponse(output, "text/plain");
		}
	}
}
