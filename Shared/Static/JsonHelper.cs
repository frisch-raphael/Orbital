using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Static
{
    public static class JsonHelper
    {
        public static async Task<T> DeserializeAsync<T>(Stream response)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            //var test = await JsonSerializer.DeserializeAsync<Object>(response);
            return await JsonSerializer.DeserializeAsync<T>(response, options);
        }
    }
}
