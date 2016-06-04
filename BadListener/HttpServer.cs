using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace BadListener
{
    public class HttpServer : IDisposable
    {
        private HttpListener _Listener;
        private List<Thread> _RequestThreads = new List<Thread>();

        public HttpServer(string prefix)
        {
            _Listener = new HttpListener();
            _Listener.Prefixes.Add(prefix);
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
                var requestThread = new Thread(() => OnRequest(context));
                requestThread.Start();
                _RequestThreads.Add(requestThread);
            }
        }

        public void Stop()
        {
            _Listener.Stop();
            foreach (var thread in _RequestThreads)
                thread.Abort();
            _RequestThreads.Clear();
        }

        private void OnRequest(HttpListenerContext context)
        {
            var request = context.Request;
            string responseString = $"Time: {DateTimeOffset.Now}\n";
            responseString += $"RawUrl: {request.RawUrl}\n";
            responseString += $"Url: {request.Url}\n";
            responseString += "\nQueryString:\n";
            foreach (string key in request.QueryString)
                responseString += $"{key}: {request.QueryString[key]}\n";
            responseString += "\nHeaders:\n";
            foreach (string key in request.Headers)
                responseString += $"{key}: {request.Headers[key]}\n";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            var response = context.Response;
            response.StatusCode = 200;
            response.ContentType = "text/plain";
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
