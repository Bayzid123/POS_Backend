using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Models;
using POS.Services;
using POS.Services.HttpsClient;

namespace POS.ViewModels;
public class LoginViewModel : ObservableRecipient
{
    private readonly IRestService _IRestService;
    private readonly IMasterDataRestService _ImasterDataRestService;
    private readonly IMasterDataSQLRestService _masterDataSQLRestService;
    private readonly IGetService _getService;
    public LoginViewModel(IRestService IRestService, IMasterDataRestService ImasterDataRestService, IMasterDataSQLRestService masterDataSQLRestService, IGetService getService)
    {
        _IRestService = IRestService;
        _ImasterDataRestService = ImasterDataRestService;
        _getService = getService;
        _masterDataSQLRestService= masterDataSQLRestService;
    }
    public async Task<MessageHelper> Login(LoginModel login)
    {
        try
        {
            var setting = await _getService.GetSettings();
            var msg = await _IRestService.UserLogIn(login);
            //var items = await _ImasterDataRestService.GetAllItems(setting.intAccountId, setting.intBranchId);
            //var warehouses = await _ImasterDataRestService.GetWarehouseForPOS(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
            //var ItemSellingPrice = await _ImasterDataRestService.GetUpdatedItemSellingcPriceForPOS(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
            //var AllPartners = await _ImasterDataRestService.GetAllPartner();
            return msg;
        }
        catch (Exception)
        {
            throw;
            //throw new Exception("LogIn Failed");
        }
    }
    public async Task<bool> CheckUserPermission(long accountId, long branchId, long officeId, long warehouseId, long counterId, long userId)
    {
        try
        {
            var dt = await _IRestService.CheckUserPermission(accountId, branchId, officeId, warehouseId, counterId, userId);
            return dt;
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<bool> AdminLogIn(LoginModel login)
    {
        try
        {
            var isUser = await _IRestService.AdminUserLogin(login);
            return isUser;
        }
        catch (Exception)
        {

            throw;
        }

    }
    public async Task<TblUser> GetUser(string username, string password)
    {
        var user = await _IRestService.GetUser(username, password);
        return user;

    }
    public async Task<CounterSession> CheckCounterSession()
    {
        return await _getService.GetCounterSession();
    }

    public async Task<MessageHelper> ItemSellingInvoiceSendToSQLDatabaseFromLogIn()
    {
        var setting = await _getService.GetSettings();

        if (setting != null && setting.OflineConnection)
        {
            AppSettings.SQLServerConnectionString = setting?.SqlServerConnString;
            var salesDeliveryInvoice = await _masterDataSQLRestService.GetSalesDeliveryInformationfromSQLite(setting.intAccountId, setting.intBranchId, 5000);

            //here isSync status will be updated as 2...........
            foreach (var singleRow in salesDeliveryInvoice)
            {
                //singleRow.pOSSalesDeliveryHeader.UserId = AppSettings.UserId;
                singleRow.pOSSalesDeliveryHeader.IsSync = 2;
                await _ImasterDataRestService.SaveItemIntoSalesDelivery(singleRow);
            }
            //here isSync status will be updated as 2...........

            if (salesDeliveryInvoice != null && salesDeliveryInvoice.Count() > 0)
            {
                var saveResponse = await _masterDataSQLRestService.CreateSalesDeliveryInformationIntoSQLServer(salesDeliveryInvoice, setting);
            }
        }
        return null;
    }


}
