using System.Net;

namespace PicoVoiceClient.Utils;

public static class NetUtils
{
    public static IPEndPoint ResolveEndpoint(string host, int port)
    {
        if (IPAddress.TryParse(host, out var ip))
            return new IPEndPoint(ip, port);

        var ips = Dns.GetHostAddresses(host);
        return new IPEndPoint(ips[0], port);
    }
}