using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace TPM_QAS.Helpers
{
    public static class FileNameHelper
    {
        public static string SanitizeFileName(string originalFileName, bool capLength = false)
        {
            if (string.IsNullOrWhiteSpace(originalFileName))
                return string.Empty;

            string sanitized;
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                var invalidChars = Path.GetInvalidFileNameChars();
                sanitized = new string(originalFileName
                    .Where(ch => !invalidChars.Contains(ch))
                    .ToArray());
                sanitized = sanitized.TrimEnd('.', ' ');
            }
            else
            {
                sanitized = originalFileName.Replace("/", "");
            }

            if (capLength)
            {
                const int maxLength = 255;
                if (sanitized.Length > maxLength)
                {
                    string ext = Path.GetExtension(sanitized);
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(sanitized);
                    int allowedLength = maxLength - ext.Length;
                    if (nameWithoutExt.Length > allowedLength)
                        nameWithoutExt = nameWithoutExt.Substring(0, allowedLength);
                    sanitized = nameWithoutExt + ext;
                }
            }

            return sanitized;
        }
    }
}
