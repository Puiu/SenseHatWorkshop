using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Utils.Models;
using Utils.Interfaces;

namespace Flow
{
    public class Flow<T> : IFlow<T>
    {
        /// <summary>
        /// Used for testing the loop functionality
        /// </summary>
        public Exception LoopException { get; set; }

        public event EventHandler<MeasurementResultEventArgs<T>> OnMeasurementReady;
        public CancellationTokenSource CancellationTokenSource { get; set; }


        private readonly ISenseHatHandler<T> _senseHat;
        private readonly ILogger<IFlow<T>> _logger;
        private readonly ISettings _settings;
        private readonly IAzureDeviceToCloudMessageHandler<T> _cloudHandler;

        private int loopInterval;

        public Flow(ISenseHatHandler<T> senseHat,
                            IAzureDeviceToCloudMessageHandler<T> cloudHandler,
                            ILogger<IFlow<T>> logger,
                            ISettings settings
                            )
        {
            _cloudHandler = cloudHandler;
            _senseHat = senseHat;
            _logger = logger;
            _settings = settings;

            CancellationTokenSource = new CancellationTokenSource();
            loopInterval = _settings.NoOfMillisecondsForLoopInterval;
        }



        public async Task RunMeasurementContinuously()
        {

            _logger.LogInformation("Starting Flow<{0}>", typeof(T).FullName);

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting a new loop in {0} milliseconds...", loopInterval);
                    await Task.Delay(TimeSpan.FromMilliseconds(loopInterval));

                    await DoMeasureAndSendAsync();

                    _logger.LogInformation("Loop finished successfully!");

                    

                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in Flow<{0}>! Error {1}", typeof(T).FullName, ex);
                    LoopException = ex;
#if DEBUG
                    Debug.WriteLine(ex.ToString());
#endif
                }
            }
        }

        public async Task DoMeasureAndSendAsync()
        {

            var measure = await DoMeasure();

            await _cloudHandler.HandleAsync(measure);

        }

        private async Task<T> DoMeasure()
        {
            var measure = await _senseHat.GetDataFromSensorsAsync();

            OnMeasurementReady?.Invoke(this, new MeasurementResultEventArgs<T> { Measurement = measure });

            _logger.LogDebug("Measuring is finished.");
            return measure;
        }

        public void StopMeasurementRun()
        {

            _logger.LogDebug("Stop Flow triggered!");

            if (CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested)
                CancellationTokenSource.Cancel();

        }

        public void Dispose()
        {
            _logger.LogDebug("Disposing...");
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }
    }
}
