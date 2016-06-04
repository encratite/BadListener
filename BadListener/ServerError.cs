using System;

namespace BadListener
{
	class ServerError : Exception
	{
		public ServerError(string message)
			: base(message)
		{
		}
	}
}
