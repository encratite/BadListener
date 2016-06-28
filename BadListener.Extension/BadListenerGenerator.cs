using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using VSLangProj80;

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
				var generator = new CodeGenerator();
				string code = generator.GenerateCode(viewName, bstrInputFileContents, wszDefaultNamespace);
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
					uint line = notSpecified;
					var compilerException = exception as CompilerException;
					if (compilerException != null && compilerException.Line.HasValue)
						line = (uint)compilerException.Line.Value;
					pGenerateProgress.GeneratorError(0, 0, exception.Message, line, notSpecified);
				}
				return VSConstants.E_FAIL;
			}
		}

		#endregion


	}
}
