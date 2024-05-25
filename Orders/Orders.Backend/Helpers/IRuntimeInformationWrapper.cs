using System.Runtime.InteropServices;

namespace Orders.Backend.Helpers
{
    public interface IRuntimeInformationWrapper
    {
        bool IsOSPlatform(OSPlatform osPlatform);
    }
}