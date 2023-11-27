using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.CounterSession;
using POS.Core.ViewModels.SalesDeliveryDTO;

namespace POS.Services;
public class GetService : IGetService
{
    readonly IDataServiceFactory _context;

    public GetService(IDataServiceFactory context)
    {
        _context = context;
    }
    public async Task<TblSettings> GetSettings()
    {
        using var dataService = _context.CreateDataService();
        return await dataService.GetSettings();
    }
    public async Task<TblUser> GetUser(long Id)
    {
        using var dataService = _context.CreateDataService();
        var dt = await dataService.GetUserAsync(Id);
        return dt;

    }
    public async Task<CounterSession> GetCounterSession()
    {
        using var dataService = _context.CreateDataService();
        var dt = await dataService.GetPosCounterSession();
        return dt;
    }
    public async Task<List<GetCounterSessionDetailsDTO>> GetCounterSessionDetails(long CounterSessionId)
    {
        using var dataService = _context.CreateDataService();
        var dt = await dataService.GetPosCounterSessionDetails(CounterSessionId);
        return dt;
    }
    public async Task<List<SalesInvoiceDTO>> GetSalesInvoice(long UserId, bool isSyn)
    {
        List<SalesInvoiceDTO> dt = new List<SalesInvoiceDTO>();
        using var dataService = _context.CreateDataService();
        if (dt != null)
        {
            dt = await dataService.GetSalesInvoice(UserId, isSyn);
        }
        return dt;
    }

    public async Task<List<SalesInvoiceDTO>> GetSalesInvoiceLiveServer(long UserId, bool isSyn,string SalesInvoice)
    {
        List<SalesInvoiceDTO> dt = new List<SalesInvoiceDTO>();
        using var dataService = _context.CreateSQLDataService();
        if (dt != null)
        {
            dt = await dataService.GetSalesInvoiceLiveServer(UserId, isSyn, SalesInvoice);
        }
        return dt;
    }

    public async Task<POSSalesDeliveryHeader> GetPosDeliveryHeader(string invoice)
    {
        using var dataService = _context.CreateDataService();
        POSSalesDeliveryHeader dt = await dataService.GetPosDeliveryHeader(invoice);
        return dt;
    }

    public async Task<POSSalesDeliveryHeader> GetPosLiveDeliveryHeader(string invoice)
    {
        using var dataService = _context.CreateSQLDataService();
        SQLServerPOSSalesDeliveryHeader dt = await dataService.GetPosLiveDeliveryHeader(invoice);

        POSSalesDeliveryHeader dtt = new POSSalesDeliveryHeader
        {
            SalesOrderId = dt.SalesOrderId,
            SalesOrderCode = dt.SalesOrderCode,
            CustomerOrderId = dt.CustomerOrderId,
            AccountId = dt.AccountId,
            BranchId = dt.BranchId,
            BranchName = dt.BranchName,
            CustomerId = dt.CustomerId,
            CustomerName = dt.CustomerName,
            Phone = dt.Phone,
            ChallanNo = dt.ChallanNo,
            OrderDate = dt.OrderDate,
            DeliveryDate = dt.DeliveryDate,
            Remarks = dt.Remarks,
            PaymentTypeId = dt.PaymentTypeId,
            PaymentTypeName = dt.PaymentTypeName,
            TotalQuantity = dt.TotalQuantity,
            ItemTotalAmount = dt.ItemTotalAmount,
            NetDiscount = dt.NetDiscount,
            OthersCost = dt.OthersCost,
            NetAmount = dt.NetAmount,
            TotalLineDiscount = dt.TotalLineDiscount,
            HeaderDiscount = dt.HeaderDiscount,
            HeaderDiscountPercentage = dt.HeaderDiscountPercentage,
            ReceiveAmount = dt.ReceiveAmount,
            PendingAmount = dt.PendingAmount,
            ReturnAmount = dt.ReturnAmount,
            InterestRate = dt.InterestRate,
            NetAmountWithInterest = dt.NetAmountWithInterest,
            TotalNoOfInstallment = dt.TotalNoOfInstallment,
            InstallmentStartDate = dt.InstallmentStartDate,
            InstallmentType = dt.InstallmentType,
            AmountPerInstallment = dt.AmountPerInstallment,
            SalesForceId = dt.SalesForceId,
            SalesForceName = dt.SalesForceName,
            ActionById = dt.ActionById,
            ActionByName = dt.ActionByName,
            ActionTime = dt.ActionTime,
            IsPosSales = dt.IsPosSales,
            isActive = dt.isActive,
            SalesOrReturn = dt.SalesOrReturn,
            AdvanceBalanceAdjust = dt.AdvanceBalanceAdjust,
            CustomerNetAmount = dt.CustomerNetAmount,
            IsComplete = dt.IsComplete,
            SalesTypeId = dt.SalesTypeId,
            SalesTypeName = dt.SalesTypeName,
            SalesOrderRefId = dt.SalesOrderRefId,
            Narration = dt.Narration,
            SmsTransactionId = dt.SmsTransactionId,
            AnonymousAddress = dt.AnonymousAddress,
            TotalSd = dt.TotalSd,
            TotalVat = dt.TotalVat,
            IsBillCreated = dt.IsBillCreated,
            DiscoundItemTotalPrice = dt.DiscoundItemTotalPrice,
            OfferItemTotal = dt.OfferItemTotal,
            WalletId = dt.WalletId,
            ComissionPercentage = dt.ComissionPercentage,
            isInclusive = dt.isInclusive,
            OfficeId = dt.OfficeId,
            CustomerPO = dt.CustomerPO,
            BillNo = dt.BillNo,
            ShippingAddressId = dt.ShippingAddressId,
            ShippingAddressName = dt.ShippingAddressName,
            ShippingContactPerson = dt.ShippingContactPerson,
            IsConfirmed = dt.IsConfirmed,
            IsApprove = dt.IsApprove,
            ProjectName = dt.ProjectName,
            FreeTypeId = dt.FreeTypeId,
            FreeTypeName = dt.FreeTypeName,
            JobOrderId = dt.JobOrderId,
            IsSync = 1,
            Draft = false,
            UserId = dt.ActionById,
            CashPayment = dt.CashPayment,
            CounterId = dt.CounterId,
            CounterName = "",
            Points = dt.Points,
            ISExchange = dt.ISExchange,
            HeaderDiscountId = dt.HeaderDiscountId,
        };

        return dtt;
    }

