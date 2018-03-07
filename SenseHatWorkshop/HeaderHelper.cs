using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Utils.Interfaces;
using Windows.ApplicationModel;
using Windows.Networking.Connectivity;

namespace SenseHatWorkshop
{
    public class HeaderHelper : IHeaderHelper
    {
        const string defaultIpAddressNoInternet = "255.255.255.255";

        public string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format($"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}");
        }

        public string TryGetLocalIPAddress()
        {
            try
            {
                var icp = NetworkInformation.GetInternetConnectionProfile();

                if (icp?.NetworkAdapter == null) return defaultIpAddressNoInternet;
                var hostname =
                    NetworkInformation.GetHostNames()
                        .FirstOrDefault(
                            hn =>
                                hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                                == icp.NetworkAdapter.NetworkAdapterId);

                // the ip address
                return hostname?.CanonicalName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public async Task<string> TryGetExternalIPAddressAsync()
        {
            try
            {
                return await new HttpClient().GetStringAsync("https://api.ipify.org/");
            }
            catch (HttpRequestException)
            {
                return defaultIpAddressNoInternet;
            }
            catch (Exception exp)
            {
                return exp.Message;
            }

        }
    }
}
