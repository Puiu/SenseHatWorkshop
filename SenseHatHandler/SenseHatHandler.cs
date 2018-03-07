using Emmellsoft.IoT.Rpi.SenseHat;
using Emmellsoft.IoT.Rpi.SenseHat.Fonts.SingleColor;
using System;
using System.Threading.Tasks;
using Utils.Interfaces;
using Utils.Models;
using Windows.UI;

namespace SenseHatHandler
{
    public class SenseHatHandler : ISenseHatHandler<SenseHatTelemetry>
    {
        public async Task<SenseHatTelemetry> GetDataFromSensorsAsync()
        {
            ISenseHat senseHat = await SenseHatFactory.GetSenseHat().ConfigureAwait(false);


            senseHat.Sensors.HumiditySensor.Update();
            senseHat.Sensors.PressureSensor.Update();

            var temperature = senseHat.Sensors.Temperature ?? -1.0;
            var humidity = senseHat.Sensors.Humidity ?? -1.0;
            var pressure = senseHat.Sensors.Pressure ?? -1.0;

            var telemetry = new SenseHatTelemetry()
            {
                Temperature = temperature,
                Humidity = humidity,
                Pressure = pressure
            };

            WriteTemperatureToLedArray(senseHat, temperature);

            return telemetry;
        }

        private static void WriteTemperatureToLedArray(ISenseHat senseHat, double temperature)
        {
            ISenseHatDisplay display = senseHat.Display;
            var tinyFont = new TinyFont();


            string text = ((int)Math.Round(temperature)).ToString();

            if (text.Length > 2)
            {
                // Too long to fit the display!
                text = "**";
            }

            display.Clear();
            tinyFont.Write(display, text, Colors.White);
            display.Update();
        }
    }
}
