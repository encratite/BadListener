using System;

namespace BadListener.Runtime.Error
{
	class ServerError : Exception
	{
		public ServerError(string message)
			: base(message)
		{
		}
	}
}
