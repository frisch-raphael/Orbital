using System.Collections.Generic;

namespace Shared.Services
{
    public static class FileConfig
    {
        public static readonly long kMaxFileSize = 1024 * 1024 * 100;

        public static readonly List<string> kSupportedExtensions = new List<string>()
        {
            ".exe",
            ".dll"
        };
    }
}
