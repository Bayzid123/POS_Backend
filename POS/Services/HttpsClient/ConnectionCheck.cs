using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.Logging;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Models;
using Windows.Media.Protection.PlayReady;
using Windows.System.UserProfile;

namespace POS.Services.HttpsClient;

public class ConnectionCheck : IConnectionCheck
{
    readonly HttpClient _client;
    readonly JsonSerializerOptions _serializerOptions;
    readonly IHttpsClientHandlerService _httpsClientHandlerService;
    readonly IDataServiceFactory _context;

    public ConnectionCheck(IHttpsClientHandlerService service, IDataServiceFactory context)
    {
        _httpsClientHandlerService = service;
        _context = context;
        HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();
        if (handler != null)
            _client = new HttpClient(handler);
        else
            _client = new HttpClient();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
    public async Task<bool> IsInternetAvailable()
    {
        try
        {
            var uri = string.Format(AppSettings.APPURL + "identity/LogIn/Check");

            MessageHelper msg = new MessageHelper();
            HttpResponseMessage response = null;

            response = await _client.GetAsync(uri);


            Ping ping = new Ping();
            PingReply reply = ping.Send("www.google.com", 1000);
            return (reply.Status == IPStatus.Success && response.StatusCode == HttpStatusCode.OK);
        }
        catch
        {
            return false;
        }
    }

    public bool IsInternetAvailableForPreAsync()
    {
        try
        {
            var uri = string.Format(AppSettings.APPURL + "identity/LogIn/Check");

            MessageHelper msg = new MessageHelper();
            HttpResponseMessage response = null;
            HttpRequestMessage hrm = new HttpRequestMessage();
            hrm.RequestUri = new Uri(uri);
            hrm.Method = HttpMethod.Get;
            response = _client.Send(hrm);


            Ping ping = new Ping();
            PingReply reply = ping.Send("www.google.com", 1000);
            return (reply.Status == IPStatus.Success && response.StatusCode == HttpStatusCode.OK);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsServerConnectionAvailable()
    {
        try
        {

            var isAvailable = false;
            var settings = new TblSettings();
            using (var dataService = _context.CreateDataService())
            {
                settings = await dataService.GetSettings();
            }
            if (isAvailable == false)
            {
                if (settings != null)
                {

                    string connectionString = settings.SqlServerConnString;

                    // Extract the IP address and port from the connection string
                    int start = connectionString.IndexOf('=') + 1;
                    int end = connectionString.IndexOf(';');
                    string dataSource = connectionString.Substring(start, end - start);

                    // Split the IP address and port
                    string[] parts = dataSource.Split(',');

                    string ipAddress = parts[0];

                    //Ping ping = new Ping();
                    //PingReply reply = ping.Send(ipAddress, 1000);
                    //if (reply.Status == IPStatus.Success)
                    //{
                    //    isAvailable = true;
                    //}
                    if (settings != null)
                    {
                        if (AppSettings.IsOnline == false)
                        {
                            isAvailable = false;
                        }
                        else
                        {
                            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                            {
                                settings.SqlServerConnString = settings.SqlServerConnString.Replace("30", "2");

                                using (var connection = new SqlConnection(settings.SqlServerConnString))
                                {
                                    try
                                    {
                                        connection.Open();
                                        if (connection != null && connection.State == System.Data.ConnectionState.Open)
                                        {
                                            isAvailable = true;
                                        }
                                        connection.Close();
                                    }
                                    catch
                                    {
                                        isAvailable = false;
                                    }

                                }

                            }
                            else
                            {
                                isAvailable = false;
                            }
                        }

                    }
                }
            }
            return isAvailable;
        }
        catch (Exception)
        {
            return false;
        }
    }



    public bool IsServerConnectionForLocal()
    {
        try
        {
            var isAvailable = false;
            var settings = new TblSettings();
            using (var dataService = _context.CreateDataService())
            {
                settings = dataService.GetSettingsLocal();
            }
            if (settings != null)
            {
                string connectionString = settings.SqlServerConnString;

                // Extract the IP address and port from the connection string
                int start = connectionString.IndexOf('=') + 1;
                int end = connectionString.IndexOf(';');
                string dataSource = connectionString.Substring(start, end - start);

                // Split the IP address and port
                string[] parts = dataSource.Split(',');

                // parts[0] contains the IP address and parts[1] contains the port
                string ipAddress = parts[0];
                //string port = parts[1];
                Ping ping = new Ping();
                PingReply reply = ping.Send(ipAddress, 1000);
                if (reply.Status == IPStatus.Success)
                {
                    isAvailable = true;
                }
                if (isAvailable == false)
                {
                    if (settings != null)
                    {
                        settings.SqlServerConnString = settings.SqlServerConnString.Replace("30", "2");
                        using (var connection = new SqlConnection(settings.SqlServerConnString))
                        {
                            try
                            {

                                connection.Open();
                                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                                {
                                    isAvailable = true;
                                }
                                connection.Close();
                            }
                            catch
                            {
                                isAvailable = false;
                            }


                        }
                    }
                }
            }
            return isAvailable;
        }
        catch (Exception)
        {
            return false;
        }
    }

}
