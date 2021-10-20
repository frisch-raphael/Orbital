using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MatBlazor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Api.ApiErrors;
using Shared.Config;
using Shared.Dtos;
using Shared.Static;

namespace Ui.Services
{
    public class OrbitalHttpClient
    {
        public readonly HttpClient Client;
        private ILogger<FileUploader> Logger { get; set; }
        private IMatToaster Toaster { get; set; }

        public OrbitalHttpClient(HttpClient client,
            IOptions<SharedOptions> sharedOptions,
            ILogger<FileUploader> logger,
            IMatToaster toaster)
        {
            var orbitalOptions = sharedOptions.Value.orbitalOptions;
            Client = client;
            Client.BaseAddress = new Uri(orbitalOptions.BaseAddress + "api/");
            Logger = logger;
            Toaster = toaster;
        }


        public async Task ShowAndLogError(HttpResponseMessage httpErrorResponse)
        {
            var errorResponse = await GetApiError(httpErrorResponse);

            var method = httpErrorResponse.RequestMessage.Method;
            var localPath = httpErrorResponse.RequestMessage.RequestUri.LocalPath;


            Logger.LogWarning(
                "Error while trying to {Method} to '{Url}'. {ErrorMessage}",
                method,
                localPath,
                errorResponse.Message);

            Toaster.Add($"Error while trying to {method} '{localPath}'. {errorResponse.Message}", MatToastType.Danger);
        }

        static private async Task<ApiError> GetApiError(HttpResponseMessage response)
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<ApiError>(
                responseStream
            );
        }

        ///<param name="endpoint">endpoint of the resource to get. Should not begin with '/'</param>
        public async Task<List<T>> GetResourceListFromOrbital<T>(string endpoint)
        {
            var resources = new List<T>();
            HttpResponseMessage response;

            try
            {
                response = await Client.GetAsync(endpoint);
            }
            catch (Exception ex)
            {
                Toaster.Add(ex.Message, MatToastType.Danger);
                return resources;
            }

            if (!response.IsSuccessStatusCode)
            {
                await ShowAndLogError(response);
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            resources = await JsonHelper.DeserializeAsync<List<T>>(responseStream);


            return resources;
        }

        ///<param name="endpointWithId">endpoint of the resource to get, with the id of the resource. Should not begin with '/'.<br></br>i.e "Payloads/1"</param>
        public async Task<T> GetResourceFromOrbital<T>(string endpointWithId)
        {
            HttpResponseMessage response;

            try
            {
                response = await Client.GetAsync(endpointWithId);
            }
            catch (Exception ex)
            {
                Toaster.Add(ex.Message, MatToastType.Danger);
                return default;
            }

            if (!response.IsSuccessStatusCode)
            {
                await ShowAndLogError(response);
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonHelper.DeserializeAsync<T>(responseStream);
        }
    }
}
