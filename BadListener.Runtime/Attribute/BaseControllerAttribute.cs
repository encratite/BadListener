using System;
using System.Net;

namespace BadListener.Runtime
{
    public enum ControllerMethod
    {
        Get,
        Post,
    }

	public abstract class BaseControllerAttribute : Attribute
	{
        public ControllerMethod Method { get; private set; }

        protected BaseControllerAttribute(ControllerMethod method)
        {
            Method = method;
        }

        public void PerformSanityChecks(HttpListenerContext context)
        {
            var request = context.Request;
            string httpMethod = GetHttpMethod(Method);
            if (httpMethod != request.HttpMethod)
                throw new ServerException("Unexpected HTTP method.", true);
            if (
                Method == ControllerMethod.Post &&
                request.Url.Host != request.UrlReferrer.Host
            )
                throw new ServerException("Invalid referrer.", true);
        }

		public abstract void Render(string name, object model, HttpListenerContext context, HttpServer server);

        private string GetHttpMethod(ControllerMethod method)
        {
            if (method == ControllerMethod.Get)
                return "GET";
            else if (method == ControllerMethod.Post)
                return "POST";
            else
                throw new ArgumentException("Unknown HTTP method.");
        }
	}
}
