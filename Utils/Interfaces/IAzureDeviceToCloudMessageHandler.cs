using System.Threading.Tasks;

namespace Utils.Interfaces
{
    public interface IAzureDeviceToCloudMessageHandler<T>
    {
        Task HandleAsync(T measure);
    }
}
