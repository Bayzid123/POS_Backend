using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.ViewModels.CounterDTO;
public class ItemDTO
{
    public List<ItemData> items
    {
    get; set; 
    }
    public long count
    {
    get; set; }
}
public class PartnerDTO
{
    public List<Partner> items
    {
        get; set;
    }
    public long count
    {
        get; set;
    }
}
public class ItemData
{
    public long ItemId
    {
        get; set;
    }
    public string ItemName {
    get; set; 
    }
    public string Barcode {
    get; set; 
    }
    public long ItemCategoryId
        {
            get; set;
        }
    public long ItemSubCategoryId
    {
        get; set;
    }
    public long UomId
    {
    get; set;
    }
    public string UomName
    {
    get; set;
    }
    public decimal Price
    {
    get; set;
    }
    public decimal Vat
    {
    get; set;
    }
    public decimal Sd
    {
        get; set;
    }
    public decimal MaximumDiscountPercent
    {
        get; set;
    }
    public decimal TotalQuantity
    {
        get; set;
    }
    public decimal CurrentSellingPrice
    {
        get; set;
    }
  

}

