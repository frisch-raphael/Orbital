using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Orbital.Pocos
{
    public class UploadedFile
    {
        public IFormFile File { get; set; }

        public string TrustedFileName { get; set; }

        public string StorageFullPath { get; set; }

        public string UploadDirectory;

        public string UntrustedFileName { get; set; }

        public string StorageFileName { get; set; }

        public string Extension { get; set; }

        public UploadedFile(IFormFile file)
        {
            File = file;
            UntrustedFileName = file.FileName;
            Extension = Path.GetExtension(UntrustedFileName);
            // random file name but keep extension
            StorageFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(UntrustedFileName));
            TrustedFileName = WebUtility.HtmlEncode(file.FileName);
            var workingDirectory = Environment.CurrentDirectory;
            UploadDirectory = Path.Combine(workingDirectory, "Uploads");
            StorageFullPath = Path.Combine(UploadDirectory, StorageFileName);
        }
    }
}
