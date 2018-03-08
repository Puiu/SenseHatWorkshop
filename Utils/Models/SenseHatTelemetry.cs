using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Models
{
    public class SenseHatTelemetry
    {

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double Pressure { get; set; }

        public override string ToString()
        {
            return $"{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.fff")}: Temp: {Temperature}; Hum: {Humidity}; Pressure: {Pressure}";
        }
    }
}
