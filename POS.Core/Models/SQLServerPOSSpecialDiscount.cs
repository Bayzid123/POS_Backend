using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("POSSpecialDiscount", Schema = "pos")]
public class SQLServerPOSSpecialDiscount
{
    [Key]
    public long HeaderId
    {
        set; get;
    }
    public string OfferName
    {
        set; get;
    }
    public long WarehouseId
    {
        set; get;
    }
    public DateTime StartDate
    {
        set; get;
    }
    public DateTime EndDate
    {
        set; get;
    }
    public string Remarks
    {
        set; get;
    }
    public long DiscountType
    {
        set; get;
    }
    public decimal Value
    {
        set; get;
    }
    public bool IsActive
    {
        set; get;
    }
    public long ActionById
    {
        set; get;
    }
    public DateTime LastActionDatetime
    {
        set; get;
    }
    public long AccountId
    {
        set; get;
    }

    public long BranchId
    {
        set; get;
    }

    public decimal MinAmount
    {
        set; get;
    }
    public decimal MaxAmount
    {
        set; get;
    }

}
