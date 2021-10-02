using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Dtos;
using Shared.Config;
using static System.Net.WebRequestMethods;
using Shared.Api.ApiErrors;

namespace Ui.Services
{
    public interface IFileUploader
    {

        Task UploadFile(HttpContent fileContent, string fileName, string endpoint);
    }

    public class FileUploader : IFileUploader
    {
        private RodinHttpClient RodinHttpClient { get; }

        private ILogger<FileUploader> Logger { get; set; }

        private IMatToaster Toaster { get; set; }

        public FileUploader(
            RodinHttpClient rodinHttpClient,
            ILogger<FileUploader> logger,
            IMatToaster toaster)
        {
            RodinHttpClient = rodinHttpClient;
            Logger = logger;
            Toaster = toaster;
        }

        public async Task UploadFile(
            HttpContent fileContent,
            string fileName,
            string endpoint)
        {
            try
            {
                ValidateArgs(fileContent, fileName, endpoint);

                HttpResponseMessage response = await PostFile(fileContent, fileName, endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    await RodinHttpClient.ShowAndLogError(response);
                    return;
                }

                Toaster.Add($"{fileName} uploaded", MatToastType.Success);
            }
            catch (Exception ex)
            {
                HandleFileUploadError(fileName, endpoint, ex.Message);
            }
        }

        private void HandleFileUploadError(
            string fileName,
            string endpoint,
            string messageError)
        {
            Logger.LogWarning(
                "Error while trying to {Method} {FileName} to '{Url}'. {ErrorMessage}",
                "Post",
                fileName,
                endpoint,
                messageError
            );

            Toaster.Add($"Could not upload {fileName}. {messageError}", MatToastType.Danger);
        }

        private void ValidateArgs(HttpContent fileContent, string fileName, string path)
        {
            if (fileContent is null)
            {
                throw new ArgumentNullException(nameof(fileContent));
            }

            if (fileContent.ReadAsStream().Length > FileUploadConfig.kMaxFileSize)
            {
                throw new ArgumentException("File too big for upload");
            }


            var extension = Path.GetExtension(fileName);
            if (!FileUploadConfig.kSupportedExtensions.Contains(extension))
            {
                throw new ArgumentException($"{extension} extension is not supported");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

        }

        private async Task<HttpResponseMessage> PostFile(HttpContent fileContent, string fileName, string endpoint)
        {
            using MultipartFormDataContent formData = new MultipartFormDataContent();

            formData.Add(
                fileContent,
                name: "\"file\"",
                fileName
            );

            var client = RodinHttpClient.Client;

            return await client.PostAsync(
                endpoint,
                formData
             );
        }
    }
}
