using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels.MainViewModelDTO;
public class MainViewModelItemDTO
{
    public long Id {get; set;}
    public long SL { get; set; }
    public long ItemId {set;get;}
    public string ItemName { get; set; }
    public string Quantity {set; get;}
    public string ExcQty
    {
        set; get;
    }
    public string SalesRate {set;get;}
    public decimal Vat {set;get;}
    public decimal SD {set;get;}
    public decimal Discount {set;get;}
    public string Amount {set;get;}


    public decimal DiscountPercentage {set;get;}
    public decimal SingleDiscountAmount {set;get;}
    public decimal VATPercentage {set;get;}
    public decimal SDPercentage {set;get;}  

    public string BarCode{set;get;}

    public long UMOid {set;get;}
    public string UMOName {set;get;}

    public string DatabaseStock {set;get;}
    public long ExchangeReferenceNo {set;get;}
    public bool isExchangeEnable {set;get;}
    public decimal Qty
    {
        set; get;
    }
    public bool IsNegativeSales
    {
        set; get;
    }

    public decimal OtherDiscount
    {
        set; get;
    }
    

}
