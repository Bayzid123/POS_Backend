using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels.CounterSession;
public class CounterSessionDetailsDTO
{
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
public class GetCounterSessionDetailsDTO
{
    public long CounterSessionId
    {
        get; set;
    }
    public decimal OpeningCash
    {
        get; set;
    }
    public decimal ClosingCash
    {
        get; set;
    }
    public string OpeningNote
    {
        get; set;
    }
    public string ClosingNote
    {
        get; set;
    }
    public string CuerrencyName
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
}
