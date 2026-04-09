using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;

namespace Mgb.Network;

public static class NetworkHelper
{
    private static int? _chosenPort;
    private static string? _chosenIp;
    private static IConfiguration? _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static string? GetPreferredLocalIPAddress()
    {
        string? wireGuardIp = null;
        string? localPrivateIp = null;
        
        
        if (!string.IsNullOrEmpty(_chosenIp))
            return _chosenIp;

        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up) continue;

            var ipProps = ni.GetIPProperties();
            foreach (var addr in ipProps.UnicastAddresses)
            {
                if (addr.Address.AddressFamily != AddressFamily.InterNetwork) continue;

                var ip = addr.Address;
                if (IPAddress.IsLoopback(ip)) continue; // skip 127.0.0.1

                if (ip.ToString().StartsWith("10.8.0."))
                {
                    wireGuardIp = ip.ToString();
                }
                else if (IsPrivateIP(ip))
                {
                    localPrivateIp ??= ip.ToString(); 
                }
            }
        }
        _chosenIp = wireGuardIp ?? localPrivateIp;
        return _chosenIp;
    }


    private static bool IsPrivateIP(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();

        return
            bytes[0] == 10 ||
            (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
            (bytes[0] == 192 && bytes[1] == 168);
    }

    public static int GetConsulChosenPort()
    {
        if (_chosenPort.HasValue)
            return _chosenPort.Value;
        var portValue = _configuration["app:port"];

        if (int.TryParse(portValue, out var port))
        {
            _chosenPort = port;
            return _chosenPort.Value;
        }
            
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        _chosenPort = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();

        return _chosenPort.Value;
    }

    public static int GetKestrelChosenPort()
    {
        GetKestrelChosenPort();
        GetPreferredLocalIPAddress();

        if (_chosenIp.StartsWith("10.8.0."))
            return _chosenPort.Value;

        return 80;
    }
}