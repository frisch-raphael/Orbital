
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orbital.Controllers;
using Orbital.Pocos;

namespace Orbital.Services
{
    public interface IFileService
    {
        Task StoreFile();
        void RollStorageBack();
    }

    public class FileStorer : IFileService
    {

        private readonly ILogger<PayloadFileStorer> Logger;

        protected readonly UploadedFile UploadedFile;
        public FileStorer(ILogger<PayloadFileStorer> logger, UploadedFile uploadedFile)
        {
            Logger = logger;
            UploadedFile = uploadedFile;
        }

        public async Task StoreFile()
        {

            await using var fs = new FileStream(UploadedFile.StorageFullPath, FileMode.Create);
            await UploadedFile.File.CopyToAsync(fs);
            Logger.LogInformation("{FileName} saved at {Path}",
                UploadedFile.TrustedFileName, UploadedFile.StorageFullPath);
        }

        public void RollStorageBack()
        {
            if (!File.Exists(UploadedFile.StorageFullPath))
            {
                Logger.LogInformation("Trying to delete {FileName} but it does not exist",
                    UploadedFile.StorageFileName);
                return;
            }

            File.Delete(UploadedFile.StorageFullPath);
        }

    }
}
