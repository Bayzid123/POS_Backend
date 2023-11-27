using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("Item")]
public class Item
{
    [Key]
    
    
    public long ItemId
    {
        get; set;
    }
    public long? ItemGlobalId
    {
        get; set;
    }
 
    
    public string ItemName
    {
        get; set;
    }

    
    public string ItemCode
    {
        get; set;
    }

    
    public string Barcode
    {
        get; set;
    }
    
    public long? ItemTypeId
    {
        get; set;
    }
  
    
    public string ItemTypeName
    {
        get; set;
    }
    
    public long? ItemCategoryId
    {
        get; set;
    }

    
    public string ItemCategoryName
    {
        get; set;
    }
    
    public long? ItemSubCategoryId
    {
        get; set;
    }
 
    
    public string ItemSubCategoryName
    {
        get; set;
    }
    
    public long? AccountId
    {
        get; set;
    }
   
    
    public string AccountName
    {
        get; set;
    }
    
    public long? BranchId
    {
        get; set;
    }

    
    public string BranchName
    {
        get; set;
    }
    
    public long? UomId
    {
        get; set;
    }
  
    
    public string UomName
    {
        get; set;
    }
    
    public long? UserId
    {
        get; set;
    }
   
    
    public string UserName
    {
        get; set;
    }
    
    public string ActionTime
    {
        get; set;
    }
    
    public string StartDate
    {
        get; set;
    }
    
    public string ExpiredDate
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
    
    public decimal SD
    {
        get; set;
    }
    public decimal? AvgRate
    {
        get; set;
    }

    public decimal TotalQuantity
    {
        get; set;
    }
    public decimal? VatPercentage
    {
        get; set;
    }
    
    public decimal CurrentSellingPrice
    {
        get; set;
    }
    
    public bool IsActive
    {
        get; set;
    }
    public decimal? StockLimitQuantity
    {
        get; set;
    }
    public long? TaxRateId
    {
        get; set;
    }

    public string Brand
    {
        get; set;
    }

    public string PartNumber
    {
        get; set;
    }

    public string ItemDescription
    {
        get; set;
    }
    public long? OriginId
    {
        get; set;
    }
  
    public string OriginName
    {
        get; set;
    }
    public decimal? StdPurchasePrice
    {
        get; set;
    }
    public long? AltUomId
    {
        get; set;
    }

    public string AltUomName
    {
        get; set;
    }
    public decimal? ConversionUnit
    {
        get; set;
    }
    
    public bool IsSerial
    {
        get; set;
    }
    
    public decimal MaximumDiscountPercent
    {
        get; set;
    }
    
    public bool IsBatchManage
    {
        get; set;
    }
    
    public string HSCode
    {
        get; set;
    }
    public decimal? MaximumDiscountAmount
    {
        set;get;
    }

    public long? SupplierId
    {
        set;get;
    }
    public bool IsNegativeSales
    {
        set; get;
    }

    public bool IsMultipleSalesPrice
    {
        set; get;
    }

}


