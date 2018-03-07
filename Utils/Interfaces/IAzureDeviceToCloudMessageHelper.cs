using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;

namespace Utils.Interfaces
{
    public interface IAzureDeviceToCloudMessageHelper<T>
    {
        Message GetMessageFromMeasure(T measure);
        
    }
}
