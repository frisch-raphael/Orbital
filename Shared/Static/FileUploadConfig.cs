using System.Collections.Generic;

namespace Shared.Config
{
    public static class FileUploadConfig
    {
        public static int kMaxFileSize = 1024 * 1024 * 100;

        public static readonly List<string> kSupportedExtensions = new List<string>()
        {
            ".exe",
            ".dll"
        };


    }
}
