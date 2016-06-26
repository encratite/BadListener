using System;

namespace BadListener.Extension
{
	class CompilerException : Exception
	{
		public CompilerException(string message)
			: base(message)
		{
		}
	}
}
