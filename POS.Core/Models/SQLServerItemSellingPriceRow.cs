using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("ItemSellingPriceRow", Schema = "pos")]
public class SQLServerItemSellingPriceRow
{
    [Key]

    //[Required]
    public long RowId
    {
        get; set;
    }
    //[Required]
    public long HeaderId
    {
        get; set;
    }
    //[Required]
    public long ItemId
    {
        get; set;
    }
    //[MaxLength(50)]
    //[Required]
    public string ItemCode
    {
        get; set;
    }
    //[Required]
    public decimal OldPrice
    {
        get; set;
    }
    //[Required]
    public decimal NewPrice
    {
        get; set;
    }
    //[Required]
    public decimal Qty
    {
        get; set;
    }
    //[Required]
    public bool IsActive
    {
        get; set;
    }
    //[Required]
    public long ActionById
    {
        get; set;
    }
    //[Required]
    public DateTime LastActionDatetime
    {
        get; set;
    }



}

[Table("ItemSellingPriceHeader", Schema = "pos")]
public class ItemSellingPriceHeader
{
    [Key]
    public long HeaderId
    {
        get; set;
    }
    public string TransactionCode
    {
        get; set;
    }
    public DateTime TransactionDate
    {
        get; set;
    }
    public long AccountId
    {
        get; set;
    }
    public long BranchId
    {
        get; set;
    }
    public long OfficeId
    {
        get; set;
    }
    public long WarehouseId
    {
        get; set;
    }
    public DateTime StartDate
    {
        get; set;
    }
    public DateTime? EndDate
    {
        get; set;
    }
    public string Remarks
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    }
    public long ActionById
    {
        get; set;
    }
    public DateTime LastActionDatetime
    {
        get; set;
    }
    public bool IsApprove
    {
        get; set;
    }
    public long ApproveById
    {
        get; set;
    }
    public DateTime? ApproveDatetime
    {
        get; set;
    }
    public long CancelById
    {
        get; set;
    }
    public DateTime? CancelDatetime
    {
        get; set;
    }
    public bool IsPercent
    {
        get; set;
    }
    public decimal AmountOrPercent
    {
        get; set;
    }
    public bool? IsMarkUp
    {
        get; set;
    }
    public long SupplierId
    {
        get; set;
    }
}