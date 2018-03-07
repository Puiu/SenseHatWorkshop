using CloudHandlers;
using Xunit;

namespace CloudHandlersTests
{
    public class AzureDeviceToCloudMessageHelperTests
    {
        private readonly AzureDeviceToCloudMessageHelper<DummyMeasure> helper;

        public AzureDeviceToCloudMessageHelperTests()
        {
            helper = new AzureDeviceToCloudMessageHelper<DummyMeasure>();
        }

        [Fact]
        public void GetMessage_ExpectedNotNull()
        {
            
            var measure = new DummyMeasure() { Temp = 12 };

            var message = helper.GetMessageFromMeasure(measure);

            Assert.NotNull(message);
        }

        
    }

    
 }
