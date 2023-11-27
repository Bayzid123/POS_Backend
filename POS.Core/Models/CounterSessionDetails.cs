using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("CounterSessionDetails")]
public class CounterSessionDetails
{
    [Key]
    
    public long CounterSessionDetailsId
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
    public long CounterId
    {
        get; set;
    }
    public long CounterSessionId
    {
        get; set;
    }
    [MaxLength(50)]
    public string CurrencyName
    {
        get; set;
    }
    public long CurrencyOpeningCount
    {
        get; set;
    }
    public long CurrencyClosingCount
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
