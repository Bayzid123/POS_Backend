using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.ViewModels.SalesDeliveryDTO;

namespace POS.Core.Models;
[Table("POSSalesDeliveryLine", Schema = "pos")]
public class SQLServerPOSSalesDeliveryLine
{
    [Key]
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
    public string DiscountType
    {
        get; set;
    }
    public decimal? DiscountAmount
    {
        get; set;
    }
    public string OfferItemName
    {
        get; set;
    }
    public decimal OfferItemQty
    {
        get; set;
    }
    public long OfferItemId
    {
        get; set;
    }
    public bool IsOfferItem
    {
        get; set;
    }
    public decimal ItemBasePriceInclusive
    {
        get; set;
    }
    public string ItemDescription
    {
        get; set;
    }
    public long? FreeTypeId
    {
        get; set;
    }
    public string FreeTypeName
    {
        get; set;
    }
    public string ItemSerial
    {
        get; set;
    }

    public string  Batch{set;get;}
    public long? ExchangeReferenceId
    {
        set; get;
    }
    public decimal? OtherDiscount
    {
        get; set;
    }
}
