using System.Net;

namespace ModuloMonolitico.Web.Extensions;

public static class ExtensoesHttpRequest
{
    public static string ObterIpCliente(this HttpRequest request)
    {
        var cfIp = request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(cfIp))
            return cfIp;

        var remoteIp = request.HttpContext.Connection.RemoteIpAddress;

        if (remoteIp is null)
            return string.Empty;

        if (remoteIp.IsIPv4MappedToIPv6)
            return remoteIp.MapToIPv4().ToString();

        if (IPAddress.IPv6Loopback.Equals(remoteIp))
            return "127.0.0.1";

        return remoteIp.ToString();
    }
}
