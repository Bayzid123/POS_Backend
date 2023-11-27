using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("POSSalesDeliveryHeader", Schema = "pos")]
public class SQLServerPOSSalesDeliveryHeader
{
    [Key]
    public long SalesOrderId {set;get;}
    public string SalesOrderCode {set;get;}
    public long? CustomerOrderId {set;get;}
    public long AccountId {set;get;}
    public string AccountName {set;get;}
    public long BranchId {set;get;}
    public string BranchName {set;get;}
    public long CustomerId {set;get;}
    public string CustomerName {set;get;}
    public string? Phone {set;get;}
    public string? ChallanNo {set;get;}
    public DateTime OrderDate {set;get;}
    public DateTime DeliveryDate {set;get;}
    public string Remarks {set;get;}
    public long PaymentTypeId {set;get;}
    public string PaymentTypeName {set;get;}
    public decimal TotalQuantity {set;get;}
    public decimal ItemTotalAmount {set;get;}
    public decimal NetDiscount {set;get;}
    public decimal OthersCost {set;get;}
    public decimal NetAmount {set;get;}
    public decimal TotalLineDiscount {set;get;}
    public decimal HeaderDiscount {set;get;}
    public decimal HeaderDiscountPercentage {set;get;}
    public decimal ReceiveAmount {set;get;}
    public decimal PendingAmount {set;get;}
    public decimal? ReturnAmount {set;get;}
    public decimal InterestRate {set;get;}
    public decimal NetAmountWithInterest { set;get;}
    public long TotalNoOfInstallment {set;get;}
    public DateTime InstallmentStartDate {set;get;}
    public string InstallmentType {set;get;}
    public decimal AmountPerInstallment {set;get;}
    public long SalesForceId {set;get;}
    public string SalesForceName {set;get;}
    public long ActionById {set;get;}
    public string ActionByName {set;get;}
    public DateTime ActionTime {set;get;}
    public bool? IsPosSales {set;get;}
    public bool isActive {set;get;}
    public string SalesOrReturn {set;get;}
    public decimal? AdvanceBalanceAdjust {set;get;}
    public decimal? CustomerNetAmount {set;get;}
    public bool? IsComplete {set;get;}
    public int? SalesTypeId {set;get;}
    public string SalesTypeName {set;get;}
    public long? SalesOrderRefId {set;get;}
    public string Narration {set;get;}
    public long? SmsTransactionId {set;get;}
    public string AnonymousAddress {set;get;}
    public decimal? TotalSd {set;get;}
    public decimal? TotalVat {set;get;}
    public bool IsBillCreated {set;get;}
    public decimal? DiscoundItemTotalPrice {set;get;}
    public decimal? OfferItemTotal {set;get;}
    public long WalletId {set;get;}
    public decimal? ComissionPercentage {set;get;}
    public bool? isInclusive {set;get;}
    public long OfficeId {set;get;}
    public string CustomerPO {set;get;}
    public string BillNo {set;get;}
    public long? ShippingAddressId {set;get;}
    public string ShippingAddressName {set;get;}
    public string ShippingContactPerson {set;get;}
    public bool IsConfirmed {set;get;}
    public bool IsApprove {set;get;}
    public string ProjectName {set;get;}
    public long FreeTypeId {set;get;}
    public string FreeTypeName {set;get;}
    public long JobOrderId {set;get;}
    public long VoucherId {set; get;}
    public bool? ISExchange {set;get;}
    public long? HeaderDiscountId {set;get;}
    public decimal? CashPayment
    {
        get;set;
    }
    public decimal? Points
    {
        get; set;
    }
    public long CounterId
    {
        set; get;
    }
    public bool? ISReturn
    {
        set; get;
    }

}
