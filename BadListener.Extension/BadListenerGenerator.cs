﻿using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using VSLangProj80;
using System.Collections.Generic;
using System.Linq;

namespace BadListener.Extension
{
    [ComVisible(true)]
    [Guid("717DB774-0CCA-42F2-885F-E211849C4FFD")]
    [CodeGeneratorRegistration(typeof(BadListenerGenerator), "BadListenerGenerator", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(BadListenerGenerator))]
    public class BadListenerGenerator : IVsSingleFileGenerator
    {
        #region Implementation of interface IVsSingleFileGenerator

        int IVsSingleFileGenerator.DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".cs";
            return VSConstants.S_OK;
        }

        int IVsSingleFileGenerator.Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            try
            {
				string viewName = Path.GetFileNameWithoutExtension(wszInputFilePath);
                string code = GenerateCode(viewName, bstrInputFileContents, wszDefaultNamespace);
                var bytes = Encoding.UTF8.GetBytes(code);
                int outputLength = bytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
                Marshal.Copy(bytes, 0, rgbOutputFileContents[0], outputLength);
                pcbOutput = (uint)outputLength;
                return VSConstants.S_OK;
            }
            catch (Exception exception)
            {
                rgbOutputFileContents = null;
                pcbOutput = 0;
				if (pGenerateProgress != null)
				{
					string message = $"{exception.GetType()}: {exception.Message}";
					const uint notSpecified = unchecked((uint)-1);
					pGenerateProgress.GeneratorError(0, 0, exception.Message, notSpecified, notSpecified);
				}
                return VSConstants.E_FAIL;
            }
        }

        #endregion

        private string GenerateCode(string viewName, string input, string @namespace)
        {
			var builder = new CodeBuilder();
			builder.AppendLine("using BadListener;");
			builder.AppendLine();
			bool addNamespace = !string.IsNullOrEmpty(@namespace);
			if (addNamespace)
			{
				builder.AppendLine($"namespace {@namespace}");
				builder.IncreaseIndentation();
			}
			builder.AppendLine($"class {viewName} : View");
			builder.IncreaseIndentation();
			GenerateRenderCode(input, builder);
			builder.DecreaseIndentation();
			if (addNamespace)
				builder.DecreaseIndentation();
			string output = builder.ToString();
			return output;
        }

		private void GenerateRenderCode(string input, CodeBuilder builder)
		{
			string model = null;
			if (model == null)
				throw new CompilerException("No model has been set.");
			builder.AppendLine($"public override void Render({model} Model)");
			builder.IncreaseIndentation();
			var lines = input.Split('\n');
			var literals = new List<string>();
			foreach (string untrimmedLine in lines)
			{
				string line = untrimmedLine.Trim();
				if (line == "{" || line == "}")
				{
					MergeAndEmitLiterals(literals, builder);
				}
				else
				{
					literals.Add(line);
				}
			}
			MergeAndEmitLiterals(literals, builder);
			builder.DecreaseIndentation();
			throw new NotImplementedException();
		}

		private void MergeAndEmitLiterals(List<string> literals, CodeBuilder builder)
		{
			if (!literals.Any())
				return;
			string mergedLiterals = string.Join("\n", literals);
			string escapedString = EscapeString(mergedLiterals);
			builder.AppendLine($"Write(\"{escapedString}\");");
			literals.Clear();
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
    }
}
