using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("SalesWallet")]
public class SalesWallet
{
    [Key]
    //[Required]
    public long WalletId
    {
        get; set;
    }
    //[MaxLength(100)]
    //[Required]
    public string WalletName
    {
        get; set;
    }
    //[Required]
    public long AccountId
    {
        get; set;
    }
    //[Required]
    public long BranchId
    {
        get; set;
    }
    //[Required]
    public decimal ComissionPercentage
    {
        get; set;
    }
    public long? BankAccountId
    {
        get; set;
    }
    //[MaxLength(100)]
    public string BankAccNo
    {
        get; set;
    }
    //[Required]
    public bool isBank
    {
        get; set;
    }
    //[Required]
    public bool isActive
    {
        get; set;
    }
    //[Required]
    public DateTime LastActionDateTime
    {
        get; set;
    }
    public bool IsSync
    {
        get; set;
    }
}
