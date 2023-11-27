using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels;
public class POSSalesPaymentDTO
{
    public long SL
    {
        get; set;
    }
    public long POSSalesPaymentId
    {
        get; set;
    }
    //[Required]
    public long SalesDeliveryId
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
    public long OfficeId
    {
        get; set;
    }
    //[Required]
    public long WalletId
    {
        get; set;
    }
    public string WalletName
    {
        get; set;
    }
    public string ReferanceNo
    {
        get; set;
    }
    //[Required]
    public decimal CollectionAmount
    {
        get; set;
    }
    //[Required]
    public DateTime TransactionDate
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
    //[Required]
    public DateTime ServerDatetime
    {
        get; set;
    }
    public bool IsSync
    {
        get; set;
    }
}
