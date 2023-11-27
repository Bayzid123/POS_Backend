using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("POSSalesDeliveryLine")]
public class POSSalesDeliveryLine
{
    [Key]
    
    //[Required]
    public long Id
    {
        get; set;
    }
    //[Required]
    public long SalesOrderId
    {
        get; set;
    }
    //[Required]
    public long ItemId
    {
        get; set;
    }
    //[MaxLength(1000)]
    //[Required]
    public string ItemName
    {
        get; set;
    }
    //[Required]
    public long UomId
    {
        get; set;
    }
    //[MaxLength(50)]
    //[Required]
    public string UomName
    {
        get; set;
    }
    //[Required]
    public decimal Quantity
    {
        get; set;
    }
    public decimal? ChangeQuantity
    {
        get; set;
    }
    //[Required]
    public decimal Price
    {
        get; set;
    }
    //[Required]
    public decimal TotalAmount
    {
        get; set;
    }
    //[Required]
    public decimal LineDiscount
    {
        get; set;
    }
    //[Required]
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
    //[MaxLength]
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
    public long? ExchangeReferenceId
    {
        set; get;
    }
    public decimal? OtherDiscount
    {
        get; set;
    }

}
