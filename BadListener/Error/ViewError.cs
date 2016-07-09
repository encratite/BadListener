using System;

namespace BadListener.Error
{
	class ViewError : Exception
	{
		public ViewError(string message)
			: base(message)
		{
		}
	}
}
