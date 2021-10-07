using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MatBlazor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Api.ApiErrors;
using Shared.Config;

namespace Ui.Services
{
    public class RodinHttpClient
    {
        public readonly HttpClient Client;
        private readonly OrbitalOptions OrbitalOptions;
        private ILogger<FileUploader> Logger { get; set; }
        private IMatToaster Toaster { get; set; }

        public RodinHttpClient(HttpClient client,
            IOptions<SharedOptions> sharedOptions,
            ILogger<FileUploader> logger,
            IMatToaster toaster)
        {

            OrbitalOptions = sharedOptions.Value.orbitalOptions;
            Client = client;
            Client.BaseAddress = new Uri(OrbitalOptions.BaseAddress + "api/");
            Logger = logger;
            Toaster = toaster;
        }


        public async Task ShowAndLogError(HttpResponseMessage httpErrorResponse)
        {
            ApiError errorResponse = await GetApiError(httpErrorResponse);

            var method = httpErrorResponse.RequestMessage.Method;
            var localPath = httpErrorResponse.RequestMessage.RequestUri.LocalPath;


            Logger.LogWarning(
                "Error while trying to {Method} to '{Url}'. {ErrorMessage}",
                method,
                localPath,
                errorResponse.Message);

            Toaster.Add($"Error while trying to {method} '{localPath}'. {errorResponse.Message}", MatToastType.Danger);
        }

        private async Task<ApiError> GetApiError(HttpResponseMessage response)
        {
            using Stream responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<ApiError>(
                responseStream
            );
        }
    }
}
