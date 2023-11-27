using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels.MainViewModelDTO;
public class MyCollection
{
    public string TotalBill
    {
        set; get;
    }
    public decimal NumtotalBill
    {
        set; get;
    }
    public string TotalSD { set; get; }
    public decimal NumtotalSD
    {
        set; get;
    }
    public string TotalVAT
    {
        set; get;
    }

    public decimal NumtotalVAT
    {
        set; get;
    }

    public string TotalDiscount
    {
        set; get;
    }

    public decimal NumTotalDiscount
    {
        set; get;
    }
    public string OtherDiscount
    {
        set; get;
    }

    public decimal NumotherDiscount
    {
        set; get;
    }

    public string GrandTotal{set;get;}
    public decimal NumGrandTotal
    {
        set;get;
    }

    public decimal ReceiveAmount
    {
        set;get;
    }
    public decimal ChangeAmount
    {
        set;get;
    }
    public long OtherDiscountType
    {
        set;get;
    }
    public decimal NumOtherDiscountPercentage
    {
        set; get;
    }

    public decimal NumOtherDiscountAmount
    {
        set;get;
    }
    public long OtherDiscountId
    {
        set;get;
    }
    public decimal CashPayment
    {
        set;get;
    }

    public string InvoiceNumber
    {
        set;get;
    }


    public long CustomerId
    {
        set; get;
    }

    public string CustomerName
    {
        set; get;
    }
    public string CustomerCode
    {
        set; get;
    }
    public string CustomerPoints
    {
        set;get;
    }

    public decimal MinDiscountAmount
    {
        set; get;
    }
    public decimal MaxDiscountAmount
    {
        set; get;
    }

    public bool IsReturn
    {
        set; get;
    }
}