    public async Task<List<POSSalesDeliveryLine>> GetPosDeliveryLine(long salesOrderId)
    {
        using var dataService = _context.CreateDataService();
        List<POSSalesDeliveryLine> row = await dataService.GetPosDeliveryLine(salesOrderId);
        return row;
    }


    public async Task<List<POSSalesDeliveryLineDTO>> GetPosLiveDeliveryLineItem(long salesOrderId)
    {
        using var dataService = _context.CreateSQLDataService();
        List<POSSalesDeliveryLineDTO> row = await dataService.GetPosLiveDeliveryLineItem(salesOrderId);
        return row;
    }
    public async Task<List<POSSalesDeliveryLine>> GetPosLiveDeliveryLine(long salesOrderId)
    {
        using var dataService = _context.CreateSQLDataService();
        List<POSSalesDeliveryLine> row = await dataService.GetPosLiveDeliveryLine(salesOrderId);
        return row;
    }

    public async Task<List<POSSalesDeliveryLineDTO>> GetPosDeliveryLineItem(long salesOrderId)
    {
        using var dataService = _context.CreateDataService();
        List<POSSalesDeliveryLineDTO> row = await dataService.GetPosDeliveryLineItem(salesOrderId);
        return row;
    }
    public async Task<List<POSSalesPaymentDTO>> GetSalesPayment(long salesOrderId)
    {
        using var dataService = _context.CreateDataService();
        List<POSSalesPaymentDTO> dt = await dataService.GetSalesPayment(salesOrderId);
        return dt;
    }

    public async Task<List<POSSalesPaymentDTO>> GetLiveSalesPayment(long salesOrderId)
    {
        using var dataService = _context.CreateSQLDataService();
        List<POSSalesPaymentDTO> dt = await dataService.GetLiveSalesPayment(salesOrderId);
        return dt;
    }

    public async Task<List<tblDataLog>> GetDataLog()
    {
        using ( var dataService = _context.CreateDataService())
        {
            var dt =await dataService.GetDataLog();
            return dt;
        }
    }
    public async Task<TblUser> GetUserAutorization(string user, string password)
    {
        using var dataService = _context.CreateDataService();
        var dt = await dataService.GetUser(user,password);
        return dt;
    }
}
