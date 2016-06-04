using System.Net;
using System.Text;

namespace BadListener
{
	static class HttpListenerResponseExtension
	{
		public static void SetStringResponse(this HttpListenerResponse response, string content, string contentType)
		{
			var buffer = Encoding.UTF8.GetBytes(content);
			response.StatusCode = 200;
            response.ContentType = contentType;
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
		}
	}
}
