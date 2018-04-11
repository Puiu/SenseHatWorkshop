using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils.Interfaces;
using Utils.Models;

namespace SenseHatWorkshop
{
    public class MainPageViewModel : BindableBase
    {
        #region Properties
        private string appVersion;
        public string AppVersion
        {
            get { return appVersion; }
            set { SetProperty(ref appVersion, value); }
        }

        private ObservableCollection<string> measurements;
        public ObservableCollection<string> Measurements
        {
            get { return measurements; }
            set { measurements = value; }
        }

        private string localIpAddress;
        public string LocalIpAddress
        {
            get { return localIpAddress; }
            set { SetProperty(ref localIpAddress, value); }
        }

        private string externalIpAddress;
        public string ExternalIpAddress
        {
            get { return externalIpAddress; }
            set { SetProperty(ref externalIpAddress, value); }
        }


        private IServiceProvider services;
        private ILogger<MainPageViewModel> logger;
        private IFlow<SenseHatTelemetry> flow;
        #endregion

        public MainPageViewModel()
        {
            Measurements = new ObservableCollection<string>();
        }


        #region Startup data load
        private ICommand loadStartupDataCommand;

        public ICommand LoadStartupDataCommand
        {
            get
            {
                return loadStartupDataCommand ?? (loadStartupDataCommand = new CommandHandler(async () => await StartCommand(), true));
            }
        }

        public async Task StartCommand()
        {
            try
            {
                GetServices();
                logger.LogInformation("Starting...");

                await SetHeader();

            }
            catch (Exception ex)
            {
                logger.LogError("Error Loading data: {0}", ex.ToString());

                Measurements.Add(ex.Message);
            }

        }

        private void GetServices()
        {
            services = SenseHatWorkshop.Services.SenseHatServices.Services;
            logger = services.GetService<ILogger<MainPageViewModel>>();
        }

        private async Task SetHeader()
        {
            var helper = new HeaderHelper();
            AppVersion = helper.GetAppVersion();
            LocalIpAddress = helper.TryGetLocalIPAddress();
            ExternalIpAddress = await helper.TryGetExternalIPAddressAsync();
        }
        #endregion

        #region Do One Measurement Command
        private ICommand doOneMeasurementCommand;

        public ICommand DoOneMeasurementCommand
        {
            get { return doOneMeasurementCommand ?? (doOneMeasurementCommand = new CommandHandler(async () => await DoOneAnalisysAsync(), true)); }

        }

        private async Task DoOneAnalisysAsync()
        {
            try
            {
                SetFlow();
                await flow.DoMeasureAndSendAsync();

                UnsubscribeAndDisposeFlow();

            }
            catch (Exception ex)
            {
                Measurements.Add(ex.Message);
            }
        }
        #endregion

        #region Measure Continuously Command
        private ICommand measureContinuouslyCommand;

        public ICommand MeasureContinuouslyCommand
        {
            get { return measureContinuouslyCommand ?? (measureContinuouslyCommand = new CommandHandler(async () => await AnalyzeForeverAsync(), true)); }
        }

        private async Task AnalyzeForeverAsync()
        {
            try
            {
                Measurements.Insert(0, "Starting a new flow...");
                SetFlow();

                await flow.RunMeasurementContinuously();

                UnsubscribeAndDisposeFlow();
            }
            catch (Exception ex)
            {
                Measurements.Add(ex.Message);
            }
        }
        #endregion

        #region Stop Measure Continuously Command 
        private ICommand stopMeasureContinuouslyCommand;

        public ICommand StopMeasureContinuouslyCommand
        {
            get { return stopMeasureContinuouslyCommand ?? (stopMeasureContinuouslyCommand = new CommandHandler(() => StopMeasureContinuously(), true)); }
        }

        private void StopMeasureContinuously()
        {
            Measurements.Insert(0, "Stopping flow...");
            flow.StopMeasurementRun();

            UnsubscribeAndDisposeFlow();
        }


        #endregion

        private void UnsubscribeAndDisposeFlow()
        {
            flow.OnMeasurementReady -= Flow_OnMeasurementReady;
            flow.Dispose();
        }

        private void SetFlow()
        {
            flow = services.GetService<IFlow<SenseHatTelemetry>>();
            flow.OnMeasurementReady += Flow_OnMeasurementReady;
        }

        private void Flow_OnMeasurementReady(object sender, MeasurementResultEventArgs<SenseHatTelemetry> e)
        {
            Measurements.Insert(0, e.Measurement.ToString());
            CleantMeasurementsList();
        }

        private void CleantMeasurementsList()
        {
            var maxRows = 100;
            if (Measurements.Count > maxRows)
            {
                for (int i = maxRows; i < Measurements.Count; i++)
                {
                    Measurements.RemoveAt(i);
                }
            }
        }
    }
}
