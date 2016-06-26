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
			string value = GetLine(line);
			_StringBuilder.AppendLine(value);
		}

		public void PrependLine(string line)
		{
			string value = GetLine(line) + Environment.NewLine;
			_StringBuilder.Insert(0, value);
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

		private string GetLine(string line)
		{
			string indentation = new string('\t', _IndentationLevel);
			return indentation + line;
		}
	}
}
