using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Identity.Client;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using POS.Core.Contracts.Services;
using POS.Core.Helpers;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.CounterSession;
using POS.Core.ViewModels.MainViewModelDTO;
using POS.Models;
using POS.Services;
using POS.Services.HttpsClient;

namespace POS.ViewModels;

public class MainViewModel : ObservableRecipient
{
    private readonly IRestService _IRestService;
    private readonly ISampleDataService _sampleDataService;
    private readonly IMasterDataRestService _masterDataRestService;
    private readonly IGetService _iGetService;
    private readonly IInvoicePrint _IInvoicePrint;
    private readonly ICreateService _createService;
    private readonly IConnectionCheck _connection;
    public bool isRecollRecallInvoice
    {
        get; set;
    }

    private readonly IMasterDataSQLRestService _masterDataSQLRestService;

    public List<SampleOrderDetail> Source { get; } = new List<SampleOrderDetail>();
    public List<SampleCustomer> smpCustomer { get; } = new List<SampleCustomer>();

    public MyCollection CollectionObj { set; get; } = new();
    public List<PaymentWalletDTO> PaymentWalletDTOList { set; get; } = new List<PaymentWalletDTO>();

    /// newly added section....
    /// </summary>
    //public List<PaymentModeInformation> itemPaymentModeList { set; get; } = new List<PaymentModeInformation>();
    public ObservableCollection<PaymentModeInformation> itemPaymentModeList { set; get; } = new ObservableCollection<PaymentModeInformation>();

    //newly added section.......

    public ObservableCollection<RecallInvoiceHomeObjDTO> recallInvoiceHomeObjDTOs { set; get; } = new ObservableCollection<RecallInvoiceHomeObjDTO>();
    public ObservableCollection<MainViewModelItemDTO> ExchangeItemInformationList { set; get; } = new ObservableCollection<MainViewModelItemDTO>();
    public ObservableCollection<MainViewModelItemDTO> MultipleBarCodeItemList { set; get; } = new ObservableCollection<MainViewModelItemDTO>();
    public MainViewModelItemDTO DeleteItemList { set; get; } = new MainViewModelItemDTO();

    public MainViewModelItemDTO selectedItem
    {
        set; get;
    }
    public ObservableCollection<MainViewModelItemDTO> homePageItemList { get; set; } = new ObservableCollection<MainViewModelItemDTO>();
    public long SelectedCustomsr
    {
        get; set;
    }
    public SampleCustomer SelectedCustomerObj { set; get; } = new SampleCustomer();
    public ObservableCollection<OtherDiscountDTO> OtherDiscountList { set; get; } = new ObservableCollection<OtherDiscountDTO>();

    public MainViewModel(ISampleDataService sampleDataService, IMasterDataRestService masterDataRestService, IGetService iGetService, ICreateService createService, IInvoicePrint iInvoicePrint, IMasterDataSQLRestService masterDataSQLRestService, IRestService IRestService, IConnectionCheck connection)
    {
        _sampleDataService = sampleDataService;
        _masterDataRestService = masterDataRestService;
        _iGetService = iGetService;
        _IInvoicePrint = iInvoicePrint;
        _createService = createService;
        _masterDataSQLRestService = masterDataSQLRestService;
        _IRestService = IRestService;
        _connection = connection;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();
        smpCustomer.Clear();
        // TODO: Replace with real data.
        var data = await _sampleDataService.GetGridItemAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
        var cus = await _sampleDataService.GetSampleCustomerAsync();

        foreach (var item in cus)
        {
            smpCustomer.Add(item);
        }
    }


    public void OnNavigatedFrom()
    {
    }

    public async Task<Partner> GetPartnerById(string PartnerCode)
    {
        var CustomerInformation = await _masterDataRestService.GetCustomerByCustomerId(PartnerCode);
        return CustomerInformation;
    }

    public async Task<Item> GetItemByBarCode(string ItemBarCode)
    {
        var Item = await _masterDataRestService.GetItemByBarCode(ItemBarCode);
        return Item;
    }

    public async Task<Partner> GetCustomerbyCustomerId(long partnerId)
    {
        var Item = await _masterDataRestService.GetPartner(partnerId);
        return Item;
    }

