using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Threading.Tasks;
using Utils.Interfaces;

namespace CloudHandlers
{
    public class AzureDeviceToCloudMessageHandler<T> : IAzureDeviceToCloudMessageHandler<T>
    {
        private readonly ISettings _settings;
        private readonly ILogger<IAzureDeviceToCloudMessageHandler<T>> _logger;
        private readonly IAzureDeviceToCloudMessageHelper<T> _helper;

        private readonly string iotHubUri = "";
        private readonly string deviceKey = "";
        private readonly string deviceId = "";

        private DeviceClient deviceClient;
        public AzureDeviceToCloudMessageHandler(ISettings settings, ILogger<IAzureDeviceToCloudMessageHandler<T>> logger, IAzureDeviceToCloudMessageHelper<T> helper)
        {
            _settings = settings;
            _logger = logger;
            _helper = helper;

            iotHubUri = _settings.IoTHubUri;
            deviceKey = settings.DeviceKey;
            deviceId = settings.DeviceId;
        }
        public async Task HandleAsync(T measure)
        {
            _logger.LogInformation("Saving to IoTHub...");
            Message deviceToCloudMessage = new Message();
            try
            {
                deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));

                deviceToCloudMessage = _helper.GetMessageFromMeasure(measure);

                
                await RetryPolicy().ExecuteAsync(() => SendDeviceToCloudMessageAsync(deviceToCloudMessage));
            }
            finally
            {
                deviceToCloudMessage?.Dispose();
            }
        }


        private Polly.Retry.RetryPolicy RetryPolicy()
        {
            return Policy
                   .Handle<Exception>()
                   .WaitAndRetryAsync(_settings.NoOfSaveRetries, count =>
                   {
                       return TimeSpan.FromMilliseconds(_settings.NoOfMillisecondsBetweenMeasureSaveRetries);
                   },
                   onRetry: (exception, timespan, retryCount, context) =>
                   {
                       _logger.LogError("Error sending to IoTHub. Retry no.: {0}; Exception: {1}", retryCount, exception.Message);
                   });
        }


        private async Task SendDeviceToCloudMessageAsync(Message message)
        {
            try
            {
                await deviceClient.SendEventAsync(message);
            }
            finally
            {
                deviceClient?.Dispose();
            }
        }
    }
}
