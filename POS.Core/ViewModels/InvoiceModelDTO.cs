using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels;
public class InvoiceModelDTO
{
    public long AccountId
    {
        get; set;
    }
    public string AccountName
    {
        get; set;
    }
    public string OfficeName
    {
        get; set;
    }
    public string Address
    {
        get; set;
    }
    public string VatName
    {
        get; set;
    }
    public string BinNo
    {
        get; set;
    }
    public DateTime Date
    {
        get; set;
    }
    public string CounterNo
    {
        get; set;
    }
    public string InvoiceNo
    {
        get; set;
    }
    public string SalesPersonName
    {
        get; set;
    }
    public string CustomerName
    {
        get; set;
    }
    public long ItemSl
    {
        get; set;
    }
    public string ItemName
    {
        get; set;
    }

    public decimal ItemQty
    {
        get; set;
    }
    public decimal ItemPrice
    {
        get; set;
    }
    public decimal ItemAmount
    {
        get; set;
    }
    public decimal Discount
    {
        get;set;
    }
    public decimal CurrentPoints
    {
        get; set;
    }
    public decimal TotalPoints
    {
        get; set;
    }
    public decimal TotalAmount
    {
        get; set;
    }
    public decimal VatAmount
    {
        get; set;
    }
    public decimal ItemVat
    {
        get; set;
    }
    public decimal Vat 
    {
        get; set;
    }
    public decimal SD
    {
        get; set;
    }
    public decimal SDAmount
    {
        get; set;
    }
    public decimal ItemSD 
    {
        get; set;
    }
    public decimal ItemDiscount
    {
        get; set;
    }
    public decimal PaidAmount
    {
        get; set;
    }
    public long PaymentMethodTypeId
    {
        get; set;
    }
    public string PaymentMethodTypeName
    {
        get; set;
    }
    public decimal PaymentMethodAmount
    {
        get; set;
    }

    public string OutletName
    {
        set;get;
    }

    public string BarCode
    {
        set; get;
    }

    public byte[] BarCodeImage
    {
        set; get;
    }
    public string Message
    {
        set; get;
    }

    public decimal changeAmount
    {
        set;get;
    }
    public decimal ExchangeAmount
    {
        set;get;
    }
    public decimal PayableAmount
    {
        set;get;
    }
}
