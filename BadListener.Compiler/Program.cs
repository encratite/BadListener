using BadListener.Extension;
using System;
using System.IO;
using System.Reflection;

namespace BadListener.Compiler
{
    class Program
    {
        static void Main(string[] arguments)
        {
            if (arguments.Length != 3)
            {
                var assembly = Assembly.GetExecutingAssembly();
                string program = Path.GetFileName(assembly.Location);
                Console.WriteLine("Usage:");
                Console.WriteLine($"{program} <Input.cshtml> <Output.cs> <namespace>");
                return;
            }
            try
            {
                string viewPath = arguments[0];
                string codePath = arguments[1];
                string @namespace = arguments[2];
                GenerateCode(viewPath, codePath, @namespace);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message} ({exception.GetType()})");
            }
        }

        private static void GenerateCode(string viewPath, string codePath, string @namespace)
        {
            string viewName = Path.GetFileNameWithoutExtension(viewPath);
            string input = File.ReadAllText(viewPath);
            var generator = new CodeGenerator();
            string output = generator.GenerateCode(viewName, input, @namespace);
            File.WriteAllText(codePath, output);
        }
    }
}
