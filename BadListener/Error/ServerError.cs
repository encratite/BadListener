using System;

namespace BadListener.Error
{
	class ServerError : Exception
	{
		public ServerError(string message)
			: base(message)
		{
		}
	}
}
