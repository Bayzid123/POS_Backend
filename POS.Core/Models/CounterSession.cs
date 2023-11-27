using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("CounterSession")]
public class CounterSession
{
    [Key]

    public long CounterSessionId
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
    public long CounterId
    {
        get; set;
    }
    [MaxLength(50)]
    public string CounterCode
    {
        get; set;
    }
    public decimal OpeningCash
    {
        get; set;
    }
    public decimal? ClosingCash
    {
        get; set;
    }
    [MaxLength(200)]
    public string OpeningNote
    {
        get; set;
    }
    public string ClosingNote
    {
        get; set;
    }
    public DateTime StartDatetime
    {
        get; set;
    }
    public long TotalInvoice
    {
        get; set;
    }
    public decimal TotalSales
    {
        get; set;
    }
    public decimal CardAmountCollection
    {
        get; set;
    }
    public decimal MFSAmountCollection
    {
        get; set;
    }
    public decimal CashAmountCollection
    {
        get; set;
    }
    public DateTime? ClosingDatetime
    {
        get; set;
    }
    public long ActionById
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    }
    public DateTime LastActionDatetime
    {
        get; set;
    }
    public DateTime ServerDatetime
    {
        get; set;
    }
    public bool IsSync
    {
        get; set;
    }

}
