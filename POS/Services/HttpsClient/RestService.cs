using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic.Logging;
using POS.Core;
using POS.Core.Helpers;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Models;
using POS.Services.HttpsClient;
using Windows.Media.Protection.PlayReady;

namespace POS.Services;

public class RestService : IRestService
{
    readonly IConnectionCheck _connectionCheck;
    readonly HttpClient _client;
    readonly JsonSerializerOptions _serializerOptions;
    readonly IHttpsClientHandlerService _httpsClientHandlerService;
    readonly IDataServiceFactory _context;

    public RestService(IHttpsClientHandlerService service, IDataServiceFactory context, IConnectionCheck connectionCheck)
    {
#if DEBUG

        _httpsClientHandlerService = service;
        HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();
        if (handler != null)
            _client = new HttpClient(handler);
        else
            _client = new HttpClient();
#else
        _client = new HttpClient();
#endif
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        _context = context;
        _connectionCheck = connectionCheck;
    }


    public async Task DeleteTodoItemAsync(string id)
    {
        Uri uri = new Uri(string.Format(Constants.RestUrl, id));

        try
        {
            HttpResponseMessage response = await _client.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\t successfully deleted.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }
    public async Task<AllDDl> GetAllDDL(string App_Url)
    {
        try
        {
            AllDDl dt = new AllDDl();
            AppSettings.APPURL = App_Url;
            var isInternet = await _connectionCheck.IsInternetAvailable();
            if (isInternet == true)
            {
                Uri uri = new Uri(string.Format(App_Url + "sme/AccountInfo/PosAllData"));
                //Uri uri = new Uri(string.Format("https://devmgm.ibos.io/sme/AccountInfo/PosAllData"));

                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var st = await response.Content.ReadAsStringAsync();
                    var dec_json = Encryption.DecryptString(st);
                    dt = Newtonsoft.Json.JsonConvert.DeserializeObject<AllDDl>(dec_json);
                }
            }


            return dt;

        }
        catch (Exception ex)
        {
            throw new System.ApplicationException(ex.Message);
        }

    }
    public async Task<AllDDl> GetAllForPreDDL(string App_Url)
    {
        try
        {
            AllDDl dt = new AllDDl();
            AppSettings.APPURL = App_Url;
            var isInternet = _connectionCheck.IsInternetAvailableForPreAsync();
            if (isInternet == true)
            {
                Uri uri = new Uri(string.Format(App_Url + "sme/AccountInfo/PosAllData"));
                //Uri uri = new Uri(string.Format("https://devmgm.ibos.io/sme/AccountInfo/PosAllData"));
                HttpRequestMessage hrm = new HttpRequestMessage();
                hrm.RequestUri = uri;
                hrm.Method = HttpMethod.Get;
                var response = _client.Send(hrm);
                if (response.IsSuccessStatusCode)
                {
                    var st = await response.Content.ReadAsStringAsync();
                    var dec_json = Encryption.DecryptString(st);
                    dt = Newtonsoft.Json.JsonConvert.DeserializeObject<AllDDl>(dec_json);
                }
            }


            return dt;

        }
        catch (Exception ex)
        {
            throw new System.ApplicationException(ex.Message);
        }

    }

