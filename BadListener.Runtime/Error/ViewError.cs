using System;

namespace BadListener.Runtime.Error
{
	class ViewError : Exception
	{
		public ViewError(string message)
			: base(message)
		{
		}
	}
}