    public async Task<List<Item>> GetItemListByItemName(string ItemName)
    {
        var Items = await _masterDataRestService.GetItemListByItemName(ItemName);
        return Items;
    }

    public async Task<List<tblPointOfferRow>> GetPointsOfferRowsByItemIds(List<long> itemIds)
    {
        var Items = await _masterDataRestService.GetPointsOfferRowsByItemIds(itemIds);
        return Items;
    }

    public async Task<List<Item>> GetItemListByBarCode(string ItemBarCode)
    {
        var Item = await _masterDataRestService.GetItemListByBarCode(ItemBarCode);
        return Item;
    }


    public async Task<Item> GetStockQtyCheckItemByItemID(long Id, decimal salesRate)
    {
        var Item = await _masterDataRestService.GetStockQtyCheckItemByItemID(Id, salesRate);
        return Item;
    }


    public async Task<List<Item>> GetMultipleSalesPrizeItemListByBarCode(string ItemBarCode)
    {
        var Item = await _masterDataRestService.GetMultipleSalesPrizeItemListByBarCode(ItemBarCode);
        return Item;
    }



    public async Task<List<Item>> GetItemByItemIDs(List<long> ids)
    {
        var Item = await _masterDataRestService.GetItemByItemIDs(ids);
        return Item;
    }



    public async Task<CreateSalesDeliveryDTO> SaveItemIntoSalesDeliveryLines(CreateSalesDeliveryDTO objCreate)
    {
        var Item = await _masterDataRestService.SaveItemIntoSalesDeliveryLines(objCreate);
        return Item;
    }


    public async Task<List<PaymentWalletDTO>> GetPaymentWalletList()
    {
        var response = await _masterDataRestService.GetPaymentWalletList();
        return response;
    }


    public async Task<List<OtherDiscountDTO>> GetOtherDiscountList()
    {
        var response = await _masterDataRestService.GetOtherDiscountList();
        return response;
    }


    public async Task<TblSettings> GetSettings()
    {
        var response = await _iGetService.GetSettings();
        return response;
    }

    public async Task<MainViewRecallInvoiceDTO> RecallInvoiceInformation(long userId, long customerId)
    {
        var response = await _masterDataRestService.RecallInvoiceInformation(userId, customerId);
        return response;
    }

    public async Task<MainViewRecallInvoiceDTO> RecallInvoice(long userId, long SalesDeliveryId)
    {
        var response = await _masterDataRestService.RecallInvoice(userId, SalesDeliveryId);
        return response;
    }

    public async Task<MainViewRecallInvoiceDTO> GetSalesDeliveryInformationFromSQLServer(string InvoiceCode)
    {
        var response = await _masterDataSQLRestService.GetSalesDeliveryInformationFromSQLServer(InvoiceCode);
        return response;
    }

    public async Task<List<RecallInvoiceHomeObjDTO>> InvoiceHomePageLanding(long userId)
    {
        var response = await _masterDataRestService.InvoiceHomePageLanding(userId);
        return response;
    }
    public async Task<bool> InvoiceCheck(long userId)
    {
        var response = await _masterDataRestService.InvoiceHomePageLanding(userId);
        return response.Count() > 0 ? true : false;
    }
    public async Task<string> InvoiceCodeGenerate(long CounterId, string CounterCode)
    {
        var setting = await _iGetService.GetSettings();
        //Thread.Sleep(900);
        var InvoiceNumber = await _masterDataRestService.InvoiceCodeGenerate(setting.intCounterId, setting.StrCounterCode);
        return InvoiceNumber;
    }

    public async Task LoadOffice()
    {
        var setting = await _iGetService.GetSettings();
        AppSettings.Message = setting.Message;
        //wait _IRestService.GetOfficeDetails(setting.intOfficeId.ToString());
    }
    public async Task<POSSalesDeliveryHeader> GetPOSSalesDeliveryHeader(string InvoiceCode)
    {
        var InvoiceNumber = await _masterDataRestService.GetPOSSalesDeliveryHeader(InvoiceCode);
        return InvoiceNumber;
    }

