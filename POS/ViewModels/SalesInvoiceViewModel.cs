using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.ReportingServices.Interfaces;
using POS.Contracts.Services;
using POS.Core.IService;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.MainViewModelDTO;
using POS.Models;
using POS.Services;
using POS.Services.HttpsClient;

namespace POS.ViewModels;
public class SalesInvoiceViewModel : ObservableRecipient
{
    public ObservableCollection<SalesInvoiceDTO> invoice { get; set; } = new ObservableCollection<SalesInvoiceDTO>();

    public bool isSyncronize { set; get; } = false;
    public bool isSyncButtonVisible {set; get; } = true;  //true means invisible..
    public bool isAllSelectVisible { set; get; } = true;   //true means invisible..
    public bool isInvoiceOnlineSearch { set; get; } = true; //true means Invoice Search from Online..
    public List<SalesInvoiceDTO> SalesInvoiceForSyncList { set; get; } = new List<SalesInvoiceDTO>();



    private readonly IGetService _iGetService;
    private readonly IMasterDataRestService _masterDataRestService;
    private readonly IInvoicePrint _IInvoicePrint;
    private readonly IRestService _irestService;
    private readonly IMasterDataSQLRestService _masterDataSQLRestService;
    private readonly IConnectionCheck _connection;
    public SalesInvoiceViewModel(IRestService irestService,IGetService iGetService, IMasterDataRestService masterDataRestService,
        IInvoicePrint iInvoicePrint, IMasterDataSQLRestService masterDataSQLRestService, IConnectionCheck connection)
    {
        _iGetService = iGetService;
        _masterDataRestService = masterDataRestService;
        _IInvoicePrint = iInvoicePrint;
        _irestService = irestService;
        _masterDataSQLRestService = masterDataSQLRestService;
        _connection = connection;
    }

