using System.Collections.Generic;
using System.Linq;

namespace BadListener.Extension
{
	class CodeGenerator
	{
		MatchState _ModelPattern = new MatchState("^@model (.+)$");
		MatchState _StatementPattern = new MatchState(@"^@(if|for|foreach|while\s*\(.+\))$");

		CodeBuilder _Builder;
		string _Model;
		List<string> _Literals;
		int _LineCounter;

		public string GenerateCode(string viewName, string input, string @namespace)
		{
			InitializeState();

			_Builder.AppendLine("using BadListener;");
			_Builder.AppendLine();
			bool addNamespace = !string.IsNullOrEmpty(@namespace);
			if (addNamespace)
			{
				_Builder.AppendLine($"namespace {@namespace}");
				_Builder.IncreaseIndentation();
			}
			_Builder.AppendLine($"class {viewName} : View");
			_Builder.IncreaseIndentation();
			GenerateRenderCode(input);
			_Builder.DecreaseIndentation();
			if (addNamespace)
				_Builder.DecreaseIndentation();
			string output = _Builder.ToString();
			return output;
		}

		private void InitializeState()
		{
			_Builder = new CodeBuilder();
			_Model = null;
			_Literals = new List<string>();
			_LineCounter = 1;
		}

		private void GenerateRenderCode(string input)
		{
			_Builder.IncreaseIndentation();
			var lines = input.Split('\n');
			foreach (string untrimmedLine in lines)
			{
				string line = untrimmedLine.Trim();
				ProcessLine(line);
				_LineCounter++;
			}
			MergeAndEmitLiterals();
			_Builder.DecreaseIndentation();
			if (_Model == null)
				throw new CompilerException("No model has been set.");
			_Builder.PrependLine($"public override void Render({_Model} Model)");
		}

		private void ProcessLine(string line)
		{
			if (line == "{")
			{
				MergeAndEmitLiterals();
				_Builder.IncreaseIndentation();
			}
			else if (line == "}")
			{
				MergeAndEmitLiterals();
				_Builder.DecreaseIndentation();
			}
			else if (line.Length > 0 && line[0] == '@')
			{
				if (_ModelPattern.Matches(line))
				{
					if (_Model != null)
						throw GetCompilerException("Encountered multiple model definitions.");
					_Model = _ModelPattern.Group(1);
				}
				if (_StatementPattern.Matches(line))
				{
					string statement = _StatementPattern.Group(1);
					_Builder.AppendLine(statement);
				}
				else
				{
					throw GetCompilerException("Unknown statement.");
				}
			}
			else
			{
				_Literals.Add(line);
			}
		}

		private void MergeAndEmitLiterals()
		{
			if (!_Literals.Any())
				return;
			string mergedLiterals = string.Join("\n", _Literals);
			string escapedString = EscapeString(mergedLiterals);
			_Builder.AppendLine($"Write(\"{escapedString}\");");
			_Literals.Clear();
		}

		private string EscapeString(string input)
		{
			var replacements = new StringReplacement[]
			{
				new StringReplacement("\\", "\\\\"),
				new StringReplacement("\r", "\\r"),
				new StringReplacement("\n", "\\n"),
				new StringReplacement("\"", "\\\""),
			};
			string output = input;
			foreach (var replacement in replacements)
				output = replacement.Execute(output);
			return output;
		}

		private CompilerException GetCompilerException(string message)
		{
			return new CompilerException(message, _LineCounter);
		}
	}
}
