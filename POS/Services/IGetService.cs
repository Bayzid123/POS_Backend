using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.CounterSession;

namespace POS.Services;
public interface IGetService
{
    Task<TblSettings> GetSettings();
    Task<TblUser> GetUser(long Id);

    Task<CounterSession> GetCounterSession();
    Task<List<GetCounterSessionDetailsDTO>> GetCounterSessionDetails(long CounterSessionId);
    Task<List<SalesInvoiceDTO>> GetSalesInvoice(long UserId, bool isSyn);
    Task<List<SalesInvoiceDTO>> GetSalesInvoiceLiveServer(long UserId, bool isSyn,string SalesInvoice);
    Task<POSSalesDeliveryHeader> GetPosDeliveryHeader(string invoice);
    Task<POSSalesDeliveryHeader> GetPosLiveDeliveryHeader(string invoice);
    Task<List<POSSalesDeliveryLine>> GetPosDeliveryLine(long salesOrderId);

    Task<List<POSSalesDeliveryLine>> GetPosLiveDeliveryLine(long salesOrderId);
    Task<List<POSSalesDeliveryLineDTO>> GetPosLiveDeliveryLineItem(long salesOrderId);
    Task<List<POSSalesDeliveryLineDTO>> GetPosDeliveryLineItem(long salesOrderId);
    Task<List<POSSalesPaymentDTO>> GetSalesPayment(long salesOrderId);
    Task<List<POSSalesPaymentDTO>> GetLiveSalesPayment(long salesOrderId);
    Task<List<tblDataLog>> GetDataLog();
    Task<TblUser> GetUserAutorization(string user, string password);
}
