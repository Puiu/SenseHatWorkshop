using CloudHandlers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Utils.Interfaces;
using Xunit;

namespace CloudHandlersTests
{
    public class AzureDeviceToCloudMessageHandlerTests
    {

        [Fact]
        public async Task SaveMeasure_ExpectedException()
        {
            AzureDeviceToCloudMessageHandler<DummyMeasure> handler = GetHandler();

            var measure = new DummyMeasure() { Temp = 12 };

            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(measure));

        }


        private static AzureDeviceToCloudMessageHandler<DummyMeasure> GetHandler()
        {
            var settingsReader = GetSettingsReader();

            var logger = new Mock<ILogger<IAzureDeviceToCloudMessageHandler<DummyMeasure>>>();

            var helper = GetHelper();

            var handler = new AzureDeviceToCloudMessageHandler<DummyMeasure>(settingsReader, logger.Object, helper);
            return handler;
        }

        private static IAzureDeviceToCloudMessageHelper<DummyMeasure> GetHelper()
        {
            var moqHelper = new Mock<IAzureDeviceToCloudMessageHelper<DummyMeasure>>();
            moqHelper.Setup(x => x.GetMessageFromMeasure(It.IsAny<DummyMeasure>()));   //.Returns((m) => { new DummyMeasure() { Temp = 12 } });
            return moqHelper.Object;
        }

        private static ISettings GetSettingsReader()
        {
            var moqSettings = new Mock<ISettings>();
            moqSettings.Setup(di => di.DeviceId).Returns("id");
            moqSettings.Setup(uri => uri.IoTHubUri).Returns("uri");
            moqSettings.Setup(dk => dk.DeviceKey).Returns("key");

            return moqSettings.Object;
        }
    }
}
