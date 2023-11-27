using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable annotations
namespace POS.Core.Models;
[Table("POSCounter")]
public class POSCounter
{
    [Key]
    
    public long CounterId
    {
        get; set;
    }
    [MaxLength(100)]
    public string CounterName
    {
        get; set;
    }
    [MaxLength(50)]
    public string CounterCode
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
    public DateTime CounterOpeningDate
    {
        get; set;
    }
    public DateTime? CounterClosingDate
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
