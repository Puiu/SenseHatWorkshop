using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            get { return doOneMeasurementCommand ?? (doOneMeasurementCommand = new CommandHandler(async () => await DoOneMeasurementsAsync(), true)); }

        }

        private async Task DoOneMeasurementsAsync()
        {
            try
            {
                SetFlow();
                await flow.DoMeasureAndSendAsync();

                UnsubscribeAndDisposeFlow();

            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion

        #region Measure Continuously Command
        
        #endregion

        #region Stop Measure Continuously Command 
        private ICommand stopMeasureContinuouslyCommand;

        public ICommand StopMeasureContinuouslyCommand
        {
            get { return stopMeasureContinuouslyCommand ?? (stopMeasureContinuouslyCommand = new CommandHandler(() => StopMeasureContinuously(), true)); }
        }

        private void StopMeasureContinuously()
        {
            
            flow.StopFlowRun();

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
            
            
        }

        
    }
}
