using System;
using System.Text;

namespace BadListener.Extension
{
	class CodeBuilder
	{
		private StringBuilder _StringBuilder = new StringBuilder();
		private int _IndentationLevel = 0;

		public void AppendLine(string line = "")
		{
			string indentation = new string('\t', _IndentationLevel);
			_StringBuilder.AppendLine(indentation + line);
		}

		public void IncreaseIndentation()
		{
			AppendLine("{");
			_IndentationLevel++;
		}

		public void DecreaseIndentation()
		{
			if (_IndentationLevel <= 0)
				throw new ApplicationException("Negative indentation level.");
			_IndentationLevel--;
			AppendLine("}");
		}

		public override string ToString()
		{
			return _StringBuilder.ToString();
		}
	}
}
