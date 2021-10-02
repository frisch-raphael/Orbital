using System.Collections.Generic;

namespace Shared.Services
{
    public static class FileService
    {
        public static readonly long MaxFileSize = 1024 * 1024 * 100;

        public static readonly List<string> SupportedExtensions = new List<string>()
        {
            ".exe",
            ".dll"
        };
    }
}
