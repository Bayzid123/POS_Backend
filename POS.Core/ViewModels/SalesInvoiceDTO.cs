using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels;
public class SalesInvoiceDTO
{
    public long Sl
    {
        get; set;
    }
    public long SalesOrderId
    {
        get; set;
    }
    public string SalesInvoice
    {
        get; set;
    }
    public DateTime InvoiceDate
    {
        get; set;
    }
    public decimal Quantity
    {
        get; set;
    }
    public decimal CashAmount
    {
        get; set;
    }
    public decimal SalesAmount
    {
        get; set;
    }

    public bool isSynchonized
    {
        set;get;
    }

    public bool isSelected
    {
        set;get;
    }
}
