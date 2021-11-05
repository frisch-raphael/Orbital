using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using MatBlazor;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Api.ApiErrors;
using Shared.Config;
using Shared.Dtos;
using Shared.Static;
using static System.Net.Mime.MediaTypeNames;

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

            var errorMessage = IsErrorFromApi(httpErrorResponse.StatusCode) ? 
                (await GetApiError(httpErrorResponse)).Message : 
                $"Status code is {httpErrorResponse.StatusCode}";


                var method = httpErrorResponse.RequestMessage?.Method;
                var localPath = httpErrorResponse.RequestMessage?.RequestUri?.LocalPath;


            Logger.LogWarning(
                "Error while trying to {Method} to '{Url}'. {ErrorMessage}",
                method,
                localPath,
                errorMessage);

            Toaster.Add($"Error while trying to {method} '{localPath}'. {errorMessage}", MatToastType.Danger);
        }

        private static bool IsErrorFromApi(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.InternalServerError;
        }

        private static async Task<ApiError> GetApiError(HttpResponseMessage response)
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<ApiError>(
                responseStream
            );
        }

        ///<param name="endpoint">endpoint of the resource to get. Should not begin with '/'</param>
        public async Task<T> GetResourceListFromOrbital<T>(string endpoint)
        {
            HttpResponseMessage response;

            try
            {
                response = await Client.GetAsync(endpoint);
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


        ///<param name="endpoint">endpoint of the resource to get. Should not begin with '/'</param>
        ///<param name="postData">Object to post</param>
        public async Task PostToOrbital(string endpoint, object postData)
        {
            HttpResponseMessage response = new();

            try
            {
                response = await Client.PostAsync(endpoint, JsonHelper.SerializeAsync(postData));
            }
            catch (Exception ex)
            {
                Toaster.Add(ex.Message, MatToastType.Danger);
            }

            if (!response.IsSuccessStatusCode)
            {
                await ShowAndLogError(response);
            }


        }
    }
}
