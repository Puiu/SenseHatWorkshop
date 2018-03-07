using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils.Models;

namespace Utils.Interfaces
{
    public interface IFlow<T> : IDisposable
    {
        event EventHandler<MeasurementResultEventArgs<T>> OnMeasurementReady;
        CancellationTokenSource CancellationTokenSource { get; set; }

        Task RunFlowAsync();
        void StopFlowRun();
        Task DoMeasureAndSendAsync();
    }
}
