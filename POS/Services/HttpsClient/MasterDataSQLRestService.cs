using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using POS.Core.Helpers;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.CounterSession;
using POS.Core.ViewModels.MainViewModelDTO;

namespace POS.Services.HttpsClient;
public class MasterDataSQLRestService : IMasterDataSQLRestService
{

    readonly HttpClient _client;

    readonly JsonSerializerOptions _serializerOptions;
    readonly IHttpsClientHandlerService _httpsClientHandlerService;
    readonly IDataServiceFactory _context;
    readonly IMasterDataRestService _ImasterDataRestService;

    public MasterDataSQLRestService(IHttpsClientHandlerService service, IDataServiceFactory context, IMasterDataRestService imasterDataRestService)
    {
#if DEBUG
        _httpsClientHandlerService = service;
        HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();
        if (handler != null)
            _client = new HttpClient(handler);
        else
            _client = new HttpClient();
#else
        _client = new HttpClient();
#endif
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        _context = context;
        _ImasterDataRestService = imasterDataRestService;
    }



    //...................section for Items.........................
    public async Task<List<Item>> GetSQLAllItems(long AccountId, long BranchId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            var totalItemCount = dataService.GetTotalItemCounts(AccountId, BranchId);

            var pageNumber = 0;

            if (totalItemCount > 0)
            {
                await _ImasterDataRestService.DeleteItemsToDatabase();
            }

            while (totalItemCount > (pageNumber * 15000))
            {
                var skipIndex = pageNumber * 15000;
                var CreateResponse = await dataService.GetSQLAllItems(AccountId, BranchId, skipIndex);
                var response = await SaveItemToLocalDatabase(CreateResponse);
                pageNumber++;
            }
            return null;
        }
    }
    public async Task<MessageHelper> SaveItemToLocalDatabase(List<Item> items)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreateItemAsync(items);
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    //section end for items..................................................






    //..................section for partner.................................
    public async Task<List<Item>> GetSQLAllPartner(long AccountId, long BranchId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            var totalItemCount = dataService.GetTotalPartnerCounts(AccountId, BranchId);
            if (totalItemCount > 0)
            {
                await _ImasterDataRestService.DeletePartnerToDatabase();
            }
            var pageNumber = 0;

            while (totalItemCount > (pageNumber * 15000))
            {
                var skipIndex = pageNumber * 15000;
                var CreateResponse = await dataService.GetSQLAllPartners(AccountId, BranchId, skipIndex);
                var response = await SavePartnerToLocalDatabase(CreateResponse);
                pageNumber++;
            }
            return null;
        }
    }
    public async Task<List<Partner>> SaveSQLPartner(List<Partner> partner)
    {

        using (var dataService = _context.CreateSQLDataService())
        {
            List<PartnerForSqlBD> lsPartner = new List<PartnerForSqlBD>();
            List<Partner> lPartner = new List<Partner>();
            var check = await dataService.GetPartnerAsyncSql(partner.FirstOrDefault().MobileNo, partner.FirstOrDefault().AccountId.Value);
            if (check == null)
            {
                foreach (var item in partner)
                {
                    lsPartner.Add(new PartnerForSqlBD()
                    {
                        PartnerId = item.PartnerId,
                        PartnerName = item.PartnerName,
                        PartnerCode = item.PartnerCode,
                        NID = item.NID,
                        PartnerTypeId = item.PartnerTypeId,
                        PartnerTypeName = item.PartnerTypeName,
                        TaggedEmployeeId = item.TaggedEmployeeId,
                        TaggedEmployeeName = item.TaggedEmployeeName,
                        Address = item.Address,
                        City = item.City,
                        Email = item.Email,
                        MobileNo = item.MobileNo,
                        AccountId = item.AccountId,
                        BranchId = item.BranchId,
                        AdvanceBalance = item.AdvanceBalance,
                        CreditLimit = item.CreditLimit,
                        ActionById = item.ActionById,
                        ActionByName = item.ActionByName,
                        ActionTime = item.ActionTime,
                        isActive = item.isActive,
                        OtherContactNumber = item.OtherContactNumber,
                        OtherContactName = item.OtherContactName,
                        PartnerBalance = item.PartnerBalance,
                        PartnerGroupId = item.PartnerGroupId,
                        PartnerGroupName = item.PartnerGroupName,
                        PriceTypeId = item.PriceTypeId,
                        PriceTypeName = item.PriceTypeName,
                        BinNumber = item.BinNumber,
                        IsForeign = item.IsForeign,
                        TerritoryId = item.TerritoryId,
                        DistrictId = item.DistrictId,
                        ThanaId = item.ThanaId,
                        Points = item.Points,
                        PointsAmount = item.PointsAmount,
                    });
                }
                var par = await dataService.CreatePartnerAsyncsql(lsPartner);
                if (par.Count > 0)
                {
                    foreach (var item in par)
                    {
                        lPartner.Add(new Partner()
                        {
                            PartnerId = item.PartnerId,
                            PartnerName = item.PartnerName,
                            PartnerCode = item.PartnerCode,
                            NID = item.NID,
                            PartnerTypeId = item.PartnerTypeId,
                            PartnerTypeName = item.PartnerTypeName,
                            TaggedEmployeeId = item.TaggedEmployeeId,
                            TaggedEmployeeName = item.TaggedEmployeeName,
                            Address = item.Address,
                            City = item.City,
                            Email = item.Email,
                            MobileNo = item.MobileNo,
                            AccountId = item.AccountId,
                            BranchId = item.BranchId,
                            AdvanceBalance = item.AdvanceBalance,
                            CreditLimit = item.CreditLimit,
                            ActionById = item.ActionById,
                            ActionByName = item.ActionByName,
                            ActionTime = item.ActionTime,
                            isActive = item.isActive,
                            OtherContactNumber = item.OtherContactNumber,
                            OtherContactName = item.OtherContactName,
                            PartnerBalance = item.PartnerBalance,
                            PartnerGroupId = item.PartnerGroupId,
                            PartnerGroupName = item.PartnerGroupName,
                            PriceTypeId = item.PriceTypeId,
                            PriceTypeName = item.PriceTypeName,
                            BinNumber = item.BinNumber,
                            IsForeign = item.IsForeign,
                            TerritoryId = item.TerritoryId,
                            DistrictId = item.DistrictId,
                            ThanaId = item.ThanaId,
                            Points = item.Points,
                            PointsAmount = item.PointsAmount,
                            //IsSync = true
                        });
                    }
                    var ds = await SavePartnerToLocalDb(lPartner);
                    lPartner = new List<Partner>();
                    lPartner.AddRange(ds);
                }
            }
            return lPartner;
        }
    }
    public async Task<List<Partner>> SavePartnerToLocalDb(List<Partner> partners)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreatePartnerAsync(partners);
            return CreateResponse;
        }
    }
    public async Task<MessageHelper> SavePartnerToLocalDatabase(List<Partner> partners)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreatePartnerAsync(partners);
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    //..................section for partner.................................








    //section for Warehouse...................................................
    public async Task<List<Item>> GetSQLAllWarehouses(long AccountId, long BranchId, long OfficeId, long WareHouseId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            var totalItemCount = dataService.GetTotalWarehouseCounts(AccountId, BranchId, OfficeId, WareHouseId);

            var pageNumber = 0;

            while (totalItemCount > (pageNumber * 5000))
            {
                var skipIndex = pageNumber * 5000;
                var CreateResponse = await dataService.GetSQLAllWarehouses(AccountId, BranchId, skipIndex, OfficeId, WareHouseId);
                var response = await SaveWarehouseToLocalDatabase(CreateResponse);
                pageNumber++;
            }
            return null;
        }
    }
    public async Task<MessageHelper> SaveWarehouseToLocalDatabase(List<Warehouse> warehouses)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreateWareHouseAsync(warehouses);
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    //section end for warehouse................................................






    //..............item selling price.........................................
    public async Task<List<Item>> GetSQLAllItemSellingPrices(long AccountId, long BranchId, long OfficeId, long WareHouseId)
    {
        using (var dataService = _context.CreateDataService())
        {
            dataService.DeleteItemSellingPriceRowsync();
        }

        using (var dataService = _context.CreateSQLDataService())
        {

            var totalItemCount = dataService.GetTotalSellingPriceCounts(AccountId, BranchId, OfficeId, WareHouseId);

            var pageNumber = 0;

            while (totalItemCount > (pageNumber * 5000))
            {
                var skipIndex = pageNumber * 5000;
                var CreateResponse = await dataService.GetSQLAllItemSellingPrice(AccountId, BranchId, skipIndex, OfficeId, WareHouseId);
                var response = await SaveItemSellingPriceToLocalDatabase(CreateResponse);
                pageNumber++;
            }
            return null;
        }
    }
    public async Task<MessageHelper> SaveItemSellingPriceToLocalDatabase(List<ItemSellingPriceRow> itemSellingPrice)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreateItemSellingPriceRowsync(itemSellingPrice);
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    //..............item selling pirce.........................................

    //............. Session.............


    public async Task<MessageHelper> CreateSessionDetails()
    {

        using (var dataService = _context.CreateSQLDataService())
        {
            var dt = await GetSessionDetails();
            var details = await dataService.CreateSqlSessionDetails(dt);
            var update = await UpdateCounterSession(dt);
            return new MessageHelper()
            {
                StatusCode = 200
            };
        }
    }
    public async Task<List<CreateCounterSessionDetails>> GetSessionDetails()
    {
        using (var dataService = _context.CreateDataService())
        {
            var session = await dataService.GetSqlSessionDetails();
            return session;
        }

    }
    public async Task<MessageHelper> UpdateCounterSession(List<CreateCounterSessionDetails> edit)
    {
        using (var dataService = _context.CreateDataService())
        {
            var session = await dataService.UpdateCounterSession(edit);
            return new MessageHelper()
            {
                StatusCode = 200
            };
        }
    }

    //............. Session.............


    //...................item stock balance warehouse............................
    public async Task<List<Item>> GetSQLAllItemBalanceWarehouse(long AccountId, long BranchId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            var totalItemCount = dataService.GetTotalWarehouseBalanceItemCounts(AccountId, BranchId);

            var pageNumber = 0;

            while (totalItemCount > (pageNumber * 5000))
            {
                var skipIndex = pageNumber * 5000;
                var CreateResponse = await dataService.GetSQLAllItemWarehouseBalance(AccountId, BranchId, skipIndex);
                var response = await SaveItemBalanceWarehouseToLocalDatabase(CreateResponse);
                pageNumber++;
            }
            return null;
        }
    }
    public async Task<MessageHelper> SaveItemBalanceWarehouseToLocalDatabase(List<ItemWarehouseBalance> itemSellingPrice)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreateItemWarehouseBalance(itemSellingPrice);
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    //...................item stock balance warehouse............................



    //..................Wallet information from SQL server....................................
    public async Task<MessageHelper> GetWalletInformationfromSQLServer(long AccountId, long BranchId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            var CreateResponse = await dataService.GetWalletInformationfromSQLServer(AccountId, BranchId);
            var response = await SaveWalletInformationtoSQLiteServer(CreateResponse);
            return response;
        }
    }

    public async Task<MessageHelper> SaveWalletInformationtoSQLiteServer(List<SalesWallet> salesWallets)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.SaveWalletInformationtoSQLiteServer(salesWallets);
            return CreateResponse;
        }
    }
    //..................Wallet information from SQL server....................................



    //..................Promotion Row information from SQL Server..............................
    public async Task<MessageHelper> GetPromotionRowfromSQLServer(long AccountId, long BranchId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            DateTime dateTime = DateTime.Now.BD();
            var CreateResponse = await dataService.GetPromotionRowfromSQLServer(AccountId, BranchId, dateTime);
            var response = await SavePromotionRowfromSQLServer(CreateResponse);
            //return response;
        }



        using (var dataService = _context.CreateSQLDataService())
        {
            DateTime dateTime = DateTime.Now.BD();
            var CreateResponse = await dataService.GetItemOfferRowfromSQLServer(AccountId, BranchId, dateTime);
            var response = await SaveItemPointOfferRowfromSQLServer(CreateResponse);
            return response;
        }

    }

    public async Task<MessageHelper> GetPromotionRowSyncSQLServer(long AccountId, long BranchId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            //try { 
            DateTime dateTime = DateTime.Now.BD();
            var CreateResponse = await dataService.GetPromotionRowfromSQLServer(AccountId, BranchId, dateTime);
            var response = await SavePromotionRowSyncSQLServer(CreateResponse);
            return response;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
    }
    public async Task<MessageHelper> SavePromotionRowSyncSQLServer(List<tblPromotionRow> promotionrowData)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreatePromotionRowfromSQLServer(promotionrowData);
            return CreateResponse;
        }
    }
    public async Task<MessageHelper> SavePromotionRowfromSQLServer(List<tblPromotionRow> promotionrowData)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreatePromotionRowfromSQLServer(promotionrowData);
            return CreateResponse;
        }
    }




    public async Task<MessageHelper> SaveItemPointOfferRowfromSQLServer(List<tblPointOfferRow> promotionrowData)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.SaveItemPointOfferRowfromSQLServer(promotionrowData);
            return CreateResponse;
        }
    }
    //..................Promotion Row information from SQL Server..............................






    //..................item selling price update to sq lite database.........................
    public async Task<MessageHelper> ItemSellingPriceRowChange(long AccountId, long BranchId, long intWarehouseId)
    {

        using (var dataService = _context.CreateSQLDataService())
        {
            try
            {
                var CreateResponse = await dataService.ItemSellingPriceRowChange(AccountId, BranchId, intWarehouseId);
                if (CreateResponse.Count > 0)
                {
                    var itemList = CreateResponse.Select(n => new ItemSellingPriceRow
                    {
                        RowId = n.RowId,
                        HeaderId = n.HeaderId,
                        ItemId = n.ItemId,
                        ItemCode = n.ItemCode,
                        OldPrice = n.OldPrice,
                        NewPrice = n.NewPrice,
                        Qty = n.Qty,
                        IsActive = n.IsActive,
                        ActionById = n.ActionById,
                        LastActionDatetime = n.LastActionDatetime,
                    }).ToList();

                    var response = await UpdateItemSellingPriceToLocalDatabase(itemList);
                }

            }
            catch
            {

            }
        }
        return new MessageHelper() { StatusCode = 200 };

    }
    public async Task<MessageHelper> ItemRowChange(long AccountId, long BranchId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            try
            {
                var itemRowList = await dataService.ItemRowChange(AccountId, BranchId);
                if (itemRowList.Count > 0)
                {
                    var response = await UpdateOrCreateItemToLocalDatabase(itemRowList);
                }
            }
            catch
            {

            }
        }
        return new MessageHelper() { StatusCode = 200 };

    }

 
    public async Task<MessageHelper> PartnerRowChange(long AccountId, long BranchId)
    {

        using (var dataService = _context.CreateSQLDataService())
        {

            var partnerList = await dataService.PartnerRowChange(AccountId, BranchId);
            if (partnerList.Count > 0)
            {
                var response = await UpdateOrCreatePartnerToLocalDatabase(partnerList);
            }
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    public async Task<MessageHelper> UpdateItemSellingPriceToLocalDatabase(List<ItemSellingPriceRow> itemSellingPrice)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.UpdateItemSellingPriceRowsync(itemSellingPrice);
            return new MessageHelper() { StatusCode = 200 };
        }
    }

    public async Task<MessageHelper> UpdateOrCreateItemToLocalDatabase(List<Item> item)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.UpdateOrCrateItemRowsync(item);
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    public async Task<MessageHelper> UpdateOrCreatePartnerToLocalDatabase(List<Partner> item)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.UpdateOrCratePartnerRowsync(item);
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    //..................item selling price update to sq lite database.........................





    //..................item delivery invoice update to sql server database.........................
    public async Task<List<CreateSalesDeliveryDTO>> GetSalesDeliveryInformationfromSQLite(long accountId, long branchId, int takeItems)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.GetSalesDeliveryInformationfromSQLite(accountId, branchId, takeItems);
            return CreateResponse;
        }
    }

    public async Task<MessageHelper> CreateSalesDeliveryInformationIntoSQLServer(List<CreateSalesDeliveryDTO> obj, TblSettings settings)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            foreach (var singleInvoice in obj)
            {
                singleInvoice.IsOnline = false;
                var CreateResponse = await dataService.CreateSalesDeliveryInformationIntoSQLServer(singleInvoice, settings);

                if (CreateResponse.StatusCode == 200)
                {
                    //singleInvoice.pOSSalesDeliveryHeader.UserId = AppSettings.UserId;
                    singleInvoice.pOSSalesDeliveryHeader.IsSync = 1;
                    await _ImasterDataRestService.SaveItemIntoSalesDelivery(singleInvoice);
                }
                else
                {
                    //singleInvoice.pOSSalesDeliveryHeader.UserId = AppSettings.UserId;
                    singleInvoice.pOSSalesDeliveryHeader.IsSync = 0;
                    await _ImasterDataRestService.SaveItemIntoSalesDelivery(singleInvoice);
                }
                //return CreateResponse;
            }
            return new MessageHelper() { StatusCode = 200 };
        }
    }

    //..................item delivery invoice update to sql server database.........................


    //..................Special Discount information from SQL Server..............................
    public async Task<MessageHelper> GetSpecialDiscountfromSQLServer(long AccountId, long BranchId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            DateTime dateTime = DateTime.Now.BD();
            var CreateResponse = await dataService.GetSpecialDiscountfromSQLServer(AccountId, BranchId, dateTime);
            var response = await SaveSpecialDiscountfromSQLServer(CreateResponse);
            return response;
        }
    }

    public async Task<MessageHelper> GetSpecialDiscountSyncSQLServer(long AccountId, long BranchId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            DateTime dateTime = DateTime.Now.BD();
            var CreateResponse = await dataService.GetSpecialDiscountfromSQLServer(AccountId, BranchId, dateTime);
            var response = await SaveSpecialDiscountSyncQLServer(CreateResponse);
            return response;
        }
    }
    public async Task<MessageHelper> SaveSpecialDiscountSyncQLServer(List<POSSpecialDiscount> specialDiscount)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.SaveSpecialDiscountSyncSQLServer(specialDiscount);
            return CreateResponse;
        }
    }

    public async Task<MessageHelper> SaveSpecialDiscountfromSQLServer(List<POSSpecialDiscount> specialDiscount)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.SaveSpecialDiscountfromSQLServer(specialDiscount);
            return CreateResponse;
        }
    }
    //..................Special Discount information from SQL Server..............................





    public async Task<MainViewRecallInvoiceDTO> GetSalesDeliveryInformationFromSQLServer(string InvoiceCode)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            var ResponseInfo = await dataService.GetSalesDeliveryInformationFromSQLServer(InvoiceCode);
            return ResponseInfo;
        }
    }
    public async Task<MessageHelper> SQLUpdatePosSalesPayment(EditPosSales edit)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            var data = await dataService.SQLEditPosPayment(edit);
            return data;
        }
    }
    public async Task SqlDataLogUpdate(List<SQLtblDataLog> log)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            List<SQLtblDataLog> up = new List<SQLtblDataLog>();
            up = log;
            await dataService.SQLCreatePosLog(log);
            await DataLogUpdate(up);
        }
    }
    public async Task DataLogUpdate(List<SQLtblDataLog> log)
    {
        using (var dataService = _context.CreateDataService())
        {
            await dataService.DataLogUpdate(log);
        }
    }
    public async Task GetUserFromSql(long accountId)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            List<User> dt = await dataService.GetAllSqlUser(accountId);
            await SaveUserToDataServer(dt);
        }
    }
    public async Task SaveUserToDataServer(List<User> us)
    {
        using (var dataService = _context.CreateDataService())
        {
            await dataService.CreateUser(us);
        }
    }
    public async Task<MessageHelper> ChangePassword(ChangePassword change)
    {
        using (var dataService = _context.CreateSQLDataService())
        {
            var dt  = await dataService.ChangePasswordSQL(change);
        }
        using (var dataService = _context.CreateDataService())
        {
            var ds  = await dataService.ChangePassword(change);
        }
        return new MessageHelper()
        {
            Message = "Password Change Successfully !",
            StatusCode = 200
        };
    }
}

