
using System.Diagnostics;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using POS.Core.Helpers;
using POS.Core.IService;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.CounterDTO;
using POS.Core.ViewModels.CounterSession;
using POS.Core.ViewModels.MainViewModelDTO;
using POS.Core.ViewModels.SalesDeliveryDTO;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Threading;
using System;
using SQLitePCL;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace POS.Core;

public abstract partial class DataServiceBase : IDataService, IDisposable
{
    private IDataSource _dataSource = null;

    public DataServiceBase(IDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    #region                         -----User----

    //  Create Or Edit User
    public async Task<TblUser> CreateUserAsync(TblUser user)
    {
        if (user.intUserID != 0)
        {
            _dataSource.Entry(user).State = EntityState.Modified;
        }
        else
        {
            _dataSource.Entry(user).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();
        return user;
    }
    public async Task<List<ItemWarehouseBalance>> CreateItemWarehouseBalance(List<ItemWarehouseBalance> items)
    {
        foreach (var item in items)
        {
            var dt = await _dataSource.ItemBalance.Where(x => x.ItemWarehouseBalanceId == item.ItemWarehouseBalanceId).AsNoTracking().FirstOrDefaultAsync();

            if (dt == null)
            {
                _dataSource.Entry(item).State = EntityState.Added;
            }
            else
            {
                _dataSource.Entry(item).State = EntityState.Modified;
            }
        }
        await _dataSource.SaveChangesAsync();

        return items;
    }
    // Get User
    public async Task<TblUser> GetUserAsync(string userName)
    {
        return await _dataSource.Users.Where(x => x.strUserName == userName).AsNoTracking().FirstOrDefaultAsync();
    }
    public async Task<TblUser> GetUserAsync(long userId)
    {
        return await _dataSource.Users.Where(x => x.intUserID == userId).AsNoTracking().FirstOrDefaultAsync();
    }
    public async Task<TblSettings> GetSettings()
    {
        return await _dataSource.Settings.FirstOrDefaultAsync();
    }
    public async Task UpdateSetting(TblSettings settings)
    {
        _dataSource.Entry(settings).State = EntityState.Modified;
        await _dataSource.SaveChangesAsync();
    }
    public TblSettings GetSettingsLocal()
    {
        return _dataSource.Settings.FirstOrDefault();
    }
    public async Task<bool> AdminLogin(string UserId, string Password)
    {
        try
        {
            bool isUser = false;
            var check = await _dataSource.Users.Where(x => x.strUserName == UserId && x.strPassword == Password && x.IsAdministration == true).FirstOrDefaultAsync();
            if (check != null)
            {
                isUser = true;
            }
            else
            {
                isUser = false;
            }
            return isUser;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<TblUser> GetUser(string userName, string password)
    {
        return await _dataSource.Users.Where(x => x.strUserName == userName && x.strPassword == password).AsNoTracking().FirstOrDefaultAsync();
    }


    public async Task<List<Warehouse>> CreateWareHouseAsync(List<Warehouse> warehouse)
    {
        await _dataSource.Warehouses.ExecuteDeleteAsync();
        foreach (var singleWareHouse in warehouse)
        {
            _dataSource.Entry(singleWareHouse).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();
        return warehouse;
    }
    public async Task<Warehouse> GetWareHouseAsync(long Id)
    {
        return await _dataSource.Warehouses.Where(x => x.WarehouseId == Id).FirstOrDefaultAsync();
    }
    public async Task<List<Item>> CreateItemAsync(List<Item> item)
    {
        //await _dataSource.Items.ExecuteDeleteAsync();
        foreach (var singleItem in item)
        {
            _dataSource.Entry(singleItem).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();
        return item;
    }
    public async Task<List<Item>> UpdateItemAsync(List<Item> item)
    {
        foreach (var singleItem in item)
        {
            _dataSource.Entry(singleItem).State = EntityState.Modified;
        }
        await _dataSource.SaveChangesAsync();
        return item;
    }
    public async Task<Item> GetItemAsync(long Id)
    {
        return await _dataSource.Items.Where(x => x.ItemId == Id).FirstOrDefaultAsync();
    }

    public async Task<Item> GetStockQtyCheckItemByItemID(long Id, decimal salesRate)
    {
        var response = await (from itm in _dataSource.Items
                              join sellingPrice in _dataSource.SellingPriceRows on itm.ItemId equals sellingPrice.ItemId into g
                              from sellingPrice in g.DefaultIfEmpty()
                              where itm.ItemId == Id
                              select new Item
                              {
                                  ItemId = itm.ItemId,
                                  ItemGlobalId = itm.ItemGlobalId,
                                  ItemName = itm.ItemName,
                                  ItemCode = itm.ItemCode,
                                  Barcode = itm.Barcode,
                                  ItemTypeId = itm.ItemTypeId,
                                  ItemTypeName = itm.ItemTypeName,
                                  ItemCategoryId = itm.ItemCategoryId,
                                  ItemCategoryName = itm.ItemCategoryName,
                                  ItemSubCategoryId = itm.ItemSubCategoryId,
                                  ItemSubCategoryName = itm.ItemSubCategoryName,
                                  AccountId = itm.AccountId,
                                  AccountName = itm.AccountName,
                                  BranchId = itm.BranchId,
                                  BranchName = itm.BranchName,
                                  UomId = itm.UomId,
                                  UomName = itm.UomName,
                                  UserId = itm.UserId,
                                  UserName = itm.UserName,
                                  ActionTime = itm.ActionTime,
                                  StartDate = itm.StartDate,
                                  ExpiredDate = itm.ExpiredDate,
                                  Price = itm.Price,
                                  Vat = itm.Vat,
                                  SD = itm.SD,
                                  AvgRate = itm.AvgRate,
                                  TotalQuantity = sellingPrice == null ? 0 : sellingPrice.Qty,
                                  VatPercentage = itm.VatPercentage,
                                  CurrentSellingPrice = sellingPrice == null ? itm.CurrentSellingPrice : sellingPrice.NewPrice,
                                  IsActive = itm.IsActive,
                                  StockLimitQuantity = itm.StockLimitQuantity,
                                  TaxRateId = itm.TaxRateId,
                                  Brand = itm.Brand,
                                  PartNumber = itm.PartNumber,
                                  ItemDescription = itm.ItemDescription,
                                  OriginId = itm.OriginId,
                                  OriginName = itm.OriginName,
                                  StdPurchasePrice = itm.StdPurchasePrice,
                                  AltUomId = itm.AltUomId,
                                  AltUomName = itm.AltUomName,
                                  ConversionUnit = itm.ConversionUnit,
                                  IsSerial = itm.IsSerial,
                                  MaximumDiscountPercent = itm.MaximumDiscountPercent,
                                  IsBatchManage = itm.IsBatchManage,
                                  HSCode = itm.HSCode,
                                  MaximumDiscountAmount = itm.MaximumDiscountAmount,
                                  IsNegativeSales = itm.IsNegativeSales
                              }).ToListAsync();
        var data = response.Where(w => w.CurrentSellingPrice == salesRate).FirstOrDefault();

        return data;
    }


    public async Task<Item> GetSQLStockQtyCheckItemByItemID(long Id, decimal salesRate, long accountId, long branchId, long warehouseId)
    {
        var response = await (from itm in _dataSource.SQLItems
                              join sellingPrice in (from sellingPrice in _dataSource.SQLServerSellingPriceRows
                                                    join header in _dataSource.SQLItemSellingPriceHeaders on sellingPrice.HeaderId equals header.HeaderId
                                                    where header.AccountId == accountId && header.BranchId == branchId && header.WarehouseId == warehouseId
                                                    select sellingPrice) on itm.ItemId equals sellingPrice.ItemId into g
                              from sellingPrice in g.DefaultIfEmpty()
                              where itm.ItemId == Id
                              select new Item
                              {
                                  ItemId = itm.ItemId,
                                  ItemGlobalId = itm.ItemGlobalId,
                                  ItemName = itm.ItemName,
                                  ItemCode = itm.ItemCode,
                                  Barcode = itm.Barcode,
                                  ItemTypeId = itm.ItemTypeId,
                                  ItemTypeName = itm.ItemTypeName,
                                  ItemCategoryId = itm.ItemCategoryId,
                                  ItemCategoryName = itm.ItemCategoryName,
                                  ItemSubCategoryId = itm.ItemSubCategoryId,
                                  ItemSubCategoryName = itm.ItemSubCategoryName,
                                  AccountId = itm.AccountId,
                                  AccountName = itm.AccountName,
                                  BranchId = itm.BranchId,
                                  BranchName = itm.BranchName,
                                  UomId = itm.UomId,
                                  UomName = itm.UomName,
                                  UserId = itm.UserId,
                                  UserName = itm.UserName,
                                  ActionTime = itm.ActionTime.ToString(),
                                  StartDate = itm.StartDate.ToString(),
                                  ExpiredDate = itm.ExpiredDate.ToString(),
                                  Price = itm.Price,
                                  Vat = itm.Vat,
                                  SD = itm.SD,
                                  AvgRate = itm.AvgRate,
                                  TotalQuantity = sellingPrice == null ? 0 : sellingPrice.Qty,
                                  VatPercentage = itm.VatPercentage,
                                  CurrentSellingPrice = sellingPrice == null ? itm.CurrentSellingPrice : sellingPrice.NewPrice,
                                  IsActive = itm.IsActive,
                                  StockLimitQuantity = itm.StockLimitQuantity,
                                  TaxRateId = itm.TaxRateId,
                                  Brand = itm.Brand,
                                  PartNumber = itm.PartNumber,
                                  ItemDescription = itm.ItemDescription,
                                  OriginId = itm.OriginId,
                                  OriginName = itm.OriginName,
                                  StdPurchasePrice = itm.StdPurchasePrice,
                                  AltUomId = itm.AltUomId,
                                  AltUomName = itm.AltUomName,
                                  ConversionUnit = itm.ConversionUnit,
                                  IsSerial = itm.IsSerial,
                                  MaximumDiscountPercent = itm.MaximumDiscountPercent,
                                  IsBatchManage = itm.IsBatchManage,
                                  HSCode = itm.HSCode,
                                  MaximumDiscountAmount = itm.MaximumDiscountAmount,
                                  IsNegativeSales = itm.IsNegativeSales
                              }).ToListAsync();
        var data = response.Where(w => w.CurrentSellingPrice == salesRate).FirstOrDefault();

        return data;
    }

    public async Task<List<Item>> GetItemByItemIDs(List<long> ids)
    {
        return await _dataSource.Items.Where(n => ids.Contains(n.ItemId)).ToListAsync();
    }

    public async Task<List<Item>> GetSQLItemByItemIDs(List<long> ids, long AccountId)
    {
        //var response2 = await _dataSource.SQLItems.Where(n => ids.Contains(n.ItemId)).ToListAsync();

        var response = await _dataSource.SQLItems.Where(n => ids.Contains(n.ItemId)).Select(itm => new Item
        {

            ItemId = itm.ItemId,
            ItemGlobalId = itm.ItemGlobalId,
            ItemName = itm.ItemName,
            ItemCode = itm.ItemCode,
            Barcode = itm.Barcode,
            ItemTypeId = itm.ItemTypeId,
            ItemTypeName = itm.ItemTypeName,
            ItemCategoryId = itm.ItemCategoryId,
            ItemCategoryName = itm.ItemCategoryName,
            ItemSubCategoryId = itm.ItemSubCategoryId,
            ItemSubCategoryName = itm.ItemSubCategoryName,
            AccountId = itm.AccountId,
            AccountName = itm.AccountName,
            BranchId = itm.BranchId,
            BranchName = itm.BranchName,
            UomId = itm.UomId,
            UomName = itm.UomName,
            UserId = itm.UserId,
            UserName = itm.UserName,
            ActionTime = itm.ActionTime.ToString(),
            StartDate = itm.StartDate.ToString(),
            ExpiredDate = itm.ExpiredDate.ToString(),
            Price = itm.Price,
            Vat = itm.Vat,
            SD = itm.SD,
            AvgRate = itm.AvgRate,
            TotalQuantity = 0,
            VatPercentage = itm.VatPercentage,
            CurrentSellingPrice = itm.CurrentSellingPrice,
            IsActive = itm.IsActive,
            StockLimitQuantity = itm.StockLimitQuantity,
            TaxRateId = itm.TaxRateId,
            Brand = itm.Brand,
            PartNumber = itm.PartNumber,
            ItemDescription = itm.ItemDescription,
            OriginId = itm.OriginId,
            OriginName = itm.OriginName,
            StdPurchasePrice = itm.StdPurchasePrice,
            AltUomId = itm.AltUomId,
            AltUomName = itm.AltUomName,
            ConversionUnit = itm.ConversionUnit,
            IsSerial = itm.IsSerial,
            MaximumDiscountPercent = itm.MaximumDiscountPercent,
            IsBatchManage = itm.IsBatchManage,
            HSCode = itm.HSCode,
            MaximumDiscountAmount = itm.MaximumDiscountAmount,
            IsNegativeSales = itm.IsNegativeSales
        }).ToListAsync();

        return response.Where(w => w.AccountId == AccountId).ToList();


    }
    public async Task<List<Item>> GetEditedItemIds(List<long> ids)
    {
        var idList = await _dataSource.Items.Where(n => ids.Contains(n.ItemId)).AsNoTracking().ToListAsync();
        return idList;
    }
    public async Task<List<ItemSellingPriceRow>> CreateItemSellingPriceRowsync(List<ItemSellingPriceRow> itemSellingPriceRow)
    {
        //await _dataSource.SellingPriceRows.ExecuteDeleteAsync();
        foreach (var singleItem in itemSellingPriceRow)
        {
            _dataSource.Entry(singleItem).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();
        return itemSellingPriceRow;
    }

    public async Task<List<tblPointOfferRow>> GetPointsOfferRowsByItemIds(List<long> itemIds)
    {
        var response = await _dataSource.PointOffer.Where(n => itemIds.Contains(n.intItemId) && n.isActive == true).ToListAsync();
        return response;
    }

    public async void DeleteItemSellingPriceRowsync()
    {
        await _dataSource.SellingPriceRows.ExecuteDeleteAsync();

    }

    public async Task<List<ItemSellingPriceRow>> UpdateItemSellingPriceRowsync(List<ItemSellingPriceRow> itemSellingPriceRow)
    {
        var itemIds = itemSellingPriceRow.Select(n => n.ItemId).ToList();
        if (itemIds.Any())
        {
            var duplicates = (await (from se in _dataSource.SellingPriceRows
                                     join i in _dataSource.Items on se.ItemId equals i.ItemId
                                     select new
                                     {
                                         se,
                                         IsMultipleSalesPrice = i.IsMultipleSalesPrice
                                     }).AsNoTracking().ToListAsync());
            var duplicatesFirst = duplicates.Where(w => w.IsMultipleSalesPrice == false).Select(s => s.se).GroupBy(x => new { x.ItemId })
                        .Where(g => g.Count() > 1)
                        .SelectMany(g => g.Skip(1)).ToList();
            if (duplicatesFirst.Any())
            {
                _dataSource.SellingPriceRows.RemoveRange(duplicatesFirst);
                await _dataSource.SaveChangesAsync();
            }

            var duplicatesSecond = duplicates.Where(w => w.IsMultipleSalesPrice == true).OrderByDescending(s => s.se.LastActionDatetime).Select(s => s.se).GroupBy(x => new { x.ItemId, x.NewPrice })
                        .Where(g => g.Count() > 1)
                        .SelectMany(g => g.Skip(1)).ToList();
            if (duplicatesSecond.Any())
            {
                _dataSource.SellingPriceRows.RemoveRange(duplicatesSecond);
                await _dataSource.SaveChangesAsync();
            }

        }

        foreach (var singleItem in itemSellingPriceRow.Distinct())
        {

            //await _dataSource.SellingPriceRows.Where(n => n.ItemId == singleItem.ItemId && n.NewPrice == singleItem.NewPrice).ExecuteDeleteAsync();
            var itemInformation = await _dataSource.SellingPriceRows.Where(n => n.ItemId == singleItem.ItemId && n.NewPrice == singleItem.NewPrice).AsNoTracking().ToListAsync();
            var flag = itemInformation.Where(n => n.ItemId == singleItem.ItemId && n.RowId == singleItem.RowId).FirstOrDefault();



            if (flag == null)
            {
                _dataSource.Entry(singleItem).State = EntityState.Added;
                // await _dataSource.SaveChangesAsync();
            }
            else
            {
                //var flag2 = itemInformation.Where(n => n.ItemId == singleItem.ItemId && n.RowId==singleItem.RowId).FirstOrDefault();
                //if (flag2 == null)
                //{
                flag.NewPrice = singleItem.NewPrice;
                flag.Qty = singleItem.Qty;
                _dataSource.Entry(flag).State = EntityState.Modified;

                //}

            }
        }
        await _dataSource.SaveChangesAsync();
        return itemSellingPriceRow;
    }
    public async Task<List<Item>> UpdateOrCrateItemRowsync(List<Item> item)
    {
        var itemIds = item.Select(n => n.ItemId).ToList();
        if (itemIds.Any())
        {
            var duplicates = _dataSource.Items.AsNoTracking().ToList()
        .GroupBy(x => new { x.ItemId })
        .Where(g => g.Count() > 1)
        .SelectMany(g => g.Skip(1)).ToList();
            if (duplicates.Any())
            {
                _dataSource.Items.RemoveRange(duplicates);
                await _dataSource.SaveChangesAsync(); ;
            }

        }
        //await _dataSource.Items.Where(n => itemIds.Contains(n.ItemId)).ExecuteDeleteAsync();
        var itemInformation = await _dataSource.Items.Where(n => itemIds.Contains(n.ItemId)).AsNoTracking().ToListAsync();
        foreach (var singleItem in item.Distinct())
        {
            var flag = itemInformation.Where(n => n.ItemId == singleItem.ItemId).FirstOrDefault();
            if (flag == null)
            {
                _dataSource.Entry(singleItem).State = EntityState.Added;
                //var rows = await _dataSource.SaveChangesAsync();
            }

            else
            {

                flag.IsNegativeSales = singleItem.IsNegativeSales;
                _dataSource.Entry(flag).State = EntityState.Modified;

            }

        }
        var rows = await _dataSource.SaveChangesAsync();

        return item;
    }


    public async Task<List<Partner>> UpdateOrCratePartnerRowsync(List<Partner> partner)
    {
        var partnerIds = partner.Select(n => n.PartnerId).ToList();
        var itemInformation = await _dataSource.Partners.Where(n => partnerIds.Contains(n.PartnerId)).AsNoTracking().ToListAsync();
        foreach (var singleItem in partner.Distinct())
        {
            var flag = itemInformation.Where(n => n.PartnerId == singleItem.PartnerId).FirstOrDefault();
            if (flag == null)
            {
                _dataSource.Entry(singleItem).State = EntityState.Added;
                var rows = await _dataSource.SaveChangesAsync();
            }

            else
            {

                flag.Points = singleItem.Points;
                _dataSource.Entry(flag).State = EntityState.Modified;
                var rows = await _dataSource.SaveChangesAsync();
            }
        }

        return partner;
    }

    public async Task<ItemSellingPriceRow> ItemSellingPriceRowsAsync(long Id)
    {
        return await _dataSource.SellingPriceRows.Where(x => x.RowId == Id).FirstOrDefaultAsync();
    }
    public async Task<Partner> GetPartner(long? partnerId)
    {
        Partner part = new Partner();
        if (partnerId != null)
        {
            var pt = await _dataSource.Partners.Where(x => x.PartnerId == partnerId).FirstOrDefaultAsync();
            if (pt != null)
                part = pt;
        }
        else
        {
            part = await _dataSource.Partners.Where(x => x.PartnerTypeId == 1).FirstOrDefaultAsync();
        }
        return part;
    }
    public async Task<List<PartnerForSqlBD>> CreatePartnerAsyncsql(List<PartnerForSqlBD> partners)
    {
        //await _dataSource.Partners.ExecuteDeleteAsync();
        foreach (var singlePartner in partners)
        {
            _dataSource.Entry(singlePartner).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();
        return partners;
    }
    public async Task<List<Partner>> CreatePartnerAsync(List<Partner> partners)
    {
        //await _dataSource.Partners.ExecuteDeleteAsync();
        foreach (var singlePartner in partners)
        {
            _dataSource.Entry(singlePartner).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();
        return partners;
    }
    public async Task<CreateSalesDeliveryDTO> SaveItemIntoSalesDelivery(CreateSalesDeliveryDTO objCreate)
    {
        try
        {
            //var userInformation = await _dataSource.Users.Where(n => n.intUserID == objCreate.pOSSalesDeliveryHeader.UserId).FirstOrDefaultAsync();
            //if (userInformation == null)
            //{
            //    userInformation = await _dataSource.Users.Where(n => n.ServerUserID == objCreate.pOSSalesDeliveryHeader.UserId).FirstOrDefaultAsync();
            //}
            if (objCreate.pOSSalesDeliveryHeader.SalesOrderId != 0)
            {
                //objCreate.pOSSalesDeliveryHeader.UserId = userInformation.ServerUserID;
                _dataSource.Entry(objCreate.pOSSalesDeliveryHeader).State = EntityState.Modified;
            }
            else
            {
                //objCreate.pOSSalesDeliveryHeader.UserId = userInformation.ServerUserID;
                _dataSource.Entry(objCreate.pOSSalesDeliveryHeader).State = EntityState.Added;

            }

            await _dataSource.SaveChangesAsync();
            return objCreate;

        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<CreateSalesDeliveryDTO> SaveItemIntoSalesDeliveryLines(CreateSalesDeliveryDTO objCreate)
    {

        try
        {

            //var userInformation = await _dataSource.Users.Where(n => n.intUserID == objCreate.pOSSalesDeliveryHeader.UserId).FirstOrDefaultAsync();
            //if (userInformation == null)
            //{
            //    userInformation = await _dataSource.Users.Where(n => n.ServerUserID == objCreate.pOSSalesDeliveryHeader.UserId).FirstOrDefaultAsync();
            //}
            var partnerInfromation = await _dataSource.Partners.Where(n => n.PartnerId == objCreate.pOSSalesDeliveryHeader.CustomerId).FirstOrDefaultAsync();

            var itemIds = objCreate.pOSSalesDeliveryLine.Select(n => n.ItemId).Distinct().ToList();
            var ItemInformations = await _dataSource.SellingPriceRows.Where(n => itemIds.Contains(n.ItemId)).AsNoTracking().ToListAsync();
            var ItemInfo = await _dataSource.Items.Where(n => itemIds.Contains(n.ItemId)).AsNoTracking().ToListAsync();
            //using(TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
            if (objCreate.pOSSalesDeliveryHeader.SalesOrderId != 0)
            {
                //objCreate.pOSSalesDeliveryHeader.UserId = userInformation.ServerUserID;
                _dataSource.Entry(objCreate.pOSSalesDeliveryHeader).State = EntityState.Modified;

                if (partnerInfromation != null && objCreate.pOSSalesDeliveryHeader.Draft == false)
                {
                    partnerInfromation.Points = partnerInfromation.Points + Math.Round((objCreate.pOSSalesDeliveryHeader.Points ?? 0), 2);
                    _dataSource.Entry(partnerInfromation).State = EntityState.Modified;
                }
            }
            else
            {
                //objCreate.pOSSalesDeliveryHeader.UserId = userInformation.ServerUserID;
                _dataSource.Entry(objCreate.pOSSalesDeliveryHeader).State = EntityState.Added;
                if (partnerInfromation != null)
                {
                    partnerInfromation.Points = partnerInfromation.Points + Math.Round((objCreate.pOSSalesDeliveryHeader.Points ?? 0), 2);
                    _dataSource.Entry(partnerInfromation).State = EntityState.Modified;
                }
            }

            await _dataSource.SaveChangesAsync();
            if (objCreate.pOSSalesDeliveryHeader.SalesOrderId != 0)
            {
                await _dataSource.SalesDeliveryLines.Where(w => w.SalesOrderId == objCreate.pOSSalesDeliveryHeader.SalesOrderId).ExecuteDeleteAsync();
            }
            foreach (var singleDeliveryLine in objCreate.pOSSalesDeliveryLine)
            {
                singleDeliveryLine.SalesOrderId = objCreate.pOSSalesDeliveryHeader.SalesOrderId;
                _dataSource.Entry(singleDeliveryLine).State = EntityState.Added;

                var updateItem = ItemInformations.Where(n => n.ItemId == singleDeliveryLine.ItemId && n.NewPrice == singleDeliveryLine.Price).FirstOrDefault();
                if (ItemInformations.Where(n => n.ItemId == singleDeliveryLine.ItemId && n.NewPrice == singleDeliveryLine.Price).ToList().Any())
                {
                    updateItem.Qty = updateItem.Qty - singleDeliveryLine.Quantity;
                    _dataSource.Entry(updateItem).State = EntityState.Modified;
                }
                else
                {
                    var itemUpdate = ItemInfo.Where(n => n.ItemId == singleDeliveryLine.ItemId).FirstOrDefault();
                    if (itemUpdate != null)
                    {
                        itemUpdate.TotalQuantity = itemUpdate.TotalQuantity - singleDeliveryLine.Quantity;
                        _dataSource.Entry(itemUpdate).State = EntityState.Modified;
                    }

                    var StockUpdate = ItemInformations.OrderByDescending(r => r.RowId).FirstOrDefault();
                    if (StockUpdate != null)
                    {
                        StockUpdate.Qty = StockUpdate.Qty - singleDeliveryLine.Quantity;
                        _dataSource.Entry(StockUpdate).State = EntityState.Modified;
                    }
                }
            }
            //await _dataSource.SaveChangesAsync();

            if (objCreate.pOSSalesDeliveryHeader.SalesOrderId != 0)
            {
                await _dataSource.SalesPayments.Where(w => w.SalesDeliveryId == objCreate.pOSSalesDeliveryHeader.SalesOrderId).ExecuteDeleteAsync();
            }

            foreach (var singlePayment in objCreate.pOSSalesPayments)
            {
                singlePayment.SalesDeliveryId = objCreate.pOSSalesDeliveryHeader.SalesOrderId;
                _dataSource.Entry(singlePayment).State = EntityState.Added;
            }

            await _dataSource.SaveChangesAsync();

            //scope.Complete();
            return objCreate;
            //}
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<MessageHelper> SQLEditPosPayment(EditPosSales edit)
    {
        try
        {
            MessageHelper msg = new MessageHelper();
            decimal receiveamount = 0;
            var Dhead = await _dataSource.SQLServerDeliveryHeaders.Where(x => x.SalesOrderCode == edit.head.SalesOrderCode && x.isActive == true).FirstOrDefaultAsync();
            var remove = await _dataSource.SqlSalesPayments.Where(x => x.SalesDeliveryId == Dhead.SalesOrderId && x.IsActive == true).AsNoTracking().ToListAsync();
            foreach (var item in remove)
            {
                item.IsActive = false;
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            if (edit.Payments != null)
            {
                receiveamount = edit.Payments.Sum(x => x.CollectionAmount);
                foreach (var item in edit.Payments)
                {
                    var pay = new SQLPosSalesPayment()
                    {
                        SalesDeliveryId = Dhead.SalesOrderId,
                        AccountId = Dhead.AccountId,
                        BranchId = Dhead.BranchId,
                        OfficeId = Dhead.OfficeId,
                        WalletId = item.WalletId,
                        CollectionAmount = item.CollectionAmount,
                        ReferanceNo = item.ReferanceNo,
                        TransactionDate = DateTime.Now,
                        IsActive = true,
                        ActionById = item.ActionById,
                        LastActionDatetime = DateTime.Now,
                        ServerDatetime = DateTime.Now,
                    };
                    _dataSource.Entry(pay).State = EntityState.Added;
                }
                await _dataSource.SaveChangesAsync();
                msg.Message = "Update Successfully";
                msg.StatusCode = 200;
            }

            if (edit.head != null)
            {
                if (Dhead.CustomerId != edit.head.CustomerId)
                {
                    var partner = await _dataSource.PartnersSql.Where(x => x.PartnerId == edit.head.CustomerId).AsNoTracking().FirstOrDefaultAsync();
                    partner.Points = partner.Points + (Dhead.Points ?? 0);
                    _dataSource.Entry(partner).State = EntityState.Modified;
                    await _dataSource.SaveChangesAsync();

                    var partner2 = await _dataSource.PartnersSql.Where(x => x.PartnerId == Dhead.CustomerId).AsNoTracking().FirstOrDefaultAsync();
                    partner2.Points = partner2.Points - (Dhead.Points ?? 0);
                    _dataSource.Entry(partner2).State = EntityState.Modified;
                    await _dataSource.SaveChangesAsync();
                }
                receiveamount = receiveamount + edit.head.CashPayment.Value;
                Dhead.ReceiveAmount = receiveamount;
                Dhead.CashPayment = edit.head.CashPayment;
                Dhead.ReturnAmount = receiveamount - edit.head.NetAmount;
                Dhead.CustomerId = edit.head.CustomerId;
                Dhead.CustomerName = edit.head.CustomerName;
                Dhead.Phone = edit.head.Phone;
                _dataSource.Entry(Dhead).State = EntityState.Modified;
                await _dataSource.SaveChangesAsync();
                msg.Message = "Update Successfully";
                msg.StatusCode = 200;
            }
            return msg;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<MessageHelper> EditPosPayment(EditPosSales edit)
    {
        MessageHelper msg = new MessageHelper();
        decimal receiveamount = 0;
        var head = await _dataSource.DeliveryHeaders.Where(x => x.SalesOrderCode == edit.head.SalesOrderCode).FirstOrDefaultAsync();
        if (head != null)
        {
            var remove = await _dataSource.SalesPayments.Where(x => x.SalesDeliveryId == head.SalesOrderId).ToListAsync();
            foreach (var item in remove)
            {
                _dataSource.Entry(item).State = EntityState.Deleted;
            }
            await _dataSource.SaveChangesAsync();
            if (edit.Payments != null)
            {
                edit.Payments.ForEach(x =>
                {
                    _dataSource.Entry(x).State = EntityState.Added;
                });
                await _dataSource.SaveChangesAsync();
                msg.Message = "Update Successfully";
                msg.StatusCode = 200;
                receiveamount = edit.Payments.Sum(x => x.CollectionAmount);
            }

            if (edit.head != null)
            {
                if (head.CustomerId != edit.head.CustomerId)
                {
                    var partner = await _dataSource.Partners.Where(x => x.PartnerId == edit.head.CustomerId).AsNoTracking().FirstOrDefaultAsync();
                    if (partner != null)
                    {
                        partner.Points = partner.Points + (head.Points ?? 0);
                        _dataSource.Entry(partner).State = EntityState.Modified;
                        await _dataSource.SaveChangesAsync();
                    }


                    var partner2 = await _dataSource.Partners.Where(x => x.PartnerId == head.CustomerId).AsNoTracking().FirstOrDefaultAsync();
                    if (partner2 != null)
                    {
                        partner2.Points = partner2.Points - (head.Points ?? 0);
                        _dataSource.Entry(partner2).State = EntityState.Modified;
                        await _dataSource.SaveChangesAsync();
                    }

                }
                receiveamount = receiveamount + edit.head.CashPayment.Value;

                head.ReceiveAmount = receiveamount;
                head.CashPayment = edit.head.CashPayment;
                head.ReturnAmount = receiveamount - edit.head.NetAmount;
                head.CustomerId = edit.head.CustomerId;
                head.CustomerName = edit.head.CustomerName;
                head.Phone = edit.head.Phone;
                _dataSource.Entry(head).State = EntityState.Modified;
                await _dataSource.SaveChangesAsync();
                msg.Message = "Update Successfully";
                msg.StatusCode = 200;
            }
        }
        return msg;
    }
    public async Task<List<POSSalesDeliveryHeader>> RecallInvoiceInformation(long userId)
    {
        var returnObj = new List<POSSalesDeliveryHeader>();
        var userInformation = await _dataSource.Users.Where(n => n.intUserID == userId).FirstOrDefaultAsync();
        var headerInfo = await _dataSource.DeliveryHeaders.Where(n => n.UserId == userInformation.ServerUserID && n.Draft == true).ToListAsync();
        if (headerInfo == null)
            return new List<POSSalesDeliveryHeader>();
        returnObj.AddRange(headerInfo);

        return returnObj;
    }


    public async Task<List<CreateSalesDeliveryDTO>> SalesInformationUsingIDs(List<long> SalesOrderIds)
    {
        List<CreateSalesDeliveryDTO> finalResponse = new List<CreateSalesDeliveryDTO>();

        var headers = await _dataSource.DeliveryHeaders.Where(n => SalesOrderIds.Contains(n.SalesOrderId)).ToListAsync();
        foreach (var singleHeader in headers)
        {
            var line = await _dataSource.SalesDeliveryLines.Where(n => n.SalesOrderId == singleHeader.SalesOrderId).ToListAsync();
            var payments = await _dataSource.SalesPayments.Where(n => n.SalesDeliveryId == singleHeader.SalesOrderId).ToListAsync();

            CreateSalesDeliveryDTO salesDatabaseDTO = new CreateSalesDeliveryDTO
            {
                pOSSalesDeliveryHeader = singleHeader,
                pOSSalesDeliveryLine = line,
                pOSSalesPayments = payments,
                IsOnline = true,
            };

            finalResponse.Add(salesDatabaseDTO);
        }

        return finalResponse;
    }

    public async Task<bool> IsServerConnectionAvailable()
    {
        try
        {
            var isAvailable = false;
            var settings = new TblSettings();

            settings = await _dataSource.Settings.FirstOrDefaultAsync();

            string connectionString = settings.SqlServerConnString;

            // Extract the IP address and port from the connection string
            int start = connectionString.IndexOf('=') + 1;
            int end = connectionString.IndexOf(';');
            string dataSource = connectionString.Substring(start, end - start);

            // Split the IP address and port
            string[] parts = dataSource.Split(',');

            // parts[0] contains the IP address and parts[1] contains the port
            string ipAddress = parts[0];
            //string port = parts[1];
            Ping ping = new Ping();
            PingReply reply = ping.Send(ipAddress, 1000);
            if (reply.Status == IPStatus.Success)
            {
                isAvailable = true;
            }
            if (isAvailable == false)
            {
                if (settings != null)
                {

                    using (var connection = new SqlConnection(settings.SqlServerConnString))
                    {
                        try
                        {
                            connection.Open();
                            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                            {
                                isAvailable = true;
                            }
                            connection.Close();
                        }
                        catch
                        {
                            isAvailable = false;
                        }


                    }
                }
            }

            return isAvailable;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<POSSalesDeliveryLine>> RecallInvoiceRow(long userId, long SalesDeliveryId)
    {

        var data = await _dataSource.SalesDeliveryLines.Where(n => n.SalesOrderId == SalesDeliveryId).OrderByDescending(n => n.Id).ToListAsync();
        if (data == null)
            return null;
        return data;


    }
    public async Task<MainViewRecallInvoiceDTO> RecallInvoice(long userId, long SalesDeliveryId, List<Item> Items)
    {
        MainViewRecallInvoiceDTO returnObj = new MainViewRecallInvoiceDTO();
        var userInformation = await _dataSource.Users.Where(n => n.intUserID == userId).FirstOrDefaultAsync();
        var headerInfo = await _dataSource.DeliveryHeaders.Where(n => n.UserId == userInformation.ServerUserID && n.SalesOrderId == SalesDeliveryId).OrderByDescending(n => n.SalesOrderId).FirstOrDefaultAsync();
        if (headerInfo == null)
            return null;

        var customerInformation = await _dataSource.Partners.Where(n => n.PartnerId == headerInfo.CustomerId).FirstOrDefaultAsync();
        var data = await _dataSource.SalesDeliveryLines.Where(n => n.SalesOrderId == headerInfo.SalesOrderId).OrderByDescending(n => n.Id).ToListAsync();
        var settings = await _dataSource.Settings.FirstOrDefaultAsync();


        var rowInfo = data.Select(n => new MainViewModelItemDTO
        {
            SL = 0,
            ItemId = n.ItemId,
            ItemName = n.ItemName,
            Quantity = n.Quantity.ToString(),
            SalesRate = Math.Round(n.Price + 0.00m, 2).ToString(),
            Vat = n.VatAmount ?? 0,
            SD = n.SdAmount ?? 0,
            Discount = n.DiscountAmount ?? 0,
            Amount = n.NetAmount.ToString(),
            DiscountPercentage = n.DiscountType == "Amount" ? 0.0M : Convert.ToDecimal(n.NetAmount != 0 ? (n.DiscountAmount * 100 / n.NetAmount) : 0),  //Convert.ToDecimal((n.DiscountAmount * 100)/ (n.NetAmount != 0 ? n.NetAmount  : 1)),
            SingleDiscountAmount = n.DiscountType == "Amount" ? n.LineDiscount : 0.0M,
            VATPercentage = n.VatPercentage ?? 0,
            SDPercentage = n.SdPercentage ?? 0,
            UMOid = n.UomId,
            UMOName = n.UomName,
            BarCode = Items.Where(r => r.ItemId == n.ItemId).Select(r => r.Barcode).FirstOrDefault(),
            ExchangeReferenceNo=n.ExchangeReferenceId.GetValueOrDefault()
        }).ToList();

        var paymentInformation = await (from a in _dataSource.SalesPayments
                                        join b in _dataSource.SalesWallets on a.WalletId equals b.WalletId
                                        where a.SalesDeliveryId == headerInfo.SalesOrderId
                                        select new PaymentModeInformation
                                        {
                                            intWalletId = a.WalletId,
                                            ReferanceNo = a.ReferanceNo,
                                            strWalletId = b.WalletName,
                                            numberAmount = a.CollectionAmount,
                                        }).ToListAsync();

        var collectionObj = new MyCollection
        {
            TotalBill = "",
            NumtotalBill = headerInfo.ItemTotalAmount,
            TotalSD = "",
            NumtotalSD = headerInfo.TotalSd ?? 0,
            TotalVAT = "",
            NumtotalVAT = headerInfo.TotalVat ?? 0,
            TotalDiscount = "",
            NumTotalDiscount = headerInfo.NetDiscount - headerInfo.HeaderDiscount,
            OtherDiscount = headerInfo.HeaderDiscount.ToString(),
            NumotherDiscount = headerInfo.HeaderDiscount, //headerInfo.HeaderDiscountPercentage,
            GrandTotal = "",
            NumGrandTotal = headerInfo.NetAmount,
            ReceiveAmount = headerInfo.ReceiveAmount,
            ChangeAmount = headerInfo.ReturnAmount ?? 0,

            NumOtherDiscountPercentage = headerInfo.HeaderDiscountPercentage,
            NumOtherDiscountAmount = headerInfo.HeaderDiscountPercentage == 0.0M ? headerInfo.HeaderDiscount : 0.0M,
            OtherDiscountType = headerInfo.HeaderDiscountPercentage > 0.0M ? 2 : 1,
            OtherDiscountId = headerInfo.HeaderDiscountId ?? 0,

            InvoiceNumber = headerInfo.SalesOrderCode,

            CustomerId = headerInfo.CustomerId,
            CustomerName = headerInfo.CustomerName,
            //CustomerCode = customerInformation != null ? customerInformation.PartnerCode : "",
            CustomerCode = customerInformation != null ? customerInformation.MobileNo : "",
            CustomerPoints = customerInformation != null ? customerInformation.Points.ToString() : "0.00",
            IsReturn = headerInfo.ISReturn.GetValueOrDefault()
        };

        returnObj.Items = rowInfo;
        returnObj.PaymentModeInformation = paymentInformation;
        returnObj.collection = collectionObj;

        return returnObj;
    }
    public async Task<MainViewRecallInvoiceDTO> RecallInvoiceInformation(long userId, long customerId)
    {
        MainViewRecallInvoiceDTO returnObj = new MainViewRecallInvoiceDTO();
        var headerInfo = await _dataSource.DeliveryHeaders.Where(n => n.UserId == userId && n.CustomerId == customerId).OrderByDescending(n => n.SalesOrderId).FirstOrDefaultAsync();
        if (headerInfo == null)
            return null;

        var rowInfo = await _dataSource.SalesDeliveryLines.Where(n => n.SalesOrderId == headerInfo.SalesOrderId).OrderByDescending(n => n.Id).Select(n => new MainViewModelItemDTO
        {
            SL = 0,
            ItemId = n.ItemId,
            ItemName = n.ItemName,
            Quantity = n.Quantity.ToString(),
            SalesRate = n.Price.ToString(),
            Vat = n.VatAmount ?? 0,
            SD = n.SdAmount ?? 0,
            Discount = n.DiscountAmount ?? 0,
            Amount = n.NetAmount.ToString(),
            DiscountPercentage = Convert.ToDecimal(n.NetAmount != 0 ? (n.DiscountAmount * 100 / n.NetAmount) : 0),  //Convert.ToDecimal((n.DiscountAmount * 100)/ (n.NetAmount != 0 ? n.NetAmount  : 1)),
            VATPercentage = n.VatPercentage ?? 0,
            SDPercentage = n.SdPercentage ?? 0,
            UMOid = n.UomId,
            UMOName = n.UomName,
        }).ToListAsync();

        var paymentInformation = await _dataSource.SalesPayments.Where(n => n.SalesDeliveryId == headerInfo.SalesOrderId).Select(n => new PaymentModeInformation
        {
            intWalletId = n.WalletId,
            strWalletId = _dataSource.SalesWallets.Where(r => r.WalletId == n.WalletId).Select(r => r.WalletName).FirstOrDefault(),
            numberAmount = n.CollectionAmount,
        }).ToListAsync();

        var collectionObj = new MyCollection
        {
            TotalBill = "",
            NumtotalBill = headerInfo.ItemTotalAmount,
            TotalSD = "",
            NumtotalSD = headerInfo.TotalSd ?? 0,
            TotalVAT = "",
            NumtotalVAT = headerInfo.TotalVat ?? 0,
            TotalDiscount = "",
            NumTotalDiscount = headerInfo.NetDiscount - headerInfo.HeaderDiscount,
            OtherDiscount = headerInfo.HeaderDiscount.ToString(),
            NumotherDiscount = headerInfo.HeaderDiscountPercentage,
            GrandTotal = "",
            NumGrandTotal = headerInfo.NetAmount,
            ReceiveAmount = headerInfo.ReceiveAmount,
            ChangeAmount = headerInfo.ReturnAmount ?? 0,
            NumOtherDiscountPercentage = headerInfo.HeaderDiscountPercentage,
            CashPayment = headerInfo.CashPayment ?? 0,
        };

        returnObj.Items = rowInfo;
        returnObj.PaymentModeInformation = paymentInformation;
        returnObj.collection = collectionObj;

        return returnObj;
    }
    public async Task<Partner> GetPartnerAsync(string strPartnerCode, long AccountId)
    {
        var data = await _dataSource.Partners.Where(x => x.AccountId == AccountId && x.PartnerTypeId == 1 && x.MobileNo.Trim().ToLower() == strPartnerCode.Trim().ToLower() && x.isActive == true).FirstOrDefaultAsync();
        if (data != null)
            data.Points = Math.Round(data.Points, 2);
        return data;
    }

    public async Task<Partner> GetPartnerIdAsync(long PartnerId, long AccountId)
    {
        var data = await _dataSource.Partners.Where(x => x.AccountId == AccountId && x.PartnerTypeId == 1 && x.PartnerId == PartnerId && x.isActive == true).FirstOrDefaultAsync();
        if (data != null)
            data.Points = Math.Round(data.Points, 2);
        return data;
    }
    public async Task<PartnerForSqlBD> GetPartnerAsyncSql(string mobileNo, long accountId)
    {
        var data = await _dataSource.PartnersSql.Where(x => x.MobileNo.Trim().ToLower() == mobileNo.Trim().ToLower() && x.AccountId == accountId && x.PartnerTypeId == 1 && x.isActive==true).FirstOrDefaultAsync();
        if (data != null)
            data.Points = Math.Round(data.Points, 2);
        return data;
    }
    public async Task<Item> GetItemByBarCode(string ItemBarCode)
    {
        return await _dataSource.Items.Where(x => x.Barcode == ItemBarCode).FirstOrDefaultAsync();
    }

    public async Task<Item> GetSQLItemByBarCode(string ItemBarCode, long AccountId)
    {
        return await _dataSource.SQLItems.Where(x => x.Barcode == ItemBarCode && x.AccountId == AccountId).Select(itm => new Item
        {
            ItemId = itm.ItemId,
            ItemGlobalId = itm.ItemGlobalId,
            ItemName = itm.ItemName,
            ItemCode = itm.ItemCode,
            Barcode = itm.Barcode,
            ItemTypeId = itm.ItemTypeId,
            ItemTypeName = itm.ItemTypeName,
            ItemCategoryId = itm.ItemCategoryId,
            ItemCategoryName = itm.ItemCategoryName,
            ItemSubCategoryId = itm.ItemSubCategoryId,
            ItemSubCategoryName = itm.ItemSubCategoryName,
            AccountId = itm.AccountId,
            AccountName = itm.AccountName,
            BranchId = itm.BranchId,
            BranchName = itm.BranchName,
            UomId = itm.UomId,
            UomName = itm.UomName,
            UserId = itm.UserId,
            UserName = itm.UserName,
            ActionTime = itm.ActionTime.ToString(),
            StartDate = itm.StartDate.ToString(),
            ExpiredDate = itm.ExpiredDate.ToString(),
            Price = itm.Price,
            Vat = itm.Vat,
            SD = itm.SD,
            AvgRate = itm.AvgRate,
            TotalQuantity = 0,
            VatPercentage = itm.VatPercentage,
            CurrentSellingPrice = itm.CurrentSellingPrice,
            IsActive = itm.IsActive,
            StockLimitQuantity = itm.StockLimitQuantity,
            TaxRateId = itm.TaxRateId,
            Brand = itm.Brand,
            PartNumber = itm.PartNumber,
            ItemDescription = itm.ItemDescription,
            OriginId = itm.OriginId,
            OriginName = itm.OriginName,
            StdPurchasePrice = itm.StdPurchasePrice,
            AltUomId = itm.AltUomId,
            AltUomName = itm.AltUomName,
            ConversionUnit = itm.ConversionUnit,
            IsSerial = itm.IsSerial,
            MaximumDiscountPercent = itm.MaximumDiscountPercent,
            IsBatchManage = itm.IsBatchManage,
            HSCode = itm.HSCode,
            MaximumDiscountAmount = itm.MaximumDiscountAmount,
            IsNegativeSales = itm.IsNegativeSales
        }).FirstOrDefaultAsync();
    }

    public async Task<List<Item>> GetItemListByItemName(string ItemName)
    {

        var response = await (from itm in _dataSource.Items
                              join sellingPrice in (from sellingPrice in _dataSource.SellingPriceRows select sellingPrice) on itm.ItemId equals sellingPrice.ItemId into g
                              from sellingPrice in g.DefaultIfEmpty()
                              where itm.ItemName.Trim().ToLower().Contains(ItemName.Trim().ToLower())
                              select new Item
                              {
                                  ItemId = itm.ItemId,
                                  ItemGlobalId = itm.ItemGlobalId,
                                  ItemName = itm.ItemName,
                                  ItemCode = itm.ItemCode,
                                  Barcode = itm.Barcode,
                                  ItemTypeId = itm.ItemTypeId,
                                  ItemTypeName = itm.ItemTypeName,
                                  ItemCategoryId = itm.ItemCategoryId,
                                  ItemCategoryName = itm.ItemCategoryName,
                                  ItemSubCategoryId = itm.ItemSubCategoryId,
                                  ItemSubCategoryName = itm.ItemSubCategoryName,
                                  AccountId = itm.AccountId,
                                  AccountName = itm.AccountName,
                                  BranchId = itm.BranchId,
                                  BranchName = itm.BranchName,
                                  UomId = itm.UomId,
                                  UomName = itm.UomName,
                                  UserId = itm.UserId,
                                  UserName = itm.UserName,
                                  ActionTime = itm.ActionTime,
                                  StartDate = itm.StartDate,
                                  ExpiredDate = itm.ExpiredDate,
                                  Price = itm.Price,
                                  Vat = itm.Vat,
                                  SD = itm.SD,
                                  AvgRate = itm.AvgRate,
                                  TotalQuantity = sellingPrice == null ? 0 : sellingPrice.Qty,
                                  VatPercentage = itm.VatPercentage,
                                  CurrentSellingPrice = sellingPrice == null ? itm.CurrentSellingPrice : sellingPrice.NewPrice,
                                  IsActive = itm.IsActive,
                                  StockLimitQuantity = itm.StockLimitQuantity,
                                  TaxRateId = itm.TaxRateId,
                                  Brand = itm.Brand,
                                  PartNumber = itm.PartNumber,
                                  ItemDescription = itm.ItemDescription,
                                  OriginId = itm.OriginId,
                                  OriginName = itm.OriginName,
                                  StdPurchasePrice = itm.StdPurchasePrice,
                                  AltUomId = itm.AltUomId,
                                  AltUomName = itm.AltUomName,
                                  ConversionUnit = itm.ConversionUnit,
                                  IsSerial = itm.IsSerial,
                                  MaximumDiscountPercent = itm.MaximumDiscountPercent,
                                  IsBatchManage = itm.IsBatchManage,
                                  HSCode = itm.HSCode,
                                  MaximumDiscountAmount = itm.MaximumDiscountAmount,
                                  IsNegativeSales = itm.IsNegativeSales
                              }).Distinct().Take(10).ToListAsync();


        return response;
    }
    public async Task<List<Item>> GetSQLItemListByItemName(string ItemName, long WarehouseId, long AccountId)
    {
        var response = await (from itm in _dataSource.SQLItems
                              join sellingPrice in (from sellingPrice in _dataSource.SQLServerSellingPriceRows
                                                    join h in _dataSource.SQLItemSellingPriceHeaders on sellingPrice.HeaderId equals h.HeaderId
                                                    where h.WarehouseId == WarehouseId
                                                    select sellingPrice) on itm.ItemId equals sellingPrice.ItemId into g
                              from sellingPrice in g.DefaultIfEmpty()
                              where itm.ItemName.Trim().ToLower().Contains(ItemName.Trim().ToLower()) && itm.AccountId == AccountId
                              select new Item
                              {
                                  ItemId = itm.ItemId,
                                  ItemGlobalId = itm.ItemGlobalId,
                                  ItemName = itm.ItemName,
                                  ItemCode = itm.ItemCode,
                                  Barcode = itm.Barcode,
                                  ItemTypeId = itm.ItemTypeId,
                                  ItemTypeName = itm.ItemTypeName,
                                  ItemCategoryId = itm.ItemCategoryId,
                                  ItemCategoryName = itm.ItemCategoryName,
                                  ItemSubCategoryId = itm.ItemSubCategoryId,
                                  ItemSubCategoryName = itm.ItemSubCategoryName,
                                  AccountId = itm.AccountId,
                                  AccountName = itm.AccountName,
                                  BranchId = itm.BranchId,
                                  BranchName = itm.BranchName,
                                  UomId = itm.UomId,
                                  UomName = itm.UomName,
                                  UserId = itm.UserId,
                                  UserName = itm.UserName,
                                  ActionTime = itm.ActionTime.ToString(),
                                  StartDate = itm.StartDate.ToString(),
                                  ExpiredDate = itm.ExpiredDate.ToString(),
                                  Price = itm.Price,
                                  Vat = itm.Vat,
                                  SD = itm.SD,
                                  AvgRate = itm.AvgRate,
                                  TotalQuantity = sellingPrice == null ? 0 : sellingPrice.Qty,
                                  VatPercentage = itm.VatPercentage,
                                  CurrentSellingPrice = sellingPrice == null ? Math.Round(itm.CurrentSellingPrice, 2, System.MidpointRounding.AwayFromZero) : Math.Round(sellingPrice.NewPrice, 2, System.MidpointRounding.AwayFromZero),
                                  IsActive = itm.IsActive,
                                  StockLimitQuantity = itm.StockLimitQuantity,
                                  TaxRateId = itm.TaxRateId,
                                  Brand = itm.Brand,
                                  PartNumber = itm.PartNumber,
                                  ItemDescription = itm.ItemDescription,
                                  OriginId = itm.OriginId,
                                  OriginName = itm.OriginName,
                                  StdPurchasePrice = itm.StdPurchasePrice,
                                  AltUomId = itm.AltUomId,
                                  AltUomName = itm.AltUomName,
                                  ConversionUnit = itm.ConversionUnit,
                                  IsSerial = itm.IsSerial,
                                  MaximumDiscountPercent = itm.MaximumDiscountPercent,
                                  IsBatchManage = itm.IsBatchManage,
                                  HSCode = itm.HSCode,
                                  MaximumDiscountAmount = itm.MaximumDiscountAmount,
                                  IsNegativeSales = itm.IsNegativeSales
                              }).Distinct().Take(10).ToListAsync();


        return response;
    }
    public async Task<List<Item>> GetItemListByBarCode(string ItemBarCode)
    {
        //return await _dataSource.Items.Where(x => x.Barcode.Trim().ToLower() == ItemBarCode.Trim().ToLower()).ToListAsync();
        var response = await (from itm in _dataSource.Items
                              join sellingPrice in (from sellingPrice in _dataSource.SellingPriceRows select sellingPrice) on itm.ItemId equals sellingPrice.ItemId into g
                              from sellingPrice in g.DefaultIfEmpty()
                              where itm.Barcode.Trim().ToLower() == ItemBarCode.Trim().ToLower()
                              select new Item
                              {
                                  ItemId = itm.ItemId,
                                  ItemGlobalId = itm.ItemGlobalId,
                                  ItemName = itm.ItemName,
                                  ItemCode = itm.ItemCode,
                                  Barcode = itm.Barcode,
                                  ItemTypeId = itm.ItemTypeId,
                                  ItemTypeName = itm.ItemTypeName,
                                  ItemCategoryId = itm.ItemCategoryId,
                                  ItemCategoryName = itm.ItemCategoryName,
                                  ItemSubCategoryId = itm.ItemSubCategoryId,
                                  ItemSubCategoryName = itm.ItemSubCategoryName,
                                  AccountId = itm.AccountId,
                                  AccountName = itm.AccountName,
                                  BranchId = itm.BranchId,
                                  BranchName = itm.BranchName,
                                  UomId = itm.UomId,
                                  UomName = itm.UomName,
                                  UserId = itm.UserId,
                                  UserName = itm.UserName,
                                  ActionTime = itm.ActionTime,
                                  StartDate = itm.StartDate,
                                  ExpiredDate = itm.ExpiredDate,
                                  Price = itm.Price,
                                  Vat = itm.Vat,
                                  SD = itm.SD,
                                  AvgRate = itm.AvgRate,
                                  TotalQuantity = sellingPrice == null ? 0 : sellingPrice.Qty,
                                  VatPercentage = itm.VatPercentage,
                                  CurrentSellingPrice = sellingPrice == null ? itm.CurrentSellingPrice : sellingPrice.NewPrice,
                                  IsActive = itm.IsActive,
                                  StockLimitQuantity = itm.StockLimitQuantity,
                                  TaxRateId = itm.TaxRateId,
                                  Brand = itm.Brand,
                                  PartNumber = itm.PartNumber,
                                  ItemDescription = itm.ItemDescription,
                                  OriginId = itm.OriginId,
                                  OriginName = itm.OriginName,
                                  StdPurchasePrice = itm.StdPurchasePrice,
                                  AltUomId = itm.AltUomId,
                                  AltUomName = itm.AltUomName,
                                  ConversionUnit = itm.ConversionUnit,
                                  IsSerial = itm.IsSerial,
                                  MaximumDiscountPercent = itm.MaximumDiscountPercent,
                                  IsBatchManage = itm.IsBatchManage,
                                  HSCode = itm.HSCode,
                                  MaximumDiscountAmount = itm.MaximumDiscountAmount,
                                  IsNegativeSales = itm.IsNegativeSales
                              }).Distinct().Take(10).ToListAsync();


        return response;
    }

    public async Task<List<Item>> GetSQLItemListByBarCode(string ItemBarCode, long WarehouseId, long AccountId)
    {
        //return await _dataSource.Items.Where(x => x.Barcode.Trim().ToLower() == ItemBarCode.Trim().ToLower()).ToListAsync();
        var response = await (from itm in _dataSource.SQLItems
                              join sellingPrice in (from sellingPrice in _dataSource.SQLServerSellingPriceRows
                                                    join h in _dataSource.SQLItemSellingPriceHeaders on sellingPrice.HeaderId equals h.HeaderId
                                                    where h.WarehouseId == WarehouseId
                                                    select sellingPrice) on itm.ItemId equals sellingPrice.ItemId into g
                              from sellingPrice in g.DefaultIfEmpty()
                              where itm.Barcode.Trim().ToLower() == ItemBarCode.Trim().ToLower() && itm.AccountId == AccountId
                              select new Item
                              {
                                  ItemId = itm.ItemId,
                                  ItemGlobalId = itm.ItemGlobalId,
                                  ItemName = itm.ItemName,
                                  ItemCode = itm.ItemCode,
                                  Barcode = itm.Barcode,
                                  ItemTypeId = itm.ItemTypeId,
                                  ItemTypeName = itm.ItemTypeName,
                                  ItemCategoryId = itm.ItemCategoryId,
                                  ItemCategoryName = itm.ItemCategoryName,
                                  ItemSubCategoryId = itm.ItemSubCategoryId,
                                  ItemSubCategoryName = itm.ItemSubCategoryName,
                                  AccountId = itm.AccountId,
                                  AccountName = itm.AccountName,
                                  BranchId = itm.BranchId,
                                  BranchName = itm.BranchName,
                                  UomId = itm.UomId,
                                  UomName = itm.UomName,
                                  UserId = itm.UserId,
                                  UserName = itm.UserName,
                                  ActionTime = itm.ActionTime.ToString(),
                                  StartDate = itm.StartDate.ToString(),
                                  ExpiredDate = itm.ExpiredDate.ToString(),
                                  Price = itm.Price,
                                  Vat = itm.Vat,
                                  SD = itm.SD,
                                  AvgRate = itm.AvgRate,
                                  TotalQuantity = sellingPrice == null ? 0 : sellingPrice.Qty,
                                  VatPercentage = itm.VatPercentage,
                                  CurrentSellingPrice = sellingPrice == null ? Math.Round(itm.CurrentSellingPrice, 2, System.MidpointRounding.AwayFromZero) : Math.Round(sellingPrice.NewPrice, 2, System.MidpointRounding.AwayFromZero),
                                  IsActive = itm.IsActive,
                                  StockLimitQuantity = itm.StockLimitQuantity,
                                  TaxRateId = itm.TaxRateId,
                                  Brand = itm.Brand,
                                  PartNumber = itm.PartNumber,
                                  ItemDescription = itm.ItemDescription,
                                  OriginId = itm.OriginId,
                                  OriginName = itm.OriginName,
                                  StdPurchasePrice = itm.StdPurchasePrice,
                                  AltUomId = itm.AltUomId,
                                  AltUomName = itm.AltUomName,
                                  ConversionUnit = itm.ConversionUnit,
                                  IsSerial = itm.IsSerial,
                                  MaximumDiscountPercent = itm.MaximumDiscountPercent,
                                  IsBatchManage = itm.IsBatchManage,
                                  HSCode = itm.HSCode,
                                  MaximumDiscountAmount = itm.MaximumDiscountAmount,
                                  IsNegativeSales = itm.IsNegativeSales
                              }).Distinct().Take(10).ToListAsync();


        return response;
    }

    public async Task<List<Item>> GetMultipleSalesPrizeItemListByBarCode(string ItemBarCode)
    {
        //return await _dataSource.Items.Where(x => x.Barcode.Trim().ToLower() == ItemBarCode.Trim().ToLower()).ToListAsync();
        var response = await (from itm in _dataSource.Items
                              join sellingPrice in (from sellingPrice in _dataSource.SellingPriceRows select sellingPrice) on itm.ItemId equals sellingPrice.ItemId into g
                              from sellingPrice in g.DefaultIfEmpty()
                              where itm.Barcode.Trim().ToLower() == ItemBarCode.Trim().ToLower()
                              select new Item
                              {
                                  ItemId = itm.ItemId,
                                  ItemGlobalId = itm.ItemGlobalId,
                                  ItemName = itm.ItemName,
                                  ItemCode = itm.ItemCode,
                                  Barcode = itm.Barcode,
                                  ItemTypeId = itm.ItemTypeId,
                                  ItemTypeName = itm.ItemTypeName,
                                  ItemCategoryId = itm.ItemCategoryId,
                                  ItemCategoryName = itm.ItemCategoryName,
                                  ItemSubCategoryId = itm.ItemSubCategoryId,
                                  ItemSubCategoryName = itm.ItemSubCategoryName,
                                  AccountId = itm.AccountId,
                                  AccountName = itm.AccountName,
                                  BranchId = itm.BranchId,
                                  BranchName = itm.BranchName,
                                  UomId = itm.UomId,
                                  UomName = itm.UomName,
                                  UserId = itm.UserId,
                                  UserName = itm.UserName,
                                  ActionTime = itm.ActionTime,
                                  StartDate = itm.StartDate,
                                  ExpiredDate = itm.ExpiredDate,
                                  Price = itm.Price,
                                  Vat = itm.Vat,
                                  SD = itm.SD,
                                  AvgRate = itm.AvgRate,
                                  TotalQuantity = sellingPrice == null ? 0 : sellingPrice.Qty,
                                  VatPercentage = itm.VatPercentage,
                                  CurrentSellingPrice = sellingPrice == null ? itm.CurrentSellingPrice : sellingPrice.NewPrice,
                                  IsActive = itm.IsActive,
                                  StockLimitQuantity = itm.StockLimitQuantity,
                                  TaxRateId = itm.TaxRateId,
                                  Brand = itm.Brand,
                                  PartNumber = itm.PartNumber,
                                  ItemDescription = itm.ItemDescription,
                                  OriginId = itm.OriginId,
                                  OriginName = itm.OriginName,
                                  StdPurchasePrice = itm.StdPurchasePrice,
                                  AltUomId = itm.AltUomId,
                                  AltUomName = itm.AltUomName,
                                  ConversionUnit = itm.ConversionUnit,
                                  IsSerial = itm.IsSerial,
                                  MaximumDiscountPercent = itm.MaximumDiscountPercent,
                                  IsBatchManage = itm.IsBatchManage,
                                  HSCode = itm.HSCode,
                                  MaximumDiscountAmount = itm.MaximumDiscountAmount,
                                  IsNegativeSales = itm.IsNegativeSales
                              }).Distinct().ToListAsync();


        return response;
    }


    public async Task<List<Item>> GetSQLMultipleSalesPrizeItemListByBarCode(string ItemBarCode, long WarehouseId, long AccountId)
    {
        //return await _dataSource.Items.Where(x => x.Barcode.Trim().ToLower() == ItemBarCode.Trim().ToLower()).ToListAsync();
        var response = await (from itm in _dataSource.SQLItems
                              join sellingPrice in (from sellingPrice in _dataSource.SQLServerSellingPriceRows
                                                    join h in _dataSource.SQLItemSellingPriceHeaders on sellingPrice.HeaderId equals h.HeaderId
                                                    where h.WarehouseId == WarehouseId
                                                    select sellingPrice) on itm.ItemId equals sellingPrice.ItemId into g
                              from sellingPrice in g.DefaultIfEmpty()
                              where itm.Barcode.Trim().ToLower() == ItemBarCode.Trim().ToLower() && itm.AccountId == AccountId
                              select new Item
                              {
                                  ItemId = itm.ItemId,
                                  ItemGlobalId = itm.ItemGlobalId,
                                  ItemName = itm.ItemName,
                                  ItemCode = itm.ItemCode,
                                  Barcode = itm.Barcode,
                                  ItemTypeId = itm.ItemTypeId,
                                  ItemTypeName = itm.ItemTypeName,
                                  ItemCategoryId = itm.ItemCategoryId,
                                  ItemCategoryName = itm.ItemCategoryName,
                                  ItemSubCategoryId = itm.ItemSubCategoryId,
                                  ItemSubCategoryName = itm.ItemSubCategoryName,
                                  AccountId = itm.AccountId,
                                  AccountName = itm.AccountName,
                                  BranchId = itm.BranchId,
                                  BranchName = itm.BranchName,
                                  UomId = itm.UomId,
                                  UomName = itm.UomName,
                                  UserId = itm.UserId,
                                  UserName = itm.UserName,
                                  ActionTime = itm.ActionTime.ToString(),
                                  StartDate = itm.StartDate.ToString(),
                                  ExpiredDate = itm.ExpiredDate.ToString(),
                                  Price = itm.Price,
                                  Vat = itm.Vat,
                                  SD = itm.SD,
                                  AvgRate = itm.AvgRate,
                                  TotalQuantity = sellingPrice == null ? 0 : sellingPrice.Qty,
                                  VatPercentage = itm.VatPercentage,
                                  CurrentSellingPrice = sellingPrice == null ? Math.Round(itm.CurrentSellingPrice, 2, System.MidpointRounding.AwayFromZero) : Math.Round(sellingPrice.NewPrice, 2, System.MidpointRounding.AwayFromZero),
                                  IsActive = itm.IsActive,
                                  StockLimitQuantity = itm.StockLimitQuantity,
                                  TaxRateId = itm.TaxRateId,
                                  Brand = itm.Brand,
                                  PartNumber = itm.PartNumber,
                                  ItemDescription = itm.ItemDescription,
                                  OriginId = itm.OriginId,
                                  OriginName = itm.OriginName,
                                  StdPurchasePrice = itm.StdPurchasePrice,
                                  AltUomId = itm.AltUomId,
                                  AltUomName = itm.AltUomName,
                                  ConversionUnit = itm.ConversionUnit,
                                  IsSerial = itm.IsSerial,
                                  MaximumDiscountPercent = itm.MaximumDiscountPercent,
                                  IsBatchManage = itm.IsBatchManage,
                                  HSCode = itm.HSCode,
                                  MaximumDiscountAmount = itm.MaximumDiscountAmount,
                                  IsNegativeSales = itm.IsNegativeSales
                              }).Distinct().ToListAsync();


        return response;
    }
    public async Task<List<PaymentWalletDTO>> GetPaymentWalletList()
    {
        var response = await _dataSource.SalesWallets.Select(n => new PaymentWalletDTO
        {
            intWalletId = n.WalletId,
            strWalletName = n.WalletName,
        }).ToListAsync();

        return response;
    }


    public async Task<List<OtherDiscountDTO>> GetOtherDiscountList()
    {
        var response = await _dataSource.SpecialDiscounts.Where(w => w.IsActive == true).Select(n => new OtherDiscountDTO
        {
            HeaderId = n.HeaderId,
            OfferName = n.OfferName,
            DiscountType = n.DiscountType,
            StrDiscountType = n.DiscountType == 1 ? "Amount" : "Percentage",
            Value = n.Value,

            MaxAmount = n.MaxAmount ?? 0.0M,
            MinAmount = n.MinAmount ?? 0.0M,
        }).ToListAsync();
        return response;
    }


    public async Task<PaymentWalletDTO> GetPaymentWalletbyId(long WalletId)
    {
        var response = await _dataSource.SalesWallets.Where(n => n.WalletId == WalletId).Select(n => new PaymentWalletDTO
        {
            intWalletId = n.WalletId,
            strWalletName = n.WalletName,
        }).FirstOrDefaultAsync();

        return response;
    }

    public async Task<List<PaymentWalletDTO>> GetPaymentWalletbyIds(List<long> WalletId)
    {
        var response = await _dataSource.SalesWallets.Where(n => WalletId.Contains(n.WalletId))
            .Select(n => new PaymentWalletDTO
            {
                intWalletId = n.WalletId,
                strWalletName = n.WalletName,
            }).ToListAsync();

        return response;
    }
    public long GetPOSSalesDeliveryHeaderInfo(long CounterId)
    {
        var date = DateTime.Today.BD().Date;
        var response = _dataSource.DeliveryHeaders.Where(n => n.CounterId == CounterId && n.OrderDate >= date).Select(n => n.SalesOrderId).Count();
        return response;
    }
    public long GetSQLServerSalesDeliveryHeaderInfo(long CounterId)
    {
        var date = DateTime.Today.BD().Date;
        var response = _dataSource.SQLServerDeliveryHeaders.Where(n => n.CounterId == CounterId && n.OrderDate >= date).Select(n => n.SalesOrderId).Count();
        return response;
    }
    public async Task<POSSalesDeliveryHeader> GetPOSSalesDeliveryHeader(string InvoiceCode)
    {
        var response = await _dataSource.DeliveryHeaders.Where(n => n.SalesOrderCode.Trim().ToLower() == InvoiceCode.Trim().ToLower()).FirstOrDefaultAsync();
        return response;
    }

    public async void RemoveDuplicateItems()
    {

        //var distinctBarCode = await _dataSource.Items.Select(n => n.Barcode).Distinct().ToListAsync();

        var distinctitems = await _dataSource.Items.Select(n => new Item
        {
            ItemId = n.ItemId,
            ItemGlobalId = n.ItemGlobalId,
            ItemName = n.ItemName,
            ItemCode = n.ItemCode,
            Barcode = n.Barcode,
            ItemTypeId = n.ItemTypeId,
            ItemTypeName = n.ItemTypeName,
            ItemCategoryId = n.ItemCategoryId,
            ItemCategoryName = n.ItemCategoryName,
            ItemSubCategoryId = n.ItemSubCategoryId,
            ItemSubCategoryName = n.ItemSubCategoryName,
            AccountId = n.AccountId,
            AccountName = n.AccountName,
            BranchId = n.BranchId,
            BranchName = n.BranchName,
            UomId = n.UomId,
            UomName = n.UomName,
            UserId = n.UserId,
            UserName = n.UserName,
            ActionTime = n.ActionTime,
            StartDate = n.StartDate,
            ExpiredDate = n.ExpiredDate,
            Price = n.Price,
            Vat = n.Vat,
            SD = n.SD,
            AvgRate = n.AvgRate,
            TotalQuantity = n.TotalQuantity,
            VatPercentage = n.VatPercentage,
            CurrentSellingPrice = n.CurrentSellingPrice,
            IsActive = n.IsActive,
            StockLimitQuantity = n.StockLimitQuantity,
            TaxRateId = n.TaxRateId,
            Brand = n.Brand,
            PartNumber = n.PartNumber,
            ItemDescription = n.ItemDescription,
            OriginId = n.OriginId,
            OriginName = n.OriginName,
            StdPurchasePrice = n.StdPurchasePrice,
            AltUomId = n.AltUomId,
            AltUomName = n.AltUomName,
            ConversionUnit = n.ConversionUnit,
            IsSerial = n.IsSerial,
            MaximumDiscountPercent = n.MaximumDiscountPercent,
            IsBatchManage = n.IsBatchManage,
            HSCode = n.HSCode,

        }).Distinct().ToListAsync();
        var data = await _dataSource.Items.ExecuteDeleteAsync();
        foreach (var singleItem in distinctitems)
        {
            _dataSource.Entry(singleItem).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();
    }

    public async Task<int> DeleteItemAsync()
    {
        await _dataSource.SellingPriceRows.ExecuteDeleteAsync();
        var data = await _dataSource.Items.ExecuteDeleteAsync();
        return data;
    }
    public async Task<int> DeletePartnerAsync()
    {
        var data = await _dataSource.Partners.ExecuteDeleteAsync();
        return data;

    }
    #endregion
    #region
    public async Task<MessageHelper> CreateSettings(TblSettings settings)
    {
        try
        {
            MessageHelper msg = new MessageHelper();
            var dt = await _dataSource.Settings.AsNoTracking().ToListAsync();
            if (!dt.Any())
            {
                _dataSource.Entry(settings).State = EntityState.Added;
                await _dataSource.SaveChangesAsync();

                msg.Message = "Successfully Created";
                msg.StatusCode = 200;

            }
            else
            {
                settings.intID = dt.FirstOrDefault().intID;
                _dataSource.Entry(settings).State = EntityState.Modified;
                await _dataSource.SaveChangesAsync();

                msg.Message = "Successfully Created";
                msg.StatusCode = 200;
                //msg.Message = "Data Already Exit";
                //msg.StatusCode = 200;
            }
            return msg;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    #endregion

    #region --- Counter ---
    public async Task<MessageHelper> CreatePosCounterAsync(CreatePoscounterDTO counter)
    {
        try
        {
            MessageHelper msg = new MessageHelper();
            DateTime currentDateTime = DateTime.Now.BD();
            if (counter.CounterId != 0)
            {
                var cter = await _dataSource.Counters.Where(x => x.CounterId == counter.CounterId
                                                                && x.AccountId == counter.AccountId
                                                                && x.IsActive == true).FirstOrDefaultAsync();

                if (cter == null)
                {
                    msg.Message = "Counter Not Found";
                    msg.StatusCode = 200;
                }
                else
                {
                    counter.CounterName = counter.CounterName;
                    counter.ActionById = counter.ActionById;

                    _dataSource.Entry(cter).State = EntityState.Modified;
                    await _dataSource.SaveChangesAsync();

                    msg.Message = "Counter Update Successfully";
                    msg.StatusCode = 200;
                }
            }
            else
            {
                var Count = await _dataSource.Counters.Where(x => x.WarehouseId == counter.WarehouseId).CountAsync();
                var Code = "COU-" + Count + 1;
                var entity = new POSCounter
                {
                    CounterName = counter.CounterName,
                    CounterCode = Code,
                    AccountId = counter.AccountId,
                    BranchId = counter.BranchId,
                    OfficeId = counter.OfficeId,
                    WarehouseId = counter.WarehouseId,
                    CounterOpeningDate = currentDateTime,
                    ActionById = counter.ActionById,
                    IsActive = true,
                    LastActionDatetime = currentDateTime
                };
                _dataSource.Entry(entity).State = EntityState.Added;
                msg.Message = "Counter Create Successfully";
                msg.StatusCode = 200;
            }
            await _dataSource.SaveChangesAsync();
            return msg;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<POSCounter> GetCounterAsync(long Id)
    {
        return await _dataSource.Counters.Where(x => x.CounterId == Id).FirstOrDefaultAsync();
    }
    #endregion

    #region --- Counter Session ---
    public async Task<MessageHelper> CreatePosCounterSessionAsync(CreateCounterSeason create)
    {
        try
        {
            MessageHelper msg = new MessageHelper();
            if (create.Session.CounterSessionId != 0)
            {
                var counterSession = await _dataSource.CounterSessions.Where(x => x.CounterSessionId == create.Session.CounterSessionId
                                                                                   && x.IsActive == true).FirstOrDefaultAsync();
                if (counterSession == null)
                {
                    msg.Message = "Session Not Found";
                    msg.StatusCode = 200;
                }
                //counterSession.TotalInvoice = create.Session.TotalInvoice;
                //counterSession.TotalSales = create.Session.TotalSales;
                //counterSession.CardAmountCollection = create.Session.CashAmountCollection;
                //counterSession.MFSAmountCollection = create.Session.MFSAmountCollection;
                //counterSession.CashAmountCollection = create.Session.CashAmountCollection;
                counterSession.ClosingDatetime = create.Session.ClosingDatetime;
                counterSession.ClosingCash = create.Session.ClosingCash;
                counterSession.ClosingNote = create.Session.ClosingNote;
                counterSession.ActionById = create.Session.ActionById;
                counterSession.IsActive = true;

                _dataSource.Entry(counterSession).State = EntityState.Modified;
                await _dataSource.SaveChangesAsync();

                var row = await _dataSource.CounterSessionDetails.Where(x => x.CounterSessionId == counterSession.CounterSessionId).ToListAsync();

                List<CounterSessionDetails> rowList = new List<CounterSessionDetails>();


                foreach (var i in row)
                {
                    var check = create.SessionDetails.Where(x => x.CurrencyName == i.CurrencyName).FirstOrDefault();
                    if (check == null)
                    {
                        continue;
                    }
                    i.CurrencyClosingCount = check.CurrencyClosingCount;
                    i.ActionById = check.ActionById;
                    rowList.Add(i);
                }
                foreach (var item in rowList)
                {
                    _dataSource.Entry(item).State = EntityState.Modified;
                }

                await _dataSource.SaveChangesAsync();

                msg.Message = "Close Successfully";
                msg.StatusCode = 200;

            }
            else
            {

                DateTime currentDate = DateTime.Now.BD();
                var check = await _dataSource.CounterSessions.Where(x => x.CounterId == create.Session.CounterId
                                                                    && x.BranchId == create.Session.BranchId && x.ClosingDatetime == null).AsNoTracking().FirstOrDefaultAsync();
                if (check == null)
                {
                    var head = new CounterSession()
                    {

                        AccountId = create.Session.AccountId,
                        BranchId = create.Session.BranchId,
                        OfficeId = create.Session.OfficeId,
                        CounterId = create.Session.CounterId,
                        CounterCode = create.Session.CounterCode,
                        OpeningCash = create.Session.OpeningCash,
                        OpeningNote = create.Session.OpeningNote,
                        StartDatetime = create.Session.StartDatetime,
                        TotalInvoice = 0,
                        TotalSales = 0,
                        CardAmountCollection = 0,
                        MFSAmountCollection = 0,
                        CashAmountCollection = 0,
                        ActionById = create.Session.ActionById,
                        IsActive = true,
                        LastActionDatetime = currentDate,
                        ServerDatetime = currentDate,
                        IsSync = false
                    };
                    _dataSource.Entry(head).State = EntityState.Added;
                    await _dataSource.SaveChangesAsync();
                    var row = create.SessionDetails.Select(x => new CounterSessionDetails
                    {
                        AccountId = x.AccountId,
                        BranchId = x.BranchId,
                        CounterId = x.CounterId,
                        CounterSessionId = head.CounterSessionId,
                        CurrencyName = x.CurrencyName,
                        CurrencyOpeningCount = x.CurrencyOpeningCount,
                        ActionById = x.ActionById,
                        LastActionDatetime = currentDate,
                        IsSync = false,

                    }).ToList();
                    foreach (var item in row)
                    {
                        _dataSource.Entry(item).State = EntityState.Added;
                    }
                    await _dataSource.SaveChangesAsync();

                    msg.Message = "Session Created Successfully";
                    msg.StatusCode = 200;
                    msg.ReferanceCode = 0;
                }
                else
                {
                    msg.Message = "Session Already Created";
                    msg.StatusCode = 200;
                    msg.ReferanceCode = 1;
                }
            }
            return msg;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

    public async Task<CounterSession> GetPosCounterSession()
    {

        CounterSession dt = new CounterSession();
        var session = await _dataSource.CounterSessions.Where(x => x.ClosingDatetime == null).FirstOrDefaultAsync();
        if (session != null)
        {
            dt = session;
        }
        else
        {
            dt = null;
        }
        return dt;
    }

    public async Task<List<GetCounterSessionDetailsDTO>> GetPosCounterSessionDetails(long counterSessionId)
    {
        try
        {
            var dt = await (from a in _dataSource.CounterSessions
                            join b in _dataSource.CounterSessionDetails on a.CounterSessionId equals b.CounterSessionId
                            where a.CounterSessionId == counterSessionId
                            select new GetCounterSessionDetailsDTO
                            {
                                CuerrencyName = b.CurrencyName,
                                CurrencyOpeningCount = b.CurrencyOpeningCount,
                                OpeningCash = a.OpeningCash,
                                OpeningNote = a.OpeningNote,
                                ClosingCash = a.ClosingCash ?? 0,
                                ClosingNote = a.ClosingNote,
                                CurrencyClosingCount = b.CurrencyClosingCount
                            }).ToListAsync();


            return dt;
        }
        catch (Exception)
        {

            throw;
        }
    }
    #endregion


    public async Task<List<SalesInvoiceDTO>> GetSalesInvoice(long UserId, bool isSyn)
    {
        List<SalesInvoiceDTO> dt = await (from h in _dataSource.DeliveryHeaders
                                          where h.UserId == UserId && h.OrderDate.Date <= DateTime.Today.Date && h.Draft == false
                                          && (isSyn == true ? true : h.IsSync != 1)
                                          select new SalesInvoiceDTO
                                          {
                                              SalesOrderId = h.SalesOrderId,
                                              SalesInvoice = h.SalesOrderCode,
                                              InvoiceDate = h.OrderDate,
                                              Quantity = h.TotalQuantity,
                                              CashAmount = h.CashPayment ?? 0,
                                              SalesAmount = h.NetAmount
                                          }).OrderByDescending(x => x.SalesOrderId).ToListAsync();
        int index = 1;
        foreach (var item in dt)
        {
            var amountList = _dataSource.SalesPayments.Where(w => w.SalesDeliveryId == item.SalesOrderId).ToList();
            var amount = amountList.Sum(w => w.CollectionAmount);
            item.CashAmount = item.CashAmount + amount;
            item.Sl = index++;
        }
        return dt;

    }





    public async Task<List<SalesInvoiceDTO>> GetSalesInvoiceLiveServer(long UserId, bool isSyn, string SalesInvoice)
    {
        List<SalesInvoiceDTO> dt = await (from h in _dataSource.SQLServerDeliveryHeaders
                                          where String.IsNullOrEmpty(SalesInvoice) ? (h.ActionById == UserId && h.OrderDate.Date == DateTime.Today.Date) : (h.SalesOrderCode == SalesInvoice)
                                          select new SalesInvoiceDTO
                                          {
                                              SalesOrderId = h.SalesOrderId,
                                              SalesInvoice = h.SalesOrderCode,
                                              InvoiceDate = h.ActionTime,
                                              Quantity = h.TotalQuantity,
                                              CashAmount = h.CashPayment ?? 0,
                                              SalesAmount = h.NetAmount
                                          }).OrderByDescending(x => x.SalesOrderId).ToListAsync();
        int index = 1;
        foreach (var item in dt)
        {
            var amountList = _dataSource.SqlSalesPayments.Where(w => w.SalesDeliveryId == item.SalesOrderId).ToList();
            var amount = amountList.Sum(w => w.CollectionAmount);
            item.CashAmount = item.CashAmount + amount;
            item.Sl = index++;
        }
        return dt;

    }





    public async Task<POSSalesDeliveryHeader> GetPosDeliveryHeader(string invoice)
    {
        POSSalesDeliveryHeader dt = await _dataSource.DeliveryHeaders.Where(x => x.SalesOrderCode == invoice).FirstOrDefaultAsync();
        return dt;
    }


    public async Task<SQLServerPOSSalesDeliveryHeader> GetPosLiveDeliveryHeader(string invoice)
    {
        SQLServerPOSSalesDeliveryHeader dt = await _dataSource.SQLServerDeliveryHeaders.Where(x => x.SalesOrderCode == invoice).FirstOrDefaultAsync();
        return dt;
    }


    public async Task<List<POSSalesDeliveryLine>> GetPosDeliveryLine(long salesOrderId)
    {
        List<POSSalesDeliveryLine> row = await _dataSource.SalesDeliveryLines.Where(x => x.SalesOrderId == salesOrderId).ToListAsync();
        return row;
    }

    public async Task<List<POSSalesDeliveryLine>> GetPosLiveDeliveryLine(long salesOrderId)
    {
        List<POSSalesDeliveryLine> row =( await _dataSource.SQLServerSalesDeliveryLines.Where(x => x.SalesOrderId == salesOrderId).ToListAsync()).Select(s=> new POSSalesDeliveryLine
        {
             
            SalesOrderId=s.SalesOrderId,
            ItemId=s.ItemId,
            ItemName=s.ItemName,
            UomId=s.UomId,
            UomName=s.UomName,
            Quantity=s.Quantity,
            ChangeQuantity=s.ChangeQuantity,
            Price=s.Price,
            TotalAmount=s.TotalAmount,
            LineDiscount=s.LineDiscount,
            NetAmount=s.NetAmount,
            VatPercentage=s.VatPercentage,
            WarrantyExpiredDate=s.WarrantyExpiredDate,
            WarrantyDescription=s.WarrantyDescription,
            WarrantyInMonth=s.WarrantyInMonth,
            HeaderDiscountProportion=s.HeaderDiscountProportion,
            HeaderCostProportion=s.HeaderCostProportion,
            CostPrice=s.CostPrice,
            CostTotal=s.CostTotal,
            AnonymousAddress=s.AnonymousAddress,
            WarehouseId=s.WarehouseId,
            SdPercentage=s.SdPercentage,
            VatAmount=s.VatAmount,
            SdAmount=s.SdAmount,
            DiscountType=s.DiscountType,
            DiscountAmount=s.DiscountAmount,
            OfferItemName=s.OfferItemName,
            OfferItemQty=s.OfferItemQty,
            OfferItemId=s.OfferItemId,
            IsOfferItem=s.IsOfferItem,
            ItemBasePriceInclusive=s.ItemBasePriceInclusive,
            ItemDescription=s.ItemDescription,
            FreeTypeId=s.FreeTypeId,
            FreeTypeName=s.FreeTypeName,
            ItemSerial=s.ItemSerial,
            Batch=s.Batch,
            ExchangeReferenceId=s.ExchangeReferenceId,
            OtherDiscount=s.OtherDiscount,

        }).ToList();
        return row;
    }
    public async Task<List<POSSalesDeliveryLineDTO>> GetPosLiveDeliveryLineItem(long salesOrderId)
    {
        List<POSSalesDeliveryLineDTO> row = await (from a in _dataSource.SQLServerSalesDeliveryLines
                                                   join b in _dataSource.SQLItems on a.ItemId equals b.ItemId
                                                   where a.SalesOrderId == salesOrderId
                                                   select new POSSalesDeliveryLineDTO
                                                   {
                                                       Id = a.Id,
                                                       SalesOrderId = salesOrderId,
                                                       ItemId = a.ItemId,
                                                       BarCode = b.Barcode,
                                                       VatAmount = Math.Round(a.VatAmount.GetValueOrDefault(), 2),
                                                       SdAmount = Math.Round(a.SdAmount.GetValueOrDefault(), 2),
                                                       ItemName = a.ItemName,
                                                       Quantity = a.Quantity,
                                                       Price = Math.Round( a.Price,2),
                                                       DiscountAmount = Math.Round(a.DiscountAmount.GetValueOrDefault(),2),
                                                       TotalAmount = Math.Round(a.TotalAmount,2),
                                                   }).ToListAsync();
        long index = 0;
        foreach (var item in row)
        {
            item.VatAmount = Math.Round(item.VatAmount.GetValueOrDefault(), 2);
            item.SdAmount = Math.Round(item.SdAmount.GetValueOrDefault(), 2);
            item.Price = Math.Round(item.Price, 2);
            item.DiscountAmount = Math.Round(item.DiscountAmount.GetValueOrDefault(), 2);
            item.SL = index + 1;
            index++;
        }
        return row;
    }


    public async Task<List<POSSalesDeliveryLineDTO>> GetPosDeliveryLineItem(long salesOrderId)
    {
        List<POSSalesDeliveryLineDTO> row = await (from a in _dataSource.SalesDeliveryLines
                                                   join b in _dataSource.Items on a.ItemId equals b.ItemId
                                                   where a.SalesOrderId == salesOrderId
                                                   select new POSSalesDeliveryLineDTO
                                                   {
                                                       Id = a.Id,
                                                       SalesOrderId = salesOrderId,
                                                       ItemId = a.ItemId,
                                                       BarCode = b.Barcode,
                                                       VatAmount = a.VatAmount,
                                                       SdAmount = a.SdAmount,
                                                       ItemName = a.ItemName,
                                                       Quantity = a.Quantity,
                                                       Price = a.Price,
                                                       DiscountAmount = a.DiscountAmount,
                                                       TotalAmount = a.TotalAmount,
                                                   }).ToListAsync();
        long index = 0;
        foreach (var item in row)
        {
            item.SL = index + 1;
            index++;
        }
        return row;
    }
    public async Task<List<POSSalesPaymentDTO>> GetSalesPayment(long salesOrderId)
    {
        List<POSSalesPaymentDTO> dt = await (from a in _dataSource.SalesPayments
                                             join b in _dataSource.SalesWallets on a.WalletId equals b.WalletId
                                             where a.SalesDeliveryId == salesOrderId
                                             && a.IsActive == true
                                             select new POSSalesPaymentDTO
                                             {
                                                 WalletId = a.WalletId,
                                                 WalletName = b.WalletName,
                                                 AccountId = a.AccountId,
                                                 BranchId = a.BranchId,
                                                 CollectionAmount = Math.Round(a.CollectionAmount,2),
                                                 POSSalesPaymentId = a.POSSalesPaymentId,
                                                 SalesDeliveryId = a.SalesDeliveryId,
                                                 OfficeId = a.OfficeId,
                                                 IsActive = a.IsActive,
                                                 ActionById = a.ActionById,
                                                 IsSync = a.IsSync,
                                                 TransactionDate = a.TransactionDate,
                                                 LastActionDatetime = a.LastActionDatetime,
                                                 ReferanceNo = a.ReferanceNo
                                             }).ToListAsync();
        long index = 1;
        foreach (var item in dt)
        {
            item.CollectionAmount = Math.Round(item.CollectionAmount, 2);
            item.SL = index;
            index++;
        }
        return dt;
    }



    public async Task<List<POSSalesPaymentDTO>> GetLiveSalesPayment(long salesOrderId)
    {
        List<POSSalesPaymentDTO> dt = await (from a in _dataSource.SqlSalesPayments  //SalesPayments
                                             join b in _dataSource.SalesWallets on a.WalletId equals b.WalletId
                                             where a.SalesDeliveryId == salesOrderId
                                             && a.IsActive == true
                                             select new POSSalesPaymentDTO
                                             {
                                                 WalletId = a.WalletId,
                                                 WalletName = b.WalletName,
                                                 AccountId = a.AccountId,
                                                 BranchId = a.BranchId,
                                                 CollectionAmount =Math.Round( a.CollectionAmount,2),
                                                 POSSalesPaymentId = a.POSSalesPaymentId,
                                                 SalesDeliveryId = a.SalesDeliveryId,
                                                 OfficeId = a.OfficeId,
                                                 IsActive = a.IsActive,
                                                 ActionById = a.ActionById,
                                                 IsSync = true,
                                                 TransactionDate = a.TransactionDate,
                                                 LastActionDatetime = a.LastActionDatetime,
                                                 ReferanceNo = a.ReferanceNo
                                             }).ToListAsync();
        long index = 1;
        foreach (var item in dt)
        {
            item.CollectionAmount = Math.Round(item.CollectionAmount, 2);
            item.SL = index;
            index++;
        }
        return dt;
    }




    #region SQL server Service
    public int GetTotalItemCounts(long AccountId, long BranchId)
    {
        var totalItemCount = _dataSource.Items.Where(n => n.AccountId == AccountId && n.BranchId == BranchId).Select(n => n.ItemId).Count();
        return totalItemCount;
    }
    public async Task<List<Item>> GetSQLAllItems(long AccountId, long BranchId, int skipIndex)
    {
        var ItemList = await _dataSource.Items.Where(n => n.AccountId == AccountId && n.BranchId == BranchId).Skip(skipIndex).Take(15000).Select(s => new Item
        {
            ItemId = Convert.ToInt64(s.ItemId),
            ItemName = s.ItemName,
            Barcode = s.Barcode,
            ItemCategoryId = s.ItemCategoryId,
            ItemSubCategoryId = s.ItemSubCategoryId,
            UomId = s.UomId,
            UomName = s.UomName,
            Price = s.Price,
            Vat = s.Vat,
            SD = s.SD,
            MaximumDiscountPercent = s.MaximumDiscountPercent,
            TotalQuantity = s.TotalQuantity,
            CurrentSellingPrice = s.CurrentSellingPrice,
            MaximumDiscountAmount = s.MaximumDiscountAmount ?? 0.0M,
            IsNegativeSales = s.IsNegativeSales,
            SupplierId = s.SupplierId,
            IsMultipleSalesPrice = s.IsMultipleSalesPrice,
        }).ToListAsync();

        return ItemList;
    }

    public int GetTotalWarehouseCounts(long AccountId, long BranchId, long OfficeId, long WareHouseId)
    {
        var totalWarehouseCount = _dataSource.Warehouses.Where(n => n.AccountId == AccountId && n.BranchId == BranchId && n.OfficeId == OfficeId && (WareHouseId == 0 || n.WarehouseId == WareHouseId)).Select(n => n.WarehouseId).Count();
        return totalWarehouseCount;
    }

    public async Task<List<Warehouse>> GetSQLAllWarehouses(long AccountId, long BranchId, int skipIndex, long OfficeId, long WareHouseId)
    {
        var WarehouseList = await _dataSource.Warehouses.Where(n => n.AccountId == AccountId && n.BranchId == BranchId && n.OfficeId == OfficeId && (WareHouseId == 0 || n.WarehouseId == WareHouseId)).Skip(skipIndex).Take(15000).Select(s => new Warehouse
        {
            WarehouseId = s.WarehouseId,
            WarehouseCode = s.WarehouseCode,
            WarehouseName = s.WarehouseName,
            WarehouseAddress = s.WarehouseAddress,
            AccountId = s.AccountId,
            IsDefaultWH = s.IsDefaultWH,
            ActionBy = s.ActionBy,
            LastActionDate = s.LastActionDate,
            IsActive = s.IsActive,
            BranchId = s.BranchId,
            OfficeId = s.OfficeId,
            isCentral = s.isCentral,
            isWastageWH = s.isWastageWH,
            IsSync = true,
        }).ToListAsync();

        return WarehouseList;
    }

    public int GetTotalPartnerCounts(long AccountId, long BranchId)
    {
        var totalPartnerCount = _dataSource.Partners.Where(n => n.AccountId == AccountId && n.BranchId == BranchId && n.PartnerTypeId == 1).Select(n => n.PartnerId).Count();
        return totalPartnerCount;
    }

    public async Task<List<Partner>> GetSQLAllPartners(long AccountId, long BranchId, int skipIndex)
    {
        var PartnerList = await _dataSource.Partners.Where(n => n.AccountId == AccountId && n.BranchId == BranchId && n.PartnerTypeId == 1).Skip(skipIndex).Take(15000).Select(s => new Partner
        {
            PartnerId = s.PartnerId,
            PartnerName = s.PartnerName,
            PartnerCode = s.PartnerCode,
            NID = s.NID,
            PartnerTypeId = s.PartnerTypeId,
            PartnerTypeName = s.PartnerTypeName,
            TaggedEmployeeId = s.TaggedEmployeeId,
            TaggedEmployeeName = s.TaggedEmployeeName,
            Address = s.Address,
            City = s.City,
            Email = s.Email,
            MobileNo = s.MobileNo,
            AccountId = s.AccountId,
            BranchId = s.BranchId,
            AdvanceBalance = s.AdvanceBalance,
            CreditLimit = s.CreditLimit,
            ActionById = s.ActionById,
            ActionByName = s.ActionByName,
            ActionTime = s.ActionTime,
            isActive = s.isActive,
            OtherContactNumber = s.OtherContactNumber,
            OtherContactName = s.OtherContactName,
            PartnerBalance = s.PartnerBalance,
            PartnerGroupId = s.PartnerGroupId,
            PartnerGroupName = s.PartnerGroupName,
            PriceTypeId = s.PriceTypeId,
            PriceTypeName = s.PriceTypeName,
            BinNumber = s.BinNumber,
            IsForeign = s.IsForeign,
            TerritoryId = s.TerritoryId,
            DistrictId = s.DistrictId,
            ThanaId = s.ThanaId,
            //IsSync = true,
            Points = s.Points,
            PointsAmount = s.PointsAmount,
        }).ToListAsync();

        return PartnerList;
    }

    public int GetTotalSellingPriceCounts(long AccountId, long BranchId, long OfficeId, long WareHouseId)
    {
        //var totalItemSellingPriceCount = _dataSource.SQLServerSellingPriceRows.Select(n => n.RowId).Count();

        var totalItemSellingPriceCount = (from p in _dataSource.SQLServerSellingPriceRows
                                          join h in _dataSource.SQLItemSellingPriceHeaders on p.HeaderId equals h.HeaderId
                                          where h.WarehouseId == WareHouseId
                                          && p.Qty != 0
                                          select p.RowId).Count();

        return totalItemSellingPriceCount;
    }
    public async Task<List<ItemSellingPriceRow>> GetSQLAllItemSellingPrice(long AccountId, long BranchId, int skipIndex, long OfficeId, long WareHouseId)
    {
        var SellingPriceList = await (from p in _dataSource.SQLServerSellingPriceRows
                                      join h in _dataSource.SQLItemSellingPriceHeaders on p.HeaderId equals h.HeaderId
                                      join i in _dataSource.Items on p.ItemId equals i.ItemId
                                      where h.WarehouseId == WareHouseId
                                      select p).Skip(skipIndex).Take(5000).Select(s => new ItemSellingPriceRow
                                      {
                                          RowId = s.RowId,
                                          HeaderId = s.HeaderId,
                                          ItemId = s.ItemId,
                                          ItemCode = s.ItemCode,
                                          OldPrice = s.OldPrice,
                                          NewPrice = s.NewPrice,
                                          Qty = s.Qty,
                                          IsActive = s.IsActive,
                                          ActionById = s.ActionById,
                                          LastActionDatetime = s.LastActionDatetime,
                                      }).ToListAsync();

        return SellingPriceList;
    }

    public int GetTotalWarehouseBalanceItemCounts(long AccountId, long BranchId)
    {
        var totalItemBalanceCount = _dataSource.ItemBalance.Select(n => n.ItemWarehouseBalanceId).Count();
        return totalItemBalanceCount;
    }
    public async Task<List<ItemWarehouseBalance>> GetSQLAllItemWarehouseBalance(long AccountId, long BranchId, int skipIndex)
    {
        var balanceItemList = await _dataSource.ItemBalance.Skip(skipIndex).Take(5000).Select(s => new ItemWarehouseBalance
        {
            ItemWarehouseBalanceId = s.ItemWarehouseBalanceId,
            WarehouseId = s.WarehouseId,
            ItemId = s.ItemId,
            CurrentStock = s.CurrentStock,
        }).ToListAsync();

        return balanceItemList;
    }

    public async Task<List<SQLServerItemSellingPriceRow>> ItemSellingPriceRowChange(long AccountId, long BranchId, long intWarehouseId)
    {

        var ItemInformationChangedList = await _dataSource.SQLServerSellingPriceRows.FromSqlRaw<SQLServerItemSellingPriceRow>(" exec GettingChangedSellingPrice " + AccountId + "," + BranchId + "," + intWarehouseId).ToListAsync();
        return ItemInformationChangedList;

    }

    public async Task<List<Item>> ItemRowChange(long AccountId, long BranchId)
    {

        var ItemInformationChangedList = await _dataSource.Items.FromSqlRaw<Item>(" exec GettingChangedItemList " + AccountId + "," + BranchId).ToListAsync();
        return ItemInformationChangedList;
    }
    public async Task<List<Partner>> PartnerRowChange(long AccountId, long BranchId)
    {
        //try
        //{
        var PartnerChangedList = await _dataSource.Partners.FromSqlRaw<Partner>(" exec GettingChangedPartnerList " + AccountId + "," + BranchId).ToListAsync();
        return PartnerChangedList;
        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}



    }

    public async Task<List<SalesWallet>> GetWalletInformationfromSQLServer(long AccountId, long BranchId)
    {
        var walletInformation = await _dataSource.SalesWallets.Where(n => n.AccountId == AccountId && n.BranchId == BranchId).Select(n => new SalesWallet
        {
            WalletId = n.WalletId,
            WalletName = n.WalletName,
            AccountId = n.AccountId,
            BranchId = n.BranchId,
            ComissionPercentage = n.ComissionPercentage,
            BankAccountId = n.BankAccountId,
            BankAccNo = n.BankAccNo,
            isBank = n.isBank,
            isActive = n.isActive,
            LastActionDateTime = n.LastActionDateTime,
            IsSync = true,
        }).ToListAsync();
        return walletInformation;
    }
    public async Task<MessageHelper> SaveWalletInformationtoSQLiteServer(List<SalesWallet> walletList)
    {
        if (walletList.Count > 0)
        {
            await _dataSource.SalesWallets.ExecuteDeleteAsync();
            foreach (var singleWaller in walletList)
            {
                _dataSource.Entry(singleWaller).State = EntityState.Added;
            }
            await _dataSource.SaveChangesAsync();

            return new MessageHelper() { StatusCode = 200 };
        }
        return new MessageHelper() { StatusCode = 400, Message = "No Data Found" };
    }

    public async Task<List<CreateSalesDeliveryDTO>> GetSalesDeliveryInformationfromSQLite(long accountId, long branchId, int takeItems)
    {
        List<CreateSalesDeliveryDTO> response = new List<CreateSalesDeliveryDTO>();

        var headerInformation = await _dataSource.DeliveryHeaders.Where(n => n.AccountId == accountId && n.BranchId == branchId && n.Draft == false && n.IsSync == 0).Take(takeItems).ToListAsync();
        if (headerInformation.Count > 0)
        {

            foreach (var singleDeliveryHeader in headerInformation)
            {
                CreateSalesDeliveryDTO createSalesDeliveryDTO = new CreateSalesDeliveryDTO();
                var rowList = await _dataSource.SalesDeliveryLines.Where(n => n.SalesOrderId == singleDeliveryHeader.SalesOrderId).ToListAsync();
                var walletInformation = await _dataSource.SalesPayments.Where(n => n.SalesDeliveryId == singleDeliveryHeader.SalesOrderId).ToListAsync();

                createSalesDeliveryDTO.pOSSalesDeliveryHeader = singleDeliveryHeader;
                createSalesDeliveryDTO.pOSSalesDeliveryLine = rowList;
                createSalesDeliveryDTO.pOSSalesPayments = walletInformation;
                response.Add(createSalesDeliveryDTO);
            }
        }

        var header = await _dataSource.DeliveryHeaders
                                        .Where(n => n.AccountId == accountId && n.BranchId == branchId && n.IsSync == 1 && n.OrderDate <= DateTime.Now.AddDays(-8)).ToListAsync();
        var headerIds = header
                            .Select(s => s.SalesOrderId).ToList();
        var rowListDelete = await _dataSource.SalesDeliveryLines
                                    .Where(n => headerIds.Contains(n.SalesOrderId)).Select(s => s.Id).ToListAsync();
        await _dataSource.SalesDeliveryLines
                         .Where(w => rowListDelete.Contains(w.Id)).ExecuteDeleteAsync();


        await _dataSource.DeliveryHeaders
                         .Where(w => headerIds.Contains(w.SalesOrderId)).ExecuteDeleteAsync();
        await _dataSource.SalesPayments
                         .Where(x => headerIds.Contains(x.SalesDeliveryId)).ExecuteDeleteAsync();
        return response;
    }

    public async Task<MessageHelper> CreateSalesDeliveryInformationIntoSQLServer(CreateSalesDeliveryDTO obj, TblSettings setting)
    {
        //var data = await _dataSource.SQLServerDeliveryHeaders.Where(w => w.SalesOrderCode == obj.pOSSalesDeliveryHeader.SalesOrderCode
        //       && w.AccountId == obj.pOSSalesDeliveryHeader.AccountId && w.BranchId == obj.pOSSalesDeliveryHeader.BranchId
        //       && w.OfficeId == obj.pOSSalesDeliveryHeader.OfficeId && w.CounterId == obj.pOSSalesDeliveryHeader.CounterId).FirstOrDefaultAsync();
        //if (data != null)
        //{
        //    return new MessageHelper() { StatusCode = 200 };
        //}
        SalesDeliveryHeaderDTO headerDTO = new SalesDeliveryHeaderDTO()
        {
            accountId = obj.pOSSalesDeliveryHeader.AccountId,
            SalesOrderCode = obj.pOSSalesDeliveryHeader.SalesOrderCode,
            accountName = obj.pOSSalesDeliveryHeader.AccountName,
            actionById = obj.pOSSalesDeliveryHeader.UserId,
            actionByName = obj.pOSSalesDeliveryHeader.ActionByName,
            advanceBalanceAdjust = obj.pOSSalesDeliveryHeader.AdvanceBalanceAdjust ?? 0.0M,
            amountPerInstallment = obj.pOSSalesDeliveryHeader.AmountPerInstallment,
            anonymousAddress = obj.pOSSalesDeliveryHeader.AnonymousAddress,
            bankReceiveAmount = 0.0M, //obj.pOSSalesDeliveryHeader.BankReceiveAmount ?? 0.0M,
            branchId = obj.pOSSalesDeliveryHeader.BranchId,
            branchName = obj.pOSSalesDeliveryHeader.BranchName,
            cashReceiveAmount = obj.pOSSalesDeliveryHeader.CashPayment ?? 0.0M,
            challanNo = obj.pOSSalesDeliveryHeader.ChallanNo,
            comissionPercentage = obj.pOSSalesDeliveryHeader.ComissionPercentage ?? 0.0M,
            createDate = obj.pOSSalesDeliveryHeader.OrderDate.ToString("yyyy-MM-dd"),
            customerId = obj.pOSSalesDeliveryHeader.CustomerId,
            customerName = obj.pOSSalesDeliveryHeader.CustomerName,
            customerNetAmount = obj.pOSSalesDeliveryHeader.CustomerNetAmount ?? 0,
            customerPO = obj.pOSSalesDeliveryHeader.CustomerPO,
            deliveryDate = obj.pOSSalesDeliveryHeader.DeliveryDate.ToString("yyyy-MM-dd"),
            discoundItemTotalPrice = obj.pOSSalesDeliveryHeader.DiscoundItemTotalPrice ?? 0.0M,
            freeTypeId = obj.pOSSalesDeliveryHeader.FreeTypeId,
            freeTypeName = obj.pOSSalesDeliveryHeader.FreeTypeName,
            headerDiscount = obj.pOSSalesDeliveryHeader.HeaderDiscount,
            headerDiscountPercentage = obj.pOSSalesDeliveryHeader.HeaderDiscountPercentage,
            installmentStartDate = obj.pOSSalesDeliveryHeader.InstallmentStartDate.ToString("yyyy-MM-dd"),
            installmentType = obj.pOSSalesDeliveryHeader.InstallmentType,
            interestRate = obj.pOSSalesDeliveryHeader.InterestRate,
            isAdvanceAdjust = false,//obj.pOSSalesDeliveryHeader.IsAdvanceAdjust,
            isBank = false,//obj.pOSSalesDeliveryHeader.IsBank,
            isCash = obj.pOSSalesDeliveryHeader.CashPayment > 0 ? true : false,
            isComplete = true,// obj.pOSSalesDeliveryHeader.isComplete
            isCompletedCustomerOrder = true,//obj.pOSSalesDeliveryHeader.isCompletedCustomerOrder
            isInclusive = false,//obj.pOSSalesDeliveryHeader.IsInclusive
            isPosSales = true, //obj.pOSSalesDeliveryHeader.isPosSales
            isReturnAdjustInAdvance = false,//obj.pOSSalesDeliveryHeader.IsReturnAdjustInAdvance,
            itemTotalAmount = obj.pOSSalesDeliveryHeader.ItemTotalAmount,
            narration = obj.pOSSalesDeliveryHeader.Narration,
            netAmount = obj.pOSSalesDeliveryHeader.NetAmount,
            netAmountWithInterest = obj.pOSSalesDeliveryHeader.NetAmountWithInterest,
            netDiscount = obj.pOSSalesDeliveryHeader.NetDiscount,
            offerItemTotal = obj.pOSSalesDeliveryHeader.OfferItemTotal ?? 0.0M,
            officeId = obj.pOSSalesDeliveryHeader.OfficeId,
            orderDate = obj.pOSSalesDeliveryHeader.OrderDate.ToString("yyyy-MM-dd"),
            orderId = obj.pOSSalesDeliveryHeader.SalesOrderId,
            othersCost = obj.pOSSalesDeliveryHeader.OthersCost,
            paymentTypeId = obj.pOSSalesDeliveryHeader.PaymentTypeId,
            paymentTypeName = obj.pOSSalesDeliveryHeader.PaymentTypeName,
            pendingAmount = obj.pOSSalesDeliveryHeader.PendingAmount,
            phone = obj.pOSSalesDeliveryHeader.Phone,
            points = obj.pOSSalesDeliveryHeader.Points ?? 0.0M,
            projectName = obj.pOSSalesDeliveryHeader.ProjectName,
            receiveAmount = obj.pOSSalesDeliveryHeader.ReceiveAmount,
            receiveBankAccountId = 1,//obj.pOSSalesDeliveryHeader.ReceiveBankAccountId,
            remarks = obj.pOSSalesDeliveryHeader.Remarks,
            salesForceId = obj.pOSSalesDeliveryHeader.SalesForceId,
            salesForceName = obj.pOSSalesDeliveryHeader.SalesForceName,
            salesOrderId = obj.pOSSalesDeliveryHeader.SalesOrderId,
            shippingAddressId = obj.pOSSalesDeliveryHeader.ShippingAddressId ?? 0,
            shippingAddressName = obj.pOSSalesDeliveryHeader.ShippingAddressName,
            shippingContactPerson = obj.pOSSalesDeliveryHeader.ShippingContactPerson ?? "Not Available",
            status = true,//obj.pOSSalesDeliveryHeader.Status
            statusType = "",//obj.pOSSalesDeliveryHeader.StatusType
            totalLineDiscount = obj.pOSSalesDeliveryHeader.TotalLineDiscount,
            totalNoOfInstallment = obj.pOSSalesDeliveryHeader.TotalNoOfInstallment,
            totalQuantity = Convert.ToInt64(obj.pOSSalesDeliveryHeader.TotalQuantity),
            totalSd = obj.pOSSalesDeliveryHeader.TotalSd ?? 0.0M,
            totalVat = obj.pOSSalesDeliveryHeader.TotalVat ?? 0.0M,
            walletId = obj.pOSSalesDeliveryHeader.WalletId ?? 0,
            warehouseId = setting.intWarehouseId,
            ReturnAmount = obj.pOSSalesDeliveryHeader.ReturnAmount.GetValueOrDefault(),
            ISExchange = obj.pOSSalesDeliveryHeader.ISExchange ?? false,
            CounterId = obj.pOSSalesDeliveryHeader.CounterId,
            HeaderDiscountId = obj.pOSSalesDeliveryHeader.HeaderDiscountId ?? 0,
            IsOnline = obj.IsOnline  ,
            IsReturn= obj.pOSSalesDeliveryHeader.ISReturn.GetValueOrDefault()
        };



        var rows = obj.pOSSalesDeliveryLine.Select(n => new ItemInfo
        {
            batchList = new List<Batch>(),
            cogs = 0,//n.cogs,
            discountAmount = n.DiscountAmount ?? 0.0M,
            discountType = n.DiscountType,
            freeItemUomId = 0,//n.FreeItemUomId,
            freeItemUomName = "", //n.freeItemUomName,
            freeTypeId = 5,// n.FreeTypeId,
            freeTypeName = "",// n.FreeTypeName,
            isOfferItem = false,//n.isOfferItem,
            itemBasePriceInclusive = 0, //n.ItemBasePriceInclusive,
            itemDescription = n.ItemDescription,
            itemId = n.ItemId,
            itemName = n.ItemName,
            lineDiscount = n.LineDiscount,
            lineDiscountRate = 0.0M, //n.LineDiscountRate ?? 0,
            netAmount = n.NetAmount,
            offerItemId = n.OfferItemId,
            offerItemName = n.OfferItemName,
            offerItemQty = n.OfferItemQty,
            price = n.Price,
            quantity = n.Quantity,
            sdAmount = n.SdAmount ?? 0.0M,
            sdPercentage = n.SdPercentage ?? 0.0M,
            serialrowData = new List<long>(), //n.SerialrowData,
            totalAmount = n.TotalAmount,
            uomId = n.UomId,
            uomName = n.UomName,
            vatAmount = n.VatAmount ?? 0.0M,
            vatPercentage = n.VatPercentage ?? 0.0M,
            warehouseId = setting.intWarehouseId, //n.warehouseId,
            warrantyDescription = n.WarrantyDescription,
            warrantyExpiredDate = n.WarrantyExpiredDate != null ? n.WarrantyExpiredDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.BD().ToString("yyyy-MM-dd"),
            warrantyInMonth = 1,//n.warrantyInMont,
            ExchangeReferenceId = n.ExchangeReferenceId,
            OtherDiscount=n.OtherDiscount.GetValueOrDefault()

        }).ToList();

        var walletInformationSalesDeliveryDTO = obj.pOSSalesPayments.Select(n => new WalletInformationSalesDeliveryDTO
        {
            ReferanceNo = n.ReferanceNo,
            walletId = n.WalletId,
            collectionAmount = n.CollectionAmount,
        }).ToList();

        var headerInfoStr = JsonConvert.SerializeObject(headerDTO);
        var rowInformation = JsonConvert.SerializeObject(rows.Select(s => { s.itemName = s.itemName.Replace("'", ""); return s; }));
        var walletInfo = JsonConvert.SerializeObject(walletInformationSalesDeliveryDTO);
        try
        {
            DataSet ds = new DataSet();
            using (var connection = new SqlConnection(setting.SqlServerConnString))
            {
                string sql = "[pos].[sprPOSSalesDelivery]";
                using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@intAccount", setting.intAccountId);
                    sqlCmd.Parameters.AddWithValue("@intBranchId", setting.intBranchId);
                    sqlCmd.Parameters.AddWithValue("@intInvTranTypeId", 3);
                    sqlCmd.Parameters.AddWithValue("@strInvTranTypeName", "Sales");
                    sqlCmd.Parameters.AddWithValue("@dteTranDate", DateTime.Now.BD());
                    sqlCmd.Parameters.AddWithValue("@intWarehouseId", setting.intWarehouseId);
                    sqlCmd.Parameters.AddWithValue("@objHeader", headerInfoStr);
                    sqlCmd.Parameters.AddWithValue("@objRow", rowInformation);
                    sqlCmd.Parameters.AddWithValue("@salesPaymentMethods", walletInfo);
                    //sqlCmd.Parameters["@msg"].Direction = ParameterDirection.Output;
                    connection.Open();

                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                    connection.Close();
                }
            }

            var response = JsonConvert.SerializeObject(ds.Tables[0]);
            var pOSSalesDeliveryHeader = JsonConvert.DeserializeObject<List<POSSalesDeliveryHeader>>(response);
            if (pOSSalesDeliveryHeader != null)
            {
                if (pOSSalesDeliveryHeader.FirstOrDefault().IsSync == 1)
                {
                    return new MessageHelper() { StatusCode = 200 };
                }
            }
        }
        catch (Exception ex)
        {
            var message = ex.Message;
        }

        //return ItemInformationChangedList;
        return new MessageHelper() { StatusCode = 400 };
    }


    public async Task<List<tblPromotionRow>> GetPromotionRowfromSQLServer(long AccountId, long BranchId, DateTime dateTime)
    {
        var response = await (from proh in _dataSource.SQLServerPromotionHeader
                              join pror in _dataSource.SQLServerPromotionRows on proh.intPromotionId equals pror.IntPromotionId
                              where proh.intAccountId == AccountId && (proh.dtePromotionStartDateTime <= dateTime && proh.dtePromotionEndDateTime >= dateTime) && proh.isActive == true && pror.IsActive == true

                              select new tblPromotionRow
                              {
                                  IntPromotionRowId = pror.IntPromotionRowId,
                                  IntPromotionId = pror.IntPromotionId,
                                  NumOrderValueFrom = pror.NumOrderValueFrom,
                                  NumOrderValueTo = pror.NumOrderValueTo,
                                  IntDiscountTypeId = pror.IntDiscountTypeId,
                                  StrDiscountTypeName = pror.StrDiscountTypeName,
                                  IntTypeId = Convert.ToInt32(pror.IntTypeId),
                                  IntItemId = pror.IntItemId,
                                  StrItemName = pror.StrItemName,
                                  NumDiscountValue = pror.NumDiscountValue,
                                  IsActive = pror.IsActive,

                              }).ToListAsync();
        return response;
    }



    public async Task<List<tblPointOfferRow>> GetItemOfferRowfromSQLServer(long AccountId, long BranchId, DateTime dateTime)
    {
        var response = await (from itmoffer in _dataSource.SqlServerPointOffer
                              where itmoffer.isActive == true
                              select new tblPointOfferRow
                              {
                                  intPointOfferRowId = itmoffer.intPointOfferRowId,
                                  intPointOfferId = itmoffer.intPointOfferId,
                                  intItemId = itmoffer.intItemId,
                                  strItemName = itmoffer.strItemName,
                                  isActive = itmoffer.isActive,

                              }).ToListAsync();
        return response;
    }



    public async Task<MessageHelper> CreatePromotionRowfromSQLServer(List<tblPromotionRow> promotionRows)
    {
        if (promotionRows.Count > 0)
        {
            await _dataSource.PromotionRows.ExecuteDeleteAsync();
            foreach (var singleRow in promotionRows)
            {
                _dataSource.Entry(singleRow).State = EntityState.Added;
            }
            await _dataSource.SaveChangesAsync();

            var pormotionAmount = promotionRows.Where(n => n.IntDiscountTypeId == 1).ToList();
            var promotionAmountItemids = pormotionAmount.Where(n => n.IntTypeId == 3).Select(n => n.IntItemId).ToList();
            var promotionAmountCategoryIds = pormotionAmount.Where(n => n.IntTypeId == 1).Select(n => n.IntItemId).ToList();
            var promotionAmountsubCategoryIds = pormotionAmount.Where(n => n.IntTypeId == 2).Select(n => n.IntItemId).ToList();
            var promotionAmountSupplierIds = pormotionAmount.Where(n => n.IntTypeId == 4).Select(n => n.IntItemId).ToList();


            var promotionPercentage = promotionRows.Where(n => n.IntDiscountTypeId == 2).ToList();
            var promotionPercentageItemids = promotionPercentage.Where(n => n.IntTypeId == 3).Select(n => n.IntItemId).ToList();
            var promotionPercentageCategoryIds = promotionPercentage.Where(n => n.IntTypeId == 1).Select(n => n.IntItemId).ToList();
            var promotionPercentageSubCategoryIds = promotionPercentage.Where(n => n.IntTypeId == 2).Select(n => n.IntItemId).ToList();
            var promotionPercentageSupplierIds = promotionPercentage.Where(n => n.IntTypeId == 4).Select(n => n.IntItemId).ToList();

            var ItemsList = await _dataSource.Items.Where(n => promotionAmountItemids.Contains(n.ItemId)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountAmount = pormotionAmount.Where(n => n.IntTypeId == 3 && n.IntItemId == item.ItemId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            ItemsList = await _dataSource.Items.Where(n => promotionAmountCategoryIds.Contains(n.ItemCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountAmount = pormotionAmount.Where(n => n.IntTypeId == 1 && n.IntItemId == item.ItemCategoryId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();

            ItemsList = await _dataSource.Items.Where(n => promotionAmountsubCategoryIds.Contains(n.ItemCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountAmount = pormotionAmount.Where(n => n.IntTypeId == 2 && n.IntItemId == item.ItemSubCategoryId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            ItemsList = await _dataSource.Items.Where(n => promotionAmountSupplierIds.Contains(n.ItemCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountAmount = pormotionAmount.Where(n => n.IntTypeId == 4 && n.IntItemId == item.SupplierId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();



            ItemsList = await _dataSource.Items.Where(n => promotionPercentageItemids.Contains(n.ItemId)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountPercent = promotionPercentage.Where(n => n.IntTypeId == 3 && n.IntItemId == item.ItemId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            ItemsList = await _dataSource.Items.Where(n => promotionPercentageCategoryIds.Contains(n.ItemCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountPercent = promotionPercentage.Where(n => n.IntTypeId == 1 && n.IntItemId == item.ItemCategoryId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();

            ItemsList = await _dataSource.Items.Where(n => promotionPercentageSubCategoryIds.Contains(n.ItemSubCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountPercent = promotionPercentage.Where(n => n.IntTypeId == 2 && n.IntItemId == item.ItemSubCategoryId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            ItemsList = await _dataSource.Items.Where(n => promotionPercentageSupplierIds.Contains(n.SupplierId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountPercent = promotionPercentage.Where(n => n.IntTypeId == 4 && n.IntItemId == item.SupplierId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();

            return new MessageHelper() { StatusCode = 200 };
        }
        return new MessageHelper() { StatusCode = 400, Message = "No Data Found" };
    }


    public async Task<MessageHelper> CreatePromotionRowSyncSQLServer(List<tblPromotionRow> promotionRows)
    {
        if (promotionRows.Count > 0)
        {
            var data = promotionRows.Select(s => s.IntPromotionRowId).ToList();

            var promotionRowsList = await _dataSource.PromotionRows.Where(n => data.Contains(n.IntPromotionRowId)).ToListAsync();
            foreach (var singleRow in promotionRows)
            {
                if (promotionRowsList.Where(w => w.IntPromotionRowId == singleRow.IntPromotionRowId).Any())
                {
                    _dataSource.Entry(singleRow).State = EntityState.Modified;
                }
                else
                {
                    _dataSource.Entry(singleRow).State = EntityState.Added;
                }

            }
            await _dataSource.SaveChangesAsync();

            var pormotionAmount = promotionRows.Where(n => n.IntDiscountTypeId == 1).ToList();
            var promotionAmountItemids = pormotionAmount.Where(n => n.IntTypeId == 3).Select(n => n.IntItemId).ToList();
            var promotionAmountCategoryIds = pormotionAmount.Where(n => n.IntTypeId == 1).Select(n => n.IntItemId).ToList();
            var promotionAmountsubCategoryIds = pormotionAmount.Where(n => n.IntTypeId == 2).Select(n => n.IntItemId).ToList();
            var promotionAmountSupplierIds = pormotionAmount.Where(n => n.IntTypeId == 4).Select(n => n.IntItemId).ToList();


            var promotionPercentage = promotionRows.Where(n => n.IntDiscountTypeId == 2).ToList();
            var promotionPercentageItemids = promotionPercentage.Where(n => n.IntTypeId == 3).Select(n => n.IntItemId).ToList();
            var promotionPercentageCategoryIds = promotionPercentage.Where(n => n.IntTypeId == 1).Select(n => n.IntItemId).ToList();
            var promotionPercentageSubCategoryIds = promotionPercentage.Where(n => n.IntTypeId == 2).Select(n => n.IntItemId).ToList();
            var promotionPercentageSupplierIds = promotionPercentage.Where(n => n.IntTypeId == 4).Select(n => n.IntItemId).ToList();

            var ItemsList = await _dataSource.Items.Where(n => promotionAmountItemids.Contains(n.ItemId)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountAmount = pormotionAmount.Where(n => n.IntTypeId == 3 && n.IntItemId == item.ItemId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            ItemsList = await _dataSource.Items.Where(n => promotionAmountCategoryIds.Contains(n.ItemCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountAmount = pormotionAmount.Where(n => n.IntTypeId == 1 && n.IntItemId == item.ItemCategoryId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();

            ItemsList = await _dataSource.Items.Where(n => promotionAmountsubCategoryIds.Contains(n.ItemCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountAmount = pormotionAmount.Where(n => n.IntTypeId == 2 && n.IntItemId == item.ItemSubCategoryId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            ItemsList = await _dataSource.Items.Where(n => promotionAmountSupplierIds.Contains(n.ItemCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountAmount = pormotionAmount.Where(n => n.IntTypeId == 4 && n.IntItemId == item.SupplierId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();



            ItemsList = await _dataSource.Items.Where(n => promotionPercentageItemids.Contains(n.ItemId)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountPercent = promotionPercentage.Where(n => n.IntTypeId == 3 && n.IntItemId == item.ItemId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            ItemsList = await _dataSource.Items.Where(n => promotionPercentageCategoryIds.Contains(n.ItemCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountPercent = promotionPercentage.Where(n => n.IntTypeId == 1 && n.IntItemId == item.ItemCategoryId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();

            ItemsList = await _dataSource.Items.Where(n => promotionPercentageSubCategoryIds.Contains(n.ItemSubCategoryId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountPercent = promotionPercentage.Where(n => n.IntTypeId == 2 && n.IntItemId == item.ItemSubCategoryId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            ItemsList = await _dataSource.Items.Where(n => promotionPercentageSupplierIds.Contains(n.SupplierId ?? 0)).ToListAsync();
            foreach (var item in ItemsList)
            {
                item.MaximumDiscountPercent = promotionPercentage.Where(n => n.IntTypeId == 4 && n.IntItemId == item.SupplierId).Select(n => n.NumDiscountValue).FirstOrDefault();
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();

            return new MessageHelper() { StatusCode = 200 };
        }
        return new MessageHelper() { StatusCode = 400, Message = "No Data Found" };
    }


    public async Task<MessageHelper> DeleteSessionData()
    {
        try
        {
            MessageHelper msg = new MessageHelper();
            var datetime = DateTime.Now.AddDays(-7);
            var session = await _dataSource.CounterSessions.Where(x => x.IsSync == true && x.ServerDatetime < datetime && x.ClosingDatetime != null).Select(x => x.CounterSessionId).ToListAsync();

            if (session.Count > 0)
            {
                await _dataSource.CounterSessionDetails.Where(x => session.Contains(x.CounterSessionId)).ExecuteDeleteAsync();
                await _dataSource.CounterSessions.Where(x => session.Contains(x.CounterSessionId)).ExecuteDeleteAsync();
                msg.StatusCode = 200;

            }
            return msg;
        }
        catch (Exception)
        {

            throw;
        }

    }

    public async Task<List<POSSpecialDiscount>> GetSpecialDiscountfromSQLServer(long accountId, long branchId, DateTime dateTime)
    {
        List<CreateSalesDeliveryDTO> response = new List<CreateSalesDeliveryDTO>();
        var headerInformation = await _dataSource.SQLServerSpecialDiscounts.Where(n => n.AccountId == accountId && n.BranchId == branchId && (n.StartDate <= dateTime && n.EndDate >= dateTime) && n.IsActive == true).Select(n => new POSSpecialDiscount
        {
            HeaderId = n.HeaderId,
            OfferName = n.OfferName,
            WarehouseId = n.WarehouseId,
            StartDate = n.StartDate,
            EndDate = n.EndDate,
            Remarks = n.Remarks,
            DiscountType = n.DiscountType,
            Value = n.Value,
            IsActive = n.IsActive,
            ActionById = n.ActionById,
            LastActionDatetime = n.LastActionDatetime,
            MinAmount = n.MinAmount,
            MaxAmount = n.MaxAmount,
        }).ToListAsync();

        return headerInformation;
    }

    public async Task<MessageHelper> SaveSpecialDiscountfromSQLServer(List<POSSpecialDiscount> specialDiscount)
    {

        await _dataSource.SpecialDiscounts.ExecuteDeleteAsync();
        foreach (var singlediscount in specialDiscount)
        {
            _dataSource.Entry(singlediscount).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();

        return new MessageHelper() { StatusCode = 200 };

    }





    public async Task<MessageHelper> SaveItemPointOfferRowfromSQLServer(List<tblPointOfferRow> pointOfferRow)
    {
        if (pointOfferRow.Count > 0)
        {
            await _dataSource.PointOffer.ExecuteDeleteAsync();
            foreach (var singlePoint in pointOfferRow)
            {
                _dataSource.Entry(singlePoint).State = EntityState.Added;
            }
            await _dataSource.SaveChangesAsync();

            return new MessageHelper() { StatusCode = 200 };
        }
        return new MessageHelper() { StatusCode = 400, Message = "No Data Found" };
    }





    public async Task<MessageHelper> SaveSpecialDiscountSyncSQLServer(List<POSSpecialDiscount> specialDiscount)
    {

        //var data = specialDiscount.Select(s => s.OfferName).ToList();

        await _dataSource.SpecialDiscounts.ExecuteDeleteAsync();
        foreach (var singlediscount in specialDiscount)
        {
            _dataSource.Entry(singlediscount).State = EntityState.Added;
        }
        //var SpecialDiscountList = await _dataSource.SpecialDiscounts.Where(w => data.Contains(w.OfferName)).ToListAsync();

        //foreach (var singlediscount in specialDiscount)
        //{
        //    if (SpecialDiscountList.Where(w => w.OfferName == singlediscount.OfferName).Any())
        //    {
        //        _dataSource.Entry(singlediscount).State = EntityState.Modified;
        //    }
        //    else
        //    {
        //        _dataSource.Entry(singlediscount).State = EntityState.Added;
        //    }

        //}
        await _dataSource.SaveChangesAsync();

        return new MessageHelper() { StatusCode = 200 };

        //return new MessageHelper() { StatusCode = 400, Message = "No Data Found" };
    }
    public async Task<MainViewRecallInvoiceDTO> GetSalesDeliveryInformationFromSQLServer(string InvoiceCode)
    {
        //var response = null;
        try
        {
            MainViewRecallInvoiceDTO returnObj = new MainViewRecallInvoiceDTO();
            var headerInfo = await _dataSource.SQLServerDeliveryHeaders.Where(n => n.SalesOrderCode == InvoiceCode).OrderByDescending(n => n.SalesOrderId).FirstOrDefaultAsync();
            if (headerInfo == null)
                return null;

            var customerInformation = await _dataSource.Partners.Where(n => n.PartnerId == headerInfo.CustomerId).Select(n => new Partner
            {
                PartnerId = n.PartnerId,
                PartnerName = n.PartnerName,
                PartnerCode = n.PartnerCode,
                NID = n.NID,
                PartnerTypeId = n.PartnerTypeId,
                PartnerTypeName = n.PartnerTypeName,
                TaggedEmployeeId = n.TaggedEmployeeId,
                TaggedEmployeeName = n.TaggedEmployeeName,
                Address = n.Address,
                City = n.City,
                Email = n.Email,
                MobileNo = n.MobileNo,
                AccountId = n.AccountId,
                BranchId = n.BranchId,
                AdvanceBalance = n.AdvanceBalance,
                CreditLimit = n.CreditLimit,
                ActionById = n.ActionById,
                ActionByName = n.ActionByName,
                ActionTime = n.ActionTime,
                isActive = n.isActive,
                OtherContactNumber = n.OtherContactNumber,
                OtherContactName = n.OtherContactName,
                PartnerBalance = n.PartnerBalance,
                PartnerGroupId = n.PartnerGroupId,
                PartnerGroupName = n.PartnerGroupName,
                PriceTypeId = n.PriceTypeId,
                PriceTypeName = n.PriceTypeName,
                BinNumber = n.BinNumber,
                IsForeign = n.IsForeign,
                TerritoryId = n.TerritoryId,
                DistrictId = n.DistrictId,
                ThanaId = n.ThanaId,


            }).FirstOrDefaultAsync();

            var exchangeReferenceAndItemInfo = await _dataSource.SQLServerSalesDeliveryLines.Where(n => n.ExchangeReferenceId == headerInfo.SalesOrderId).Select(n => new
            {
                n.SalesOrderId,
                n.ItemId,
                n.Price ,
                n.Quantity
            }).ToListAsync();

            var rowInfo = await _dataSource.SQLServerSalesDeliveryLines.Where(n => n.SalesOrderId == headerInfo.SalesOrderId && n.Quantity > 0).OrderByDescending(n => n.Id).Select(n => new MainViewModelItemDTO
            {
                SL = 0,
                ItemId = n.ItemId,
                ItemName = n.ItemName,
                Quantity = n.Quantity.ToString(),
                SalesRate = n.Price.ToString(),
                Vat = n.VatAmount ?? 0,
                SD = n.SdAmount ?? 0,
                Discount = n.DiscountAmount ?? 0,
                Amount = n.NetAmount.ToString(),
                DiscountPercentage = n.DiscountType == "Amount" ? 0.0M : Convert.ToDecimal(n.NetAmount != 0 ? (n.DiscountAmount * 100 / n.NetAmount) : 0),  //Convert.ToDecimal((n.DiscountAmount * 100)/ (n.NetAmount != 0 ? n.NetAmount  : 1)),
                SingleDiscountAmount = n.DiscountType == "Amount" ? n.LineDiscount : 0.0M,
                VATPercentage = n.VatPercentage ?? 0,
                SDPercentage = n.SdPercentage ?? 0,
                UMOid = n.UomId,
                UMOName = n.UomName,
                //BarCode = _dataSource.Items.Where(r => r.ItemId == n.ItemId).Select(r => r.Barcode).FirstOrDefault(),
                ExchangeReferenceNo = headerInfo.SalesOrderId,
                isExchangeEnable = true,
                OtherDiscount = n.OtherDiscount ?? 0.0M,
            }).ToListAsync();
            rowInfo = rowInfo.Select(s => { 
                s.isExchangeEnable = exchangeReferenceAndItemInfo.Count > 0 ? (exchangeReferenceAndItemInfo.Where(r => r.ItemId == s.ItemId && r.Quantity*-1 == Convert.ToDecimal(s.Quantity)).ToList().Count() > 0 ? false : true) : true;
                s.Quantity = exchangeReferenceAndItemInfo.Count > 0 ? (Convert.ToDecimal(s.Quantity)-(exchangeReferenceAndItemInfo.Where(r => r.ItemId == s.ItemId).Sum(a=>a.Quantity) *-1) ).ToString(): s.Quantity;
                return s; }).ToList();
            //var paymentInformation = await _dataSource.SalesPayments.Where(n => n.SalesDeliveryId == headerInfo.SalesOrderId).Select(n => new PaymentModeInformation
            //{
            //    intWalletId = n.WalletId,
            //    strWalletId = "",
            //    numberAmount = n.CollectionAmount,
            //}).ToListAsync();
            var paymentInformation = new List<PaymentModeInformation>();

            var collectionObj = new MyCollection
            {
                TotalBill = "",
                NumtotalBill = headerInfo.ItemTotalAmount,
                TotalSD = "",
                NumtotalSD = headerInfo.TotalSd ?? 0,
                TotalVAT = "",
                NumtotalVAT = headerInfo.TotalVat ?? 0,
                TotalDiscount = "",
                NumTotalDiscount = headerInfo.NetDiscount - headerInfo.HeaderDiscount,
                OtherDiscount = headerInfo.HeaderDiscount.ToString(),
                NumotherDiscount = headerInfo.HeaderDiscount, //headerInfo.HeaderDiscountPercentage,
                GrandTotal = "",
                NumGrandTotal = headerInfo.NetAmount,
                ReceiveAmount = headerInfo.ReceiveAmount,
                ChangeAmount = headerInfo.ReturnAmount ?? 0,

                NumOtherDiscountPercentage = headerInfo.HeaderDiscountPercentage,
                NumOtherDiscountAmount = headerInfo.HeaderDiscountPercentage == 0.0M ? headerInfo.HeaderDiscount : 0.0M,
                OtherDiscountType = headerInfo.HeaderDiscountPercentage > 0.0M ? 2 : 1,
                OtherDiscountId = 0,

                InvoiceNumber = headerInfo.SalesOrderCode,

                CustomerId = headerInfo.CustomerId,
                CustomerName = headerInfo.CustomerName,
                CustomerCode = customerInformation != null ? customerInformation.MobileNo : "",
                CustomerPoints = customerInformation != null ? customerInformation.Points.ToString() : "0.00",
            };

            returnObj.Items = rowInfo;
            returnObj.PaymentModeInformation = paymentInformation;
            returnObj.collection = collectionObj;

            return returnObj;
        }
        catch (Exception ex)
        {
            var message = ex.Message;
            return null;
        }

    }


    public async Task<MessageHelper> CreateSqlSessionDetails(List<CreateCounterSessionDetails> create)
    {
        try
        {
            foreach (var item in create)
            {
                var session = new SQLCounterSession
                {
                    AccountId = item.session.AccountId,
                    BranchId = item.session.BranchId,
                    OfficeId = item.session.OfficeId,
                    CounterId = item.session.CounterId,
                    CounterCode = item.session.CounterCode,
                    OpeningCash = item.session.OpeningCash,
                    ClosingCash = item.session.ClosingCash,
                    OpeningNote = item.session.OpeningNote,
                    ClosingNote = item.session.ClosingNote,
                    StartDatetime = item.session.StartDatetime,
                    TotalInvoice = item.session.TotalInvoice,
                    TotalSales = item.session.TotalSales,
                    CardAmountCollection = item.session.CardAmountCollection,
                    MFSAmountCollection = item.session.MFSAmountCollection,
                    CashAmountCollection = item.session.CashAmountCollection,
                    ClosingDatetime = item.session.ClosingDatetime,
                    ActionById = item.session.ActionById,
                    IsActive = item.session.IsActive,
                    LastActionDatetime = item.session.LastActionDatetime,
                    ServerDatetime = item.session.ServerDatetime
                };
                _dataSource.Entry(session).State = EntityState.Added;
                await _dataSource.SaveChangesAsync();
                var details = item.details.Select(x => new SqlCounterSessionDetails
                {
                    AccountId = x.AccountId,
                    BranchId = x.BranchId,
                    CounterId = x.CounterId,
                    CounterSessionId = session.CounterSessionId,
                    CurrencyName = x.CurrencyName,
                    CurrencyOpeningCount = x.CurrencyOpeningCount,
                    CurrencyClosingCount = x.CurrencyClosingCount,
                    ActionById = x.ActionById,
                    IsActive = x.IsActive,
                    LastActionDatetime = x.LastActionDatetime,
                }).ToList();
                foreach (var it in details)
                {

                    _dataSource.Entry(it).State = EntityState.Added;
                }
                await _dataSource.SaveChangesAsync();
            }
            return new MessageHelper() { StatusCode = 200 };
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<MessageHelper> UpdateCounterSession(List<CreateCounterSessionDetails> edit)
    {
        try
        {
            var sessionId = edit.Select(x => x.session.CounterSessionId).ToList();
            var session = await _dataSource.CounterSessions.Where(x => sessionId.Contains(x.CounterSessionId)).ToListAsync();
            foreach (var item in session)
            {
                item.IsSync = true;
                _dataSource.Entry(item).State = EntityState.Modified;
            }
            await _dataSource.SaveChangesAsync();
            return new MessageHelper()
            {
                StatusCode = 200
            };
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<List<CreateCounterSessionDetails>> GetSqlSessionDetails()
    {

        List<CreateCounterSessionDetails> lisDetails = new List<CreateCounterSessionDetails>();
        var sessionId = await _dataSource.CounterSessions.Where(x => x.IsSync == false && x.ClosingDatetime != null).Select(x => x.CounterSessionId).ToListAsync();
        var session = await _dataSource.CounterSessions.Where(x => sessionId.Contains(x.CounterSessionId)).Select(x => new CounterSessionDTO
        {
            CounterSessionId = x.CounterSessionId,
            AccountId = x.AccountId,
            BranchId = x.BranchId,
            OfficeId = x.OfficeId,
            CounterId = x.CounterId,
            CounterCode = x.CounterCode,
            OpeningCash = x.OpeningCash,
            ClosingCash = x.ClosingCash,
            OpeningNote = x.OpeningNote,
            ClosingNote = x.ClosingNote,
            StartDatetime = x.StartDatetime,
            TotalInvoice = x.TotalInvoice,
            TotalSales = x.TotalSales,
            CardAmountCollection = x.CardAmountCollection,
            MFSAmountCollection = x.MFSAmountCollection,
            CashAmountCollection = x.CashAmountCollection,
            ClosingDatetime = x.ClosingDatetime,
            ActionById = x.ActionById,
            IsActive = x.IsActive,
            LastActionDatetime = x.LastActionDatetime,
            ServerDatetime = x.ServerDatetime,
        }).AsNoTracking().ToListAsync();
        var sessionDetais = await _dataSource.CounterSessionDetails.Where(x => sessionId.Contains(x.CounterSessionId)).Select(x => new CounterSessionDetailsDTO
        {
            CounterSessionDetailsId = x.CounterSessionDetailsId,
            AccountId = x.AccountId,
            BranchId = x.BranchId,
            CounterId = x.CounterId,
            CounterSessionId = x.CounterSessionId,
            CurrencyName = x.CurrencyName,
            CurrencyOpeningCount = x.CurrencyOpeningCount,
            CurrencyClosingCount = x.CurrencyClosingCount,
            ActionById = x.ActionById,
            IsActive = x.IsActive,
            LastActionDatetime = x.LastActionDatetime,
            ServerDatetime = x.ServerDatetime,
        }).AsNoTracking().ToListAsync();
        foreach (var item in session)
        {
            CreateCounterSessionDetails lisCounter = new CreateCounterSessionDetails();
            lisCounter.session = new CounterSessionDTO();
            lisCounter.details = new List<CounterSessionDetailsDTO>();
            lisCounter.session = item;
            var details = sessionDetais.Where(x => x.CounterSessionId == item.CounterSessionId).ToList();
            lisCounter.details.AddRange(details);
            lisDetails.Add(lisCounter);
        }

        return lisDetails;

    }
    #endregion

    public async Task CreatePosLog(tblDataLog log)
    {
        _dataSource.Entry(log).State = EntityState.Added;
        await _dataSource.SaveChangesAsync();
    }
    public async Task SQLCreatePosLog(List<SQLtblDataLog> log)
    {
        foreach (var item in log)
        {

            _dataSource.Entry(item).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();
    }
    public async Task DeleteDataLog()
    {
        var date = DateTime.Now.BD();
        var threeDays = date.AddDays(-3);
        var data = await _dataSource.DataLogs.Where(x => x.LastActionDateTime < threeDays && x.IsSync == 1).ExecuteDeleteAsync();

    }
    public async Task DataLogUpdate(List<SQLtblDataLog> log)
    {
        foreach (var item in log)
        {
            var dt = new tblDataLog
            {
                LogId = item.LogId,
                AccountId = item.AccountId,
                BranchId = item.BranchId,
                OfficeId = item.OfficeId,
                CounterId = item.CounterId,
                ServerUserId = item.ServerUserId,
                strLogMessage = item.strLogMessage,
                strEntityData = item.strEntityData,
                LogType = item.LogType,
                LastActionDateTime = item.LastActionDateTime,
                ServerDateTime = item.ServerDateTime,
                IsSync = 1,
            };
            _dataSource.Entry(dt).State = EntityState.Modified;
        }
        await _dataSource.SaveChangesAsync();
    }
    public async Task<List<tblDataLog>> GetDataLog()
    {
        var date = DateTime.Now.BD();
        var threeDays = date.AddDays(-3);
        var data = await _dataSource.DataLogs.Where(x => x.LastActionDateTime < threeDays && x.IsSync != 1).ToListAsync();
        return data;
    }
    public async Task<MessageHelper> DeleteRecallInvoice(long SalesOrderId, string SalesOrderCode)
    {
        var dt = await _dataSource.DeliveryHeaders.Where(x => x.SalesOrderId == SalesOrderId && x.SalesOrderCode == SalesOrderCode).ExecuteDeleteAsync();
        await _dataSource.SalesDeliveryLines.Where(x => x.SalesOrderId == SalesOrderId).ExecuteDeleteAsync();
        await _dataSource.SalesPayments.Where(x => x.SalesDeliveryId == SalesOrderId).ExecuteDeleteAsync();
        return new MessageHelper()
        {
            StatusCode = 200,
            Message = "Delete Successfully"
        };
    }
    public async Task<List<User>> GetAllSqlUser(long accountId)
    {
        List<User> data = await _dataSource.SQLUser.Where(x => x.AccountId == accountId && x.IsActive == true).ToListAsync();
        return data;
    }
    public async Task CreateUser(List<User> us)
    {
        //var SelectUser = await _dataSource.Users.Where(x => x.strUserName != "Admin").FirstOrDefaultAsync();
        await _dataSource.Users.Where(x => x.strUserJWTToken == "").ExecuteDeleteAsync();
        var SelectUser = await _dataSource.Users.Where(x => x.strUserJWTToken != "" && x.strUserJWTToken != "default").AsNoTracking().ToListAsync();
        var UserJWTToken = SelectUser.Select(s => s.ServerUserID).ToList();
        DateTime date = DateTime.Now.BD();
        foreach (var item in us)
        {
            TblUser user = new TblUser()
            {
                strUserName = item.MobileNumber,
                ServerUserID = item.UserId,
                bolIsPOSAdmin = item.IsPosAdmin,
                strUserJWTToken = "",
                intAccountId = item.AccountId.Value,
                intBusinessUnitId = SelectUser.FirstOrDefault().intBusinessUnitId,
                intOfficeId = SelectUser.FirstOrDefault().intOfficeId,
                intEmpoyeeId = item.EmployeeId ?? 0,
                strEmployeeName = item.EmployeeName,
                strPassword = item.Password,
                IsAdministration = false,
                LastDateTime = date,
                IsExchange = item.IsExchange,
                IsItemDelete = item.IsItemDelete,
                IsSpecialDiscount = item.IsSpecialDiscount
            };
            if (UserJWTToken.Contains(item.UserId) == true)
            {
                user.strUserJWTToken = SelectUser.Where(w => w.ServerUserID == item.UserId).FirstOrDefault().strUserJWTToken;
                user.intUserID = SelectUser.Where(w => w.ServerUserID == item.UserId).FirstOrDefault().intUserID;
                _dataSource.Entry(user).State = EntityState.Modified;
                await _dataSource.SaveChangesAsync();
            }

            else
                _dataSource.Entry(user).State = EntityState.Added;
        }
        await _dataSource.SaveChangesAsync();
    }

    public async Task<TblUser> ChangePassword(ChangePassword change)
    {
        
        var dt = _dataSource.Users.Where(x=>x.strUserName==change.UserName && x.strPassword==change.Password).AsNoTracking().FirstOrDefault();
        dt.strPassword = change.NewPassword;
        _dataSource.Entry(dt).State = EntityState.Modified;
        await _dataSource.SaveChangesAsync();
        return dt;
    }
    public async Task<User> ChangePasswordSQL(ChangePassword change)
    {

        var dt = _dataSource.SQLUser.Where(x => x.MobileNumber== change.UserName && x.Password== change.Password).AsNoTracking().FirstOrDefault();
        dt.Password = change.NewPassword;
        dt.OldPassword = change.Password;
        _dataSource.Entry(dt).State = EntityState.Modified;
        await _dataSource.SaveChangesAsync();
        return dt;
    }
    #region Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_dataSource != null)
            {
                _dataSource.Dispose();
            }
        }
    }


    #endregion

}
