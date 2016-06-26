using System;

namespace BadListener.Extension
{
	class CompilerException : Exception
	{
		public int? Line { get; private set; }

		public CompilerException(string message, int? line = null)
			: base(message)
		{
			Line = line;
		}
	}
}
