using System.Runtime.CompilerServices;

namespace Services.Helpers
{
    public static class LoggingHelper
    {
        public static string GetCodeInfo([CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null, [CallerFilePath] string filePath = null)
        {
            return $"File Path: {filePath}, Method: {caller}, Line Number:{lineNumber}. ";
        }
    }
}
