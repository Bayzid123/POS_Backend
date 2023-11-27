using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels.SalesDeliveryDTO;
public class SalesDeliveryHeaderDTO
{
    public long accountId
    {
        get; set;
    }
    public string SalesOrderCode
    {
        get; set;
    }
    public string accountName
    {
        get; set;
    }
    public long actionById
    {
        get; set;
    }
    public string actionByName
    {
        get; set;
    }
    public decimal advanceBalanceAdjust
    {
        get; set;
    }
    public decimal amountPerInstallment
    {
        get; set;
    }
    public string anonymousAddress
    {
        get; set;
    }
    public decimal bankReceiveAmount
    {
        get; set;
    }
    public long branchId
    {
        get; set;
    }
    public string branchName
    {
        get; set;
    }
    public decimal cashReceiveAmount
    {
        get; set;
    }
    public string challanNo
    {
        get; set;
    }
    public decimal comissionPercentage
    {
        get; set;
    }
    public string createDate
    {
        get; set;
    }
    public long customerId
    {
        get; set;
    }
    public string customerName
    {
        get; set;
    }
    public decimal customerNetAmount
    {
        get; set;
    }
    public string customerPO
    {
        get; set;
    }
    public string deliveryDate
    {
        get; set;
    }
    public decimal discoundItemTotalPrice
    {
        get; set;
    }
    public long freeTypeId
    {
        get; set;
    }
    public string freeTypeName
    {
        get; set;
    }
    public decimal headerDiscount
    {
        get; set;
    }
    public decimal headerDiscountPercentage
    {
        get; set;
    }
    public string installmentStartDate
    {
        get; set;
    }
    public string installmentType
    {
        get; set;
    }
    public decimal interestRate
    {
        get; set;
    }
    public bool isAdvanceAdjust
    {
        get; set;
    }
    public bool isBank
    {
        get; set;
    }
    public bool isCash
    {
        get; set;
    }
    public bool isComplete
    {
        get; set;
    }
    public bool isCompletedCustomerOrder
    {
        get; set;
    }
    public bool isInclusive
    {
        get; set;
    }
    public bool isPosSales
    {
        get; set;
    }
    public bool isReturnAdjustInAdvance
    {
        get; set;
    }
    public decimal itemTotalAmount
    {
        get; set;
    }
    public string narration
    {
        get; set;
    }
    public decimal netAmount
    {
        get; set;
    }
    public decimal netAmountWithInterest
    {
        get; set;
    }
    public decimal netDiscount
    {
        get; set;
    }
    public decimal offerItemTotal
    {
        get; set;
    }
    public long officeId
    {
        get; set;
    }
    public string orderDate
    {
        get; set;
    }
    public long orderId
    {
        get; set;
    }
    public decimal othersCost
    {
        get; set;
    }
    public long paymentTypeId
    {
        get; set;
    }
    public string paymentTypeName
    {
        get; set;
    }
    public decimal pendingAmount
    {
        get; set;
    }
    public string phone
    {
        get; set;
    }
    public decimal points
    {
        get; set;
    }
    public string projectName
    {
        get; set;
    }
    public decimal receiveAmount
    {
        get; set;
    }
    public long receiveBankAccountId
    {
        get; set;
    }
    public string remarks
    {
        get; set;
    }
    public long salesForceId
    {
        get; set;
    }
    public string salesForceName
    {
        get; set;
    }
    public long salesOrderId
    {
        get; set;
    }
    public long shippingAddressId
    {
        get; set;
    }
    public string shippingAddressName
    {
        get; set;
    }
    public string shippingContactPerson
    {
        get; set;
    }
    public bool status
    {
        get; set;
    }
    public string statusType
    {
        get; set;
    }
    public decimal totalLineDiscount
    {
        get; set;
    }
    public long totalNoOfInstallment
    {
        get; set;
    }
    public long totalQuantity
    {
        get; set;
    }

    public decimal totalSd
    {
        set; get;
    }
    public decimal totalVat
    {
        set; get;
    }
    public long walletId
    {
        set; get;
    }
    public long warehouseId
    {
        set; get;
    }
    public decimal ReturnAmount
    {
        set; get;
    }
    
    public bool? ISExchange
    {
        set;get;
    }

    public long? CounterId
    {
        set; get;
    }

    public long? HeaderDiscountId
    {
        set;get;
    }
    public bool IsOnline
    {
        set; get;
    }

    public bool IsReturn
    {
        set; get;
    }
}





