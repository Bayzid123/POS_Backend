using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels;
public class POSSalesDeliveryLineDTO
{
    public long SL
    {
        get; set;
    }
    public long Id
    {
        get; set;
    }
    public long SalesOrderId
    {
        get; set;
    }
    public long ItemId
    {
        get; set;
    }
    public string BarCode
    {
        get;set;
    }
    public string ItemName
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
    public decimal Quantity
    {
        get; set;
    }
    public decimal? ChangeQuantity
    {
        get; set;
    }
    public decimal Price
    {
        get; set;
    }
    public decimal TotalAmount
    {
        get; set;
    }
    public decimal LineDiscount
    {
        get; set;
    }
    public decimal NetAmount
    {
        get; set;
    }
    public decimal? VatPercentage
    {
        get; set;
    }
    public DateTime? WarrantyExpiredDate
    {
        get; set;
    }
    public string WarrantyDescription
    {
        get; set;
    }
    public long? WarrantyInMonth
    {
        get; set;
    }
    public decimal? HeaderDiscountProportion
    {
        get; set;
    }
    public decimal? HeaderCostProportion
    {
        get; set;
    }
    public decimal? CostPrice
    {
        get; set;
    }
    public decimal? CostTotal
    {
        get; set;
    }
    //[MaxLength(500)]
    public string AnonymousAddress
    {
        get; set;
    }
    public long? WarehouseId
    {
        get; set;
    }
    public decimal? SdPercentage
    {
        get; set;
    }
    public decimal? VatAmount
    {
        get; set;
    }
    public decimal? SdAmount
    {
        get; set;
    }
    //[MaxLength(200)]
    public string DiscountType
    {
        get; set;
    }
    public decimal? DiscountAmount
    {
        get; set;
    }
    //[MaxLength(250)]
    //[Required]
    public string OfferItemName
    {
        get; set;
    }
    //[Required]
    public decimal OfferItemQty
    {
        get; set;
    }
    //[Required]
    public long OfferItemId
    {
        get; set;
    }
    //[Required]
    public bool IsOfferItem
    {
        get; set;
    }
    //[Required]
    public decimal ItemBasePriceInclusive
    {
        get; set;
    }
    //[MaxLength(2000)]
    public string ItemDescription
    {
        get; set;
    }
    public long? FreeTypeId
    {
        get; set;
    }
    //[MaxLength(100)]
    public string FreeTypeName
    {
        get; set;
    }
    //[MaxLength]
    public string ItemSerial
    {
        get; set;
    }
    //[MaxLength]
    public string Batch
    {
        get; set;
    }
    public bool IsSync
    {
        get; set;
    }
}
