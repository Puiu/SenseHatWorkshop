using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Utils.Interfaces;

namespace CloudHandlers
{
    public class AzureDeviceToCloudMessageHelper<T> : IAzureDeviceToCloudMessageHelper<T>
    {
        public Message GetMessageFromMeasure(T measure)
        {
            var messageString = JsonConvert.SerializeObject(measure);

            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            message.Properties.Add("measure", measure.GetType().Name);
            return message;
        }

        
    }
}