    public async Task<bool> ConnectionCheck()
    {
        var data=await _connection.IsServerConnectionAvailable();
        return data;
    }
    public async void GetSalesInvoice(object parameter,bool isSync)
    {
        try
        {
            invoice.Clear();
            long userid = AppSettings.UserId;
            var user = await _iGetService.GetUser(userid);
            var dt = await _iGetService.GetSalesInvoice(user.ServerUserID, isSync);
            if (dt.Count > 0)
            {
                var daa=dt.Select(x => new SalesInvoiceDTO
                {
                    CashAmount = x.CashAmount,
                    InvoiceDate = x.InvoiceDate,
                    Quantity = x.Quantity,
                    SalesAmount = x.SalesAmount,
                    SalesInvoice = x.SalesInvoice,
                    Sl = x.Sl,
                    isSynchonized = true,
                    isSelected = false,
                    SalesOrderId = x.SalesOrderId
                }).ToList();
                foreach (var item in daa)
                {
                    invoice.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }




    public async void GetSalesInvoiceLiveServer(object parameter, bool isSync)
    {
        try
        {
            invoice.Clear();
            
            if (AppSettings.IsOnline == true && await _connection.IsServerConnectionAvailable() == true)
            {
                var SalesInvoice = parameter.ToString();

              
                long userid = AppSettings.UserId;
                var user = await _iGetService.GetUser(userid);
                var dt = await _iGetService.GetSalesInvoiceLiveServer(user.ServerUserID, isSync, SalesInvoice);
                if (dt.Count > 0)
                {
                    var daa = dt.Select(x => new SalesInvoiceDTO
                    {
                        CashAmount = x.CashAmount,
                        InvoiceDate = x.InvoiceDate,
                        Quantity = x.Quantity,
                        SalesAmount = x.SalesAmount,
                        SalesInvoice = x.SalesInvoice,
                        Sl = x.Sl,
                        isSynchonized = true,
                        isSelected = false,
                        SalesOrderId = x.SalesOrderId
                    }).ToList();
                    foreach (var item in daa)
                    {
                        invoice.Add(item);
                    }
                }
                else
                {
                    invoice = new ObservableCollection<SalesInvoiceDTO>();
                }
            }
          
     
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }






    public async void GetUnSyncronizedSalesInvoice(object parameter,bool isSync)
    {
        try
        {
            invoice.Clear();
            long userid = AppSettings.UserId;
            var user = await _iGetService.GetUser(userid);
            var dt = await _iGetService.GetSalesInvoice(user.ServerUserID, isSync);
            if (dt.Count > 0)
            {
                var daa = dt.Select(x => new SalesInvoiceDTO
                {
                    CashAmount = x.CashAmount,
                    InvoiceDate = x.InvoiceDate,
                    Quantity = x.Quantity,
                    SalesAmount = x.SalesAmount,
                    SalesInvoice = x.SalesInvoice,
                    Sl = x.Sl,
                    isSynchonized = false,
                    isSelected = false,
                    SalesOrderId = x.SalesOrderId,
                }).ToList();
                foreach (var item in daa)
                {
                    invoice.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }


    public async Task<List<CreateSalesDeliveryDTO>> SalesInformationUsingIDs(List<long> SalesOrderIds)
    {
        var dt = await _masterDataRestService.SalesInformationUsingIDs(SalesOrderIds);
        return dt;
    }

    public async Task<MessageHelper> CreateSalesDeliveryInformationIntoSQLServer(List<CreateSalesDeliveryDTO> salesDeliveryInvoice, TblSettings setting)
    {
        var saveResponse = await _masterDataSQLRestService.CreateSalesDeliveryInformationIntoSQLServer(salesDeliveryInvoice, setting);
        return saveResponse;
    }

 

    public async Task<POSSalesDeliveryHeader> GetPosDeliveryHeader(string invoice, bool isLive)
    {
        POSSalesDeliveryHeader dt = new POSSalesDeliveryHeader();
        if (isLive == false)
        {
             dt = await _iGetService.GetPosDeliveryHeader(AppSettings.salesOrderId);
 
        }
        else
        {
             dt = await _iGetService.GetPosLiveDeliveryHeader(AppSettings.salesOrderId);
 
        }
        return dt;
    }
    public async Task<List<POSSalesDeliveryLine>> GetPosDeliveryLine(long salesOrderId, bool isLive)
    {
        List<POSSalesDeliveryLine> row = new List<POSSalesDeliveryLine>();
        if (isLive == false)
        {
            row = await _iGetService.GetPosDeliveryLine(salesOrderId);

        }
        else
        {
            row = await _iGetService.GetPosLiveDeliveryLine(salesOrderId);

        }
        return row;
    }
    public async Task<List<POSSalesPaymentDTO>> GetSalesPayment(long salesOrderId, bool isLive)
    {
        List<POSSalesPaymentDTO> dt = new List<POSSalesPaymentDTO>();
        if (isLive == false)
        {
             dt = await _iGetService.GetSalesPayment(salesOrderId);
       
        }
        else
        {
             dt = await _iGetService.GetLiveSalesPayment(salesOrderId);
         
        }
        return dt;
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
    public async Task<Partner> GetPartnerData(long PartnerId)
    {
        var CustomerInformation = await _masterDataRestService.GetPartnerById(PartnerId);
        return CustomerInformation;
    }
    public async Task<Item> GetItemByItemID(long Id)
    {
        var item = await _masterDataRestService.GetItemByItemID(Id);
        return item;
    }
    public async Task<List<Item>> GetItemByItemIDs(List<long> ids)
    {
        var Item = await _masterDataRestService.GetItemByItemIDs(ids);
        return Item;
    }
    public void PrintInvoice(List<InvoiceModelDTO> obj)
    {
        _IInvoicePrint.GenerateReprintInvoice(obj);
    }
    public async Task<bool> Authorization(LoginModel login)
    {
        var auth = await _irestService.AuthorizeUser(login);
        return auth;
    }
    public async Task<TblUser> GetUser(string User, string Password)
    {
        var dt = await _iGetService.GetUserAutorization(User, Password);
        return dt;
    }
}
