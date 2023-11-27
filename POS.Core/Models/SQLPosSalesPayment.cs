using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("POSSalesPayment", Schema = "pos")]
public class SQLPosSalesPayment
{
    [Key]
    public long POSSalesPaymentId
    {
        get; set;
    }
    public long SalesDeliveryId
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
    public long WalletId
    {
        get; set;
    }
    public decimal CollectionAmount
    {
        get; set;
    }
    public string ReferanceNo
    {
        get; set;
    }
    public DateTime TransactionDate
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
    public DateTime ServerDatetime
    {
        get; set;
    }
}
