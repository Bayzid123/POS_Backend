using POS.Core.Models;
using POS.Core.ViewModels.CounterSession;
using POS.Core.ViewModels;
using POS.Core.ViewModels.CounterDTO;
using POS.Core.ViewModels.MainViewModelDTO;

namespace POS.Core.IService;

public interface IDataService : IDisposable
{
    Task<TblUser> CreateUserAsync(TblUser user);
    public Task<List<ItemWarehouseBalance>> CreateItemWarehouseBalance(List<ItemWarehouseBalance> items);
    Task<TblUser> GetUserAsync(string userName);
    Task<TblUser> GetUserAsync(long userId);
    Task<TblSettings> GetSettings();
    Task UpdateSetting(TblSettings settings);
    TblSettings GetSettingsLocal();
    Task<MessageHelper> CreateSettings(TblSettings settings);
    Task<bool> AdminLogin(string UserId, string Password);
    Task<TblUser> GetUser(string userName, string password);
    Task<MessageHelper> CreatePosCounterAsync(CreatePoscounterDTO counter);
    Task<POSCounter> GetCounterAsync(long Id);

    Task<MessageHelper> CreatePosCounterSessionAsync(CreateCounterSeason create);
    Task<CounterSession> GetPosCounterSession();
    Task<List<GetCounterSessionDetailsDTO>> GetPosCounterSessionDetails(long counterSessionId);
    Task<List<SalesInvoiceDTO>> GetSalesInvoice(long UserId, bool isSyn);
    public Task<List<SalesInvoiceDTO>> GetSalesInvoiceLiveServer(long UserId, bool isSyn,string SalesInvoice);
    Task<POSSalesDeliveryHeader> GetPosDeliveryHeader(string invoice);
    public Task<SQLServerPOSSalesDeliveryHeader> GetPosLiveDeliveryHeader(string invoice);
    Task<List<POSSalesDeliveryLine>> GetPosDeliveryLine(long salesOrderId);
    Task<List<POSSalesDeliveryLine>> GetPosLiveDeliveryLine(long salesOrderId);

    public Task<List<POSSalesDeliveryLineDTO>> GetPosLiveDeliveryLineItem(long salesOrderId);

    Task<List<POSSalesDeliveryLineDTO>> GetPosDeliveryLineItem(long salesOrderId);
    Task<List<POSSalesPaymentDTO>> GetSalesPayment(long salesOrderId);

    Task<List<POSSalesPaymentDTO>> GetLiveSalesPayment(long salesOrderId);

    Task<List<Warehouse>> CreateWareHouseAsync(List<Warehouse> warehouse);
    Task<Warehouse> GetWareHouseAsync(long Id);
    Task<List<Item>> CreateItemAsync(List<Item> item);
    Task<int> DeleteItemAsync();
    Task<int> DeletePartnerAsync();
    Task<List<Item>> UpdateItemAsync(List<Item> item);
    Task<Item> GetItemAsync(long Id);
    Task<List<Item>> GetEditedItemIds(List<long> ids);
    Task<ItemSellingPriceRow> ItemSellingPriceRowsAsync(long Id);
    Task<Partner> GetPartner(long? partnerId);
    Task<List<ItemSellingPriceRow>> CreateItemSellingPriceRowsync(List<ItemSellingPriceRow> itemSellingPriceRow);
    Task<List<ItemSellingPriceRow>> UpdateItemSellingPriceRowsync(List<ItemSellingPriceRow> itemSellingPriceRow);
    Task<List<Item>> UpdateOrCrateItemRowsync(List<Item> item);
    Task<List<Partner>> UpdateOrCratePartnerRowsync(List<Partner> partner);
    Task<List<PartnerForSqlBD>> CreatePartnerAsyncsql(List<PartnerForSqlBD> partners);
    Task<List<Partner>> CreatePartnerAsync(List<Partner> partners);
    Task<Partner> GetPartnerAsync(string strPartnerCode,long AccountId);
    Task<Partner> GetPartnerIdAsync(long PartnerId, long AccountId);
    Task<PartnerForSqlBD> GetPartnerAsyncSql(string mobileNo,long accountId);
    Task<Item> GetItemByBarCode(string ItemBarCode);
    Task<Item> GetSQLItemByBarCode(string ItemBarCode, long AccountId);
    Task<List<Item>> GetItemListByItemName(string ItemName);
    Task<List<Item>> GetSQLItemListByItemName(string ItemName, long WarehouseId, long AccountId);
    Task<List<Item>> GetItemListByBarCode(string ItemBarCode);
    Task<List<Item>> GetSQLItemListByBarCode(string ItemBarCode, long WarehouseId, long AccountId);
    Task<List<Item>> GetMultipleSalesPrizeItemListByBarCode(string ItemBarCode);
    Task<List<Item>> GetSQLMultipleSalesPrizeItemListByBarCode(string ItemBarCode, long WarehouseId,long AccountId);
    //Task<List<Item>> GetSQLMultipleSalesPrizeItemListByBarCode(string ItemBarCode);
    Task<List<Item>> GetItemByItemIDs(List<long> ids);
    Task<List<Item>> GetSQLItemByItemIDs(List<long> ids, long AccountId);
    public Task<Item> GetStockQtyCheckItemByItemID(long Id, decimal salesRate);
    Task<Item> GetSQLStockQtyCheckItemByItemID(long Id, decimal salesRate, long accountId, long branchId, long warehouseId);
    Task<CreateSalesDeliveryDTO> SaveItemIntoSalesDeliveryLines(CreateSalesDeliveryDTO objCreate);
    Task<MessageHelper> SQLEditPosPayment(EditPosSales edit);
    Task<MessageHelper> EditPosPayment(EditPosSales edit);
    Task<List<PaymentWalletDTO>> GetPaymentWalletList();
    Task<List<OtherDiscountDTO>> GetOtherDiscountList();
    Task<PaymentWalletDTO> GetPaymentWalletbyId(long WalletId);
    Task<MainViewRecallInvoiceDTO> RecallInvoiceInformation(long userId, long customerId);
    Task<List<POSSalesDeliveryHeader>> RecallInvoiceInformation(long userId);
    public Task<List<CreateSalesDeliveryDTO>> SalesInformationUsingIDs(List<long> SalesOrderIds);
    Task<MainViewRecallInvoiceDTO> RecallInvoice(long userId, long SalesDeliveryId, List<Item> Items);
    Task<List<POSSalesDeliveryLine>> RecallInvoiceRow(long userId, long SalesDeliveryId);
    long GetPOSSalesDeliveryHeaderInfo(long CounterId);
    long GetSQLServerSalesDeliveryHeaderInfo(long CounterId);
    Task<POSSalesDeliveryHeader> GetPOSSalesDeliveryHeader(string InvoiceCode);
    Task<List<PaymentWalletDTO>> GetPaymentWalletbyIds(List<long> WalletId);
    void RemoveDuplicateItems();
    Task CreatePosLog(tblDataLog log);






