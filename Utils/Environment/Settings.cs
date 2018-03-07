using System;
using System.Collections.Generic;
using System.Text;
using Utils.Interfaces;

namespace Utils.Environment
{
    public class Settings : ISettings
    {
        public string DeviceId => "deviceId";

        public string IoTHubUri => "uri";

        public string DeviceKey => "deviceKey";

        public int NoOfMillisecondsForLoopInterval => 10000;

        public int MeasureTaskMaxDurationInMilliseconds => 500;

        public string LogFilePathAndName => @"\Logs\sensehat-{Date}.log";

        public int NoOfSaveRetries => 3;

        public double NoOfMillisecondsBetweenMeasureSaveRetries => 2000;
    }
}
