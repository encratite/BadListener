using System;

namespace BadListener.Runtime
{
	class ServerException : Exception
	{
		public bool SendToBrowser { get; private set; }

		public ServerException(string message, bool sendToBrowser = false)
			: base(message)
		{
			SendToBrowser = sendToBrowser;
		}
	}
}
