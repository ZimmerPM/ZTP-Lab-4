using System.Diagnostics;

namespace ProductCart.MAUI.Helpers;


public static class ConnectivityHelper
{
    public static bool IsOnline()
    {
        var current = Connectivity.Current.NetworkAccess;
        var isOnline = current == NetworkAccess.Internet;

        Debug.WriteLine($"Network status: {(isOnline ? "ONLINE" : "OFFLINE")}");
        return isOnline;
    }

    public static async Task<bool> CanReachApiAsync(string url)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var response = await client.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
