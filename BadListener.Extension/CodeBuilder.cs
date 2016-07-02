using System;
using System.Text;

namespace BadListener.Extension
{
	class CodeBuilder
	{

		private StringBuilder _StringBuilder = new StringBuilder();
		private int _IndentationLevel = 0;

		private StringBuilder _HelperStringBuilder = null;
		private int? _HelperOffset = null;
		private int? _HelperIndentationLevel = null;

		private int? _SectionIndentationLevel = null;

		public void AppendLine(string line = "")
		{
			string value = GetLine(line);
			var activeStringBuilder = _HelperStringBuilder ?? _StringBuilder;
			activeStringBuilder.AppendLine(value);
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
			if (_SectionIndentationLevel == _IndentationLevel)
			{
				AppendLine("});");
				_SectionIndentationLevel = null;
			}
			else
			{
				AppendLine("}");
				if (_HelperIndentationLevel == _IndentationLevel)
				{
					string helperBody = _HelperStringBuilder.ToString();
					_StringBuilder.Insert(_HelperOffset.Value, helperBody);
					_HelperOffset += helperBody.Length;
					_HelperStringBuilder = null;
					_HelperIndentationLevel = null;
				}
			}
		}

		public void SetHelperOffset()
		{
			_HelperOffset = _StringBuilder.Length;
		}

		public void EnterHelper()
		{
			IndentationSanityCheck();
			if (!_HelperOffset.HasValue)
				throw new ArgumentException("Helper offset has not been set.");
			if (_HelperStringBuilder != null)
				throw new ArgumentException("Nesting helpers is not permitted.");
			_HelperStringBuilder = new StringBuilder();
			_HelperIndentationLevel = _IndentationLevel;
		}

		public void EnterSection()
		{
			IndentationSanityCheck();
			_SectionIndentationLevel = _IndentationLevel;
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

		private void IndentationSanityCheck()
		{
			if (_HelperIndentationLevel.HasValue || _SectionIndentationLevel.HasValue)
				throw new ArgumentException("Nesting functions is not permitted.");
		}
	}
}
