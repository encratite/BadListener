using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BadListener.Extension
{
	class CodeGenerator
	{
		private const string _Prefix = "@";

		private List<string> _Lines;
		private CodeBuilder _Builder;
		private List<string> _Literals;
		private int _LineCounter;

		public string GenerateCode(string viewName, string input, string @namespace)
		{
			InitializeState();
			SetLines(input);
			GenerateUsingStatements();
			GenerateNamespace(viewName, @namespace);
			string output = _Builder.ToString();
			return output;
		}

		private void InitializeState()
		{
			_Builder = new CodeBuilder();
			_Literals = new List<string>();
			_LineCounter = 1;
			_Lines = new List<string>();
		}

		private void GenerateNamespace(string viewName, string @namespace)
		{
            bool useNamespace = !string.IsNullOrEmpty(@namespace);
            if (useNamespace)
            {
			    _Builder.AppendLine($"namespace {@namespace}");
			    _Builder.IncreaseIndentation();
            }
			GenerateClass(viewName);
            if (useNamespace)
			    _Builder.DecreaseIndentation();
		}

		private void GenerateUsingStatements()
		{
			GenerateDefaultUsingStatements();
			var usingPattern = new MatchState("^" + _Prefix + "using (.+)$");
			var newLines = new List<string>();
			foreach (string line in _Lines)
			{
				if (usingPattern.Matches(line))
				{
					string @namespace = usingPattern.GetGroup(1);
					AddNamespace(@namespace);
				}
				else
				{
					newLines.Add(line);
				}
			}
			_Lines = newLines;
			_Builder.AppendLine();
		}

		private void GenerateDefaultUsingStatements()
		{
			var defaultNamespaces = new string[]
			{
				"BadListener",
				"System",
				"System.Collections.Generic",
				"System.Linq",
				"System.Text",
			};
			foreach (string @namespace in defaultNamespaces)
				AddNamespace(@namespace);
		}

		private void GenerateRenderFunction()
		{
			_Builder.AppendLine("protected override void Execute()");
			_Builder.IncreaseIndentation();
			foreach (string line in _Lines)
			{
				ProcessLine(line);
				_LineCounter++;
			}
			MergeAndEmitLiterals();
			_Builder.DecreaseIndentation();
		}

		private void GenerateClass(string viewName)
		{
			string model = null;
			var modelPattern = new MatchState("^" + _Prefix + "model (.+)$");
			var newLines = new List<string>();
			foreach (string line in _Lines)
			{
				if (modelPattern.Matches(line))
				{
					if (model != null)
						throw GetCompilerException("Encountered multiple model definitions.");
					model = modelPattern.GetGroup(1);
				}
				else
				{
					newLines.Add(line);
				}
			}
            if (model == null)
                model = "object";
			_Lines = newLines;
			_Builder.AppendLine($"public class {viewName} : View<{model}>");
			_Builder.IncreaseIndentation();
			_Builder.SetHelperOffset();
			GenerateRenderFunction();
			_Builder.DecreaseIndentation();
		}

		private void ProcessLine(string line)
		{
			var sectionPattern = new MatchState("^" + _Prefix + "section (.+)$");
			var helperPattern = new MatchState("^" + _Prefix + "helper (.+)$");
			var blockPattern = new MatchState("^" + _Prefix + "((?:if|for|foreach|while)\\s*\\(.+\\))$");
			var callPattern = new MatchState("^" + _Prefix + "(.*)$");
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
			else if (sectionPattern.Matches(line))
			{
				MergeAndEmitLiterals();
				string section = sectionPattern.GetGroup(1);
				string escapedSection = EscapeString(section);
				_Builder.AppendLine($"DefineSection(\"{escapedSection}\", () =>");
				_Builder.EnterSection();
			}
			else if (helperPattern.Matches(line))
			{
				MergeAndEmitLiterals();
				string signature = helperPattern.GetGroup(1);
				_Builder.EnterHelper();
				_Builder.AppendLine($"public void {signature}");
			}
			else if (blockPattern.Matches(line))
			{
				MergeAndEmitLiterals();
				string block = blockPattern.GetGroup(1);
				_Builder.AppendLine(block);
			}
			else if (callPattern.Matches(line))
			{
				MergeAndEmitLiterals();
				string call = callPattern.GetGroup(1);
				_Builder.AppendLine($"{call};");
			}
			else
			{
				ProcesLiteralsAndInlineStatements(line);
			}
		}

		private void ProcesLiteralsAndInlineStatements(string line)
		{
			var inlinePattern = new Regex("[^" + _Prefix + "]+|" + _Prefix + "([A-Za-z0-9_.\\[\\]]+)|" + _Prefix + "{(.+?)}");
			var matches = inlinePattern.Matches(line + "\n");
			foreach (Match match in matches)
			{
				var groups = match.Groups;
				var literalGroup = groups[0];
				var expressionGroup = groups[1];
				var extendedExpressionGroup = groups[2];
				if (extendedExpressionGroup.Success)
				{
					MergeAndEmitLiterals();
					GenerateWrite(extendedExpressionGroup.Value);
				}
				else if (expressionGroup.Success)
				{
					MergeAndEmitLiterals();
					GenerateWrite(expressionGroup.Value);
				}
				else if (literalGroup.Success)
				{
					string literal = literalGroup.Value;
					_Literals.Add(literal);
				}
			}
		}

		private void MergeAndEmitLiterals()
		{
			if (!_Literals.Any())
				return;
			string mergedLiterals = string.Join("", _Literals);
			if (mergedLiterals.Length > 0 && mergedLiterals.Any(c => c != '\n'))
			{
				string escapedString = EscapeString(mergedLiterals);
				GenerateWrite($"\"{escapedString}\"");
			}
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

		private void SetLines(string input)
		{
			var lines = input.Split('\n');
			foreach (string line in lines)
			{
				string trimmedLine = line.Trim();
				_Lines.Add(trimmedLine);
			}
		}

		private void AddNamespace(string @namespace)
		{
			_Builder.AppendLine($"using {@namespace};");
		}

		private void GenerateWrite(string expression)
		{
			_Builder.AppendLine($"Write({expression});");
		}
	}
}
