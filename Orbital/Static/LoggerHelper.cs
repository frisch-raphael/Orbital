using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Orbital.Static
{
    public static class LoggerHelper
    {
        public static void LogStream(Stream stream, ILogger logger)
        {
            //stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    logger.LogInformation(reader.ReadLine());
                }
            }
        }
    }
}
