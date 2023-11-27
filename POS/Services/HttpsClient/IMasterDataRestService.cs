using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.MainViewModelDTO;

namespace POS.Services.HttpsClient;
public interface IMasterDataRestService
{
    public Task<List<Item>> GetAllItems(long AccountId, long BranchId);
    public Task<List<Warehouse>> GetWarehouseForPOS(long accountingId, long branchId, long officeId, long warehouseId);
    public Task<List<ItemSellingPriceRow>> GetUpdatedItemSellingcPriceForPOS(long accountId, long branchId, long officeId, long warehouseId);
    public Task<List<Partner>> GetAllPartner(long AccountId, long BranchId);

    public Task<MessageHelper> SaveItemsToDatabase(List<Item> items);
    public Task<MessageHelper> SaveWareHouseToDatabase(List<Warehouse> warehouses);


    public Task<Partner> GetCustomerByCustomerId(string strPartnerCode);
    Task<Partner> GetPartnerById(long PartnerId);
    public Task<Item> GetItemByBarCode(string ItemBarCode);

    public Task<List<Item>> GetItemListByBarCode(string ItemBarCode);
    public Task<List<Item>> GetMultipleSalesPrizeItemListByBarCode(string ItemBarCode);
    public Task<List<Item>> GetItemListByItemName(string ItemName);
    public Task<List<Item>> GetItemByItemIDs(List<long> ids);
    Task<Partner> GetPartner();
    Task<Partner> GetPartner(long? partnerId);
    Task<MessageHelper> SavePartnerToDatabase(List<Partner> partners);
    public Task<CreateSalesDeliveryDTO> SaveItemIntoSalesDeliveryLines(CreateSalesDeliveryDTO objCreate);
    Task<MessageHelper> EditPosPayment(EditPosSales edit);
    public Task<List<PaymentWalletDTO>> GetPaymentWalletList();
    public Task<List<OtherDiscountDTO>> GetOtherDiscountList();

    public Task<MainViewRecallInvoiceDTO> RecallInvoiceInformation(long userId, long customerId);
    Task<MessageHelper> DeleteItemsToDatabase();
    public Task<MessageHelper> DeletePartnerToDatabase();
    public Task<List<CreateSalesDeliveryDTO>> SalesInformationUsingIDs(List<long> SalesOrderIds);
    public Task<MainViewRecallInvoiceDTO> RecallInvoice(long userId, long SalesDeliveryId);

    public Task<string> InvoiceCodeGenerate(long CounterId, string CounterCode);

    public Task<List<RecallInvoiceHomeObjDTO>> InvoiceHomePageLanding(long userId);

    public Task<POSSalesDeliveryHeader> GetPOSSalesDeliveryHeader(string InvoiceCode);

    public Task<PaymentWalletDTO> GetPaymentWalletbyId(long WalletId);

    Task<List<PaymentWalletDTO>> GetPaymentWalletbyIds(List<long> WalletIds);

    void RemoveDuplicateItems();
    Task<Item> GetItemByItemID(long Id);
    Task<MessageHelper> DeleteSessionData();
    Task<CreateSalesDeliveryDTO> SaveItemIntoSalesDelivery(CreateSalesDeliveryDTO objCreate);
    public Task<List<tblPointOfferRow>> GetPointsOfferRowsByItemIds(List<long> itemIds);
    public Task<Item> GetStockQtyCheckItemByItemID(long Id, decimal salesRate);
     Task DeleteDataLog();
}
