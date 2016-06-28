using System.Collections.Generic;
using System.Linq;

namespace BadListener.Extension
{
	class CodeGenerator
	{
		List<string> _Lines;
		CodeBuilder _Builder;
		List<string> _Literals;
		int _LineCounter;

		public string GenerateCode(string viewName, string input, string @namespace)
		{
			InitializeState();
			SetLines(input);
			GenerateUsingStatements();
			_Builder.AppendLine($"namespace {@namespace}");
			_Builder.IncreaseIndentation();
			_Builder.AppendLine($"class {viewName} : View");
			_Builder.IncreaseIndentation();
			GenerateRenderFunction();
			_Builder.DecreaseIndentation();
			_Builder.DecreaseIndentation();
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

		private void GenerateUsingStatements()
		{
			GenerateDefaultUsingStatements();
			var usingPattern = new MatchState("^@using (.+)$");
			var newLines = new List<string>();
			foreach (string line in _Lines)
			{
				if (usingPattern.Matches(line))
				{
					string @namespace = usingPattern.Group(1);
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
				// "System.Threading.Tasks",
			};
			foreach (string @namespace in defaultNamespaces)
				AddNamespace(@namespace);
		}

		private void GenerateRenderFunction()
		{
			GenerateRenderSignature();
			_Builder.IncreaseIndentation();
			foreach (string line in _Lines)
			{
				ProcessLine(line);
				_LineCounter++;
			}
			MergeAndEmitLiterals();
			_Builder.DecreaseIndentation();
		}

		private void GenerateRenderSignature()
		{
			string model = null;
			var modelPattern = new MatchState("^@model (.+)$");
			var newLines = new List<string>();
			foreach (string line in _Lines)
			{
				if (modelPattern.Matches(line))
				{
					if (model != null)
						throw GetCompilerException("Encountered multiple model definitions.");
					model = modelPattern.Group(1);
				}
				else
				{
					newLines.Add(line);
				}
			}
			_Lines = newLines;
			if (model == null)
				throw new CompilerException("No model has been set.");
			_Builder.AppendLine($"public override void Render({model} Model)");
		}

		private void ProcessLine(string line)
		{
			var sectionPattern = new MatchState("^@section (.+)$");
			var statementPattern = new MatchState(@"^@((?:if|for|foreach|while)\s*\(.+\))$");
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
				MergeAndEmitLiterals();
				if (sectionPattern.Matches(line))
				{
					string section = sectionPattern.Group(1);
					string escapedSection = EscapeString(section);
					_Builder.AppendLine($"DefineSection(\"{escapedSection}\", () =>");
					_Builder.AddLambdaIndentationLevel();
				}
				else if (statementPattern.Matches(line))
				{
					string statement = statementPattern.Group(1);
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
	}
}
