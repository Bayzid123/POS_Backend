using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels.SalesDeliveryDTO;
public class SalesDeliveryRowDTO
{
    public List<ItemInfo> items
    {
        get; set;
    }
}


public class Batch
{
    public long batchId
    {
        get; set;
    }
    public long itemId
    {
        get; set;
    }
    public decimal quantity
    {
        get; set;
    }
    public int warehouseId
    {
        get; set;
    }
}

public class ItemInfo
{
    public List<Batch> batchList
    {
        get; set;
    }
    public long cogs
    {
        get; set;
    }
    public decimal discountAmount
    {
        get; set;
    }
    public string discountType
    {
        get; set;
    }
    public long freeItemUomId
    {
        get; set;
    }
    public string freeItemUomName
    {
        get; set;
    }
    public long freeTypeId
    {
        get; set;
    }
    public string freeTypeName
    {
        get; set;
    }
    public bool isOfferItem
    {
        get; set;
    }
    public long itemBasePriceInclusive
    {
        get; set;
    }
    public string itemDescription
    {
        get; set;
    }
    public long itemId
    {
        get; set;
    }
    public string itemName
    {
        get; set;
    }
    public decimal lineDiscount
    {
        get; set;
    }
    public decimal lineDiscountRate
    {
        get; set;
    }
    public decimal netAmount
    {
        get; set;
    }
    public long offerItemId
    {
        get; set;
    }
    public string offerItemName
    {
        get; set;
    }
    public decimal offerItemQty
    {
        get; set;
    }
    public decimal price
    {
        get; set;
    }
    public decimal quantity
    {
        get; set;
    }
    public decimal sdAmount
    {
        get; set;
    }
    public decimal sdPercentage
    {
        get; set;
    }
    public List<long> serialrowData
    {
        get; set;
    }
    public decimal totalAmount
    {
        get; set;
    }
    public long uomId
    {
        get; set;
    }
    public string uomName
    {
        get; set;
    }
    public decimal vatAmount
    {
        get; set;
    }
    public decimal vatPercentage
    {
        get; set;
    }
    public long warehouseId
    {
        get; set;
    }
    public string warrantyDescription
    {
        get; set;
    }
    public string warrantyExpiredDate
    {
        get; set;
    }
    public long warrantyInMonth
    {
        get; set;
    }
    public long? ExchangeReferenceId
    {
        set; get;
    }
    public decimal OtherDiscount
    {
        get; set;
    }
}