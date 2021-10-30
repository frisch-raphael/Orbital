using System.IO;
using System.Linq;
using Orbital.Model;
using Shared.Dtos;

namespace Orbital.Services
{
    public interface IPayloadDeleter
    {
        void Clean(BackendPayload payloadToDelete);
    }

    public class PayloadDeleter : IPayloadDeleter
    {
        public void Clean(BackendPayload payloadToDelete)
        {
            File.Delete(payloadToDelete.StoragePath);
            // payloadToDelete.Functions.ToList().ForEach(f => File.Delete(f.));
        }
    }
}