    #region sql server......
    public int GetTotalItemCounts(long AccountId, long BranchId);
    public Task<List<Item>> GetSQLAllItems(long AccountId, long BranchId, int skipIndex);
    public int GetTotalWarehouseCounts(long AccountId, long BranchId, long OfficeId, long WareHouseId);
    public Task<List<Warehouse>> GetSQLAllWarehouses(long AccountId, long BranchId, int skipIndex, long OfficeId, long WareHouseId);
    public int GetTotalPartnerCounts(long AccountId, long BranchId);
    public Task<List<Partner>> GetSQLAllPartners(long AccountId, long BranchId, int skipIndex);
    public int GetTotalSellingPriceCounts(long AccountId, long BranchId, long OfficeId, long WareHouseId);
    public Task<List<ItemSellingPriceRow>> GetSQLAllItemSellingPrice(long AccountId, long BranchId, int skipIndex, long OfficeId, long WareHouseId);
    public int GetTotalWarehouseBalanceItemCounts(long AccountId, long BranchId);
    public Task<List<ItemWarehouseBalance>> GetSQLAllItemWarehouseBalance(long AccountId, long BranchId, int skipIndex);
    public Task<List<SQLServerItemSellingPriceRow>> ItemSellingPriceRowChange(long AccountId, long BranchId,long intWarehouseId);
    Task<List<Item>> ItemRowChange(long AccountId, long BranchId);
    Task<List<Partner>> PartnerRowChange(long AccountId, long BranchId);
    #endregion SQL Server....


    Task<List<CreateSalesDeliveryDTO>> GetSalesDeliveryInformationfromSQLite(long accountId, long branchId, int takeItems);
    Task<MessageHelper> CreateSalesDeliveryInformationIntoSQLServer(CreateSalesDeliveryDTO obj, TblSettings settings);
    Task<List<SalesWallet>> GetWalletInformationfromSQLServer(long AccountId, long BranchId);
    Task<MessageHelper> SaveWalletInformationtoSQLiteServer(List<SalesWallet> walletList);
    Task<List<tblPromotionRow>> GetPromotionRowfromSQLServer(long AccountId, long BranchId, DateTime dateTime);

    Task<List<tblPointOfferRow>> GetItemOfferRowfromSQLServer(long AccountId, long BranchId, DateTime dateTime);
    public Task<MessageHelper> SaveItemPointOfferRowfromSQLServer(List<tblPointOfferRow> pointOfferRow);
    Task<MessageHelper> CreatePromotionRowfromSQLServer(List<tblPromotionRow> promotionRows);
    Task<MessageHelper> CreatePromotionRowSyncSQLServer(List<tblPromotionRow> promotionRows);
    Task<MessageHelper> DeleteSessionData();
    public Task<List<POSSpecialDiscount>> GetSpecialDiscountfromSQLServer(long accountId, long branchId, DateTime dateTime);
    public Task<MessageHelper> SaveSpecialDiscountfromSQLServer(List<POSSpecialDiscount> walletList);
    Task<MessageHelper> SaveSpecialDiscountSyncSQLServer(List<POSSpecialDiscount> specialDiscount);
    public Task<MainViewRecallInvoiceDTO> GetSalesDeliveryInformationFromSQLServer(string InvoiceCode);
  
    Task<MessageHelper> CreateSqlSessionDetails(List<CreateCounterSessionDetails> create);
    Task<MessageHelper> UpdateCounterSession(List<CreateCounterSessionDetails> edit);
    Task<List<CreateCounterSessionDetails>> GetSqlSessionDetails();
    Task<CreateSalesDeliveryDTO> SaveItemIntoSalesDelivery(CreateSalesDeliveryDTO objCreate);
    void DeleteItemSellingPriceRowsync();
    Task SQLCreatePosLog(List<SQLtblDataLog> log);
    Task DeleteDataLog();
    Task DataLogUpdate(List<SQLtblDataLog> log);
    Task<List<tblDataLog>> GetDataLog();
    Task<MessageHelper> DeleteRecallInvoice(long SalesOrderId, string SalesOrderCode);
    Task<List<User>> GetAllSqlUser(long accountId);
    Task CreateUser(List<User> us);
    Task<List<tblPointOfferRow>> GetPointsOfferRowsByItemIds(List<long> itemIds);
    Task<TblUser> ChangePassword(ChangePassword change);
    Task<User> ChangePasswordSQL(ChangePassword change);
}
