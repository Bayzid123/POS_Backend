using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using POS.Services.HttpsClient;
using POS.Services;
using POS.Core.Models;
using CommunityToolkit.WinUI.UI.Automation.Peers;
using POS.Core.ViewModels;
using POS.Core.ViewModels.MainViewModelDTO;
using System.Collections.ObjectModel;

namespace POS.ViewModels;
public class InvoiceViewModel : ObservableRecipient
{
    public POSSalesDeliveryHeader head { get; set; } = new POSSalesDeliveryHeader();
    public string CashAmountHead
    {
        get; set;
    }
    public ObservableCollection<POSSalesDeliveryLineDTO> rows { get; set; } = new ObservableCollection<POSSalesDeliveryLineDTO>();
    public ObservableCollection<POSSalesPaymentDTO> payment { get; set; } = new ObservableCollection<POSSalesPaymentDTO>();
    public List<PaymentWalletDTO> PaymentWalletDTOList { set; get; } = new List<PaymentWalletDTO>();
    public ObservableCollection<PaymentModeInformation> itemPaymentModeList { set; get; } = new ObservableCollection<PaymentModeInformation>();



    private readonly IGetService _iGetService;
    private readonly IMasterDataRestService _masterDataRestService;
    private readonly IMasterDataSQLRestService _masterDataSQLRestService;
    public InvoiceViewModel(IGetService iGetService, IMasterDataRestService masterDataRestService, IMasterDataSQLRestService masterDataSQLRestService)
    {
        _iGetService = iGetService;
        _masterDataRestService = masterDataRestService;
        _masterDataSQLRestService = masterDataSQLRestService;
    }
    public async Task<int> GetPosDeliveryHeader(object paramater,bool isLive)
    {
        if(isLive == false)
        {
            POSSalesDeliveryHeader dt = await _iGetService.GetPosDeliveryHeader(AppSettings.salesOrderId);
            head = dt;
            CashAmountHead = dt.CashPayment.ToString();
        }
        else
        {
            POSSalesDeliveryHeader dt = await _iGetService.GetPosLiveDeliveryHeader(AppSettings.salesOrderId);
            head = dt;
            CashAmountHead = Math.Round(dt.CashPayment.GetValueOrDefault(),2).ToString();
        }
        return 1;
    }
    public async Task<int> GetPosDeliveryLineItem(long salesOrderId, bool isLive)
    {
        if(isLive == false)
        {
            List<POSSalesDeliveryLineDTO> row = await _iGetService.GetPosDeliveryLineItem(salesOrderId);
            rows = new ObservableCollection<POSSalesDeliveryLineDTO>(row);
        }
        else
        {
            List<POSSalesDeliveryLineDTO> row = await _iGetService.GetPosLiveDeliveryLineItem(salesOrderId);
            rows = new ObservableCollection<POSSalesDeliveryLineDTO>(row);
        }
        return 1;
    }
    public async Task<int> GetSalesPayment(long salesOrderId , bool isLive)
    {
        if(isLive == false)
        {
            var dt = await _iGetService.GetSalesPayment(salesOrderId);
            foreach (var item in dt)
            {
                payment.Add(item);
            }
        }
        else
        {
            var dt = await _iGetService.GetLiveSalesPayment(salesOrderId);
            foreach (var item in dt)
            {
                payment.Add(item);
            }
        }
        return 1;

    }
    public async Task<List<PaymentWalletDTO>> GetPaymentWalletList()
    {
        var response = await _masterDataRestService.GetPaymentWalletList();
        return response;
    }
    public async Task<MessageHelper> EditPosPayment(EditPosSales edit)
    {
        var response = await _masterDataRestService.EditPosPayment(edit);
        return response;
    }
    public async Task<MessageHelper> EditPosPaymentSQL(EditPosSales edit)
    {
        var response = await _masterDataSQLRestService.SQLUpdatePosSalesPayment(edit);
        return response;
    }
    public async Task<Partner> GetPartnerById(string PartnerCode)
    {
        var CustomerInformation = await _masterDataRestService.GetCustomerByCustomerId(PartnerCode);
        return CustomerInformation;
    }

    public async Task<Partner> GetPartnerData(long PartnerId)
    {
        var CustomerInformation = await _masterDataRestService.GetPartnerById(PartnerId);
        return CustomerInformation;
    }
}