    public async Task<TblSettings> GetSettingInformation()
    {
        var setting = await _iGetService.GetSettings();
        return setting;
    }

    public async Task<TblUser> GetUserInformation(long userId)
    {
        var UserInformation = await _iGetService.GetUser(userId);
        return UserInformation;
    }

    public async Task<PaymentWalletDTO> GetWalletInformationbyId(long walletId)
    {
        var walletInformation = await _masterDataRestService.GetPaymentWalletbyId(walletId);
        return walletInformation;
    }

    public async Task<List<PaymentWalletDTO>> GetWalletInformationbyIds(List<long> walletIds)
    {
        var walletInformation = await _masterDataRestService.GetPaymentWalletbyIds(walletIds);
        return walletInformation;
    }
    public async Task<Item> GetItemByItemID(long Id)
    {
        var item = await _masterDataRestService.GetItemByItemID(Id);
        return item;
    }

    public void PrintInvoice(List<InvoiceModelDTO> obj)
    {
        _IInvoicePrint.GenerateInvoice(obj);
    }

    public void OnlyPrint()
    {
        _IInvoicePrint.GenerateOnlyInvoice();
    }
    public async Task<MessageHelper> CloseCounterSession(CreateCounterSeason create)
    {
        MessageHelper msg = new MessageHelper();
        var counterSession = await _iGetService.GetCounterSession();
        if (counterSession != null)
        {
            create.Session.CounterSessionId = counterSession.CounterSessionId;

            msg = await _createService.CreateCountersSession(create);
        }
        else
        {
            msg.Message = "Session Not Found";
            msg.StatusCode = 200;

        }
        return msg;
    }
    public async Task<List<GetCounterSessionDetailsDTO>> GetCounterSessionDetails()
    {
        try
        {
            List<GetCounterSessionDetailsDTO> dt = new List<GetCounterSessionDetailsDTO>();
            var session = await _iGetService.GetCounterSession();
            if (session != null)
            {
                dt = await _iGetService.GetCounterSessionDetails(session.CounterSessionId);
            }
            return dt;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }
    public async Task<bool> Authorization(LoginModel login)
    {
        var auth = await _IRestService.AuthorizeUser(login);
        return auth;
    }



    public async Task<MessageHelper> ItemSellingPriceRowChange()
    {
        var setting = await _iGetService.GetSettings();

        if (setting != null && setting.OflineConnection)
        {
            AppSettings.SQLServerConnectionString = setting?.SqlServerConnString;
            var walletInformation = await _masterDataSQLRestService.ItemSellingPriceRowChange(setting.intAccountId, setting.intBranchId, setting.intWarehouseId);
            return walletInformation;
        }
        return new MessageHelper { StatusCode = 200 };
    }

    public async Task<MessageHelper> ItemRowChange()
    {
        var setting = await _iGetService.GetSettings();

        if (setting != null && setting.OflineConnection)
        {
            AppSettings.SQLServerConnectionString = setting?.SqlServerConnString;
            var walletInformation = await _masterDataSQLRestService.ItemRowChange(setting.intAccountId, setting.intBranchId);
            return walletInformation;
        }
        return new MessageHelper { StatusCode = 200 };
    }

    public async Task<MessageHelper> PartnerRowChange()
    {
        var setting = await _iGetService.GetSettings();
        AppSettings.SQLServerConnectionString = setting?.SqlServerConnString;
        if (setting != null && setting.OflineConnection)
        {
            var walletInformation = await _masterDataSQLRestService.PartnerRowChange(setting.intAccountId, setting.intBranchId);
            return walletInformation;
        }
        return new MessageHelper { StatusCode = 200 };
    }

    public async Task<MessageHelper> ItemSellingInvoiceSendToSQLDatabase()
    {
        var setting = await _iGetService.GetSettings();

        if (setting != null && setting.OflineConnection)
        {
            AppSettings.SQLServerConnectionString = setting?.SqlServerConnString;
            var salesDeliveryInvoice = await _masterDataSQLRestService.GetSalesDeliveryInformationfromSQLite(setting.intAccountId, setting.intBranchId, 10);

            //here isSync status will be updated as 2...........
            foreach (var singleRow in salesDeliveryInvoice)
            {
                //singleRow.pOSSalesDeliveryHeader.UserId = AppSettings.UserId;
                singleRow.pOSSalesDeliveryHeader.IsSync = 2;
                await _masterDataRestService.SaveItemIntoSalesDelivery(singleRow);
            }
            //here isSync status will be updated as 2...........

            if (salesDeliveryInvoice != null && salesDeliveryInvoice.Count() > 0)
            {
                var saveResponse = await _masterDataSQLRestService.CreateSalesDeliveryInformationIntoSQLServer(salesDeliveryInvoice, setting);
            }
        }
        return null;
    }


    public async Task PromotionDiscountFromSQLServerToSQLiteDatabase()
    {
        //try
        //{
        var setting = await _iGetService.GetSettings();

        if (setting != null && setting.OflineConnection)
        {
            AppSettings.SQLServerConnectionString = setting?.SqlServerConnString;
            await _masterDataSQLRestService.GetSpecialDiscountSyncSQLServer(setting.intAccountId, setting.intBranchId);
            await _masterDataSQLRestService.GetPromotionRowSyncSQLServer(setting.intAccountId, setting.intBranchId);
          

        }

        //}
        //catch(Exception ex)
        //{
        //    throw ex;
        //}

    }

    public async Task<MessageHelper> FetchWalletInformationFromSQLServerToSQLiteDatabase()
    {
        var setting = await _iGetService.GetSettings();

        if (setting != null)
        {
            if (setting.OflineConnection)
            {
                var wallletInformation = await _masterDataSQLRestService.GetWalletInformationfromSQLServer(setting.intAccountId, setting.intBranchId);
                return wallletInformation;
            }
            else
            {
                return new MessageHelper() { StatusCode = 500, Message = "Syncronize with the Database" };
            }
        }
        else
        {
            return new MessageHelper() { StatusCode = 500, Message = "Setting Information not found" };
        }
    }
    public async Task<List<Partner>> CreatePartner(List<Partner> create)
    {
        try
        {
            List<Partner> partner = new List<Partner>();

            partner = await _masterDataSQLRestService.SaveSQLPartner(create);

            return partner;

        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<Partner> GetPartner()
    {
        try
        {
            var partner = await _masterDataRestService.GetPartner();
            return partner;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Partner> GetPartner(long PartnerId)
    {
        try
        {
            var partner = await _masterDataRestService.GetPartner(PartnerId);
            return partner;
        }
        catch (Exception)
        {
            throw;
        }
    }



    public async Task<MessageHelper> FetchPromotionRowsFromSQLServerToSQLiteDatabase()
    {
        var setting = await _iGetService.GetSettings();

        if (setting != null)
        {
            if (setting.OflineConnection)
            {
                var wallletInformation = await _masterDataSQLRestService.GetPromotionRowfromSQLServer(setting.intAccountId, setting.intBranchId);
                return wallletInformation;
            }
            else
            {
                return new MessageHelper() { StatusCode = 500, Message = "Syncronize with the Database" };
            }
        }
        else
        {
            return new MessageHelper() { StatusCode = 500, Message = "Setting Information not found" };
        }
    }




    public async Task<MessageHelper> FetchSpecialDiscountFromSQLServerToSQLiteDatabase()
    {
        var setting = await _iGetService.GetSettings();

        if (setting != null)
        {
            if (setting.OflineConnection)
            {
                var specialPromotionResponse = await _masterDataSQLRestService.GetSpecialDiscountfromSQLServer(setting.intAccountId, setting.intBranchId);
                return specialPromotionResponse;
            }
            else
            {
                return new MessageHelper() { StatusCode = 500, Message = "Syncronize with the Database" };
            }
        }
        else
        {
            return new MessageHelper() { StatusCode = 500, Message = "Setting Information not found" };
        }
    }

    public async Task<MessageHelper> DeleteSessionData()
    {
        try
        {
            var data = await _masterDataRestService.DeleteSessionData();
            return data;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<MessageHelper> CreateSqlServerSession()
    {
        try
        {
            var data = await _masterDataSQLRestService.CreateSessionDetails();
            return data;
        }
        catch (Exception)
        {

            throw;
        }
    }
    public bool IsServerConnectionForLocal()
    {
        return _connection.IsServerConnectionForLocal();
    }
    public async Task<bool> DataProcessing()
    {
        try
        {
            var setting = await _iGetService.GetSettings();
            if (_connection.IsServerConnectionForLocal() == true)
            {
                if (setting.OflineConnection)
                {
                    var items = await _masterDataSQLRestService.GetSQLAllItems(setting.intAccountId, setting.intBranchId);
                    var warehouses = await _masterDataSQLRestService.GetSQLAllWarehouses(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
                    var ItemSellingPrice = await _masterDataSQLRestService.GetSQLAllItemSellingPrices(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
                    var AllPartners = await _masterDataSQLRestService.GetSQLAllPartner(setting.intAccountId, setting.intBranchId);
                    var AllItemStock = await _masterDataSQLRestService.GetSQLAllItemBalanceWarehouse(setting.intAccountId, setting.intBranchId);
                    var responsefrom = await _masterDataSQLRestService.GetWalletInformationfromSQLServer(setting.intAccountId, setting.intBranchId);
                    var Promotion = await _masterDataSQLRestService.GetPromotionRowfromSQLServer(setting.intAccountId, setting.intBranchId);
                    var DiscountInfo = await _masterDataSQLRestService.GetSpecialDiscountfromSQLServer(setting.intAccountId, setting.intBranchId);
                    await _masterDataSQLRestService.GetUserFromSql(setting.intAccountId);
                }
                else
                {
                    var items = await _masterDataRestService.GetAllItems(setting.intAccountId, setting.intBranchId);
                    var warehouses = await _masterDataRestService.GetWarehouseForPOS(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
                    var ItemSellingPrice = await _masterDataRestService.GetUpdatedItemSellingcPriceForPOS(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
                    var AllPartners = await _masterDataRestService.GetAllPartner(setting.intAccountId, setting.intBranchId);
                }
            }
            return true;

        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

    public async Task<bool> UserProcess()
    {
        try
        {
            var setting = await _iGetService.GetSettings();
            if ((await _connection.IsServerConnectionAvailable()) == true)
            {
                await _masterDataSQLRestService.GetUserFromSql(setting.intAccountId);

            }
            return true;

        }
        catch
        {
            return true;

        }
    }

    public async Task DataLog(string logMessage, string entityData, string logType)
    {
        try
        {
            DateTime currentDate = DateTime.Now.BD();
            var setting = await _iGetService.GetSettings();
            var user = await _iGetService.GetUser(AppSettings.UserId);
            var log = new tblDataLog()
            {
                AccountId = setting.intAccountId,
                BranchId = setting.intBranchId,
                OfficeId = setting.intOfficeId,
                CounterId = setting.intCounterId,
                ServerUserId = user.ServerUserID,
                strLogMessage = logMessage,
                strEntityData = entityData,
                LogType = logType,
                LastActionDateTime = currentDate,
                ServerDateTime = currentDate,
                IsSync = 0,
            };
            await _createService.CreatedataLog(log);
        }
        catch (Exception)
        {

           // throw;
        }
    }
    public async Task DeleteDataLog()
    {
        try
        {
            await _masterDataRestService.DeleteDataLog();
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task SqlDataLog(List<SQLtblDataLog> log)
    {
        await _masterDataSQLRestService.SqlDataLogUpdate(log);
    }
    public async Task<List<tblDataLog>> GetDataLogs()
    {
        var dt = await _iGetService.GetDataLog();
        return dt;
    }
    public async Task<MessageHelper> DeleteRecallInvoice(long SalesOrderId, string SalesOrderCode)
    {
        var dt = await _createService.DeleteRecallInvoice(SalesOrderId, SalesOrderCode);
        return dt;
    }
    public async Task<TblUser> GetUser(string User, string Password)
    {
        var dt = await _iGetService.GetUserAutorization(User, Password);
        return dt;
    }
    public async Task<bool> ConnectionCheck()
    {
        var data = await _connection.IsServerConnectionAvailable();
        return data;
    }
}
