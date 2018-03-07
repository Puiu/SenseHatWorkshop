using CloudHandlers;
using Flow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SenseHatHandler;
using Serilog;
using System;
using System.Diagnostics;
using Utils.Environment;
using Utils.Interfaces;
using Utils.Models;
using Windows.Storage;

namespace SenseHatWorkshop.Services
{
    public static class SenseHatServices
    {
        public static IServiceProvider Services { get; private set; }

        static SenseHatServices()
        {
            RegisterImplementations();

            ConfigureLogger();
        }

        private static void RegisterImplementations()
        {
            var services = new ServiceCollection()
                        .AddLogging()

                        .AddSingleton<ISettings, Settings>()

                        

                        .AddTransient(typeof(IAzureDeviceToCloudMessageHandler<>), typeof(AzureDeviceToCloudMessageHandler<>))
                        .AddTransient(typeof(IAzureDeviceToCloudMessageHelper<>), typeof(AzureDeviceToCloudMessageHelper<>))
                        
                        .AddTransient<ISenseHatHandler<SenseHatTelemetry>, SenseHatHandler.SenseHatHandler>()

                        .AddTransient(typeof(IFlow<>), typeof(Flow.Flow<>))
                        ;

            Services = services.BuildServiceProvider();
        }

        private static void ConfigureLogger()
        {
            var settingsReader = Services.GetService<ISettings>();

            string logFilPath = GetFullFilePath(settingsReader);


            Log.Logger = new LoggerConfiguration()

               .MinimumLevel.Debug()

               .WriteTo.File(logFilPath)

               .CreateLogger();

            //https://github.com/serilog/serilog/wiki/Debugging-and-Diagnostics
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            var loggerFactory = Services.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddSerilog();
        }

        private static string GetFullFilePath(ISettings settingsReader)
        {
            StorageFolder appLocalFolder = ApplicationData.Current.LocalFolder;
            var logFilPath = appLocalFolder.Path + settingsReader.LogFilePathAndName;
            return logFilPath;
        }
    }
}
