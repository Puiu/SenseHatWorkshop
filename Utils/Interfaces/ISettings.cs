using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Interfaces
{
    public interface ISettings
    {
        string DeviceId { get; }

        string IoTHubUri { get; }

        string DeviceKey { get; }

        int NoOfMillisecondsForLoopInterval { get; }
        int MeasureTaskMaxDurationInMilliseconds { get; }
        string LogFilePathAndName { get; }
        int NoOfSaveRetries { get; }
        double NoOfMillisecondsBetweenMeasureSaveRetries { get; }
    }
}
