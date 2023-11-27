using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
public class ItemSellingPriceHeader222
{
    [Key]
    
    [Required]
    public long HeaderId
    {
        get; set;
    }
    [MaxLength(30)]
    [Required]
    public string TransactionCode
    {
        get; set;
    }
    [Required]
    public DateTime TransactionDate
    {
        get; set;
    }
    [Required]
    public long AccountId
    {
        get; set;
    }
    [Required]
    public long BranchId
    {
        get; set;
    }
    [Required]
    public long OfficeId
    {
        get; set;
    }
    [Required]
    public long WarehouseId
    {
        get; set;
    }
    [Required]
    public DateTime StartDate
    {
        get; set;
    }
    public DateTime? EndDate
    {
        get; set;
    }
    [MaxLength(300)]
    public string Remarks
    {
        get; set;
    }
    [Required]
    public bool IsActive
    {
        get; set;
    }
    [Required]
    public long ActionById
    {
        get; set;
    }
    [Required]
    public DateTime LastActionDatetime
    {
        get; set;
    }
    [Required]
    public bool IsApprove
    {
        get; set;
    }
    [Required]
    public long ApproveById
    {
        get; set;
    }
    public DateTime? ApproveDatetime
    {
        get; set;
    }
    [Required]
    public long CancelById
    {
        get; set;
    }
    public DateTime? CancelDatetime
    {
        get; set;
    }
}
