using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Interfaces
{
    public interface IHeaderHelper
    {
        string GetAppVersion();
        string TryGetLocalIPAddress();
        Task<string> TryGetExternalIPAddressAsync();
    }
}
