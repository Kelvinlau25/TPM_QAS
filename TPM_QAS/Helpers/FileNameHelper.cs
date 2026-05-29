using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace TPM_QAS.Helpers
{
    /// <summary>
    /// Provides helper methods for sanitizing filenames to make them safe
    /// for saving on Windows, Linux, or macOS file systems.
    /// </summary>
    public static class FileNameHelper
    {
        /// <summary>
        /// Sanitizes a filename by removing or adjusting invalid characters
        /// depending on the current OS. 
        /// Optionally enforces a maximum length of 255 characters.
        /// </summary>
        /// <param name="originalFileName">The original filename (with extension).</param>
        /// <param name="capLength">True to trim the filename to 255 chars max; false to keep full length.</param>
        /// <returns>A safe filename string.</returns>
        public static string SanitizeFileName(string originalFileName, bool capLength = false)
        {
            if (string.IsNullOrWhiteSpace(originalFileName))
                return string.Empty;

            string sanitized;

            // --- Detect OS ---
            bool isWindows = IsWindows();

            if (isWindows)
            {
                // Windows has strict invalid characters (\ / : * ? " < > |)
                var invalidChars = Path.GetInvalidFileNameChars();

                // Replace invalid chars (remove them if not replaced)
                sanitized = new string(originalFileName
                    .Where(ch => !invalidChars.Contains(ch))
                    .ToArray());

                // Windows also doesn't allow trailing dot or space
                sanitized = sanitized.TrimEnd('.', ' ');
            }
            else
            {
                // Linux/macOS only forbid '/'
                sanitized = originalFileName.Replace("/", "");
            }

            // --- Cap length if requested ---
            if (capLength)
            {
                const int maxLength = 255;

                if (sanitized.Length > maxLength)
                {
                    // Keep the extension intact
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

        /// <summary>
        /// Determines if the current OS is Windows.
        /// Uses RuntimeInformation when available (.NET 4.7.1+ and .NET Core),
        /// falls back to Environment.OSVersion for older frameworks.
        /// </summary>
        private static bool IsWindows()
        {
#if NETFRAMEWORK
            // For older .NET Framework
            var platform = Environment.OSVersion.Platform;
            return platform == PlatformID.Win32NT;
#else
            // For .NET Core / .NET 5+
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
        }
    }
}
