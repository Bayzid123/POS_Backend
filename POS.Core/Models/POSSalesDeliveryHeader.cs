using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("POSSalesDeliveryHeader")]
public class POSSalesDeliveryHeader
{
    [Key]
    
    //[Required]
    public long SalesOrderId
    {
        get; set;
    }
    //[MaxLength(50)]
    //[Required]
    public string SalesOrderCode
    {
        get; set;
    }
    public long? CustomerOrderId
    {
        get; set;
    }
    //[Required]
    public long AccountId
    {
        get; set;
    }
    //[MaxLength(100)]
    //[Required]
    public string AccountName
    {
        get; set;
    }
    //[Required]
    public long BranchId
    {
        get; set;
    }
    //[MaxLength(100)]
    //[Required]
    public string BranchName
    {
        get; set;
    }
    //[Required]
    public long CustomerId
    {
        get; set;
    }
    //[MaxLength(100)]
    //[Required]
    public string CustomerName
    {
        get; set;
    }
    //[MaxLength(100)]
    public string Phone
    {
        get; set;
    }
    //[MaxLength(250)]
    public string ChallanNo
    {
        get; set;
    }
    //[Required]
    public DateTime OrderDate
    {
        get; set;
    }
    //[Required]
    public DateTime DeliveryDate
    {
        get; set;
    }
    //[MaxLength]
    //[Required]
    public string Remarks
    {
        get; set;
    }
    //[Required]
    public long PaymentTypeId
    {
        get; set;
    }
    //[MaxLength(100)]
    //[Required]
    public string PaymentTypeName
    {
        get; set;
    }
    //[Required]
    public decimal TotalQuantity
    {
        get; set;
    }
    //[Required]
    public decimal ItemTotalAmount
    {
        get; set;
    }
    //[Required]
    public decimal NetDiscount
    {
        get; set;
    }
    //[Required]
    public decimal OthersCost
    {
        get; set;
    }
    //[Required]
    public decimal NetAmount
    {
        get; set;
    }
    //[Required]
    public decimal TotalLineDiscount
    {
        get; set;
    }
    //[Required]
    public decimal HeaderDiscount
    {
        get; set;
    }
    //[Required]
    public decimal HeaderDiscountPercentage
    {
        get; set;
    }
    //[Required]
    public decimal ReceiveAmount
    {
        get; set;
    }
    //[Required]
    public decimal PendingAmount
    {
        get; set;
    }
    public decimal? ReturnAmount
    {
        get; set;
    }
    //[Required]
    public decimal InterestRate
    {
        get; set;
    }
    //[Required]
    public decimal NetAmountWithInterest
    {
        get; set;
    }
    //[Required]
    public long TotalNoOfInstallment
    {
        get; set;
    }
    //[Required]
    public DateTime InstallmentStartDate
    {
        get; set;
    }
    //[MaxLength(20)]
    //[Required]
    public string InstallmentType
    {
        get; set;
    }
    //[Required]
    public decimal AmountPerInstallment
    {
        get; set;
    }
    //[Required]
    public long SalesForceId
    {
        get; set;
    }
    //[MaxLength(100)]
    //[Required]
    public string SalesForceName
    {
        get; set;
    }
    //[Required]
    public long ActionById
    {
        get; set;
    }
    //[MaxLength(100)]
    //[Required]
    public string ActionByName
    {
        get; set;
    }
    //[Required]
    public DateTime ActionTime
    {
        get; set;
    }
    public bool? IsPosSales
    {
        get; set;
    }
    //[Required]
    public bool isActive
    {
        get; set;
    }
    //[MaxLength(50)]
    public string SalesOrReturn
    {
        get; set;
    }
    public decimal? AdvanceBalanceAdjust
    {
        get; set;
    }
    public decimal? CustomerNetAmount
    {
        get; set;
    }
    public bool? IsComplete
    {
        get; set;
    }
    public long? SalesTypeId
    {
        get; set;
    }
    //[MaxLength(100)]
    public string SalesTypeName
    {
        get; set;
    }
    public long? SalesOrderRefId
    {
        get; set;
    }
    //[MaxLength]
    public string Narration
    {
        get; set;
    }
    public long? SmsTransactionId
    {
        get; set;
    }
    //[MaxLength(500)]
    public string AnonymousAddress
    {
        get; set;
    }
    public decimal? TotalSd
    {
        get; set;
    }
    public decimal? TotalVat
    {
        get; set;
    }
    //[Required]
    public bool IsBillCreated
    {
        get; set;
    }
    public decimal? DiscoundItemTotalPrice
    {
        get; set;
    }
    public decimal? OfferItemTotal
    {
        get; set;
    }
    public long? WalletId
    {
        get; set;
    }
    public decimal? ComissionPercentage
    {
        get; set;
    }
    public bool? isInclusive
    {
        get; set;
    }
    //[Required]
    public long OfficeId
    {
        get; set;
    }
    //[MaxLength(100)]
    public string CustomerPO
    {
        get; set;
    }
    //[MaxLength(100)]
    public string BillNo
    {
        get; set;
    }
    public long? ShippingAddressId
    {
        get; set;
    }
    //[MaxLength(200)]
    public string ShippingAddressName
    {
        get; set;
    }
    //[MaxLength(200)]
    //[Required]
    public string ShippingContactPerson
    {
        get; set;
    }
    //[Required]
    public bool IsConfirmed
    {
        get; set;
    }
    //[Required]
    public bool IsApprove
    {
        get; set;
    }
    //[MaxLength(300)]
    public string ProjectName
    {
        get; set;
    }
    //[Required]
    public long FreeTypeId
    {
        get; set;
    }
    //[MaxLength(100)]
    public string FreeTypeName
    {
        get; set;
    }
    //[Required]
    public long JobOrderId
    {
        get; set;
    }
    public long IsSync
    {
        get; set;
    }

    public bool Draft
    {
        set;get;
    }

    public long UserId
    {
        set; get;
    }

    public decimal? CashPayment
    {
        set;get;
    }

    public long CounterId
    {
        set;get;
    }
    public string CounterName
    {
        set;get;
    }

    public decimal? Points
    {
        set;get;
    }
    public bool? ISExchange
    {
        set;get;
    }
    public long? HeaderDiscountId
    {
        set;get;
    }
    public bool? ISReturn
    {
        set; get;
    }
}
