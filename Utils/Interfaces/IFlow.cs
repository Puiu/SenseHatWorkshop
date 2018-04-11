using System;
using System.Threading;
using System.Threading.Tasks;
using Utils.Models;

namespace Utils.Interfaces
{
    public interface IFlow<T> : IDisposable
    {
        event EventHandler<MeasurementResultEventArgs<T>> OnMeasurementReady;
        CancellationTokenSource CancellationTokenSource { get; set; }

        Task RunMeasurementContinuously();
        void StopMeasurementRun();
        Task DoMeasureAndSendAsync();
    }
}
