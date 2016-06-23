using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BadListener.Extension
{
    [ComVisible(true)]
    [CodeGeneratorRegistrationAttribute(typeof(BadListenerGenerator), "BadListenerGenerator", "20443771-0bd7-4631-9a37-9f9ffe7c2a04")]
    public class BadListenerGenerator : IVsSingleFileGenerator
    {
        #region Implementation of interface IVsSingleFileGenerator

        int IVsSingleFileGenerator.DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".html.cs";
            return VSConstants.S_OK;
        }

        int IVsSingleFileGenerator.Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            try
            {
                string code = GenerateCode(bstrInputFileContents);
                var bytes = Encoding.UTF8.GetBytes(code);
                int outputLength = bytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
                Marshal.Copy(bytes, 0, rgbOutputFileContents[0], outputLength);
                pcbOutput = (uint)outputLength;
                return VSConstants.S_OK;
            }
            catch
            {
                rgbOutputFileContents = null;
                pcbOutput = 0;
                return VSConstants.E_FAIL;
            }
        }

        #endregion

        private string GenerateCode(string input)
        {
            return $"{DateTimeOffset.Now}\n{input}";
        }
    }
}
