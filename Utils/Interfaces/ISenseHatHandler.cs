using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Interfaces
{
    public interface ISenseHatHandler<T>
    {
        Task<T> GetDataFromSensorsAsync();
    }
}
