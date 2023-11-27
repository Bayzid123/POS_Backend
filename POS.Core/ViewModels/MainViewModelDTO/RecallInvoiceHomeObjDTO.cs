using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels.MainViewModelDTO;
public class RecallInvoiceHomeObjDTO
{
    public long SL { get; set; }
    public long SalesOrderId {set;get;}
    public string CustomerName
    {
        set; get;
    }
    public string SalesInvoice {set; get;}
    public string OrderDate {set; get;}
    public decimal Quantity {set; get;}
    public decimal CashAmount {set; get;}
    public decimal SalesAmount {set;get;}
}
