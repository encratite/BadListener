using System;
using System.Collections.Generic;
using System.Text;

namespace BadListener.Extension
{
	class CodeBuilder
	{
		private StringBuilder _StringBuilder = new StringBuilder();
		private int _IndentationLevel = 0;
        private HashSet<int> _LambdaIndentationLevels = new HashSet<int>();

		public void AppendLine(string line = "")
		{
			string value = GetLine(line);
			_StringBuilder.AppendLine(value);
		}

		public void PrependLine(string line = "")
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
            if (_LambdaIndentationLevels.Contains(_IndentationLevel))
            {
			    AppendLine("});");
                _LambdaIndentationLevels.Remove(_IndentationLevel);
            }
            else
            {
                AppendLine("}");
            }
		}

        public void AddLambdaIndentationLevel()
        {
            _LambdaIndentationLevels.Add(_IndentationLevel);
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
