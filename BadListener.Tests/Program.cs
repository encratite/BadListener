using System;
using System.Linq;

namespace BadListener.Tests
{
    class Program
    {
        static void Main(string[] arguments)
        {
            if (arguments.Length != 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("    <URL prefix of HTTP server>");
                return;
            }
            string prefix = arguments.First();
            var server = new HttpServer(prefix);
            server.Start();
        }
    }
}
