using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.MainViewModelDTO;

namespace POS.Services.HttpsClient;
public interface IMasterDataSQLRestService
{
    Task<List<Item>> GetSQLAllItems(long AccountId, long BranchId);
    Task<List<Item>> GetSQLAllWarehouses(long AccountId, long BranchId, long OfficeId, long WareHouseId);
    Task<List<Partner>> SaveSQLPartner(List<Partner> partner);
    public Task<List<Item>> GetSQLAllPartner(long AccountId, long BranchId);
    public Task<List<Item>> GetSQLAllItemSellingPrices(long AccountId, long BranchId, long OfficeId, long WareHouseId);
    public Task<List<Item>> GetSQLAllItemBalanceWarehouse(long AccountId, long BranchId);
    public Task<MessageHelper> ItemSellingPriceRowChange(long AccountId, long BranchId,long intWarehouseId);
    Task<MessageHelper> ItemRowChange(long AccountId, long BranchId);
    Task<MessageHelper> PartnerRowChange(long AccountId, long BranchId);
    public Task<List<CreateSalesDeliveryDTO>> GetSalesDeliveryInformationfromSQLite(long accountId, long branchId, int takeItems);
    public Task<MessageHelper> CreateSalesDeliveryInformationIntoSQLServer(List<CreateSalesDeliveryDTO> obj,TblSettings settings);

    public Task<MessageHelper> GetWalletInformationfromSQLServer(long AccountId, long BranchId);

    public Task<MessageHelper> GetPromotionRowfromSQLServer (long AccountId, long BranchId);
    Task<MessageHelper> GetPromotionRowSyncSQLServer(long AccountId, long BranchId);

    public Task<MessageHelper> GetSpecialDiscountfromSQLServer(long AccountId, long BranchId);
    Task<MessageHelper> GetSpecialDiscountSyncSQLServer(long AccountId, long BranchId);
    public Task<MainViewRecallInvoiceDTO> GetSalesDeliveryInformationFromSQLServer(string InvoiceCode);
    Task<MessageHelper> CreateSessionDetails();
    Task<MessageHelper> SQLUpdatePosSalesPayment(EditPosSales edit);
    Task SqlDataLogUpdate(List<SQLtblDataLog> log);
    Task GetUserFromSql(long accountId);
    Task<MessageHelper> ChangePassword(ChangePassword change);
    //Task<List<Item>> GetSQLAllPartner(long AccountId, long BranchId);
    //Task<List<Item>> GetSQLAllItemSellingPrices(long AccountId, long BranchId, long OfficeId, long WareHouseId);
    //Task<List<Item>> GetSQLAllItemBalanceWarehouse(long AccountId, long BranchId);
    //Task<MessageHelper> ItemSellingPriceRowChange(long AccountId, long BranchId);
    //Task<List<CreateSalesDeliveryDTO>> GetSalesDeliveryInformationfromSQLite(long accountId, long branchId, int takeItems);
    //Task<MessageHelper> CreateSalesDeliveryInformationIntoSQLServer(List<CreateSalesDeliveryDTO> obj, TblSettings settings);
    //Task<MessageHelper> GetWalletInformationfromSQLServer(long AccountId, long BranchId);
    //Task<MessageHelper> GetPromotionRowfromSQLServer(long AccountId, long BranchId);
    //Task<MessageHelper> GetSpecialDiscountfromSQLServer(long AccountId, long BranchId);
    //Task<CreateSalesDeliveryDTO> GetSalesDeliveryInformationFromSQLServer(string InvoiceCode);
}