    public async Task<GetPartnerDTO> GetAllPartner()
    {
        try
        {
            GetPartnerDTO partner = new GetPartnerDTO();
            var isInternet = await _connectionCheck.IsInternetAvailable();
            if (isInternet == true)
            {

                // encryption

                Uri uri = new Uri(string.Format(AppSettings.APPURL + "sme/Partner/GetPartners"));
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var dt = await response.Content.ReadAsStringAsync();
                    var dec_json = Encryption.DecryptString(dt);
                    partner = Newtonsoft.Json.JsonConvert.DeserializeObject<GetPartnerDTO>(dec_json);
                }
            }
            return partner;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<bool> AuthorizeUser(LoginModel login)
    {
        var isAuthorizeUser = false;
        string json = JsonSerializer.Serialize<LoginModel>(login, _serializerOptions);

        // encryption
        var enc_json = Encryption.EncryptString(json);

        StringContent content = new StringContent(enc_json, Encoding.UTF8, "application/json");


        Uri uri = new Uri(string.Format(AppSettings.APPURL + "identity/LogIn/POSUserLogIn"));
        try
        {

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);
            if (!response.IsSuccessStatusCode)
            {
                var st = await response.Content.ReadAsStringAsync();

                // decrtyption
                var dec_json = Encryption.DecryptString(st);

                var dt = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageHelper>(dec_json);
                if (dt.StatusCode != 200)
                {
                    isAuthorizeUser = false;
                }
            }
            else
            {

                try
                {
                    var st = await response.Content.ReadAsStringAsync();

                    // decrtyption
                    var dec_json = Encryption.DecryptString(st);

                    SmeUserInfoDTO DT = new SmeUserInfoDTO();

                    //DT = JsonSerializer.Deserialize<SmeUserInfoDTO>(st);
                    DT = Newtonsoft.Json.JsonConvert.DeserializeObject<SmeUserInfoDTO>(dec_json);
                    DateTime currentDate = DateTime.Now.BD();



                    if (response.IsSuccessStatusCode && DT != null && DT.IsPosAdmin == true)
                    {
                        isAuthorizeUser = true;
                    }
                }
                catch (Exception)
                {
                    throw new Exception("LogIn Failed");
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
        return isAuthorizeUser;
    }
    public async Task<MessageHelper> UserLogIn(LoginModel login)
    {

        string json = JsonSerializer.Serialize<LoginModel>(login, _serializerOptions);

        // encryption
        var enc_json = Encryption.EncryptString(json);

        StringContent content = new StringContent(enc_json, Encoding.UTF8, "application/json");


        Uri uri = new Uri(string.Format(AppSettings.APPURL + "identity/LogIn/POSUserLogIn"));

        ///................

        MessageHelper msg = new MessageHelper();
        try
        {
            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);
            if (!response.IsSuccessStatusCode)
            {
                var st = await response.Content.ReadAsStringAsync();

                // decrtyption
                var dec_json = Encryption.DecryptString(st);

                msg = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageHelper>(dec_json);
                msg.StatusCode = 500;
                return msg;
            }
            else
            {

                try
                {
                    var st = await response.Content.ReadAsStringAsync();

                    // decrtyption
                    var dec_json = Encryption.DecryptString(st);

                    SmeUserInfoDTO DT = new SmeUserInfoDTO();

                    //DT = JsonSerializer.Deserialize<SmeUserInfoDTO>(st);
                    DT = Newtonsoft.Json.JsonConvert.DeserializeObject<SmeUserInfoDTO>(dec_json);

                    // Authorization check
                    var check = false;

                    var queryString = "accountId=" + login.AccountId + "&branchId=" + login.BranchId + "&officeId=" + login.OfficeId + "&userId=" + DT.UserId + "&warehouseId=" + login.WarehouseId + "&counterId=" + login.CounterId;
                    var enc_json2 = Encryption.EncryptString(queryString);
                    using var client = new HttpClient();
                    client.BaseAddress = new Uri(AppSettings.APPURL);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "sme/POS/CheckCounterAccess?" + enc_json2);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", DT.auth.Token);

                    HttpResponseMessage response2 = client.Send(request);
                    if (!response2.IsSuccessStatusCode)
                    {
                        msg.StatusCode = 500;
                        return msg;
                    }
                    if (response2.IsSuccessStatusCode)
                    {
                        var dt = await response2.Content.ReadAsStringAsync();
                        var dec_json2 = Encryption.DecryptString(dt);
                        check = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(dec_json2);
                        if (check == false)
                        {
                            msg.StatusCode = 404;
                            return msg;
                        }
                        if (check == true)
                        {
                            DateTime currentDate = DateTime.Now.BD();
                            TblUser user = new TblUser()
                            {
                                strUserName = login.UserName,
                                bolIsPOSAdmin = DT.IsPosAdmin,
                                strUserJWTToken = DT.auth.Token,
                                intAccountId = DT.AccountId.Value,
                                intBusinessUnitId = DT.branchList,
                                intOfficeId = DT.officeList,
                                intEmpoyeeId = DT.EmployeeId.Value,
                                strEmployeeName = DT.EmployeeName,
                                strPassword = login.Password,
                                IsAdministration = false,
                                ServerUserID = DT.UserId,
                                LastDateTime = currentDate,
                                IsExchange = DT.IsExchange,
                                IsItemDelete = DT.IsItemDelete,
                                IsSpecialDiscount = DT.IsSpecialDiscount
                            };

                            using (var dataService = _context.CreateDataService())
                            {
                                var getuser = await dataService.GetUserAsync(login.UserName);
                                user.intUserID = getuser == null ? 0 : getuser.intUserID;
                                var us = await dataService.CreateUserAsync(user);
                                AppSettings.UserId = us.intUserID;
                                AppSettings.JWT_Token = us.strUserJWTToken;
                                AppSettings.ServerUserId = us.ServerUserID;
                            }
                            if (response.IsSuccessStatusCode)
                            {
                                msg.Message = "Login Successfully";
                                msg.StatusCode = 200;
                            }
                        }
                    }
                    //if (DT.IsPosAdmin == true)
                    //{
                    //    var setting = new TblSettings();
                    //    using (var service = _context.CreateDataService())
                    //    {
                    //        setting = await service.GetSettings();
                    //    }

                    //    if (setting != null)
                    //    {
                    //        var json_p = "warehouseId=" + setting.intWarehouseId + "&accountId=" + setting.intAccountId + "&branchId=" + setting.intBranchId;
                    //        var enc_json_P = Encryption.EncryptString(json);
                    //        HttpClient client = new HttpClient();
                    //        client.BaseAddress = new Uri(AppSettings.APPURL);
                    //        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "sme/OfferSetup/GetCurrentStockOfWarehouse?" + enc_json);
                    //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.JWT_Token);
                    //        HttpResponseMessage resp = await client.SendAsync(request);
                    //        if (resp.IsSuccessStatusCode)
                    //        {
                    //            var st_p = await resp.Content.ReadAsStringAsync();
                    //            var dec_json_p = Encryption.DecryptString(st_p);
                    //            var items = new List<GetItemWarehouseBalance>();
                    //            items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GetItemWarehouseBalance>>(dec_json_p);
                    //            if (items.Count > 0)
                    //            {
                    //                List<ItemWarehouseBalance> entity = new List<ItemWarehouseBalance>();

                    //                entity.AddRange(items.Select(x => new ItemWarehouseBalance
                    //                {
                    //                    ItemWarehouseBalanceId = x.ItemBalanceWarehouseId,
                    //                    ItemId = x.Value,
                    //                    WarehouseId = x.WarehouseId,
                    //                    CurrentStock = x.Quantity
                    //                }).ToList());
                    //                using var dataService = _context.CreateDataService();
                    //                await dataService.CreateItemWarehouseBalance(entity);

                    //            }
                    //        }
                    //    }
                    //}
                }
                catch (Exception)
                {
                    throw new Exception("LogIn Failed");
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("LogIn Failed");
        }
        return msg;

    }
    public async Task<bool> CheckUserPermission(long accountId, long branchId, long officeId, long warehouseId, long counterId, long userId)
    {
        try
        {
            var check = false;
            var isInternet = await _connectionCheck.IsInternetAvailable();
            if (isInternet == true)
            {
                var queryString = "accountId =" + accountId + "&branchId=" + branchId + "&officeId=" + officeId + "&warehouseId=" + warehouseId + "&counterId=" + counterId + "&userId=" + userId;
                var enc_json = Encryption.EncryptString(queryString);
                using var client = new HttpClient();
                client.BaseAddress = new Uri(AppSettings.APPURL);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "sme/POS/CheckCounterAccess?" + enc_json);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.JWT_Token);

                HttpResponseMessage response = client.Send(request);

                if (response.IsSuccessStatusCode)
                {
                    var dt = await response.Content.ReadAsStringAsync();
                    var dec_json = Encryption.DecryptString(dt);
                    check = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(dec_json);
                }
            }
            return check;
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<OfficeDetails> GetOfficeDetails(string officeId)
    {

        try
        {
            OfficeDetails off = new OfficeDetails();
            var isInternet = await _connectionCheck.IsInternetAvailable();
            if (isInternet == true)
            {
                var queryString = "officeId=" + officeId;
                // encryption
                var enc_json = Encryption.EncryptString(queryString);
                using var client = new HttpClient();
                client.BaseAddress = new Uri(AppSettings.APPURL);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "sme/OfferSetup/GetOfficeDetails?" + enc_json);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.JWT_Token);


                HttpResponseMessage response = client.Send(request);
                if (response.IsSuccessStatusCode)
                {
                    var dt = await response.Content.ReadAsStringAsync();
                    var dec_json = Encryption.DecryptString(dt);
                    off = Newtonsoft.Json.JsonConvert.DeserializeObject<OfficeDetails>(dec_json);
                }
                using (var dataService = _context.CreateDataService())
                {
                    var settings = await dataService.GetSettings();
                    if (off.officeLists.Count > 0)
                    {

                        off.officeLists = off.officeLists.Where(x => x.OfficeId == settings.intOfficeId).ToList();
                        off.counterList = off.counterList.Where(x => x.CounterId == settings.intCounterId).ToList();

                        AppSettings.Message = off.counterList.FirstOrDefault().Message;

                        settings.Message = off.counterList.FirstOrDefault().Message;
                    }
                    await dataService.UpdateSetting(settings);
                }
            }
            return off;
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<bool> AdminUserLogin(LoginModel login)
    {
        bool isUser = false;
        using (var dataService = _context.CreateDataService())
        {
            isUser = await dataService.AdminLogin(login.UserName, login.Password);
        }
        return isUser;

    }
    public async Task<TblUser> GetUser(string username, string password)
    {
        var user = new TblUser();
        using (var dataService = _context.CreateDataService())
        {
            user = await dataService.GetUser(username, password);
        }

        return user;
    }
    public async Task<MessageHelper> CreateSetting(TblSettings settings)
    {
        MessageHelper msg = new MessageHelper();
        using (var dataService = _context.CreateDataService())
        {
            msg = await dataService.CreateSettings(settings);
        }
        return msg;
    }

}
