namespace POS; 

public static class Constants
{     
    public static string LocalhostUrl = "";
    public static string Scheme = "https"; // or http
    public static string Port = "5001";
    public static string RestUrl = $"{Scheme}://{LocalhostUrl}:{Port}/api/Host/{{0}}";
    public static string DeviceMAC = "";
    public static string DeviceID = "";
}
