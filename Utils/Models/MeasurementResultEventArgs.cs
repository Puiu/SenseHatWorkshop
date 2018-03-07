using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Models
{
    public class MeasurementResultEventArgs<T> : EventArgs
    {
        public T Measurement { get; set; }
    }
}
