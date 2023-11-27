using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Shapes;
using POS.Core.Helpers;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.CounterDTO;
using POS.Core.ViewModels.MainViewModelDTO;
using POS.Core.ViewModels.SalesDeliveryDTO;
using POS.Models;



namespace POS.Services.HttpsClient;
public class MasterDataRestService : IMasterDataRestService
{
    readonly HttpClient _client;

    readonly JsonSerializerOptions _serializerOptions;
    readonly IHttpsClientHandlerService _httpsClientHandlerService;
    readonly IDataServiceFactory _context;
    readonly IConnectionCheck _connectionCheck;

    public MasterDataRestService(IHttpsClientHandlerService service, IDataServiceFactory context, IConnectionCheck connectionCheck)
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
        _connectionCheck = connectionCheck;
    }



    public async Task<List<Item>> GetAllItems(long AccountId, long BranchId)
    {
        var json = "AccountId=" + AccountId + "&BranchId=" + BranchId + "&page=" + 0 + "&row=" + 5000;
        var enc_json = Encryption.EncryptString(json);
        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
        client.Timeout = TimeSpan.FromMinutes(30);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(AppSettings.APPURL);
        System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, "sme/Item/GetAllItems?" + json);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.JWT_Token);
        long count = 0;
        try
        {
            await DeleteItemsToDatabase();

            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                //var dec_json = Encryption.DecryptString(responseContent);
                return null;
            }
            else
            {
                try
                {
                    var st = await response.Content.ReadAsStringAsync();
                    // var dec_json = Encryption.DecryptString(st);
                    var items = new ItemDTO();
                    var qqq = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(st);
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemDTO>(qqq.ToString());
                    var itemList = items.items.Select(s => new Item
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
                        SD = s.Sd,
                        MaximumDiscountPercent = s.MaximumDiscountPercent,
                        TotalQuantity = s.TotalQuantity,
                        CurrentSellingPrice = s.CurrentSellingPrice,

                    }).ToList();
                    //save item api called.......
                    var saveItemResponse = await SaveItemsToDatabase(itemList);
                    //save item api called.......
                    var i = 0;
                    count = items.count - 5000;
                    if (count > 5000)
                    {

                        while (count >= 0)
                        {
                            var page = i + 1;
                            count -= 5000;
                            i++;
                            json = "AccountId=" + AccountId + "&BranchId=" + BranchId + "&page=" + page + "&row=" + 5000;
                            enc_json = Encryption.EncryptString(json);
                            client = new System.Net.Http.HttpClient();
                            client.Timeout = TimeSpan.FromMinutes(30);
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.BaseAddress = new Uri(AppSettings.APPURL);
                            request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, "sme/Item/GetAllItems?" + json);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.JWT_Token);

                            try
                            {


                                response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                                if (!response.IsSuccessStatusCode)
                                {
                                    var responseContent = await response.Content.ReadAsStringAsync();
                                    //dec_json = Encryption.DecryptString(responseContent);
                                    return null;
                                }
                                else
                                {
                                    try
                                    {
                                        st = await response.Content.ReadAsStringAsync();
                                        // dec_json = Encryption.DecryptString(st);
                                        items = new ItemDTO();
                                        qqq = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(st);
                                        items = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemDTO>(qqq.ToString());
                                        itemList = items.items.Select(s => new Item
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
                                            SD = s.Sd,
                                            MaximumDiscountPercent = s.MaximumDiscountPercent,
                                            TotalQuantity = s.TotalQuantity,
                                            CurrentSellingPrice = s.CurrentSellingPrice,

                                        }).ToList();
                                        //save item api called.......
                                        saveItemResponse = await SaveItemsToDatabase(itemList);

                                    }
                                    catch
                                    {
                                        throw new Exception("Getting Items Failed");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                            }
                        }
                    }
                    RemoveDuplicateItems();
                    return itemList;
                }
                catch (Exception ex)
                {
                    throw new Exception("Getting Items Failed");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return new List<Item>();
    }
    public async Task<List<Warehouse>> GetWarehouseForPOS(long accountingId, long branchId, long officeId, long warehouseId)
    {
        var json = "accountId=" + accountingId + "&branchId=" + branchId + "&officeId=" + officeId + "&warehouseId=" + warehouseId;
        var enc_json = Encryption.EncryptString(json);
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(AppSettings.APPURL);
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "sme/Item/GetWarehouseForPOS?" + enc_json);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.JWT_Token);

        try
        {
            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var dec_json = Encryption.DecryptString(responseContent);
                return null;
            }
            else
            {
                try
                {
                    var st = await response.Content.ReadAsStringAsync();
                    var dec_json = Encryption.DecryptString(st);
                    var items = new List<Warehouse>();
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Warehouse>>(dec_json);

                    var CreateWarehouseResponse = await SaveWareHouseToDatabase(items);
                    return items;
                }
                catch (Exception)
                {
                    throw new Exception("Getting Warehouse Failed");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return new List<Warehouse>();
    }
    public async Task<List<ItemSellingPriceRow>> GetUpdatedItemSellingcPriceForPOS(long accountId, long branchId, long officeId, long warehouseId)
    {
        var json = "accountId=" + accountId + "&branchId=" + branchId + "&officeId=" + officeId;//+ "&warehouseId=" + warehouseId;
        var enc_json = Encryption.EncryptString(json);
        HttpClient client1 = new HttpClient();
        client1.BaseAddress = new Uri(AppSettings.APPURL);
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "Item/GetUpdatedItemSellingcPriceForPOS?" + enc_json);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.JWT_Token);

        try
        {
            HttpResponseMessage response = await client1.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var dec_json = Encryption.DecryptString(responseContent);
                return null;
            }
            else
            {
                try
                {
                    var st = await response.Content.ReadAsStringAsync();
                    var dec_json = Encryption.DecryptString(st);
                    var items = new List<ItemSellingPriceRow>();
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemSellingPriceRow>>(dec_json);
                    var CreateItemSellingPriceResponse = await SaveItemSellingPriceToDatabase(items);
                    return items;
                }
                catch (Exception)
                {
                    throw new Exception("Getting Warehouse Failed");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return new List<ItemSellingPriceRow>();
    }
    public async Task<List<Partner>> GetAllPartner(long AccountId, long BranchId)
    {
        var json = "AccountId=" + AccountId + "&BranchId=" + BranchId + "&page=" + 0 + "&row=" + 5000;
        var enc_json = Encryption.EncryptString(json);
        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
        client.Timeout = TimeSpan.FromMinutes(30);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(AppSettings.APPURL);
        System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, "sme/Partner/GetPartners?" + json);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.JWT_Token);
        long count = 0;
        try
        {
            await DeletePartnerToDatabase();

            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                //var dec_json = Encryption.DecryptString(responseContent);
                return null;
            }
            else
            {
                try
                {
                    var st = await response.Content.ReadAsStringAsync();
                    // var dec_json = Encryption.DecryptString(st);
                    var items = new PartnerDTO();
                    var qqq = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(st);
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<PartnerDTO>(qqq.ToString());
                    var itemList = items.items.Select(s => new Partner
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
                        //IsSync = s.IsSync,
                        Points = s.Points,

                    }).ToList();
                    //save item api called.......
                    var savePartnerResponse = await SavePartnerToDatabase(itemList);

                    //save item api called.......
                    var i = 0;
                    count = items.count - 5000;
                    if (count > 5000)
                    {

                        while (count >= 0)
                        {
                            var page = i + 1;
                            count -= 5000;
                            i++;
                            json = "AccountId=" + AccountId + "&BranchId=" + BranchId + "&page=" + page + "&row=" + 5000;
                            enc_json = Encryption.EncryptString(json);
                            client = new System.Net.Http.HttpClient();
                            client.Timeout = TimeSpan.FromMinutes(30);
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.BaseAddress = new Uri(AppSettings.APPURL);
                            request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, "sme/Partner/GetPartners?" + json);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.JWT_Token);

                            try
                            {


                                response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                                if (!response.IsSuccessStatusCode)
                                {
                                    var responseContent = await response.Content.ReadAsStringAsync();
                                    //dec_json = Encryption.DecryptString(responseContent);
                                    return null;
                                }
                                else
                                {
                                    try
                                    {
                                        st = await response.Content.ReadAsStringAsync();
                                        // dec_json = Encryption.DecryptString(st);
                                        items = new PartnerDTO();
                                        qqq = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(st);
                                        items = Newtonsoft.Json.JsonConvert.DeserializeObject<PartnerDTO>(qqq.ToString());
                                        itemList = items.items.Select(s => new Partner
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
                                            //IsSync = s.IsSync,
                                            Points = s.Points,

                                        }).ToList();
                                        //save item api called.......
                                        savePartnerResponse = await SavePartnerToDatabase(itemList);


                                    }
                                    catch
                                    {
                                        throw new Exception("Getting Items Failed");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                            }
                        }
                    }

                    return itemList;
                }
                catch (Exception ex)
                {
                    throw new Exception("Getting Items Failed");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return new List<Partner>();
    }

    public async Task<MessageHelper> SaveItemsToDatabase(List<Item> items)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreateItemAsync(items);
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    public async Task<MessageHelper> DeleteItemsToDatabase()
    {
        using (var dataService = _context.CreateDataService())
        {
            await dataService.DeleteItemAsync();
            return new MessageHelper() { StatusCode = 200 };
        }
    }

    public async Task<MessageHelper> DeletePartnerToDatabase()
    {
        using (var dataService = _context.CreateDataService())
        {
            await dataService.DeletePartnerAsync();
            return new MessageHelper() { StatusCode = 200 };
        }
    }
    public async Task<MessageHelper> SaveWareHouseToDatabase(List<Warehouse> warehouses)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreateWareHouseAsync(warehouses);
            return new MessageHelper() { StatusCode = 200, Message = "" };
        }
    }

    public async Task<MessageHelper> SaveItemSellingPriceToDatabase(List<ItemSellingPriceRow> itemSellingPriceRows)
    {
        Random rnd = new Random();
        itemSellingPriceRows.ForEach(n => { n.RowId = rnd.Next(100000); n.HeaderId = 1; });
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreateItemSellingPriceRowsync(itemSellingPriceRows);
            return new MessageHelper() { StatusCode = 200, Message = "" };
        }
    }
    public async Task<Partner> GetPartner()
    {
        using (var dataService = _context.CreateDataService())
        {
            var partner = await dataService.GetPartner(null);
            return partner;
        }
    }
    public async Task<Partner> GetPartner(long? partnerId)
    {
        using (var dataService = _context.CreateDataService())
        {
            var partner = await dataService.GetPartner(partnerId);
            return partner;
        }
    }


    public async Task<MessageHelper> SavePartnerToDatabase(List<Partner> partners)
    {
        using (var dataService = _context.CreateDataService())
        {
            var CreateResponse = await dataService.CreatePartnerAsync(partners);
            return new MessageHelper() { StatusCode = 200, Message = "" };
        }
    }


    public async Task<CreateSalesDeliveryDTO> SaveItemIntoSalesDeliveryLines(CreateSalesDeliveryDTO objCreate)
    {
        var itemInfo = new CreateSalesDeliveryDTO();
        //itemInfo = objCreate;
        try
        {
            var settings = new TblSettings();
            var userInformation = new TblUser();
            using (var dataService = _context.CreateDataService())
            {
                settings = await dataService.GetSettings();
                userInformation = await dataService.GetUserAsync(objCreate.pOSSalesDeliveryHeader.UserId);

            }
            if (userInformation != null)
            {
                objCreate.pOSSalesDeliveryHeader.UserId = userInformation.ServerUserID;
            }
            if (await _connectionCheck.IsServerConnectionAvailable() == true)
            {

                if (objCreate.pOSSalesDeliveryHeader.Draft == false)
                {
                    if (objCreate.pOSSalesDeliveryHeader.SalesOrderId != 0)
                    {
                        objCreate.pOSSalesDeliveryHeader.SalesOrderCode = await InvoiceCodeGenerate(settings.intCounterId, settings.StrCounterCode);
                    }
                    using (var dataService = _context.CreateSQLDataService())
                    {
                        objCreate.IsOnline = true;
                        var CreateResponse = await dataService.CreateSalesDeliveryInformationIntoSQLServer(objCreate, settings);
                        if (CreateResponse.StatusCode == 200)
                            itemInfo = objCreate;
                        else
                        {
                            return null;
                        }
                      
                    }

                }
                using (var dataService = _context.CreateDataService())
                {
                    objCreate.pOSSalesDeliveryHeader.IsSync = 1;
                    var data = await dataService.SaveItemIntoSalesDeliveryLines(objCreate);
                    if (objCreate.pOSSalesDeliveryHeader.Draft == true)
                    {
                        itemInfo = objCreate;

                    }
                }
               
                return itemInfo;
            }
            else
            {    if(AppSettings.IsOnline== false) {
                    if (objCreate.pOSSalesDeliveryHeader.SalesOrderId != 0)
                    {
                        objCreate.pOSSalesDeliveryHeader.SalesOrderCode = await InvoiceCodeGenerate(settings.intCounterId, settings.StrCounterCode);
                    }
                    using (var dataService = _context.CreateDataService())
                    {
                        objCreate.pOSSalesDeliveryHeader.IsSync = 0;
                        itemInfo = await dataService.SaveItemIntoSalesDeliveryLines(objCreate);

                    }
                    return itemInfo;
                }
                else
                {
                    return null;
                }
              
            }
        }
        catch 
        {
            //try
            //{
            //    using (var dataService = _context.CreateDataService())
            //    {
            //        objCreate.pOSSalesDeliveryHeader.IsSync = 0;
            //        itemInfo = await dataService.SaveItemIntoSalesDeliveryLines(objCreate);

            //    }
            //    return itemInfo;

            //}
            //catch
            //{
            //    return itemInfo;

            //}
            throw new Exception("The server is offline. Please contact to the administrator.");

        }



    }

    public async Task<CreateSalesDeliveryDTO> SaveItemIntoSalesDelivery(CreateSalesDeliveryDTO objCreate)
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.SaveItemIntoSalesDelivery(objCreate);
            return itemInfo;
        }
    }
    public async Task<MessageHelper> EditPosPayment(EditPosSales edit)
    {
        using (var dataService = _context.CreateDataService())
        {
            var payment = await dataService.EditPosPayment(edit);
            return payment;
        }
    }

    public async Task<Partner> GetCustomerByCustomerId(string strPartnerCode)
    {
        var partner = new Partner();
        var settings = new TblSettings();
        using (var dataService = _context.CreateDataService())
        {
            settings = await dataService.GetSettings();

        }
        if (await _connectionCheck.IsServerConnectionAvailable() == true)
        {
            try
            {
                using (var dataService = _context.CreateSQLDataService())
                {
                    partner = await dataService.GetPartnerAsync(strPartnerCode, settings.intAccountId);

                }
            }
            catch
            {
                {
                    using (var dataService = _context.CreateDataService())
                    {
                        partner = await dataService.GetPartnerAsync(strPartnerCode, settings.intAccountId);

                    }

                }

            }

        }
        else
        {
            using (var dataService = _context.CreateDataService())
            {
                partner = await dataService.GetPartnerAsync(strPartnerCode, settings.intAccountId);

            }

        }

        return partner;
    }


    public async Task<Partner> GetPartnerById(long PartnerId)
    {
        var partner = new Partner();
        var settings = new TblSettings();
        using (var dataService = _context.CreateDataService())
        {
            settings = await dataService.GetSettings();

        }
        if (await _connectionCheck.IsServerConnectionAvailable() == true)
        {
            try
            {
                using (var dataService = _context.CreateSQLDataService())
                {
                    partner = await dataService.GetPartnerIdAsync(PartnerId, settings.intAccountId);

                }
            }
            catch
            {
                {
                    using (var dataService = _context.CreateDataService())
                    {
                        partner = await dataService.GetPartnerIdAsync(PartnerId, settings.intAccountId);

                    }

                }

            }

        }
        else
        {
            using (var dataService = _context.CreateDataService())
            {
                partner = await dataService.GetPartnerIdAsync(PartnerId, settings.intAccountId);

            }

        }

        return partner;
    }

    public async Task<Item> GetItemByBarCode(string ItemBarCode)
    {

        var settings = new TblSettings();
        using (var dataService = _context.CreateDataService())
        {
            settings = await dataService.GetSettings();

        }

        var itemInfo = new Item();
        if (await _connectionCheck.IsServerConnectionAvailable() == true)
        {
            try
            {
                using (var dataService = _context.CreateSQLDataService())
                {
                    itemInfo = await dataService.GetSQLItemByBarCode(ItemBarCode, settings.intAccountId);

                }

            }
            catch
            {
                using (var dataService = _context.CreateDataService())
                {
                    itemInfo = await dataService.GetItemByBarCode(ItemBarCode);

                }
            }

        }
        else
        {
            using (var dataService = _context.CreateDataService())
            {
                itemInfo = await dataService.GetItemByBarCode(ItemBarCode);

            }

        }
        return itemInfo;

    }

    public async Task<List<Item>> GetItemListByBarCode(string ItemBarCode)
    {
        var itemInfo = new List<Item>();
        var settings = new TblSettings();
        using (var dataService = _context.CreateDataService())
        {
            settings = await dataService.GetSettings();

        }

        if (await _connectionCheck.IsServerConnectionAvailable() == true)
        {
            try
            {
                using (var dataService = _context.CreateSQLDataService())
                {
                    itemInfo = await dataService.GetSQLItemListByBarCode(ItemBarCode, settings.intWarehouseId, settings.intAccountId);

                }
            }
            catch
            {

                using (var dataService = _context.CreateDataService())
                {
                    itemInfo = await dataService.GetItemListByBarCode(ItemBarCode);

                }

            }

        }
        else
        {

            using (var dataService = _context.CreateDataService())
            {
                itemInfo = await dataService.GetItemListByBarCode(ItemBarCode);

            }
        }
        return itemInfo;

    }

    public async Task<List<Item>> GetMultipleSalesPrizeItemListByBarCode(string ItemBarCode)
    {
        var itemInfo = new List<Item>();
        var settings = new TblSettings();
        using (var dataService = _context.CreateDataService())
        {
            settings = await dataService.GetSettings();

        }
        if (await _connectionCheck.IsServerConnectionAvailable() == true)
        {
            try
            {
                using (var dataService = _context.CreateSQLDataService())
                {
                    itemInfo = await dataService.GetSQLMultipleSalesPrizeItemListByBarCode(ItemBarCode, settings.intWarehouseId, settings.intAccountId);

                }
            }
            catch
            {

                using (var dataService = _context.CreateDataService())
                {
                    itemInfo = await dataService.GetMultipleSalesPrizeItemListByBarCode(ItemBarCode);

                }

            }

        }
        else
        {

            using (var dataService = _context.CreateDataService())
            {
                itemInfo = await dataService.GetMultipleSalesPrizeItemListByBarCode(ItemBarCode);

            }
        }
        return itemInfo;
    }

    public async Task<List<Item>> GetItemListByItemName(string ItemName)
    {
        var itemInfo = new List<Item>();
        var settings = new TblSettings();
        using (var dataService = _context.CreateDataService())
        {
            settings = await dataService.GetSettings();

        }
        if (await _connectionCheck.IsServerConnectionAvailable() == true)
        {
            try
            {
                using (var dataService = _context.CreateSQLDataService())
                {
                    itemInfo = await dataService.GetSQLItemListByItemName(ItemName, settings.intWarehouseId, settings.intAccountId);

                }
            }
            catch
            {
                using (var dataService = _context.CreateDataService())
                {
                    itemInfo = itemInfo = await dataService.GetItemListByItemName(ItemName);

                }


            }

        }
        else
        {

            using (var dataService = _context.CreateDataService())
            {
                itemInfo = itemInfo = await dataService.GetItemListByItemName(ItemName);

            }
        }
        return itemInfo;

    }



    public async Task<List<Item>> GetItemByItemIDs(List<long> ids)
    {
        var itemInfo = new List<Item>();
        var settings = new TblSettings();
        using (var dataService = _context.CreateDataService())
        {
            settings = await dataService.GetSettings();

        }
        if (await _connectionCheck.IsServerConnectionAvailable() == true)
        {
            try
            {
                using (var dataService = _context.CreateSQLDataService())
                {
                    itemInfo = await dataService.GetSQLItemByItemIDs(ids, settings.intAccountId);

                }
            }
            catch
            {

                using (var dataService = _context.CreateDataService())
                {
                    itemInfo = await dataService.GetItemByItemIDs(ids);

                }

            }

        }
        else
        {
            using (var dataService = _context.CreateDataService())
            {
                itemInfo = await dataService.GetItemByItemIDs(ids);

            }


        }
        return itemInfo;

    }



    public async Task<List<PaymentWalletDTO>> GetPaymentWalletList()
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.GetPaymentWalletList();
            return itemInfo;
        }
    }


    public async Task<List<OtherDiscountDTO>> GetOtherDiscountList()
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.GetOtherDiscountList();
            return itemInfo;
        }
    }

    public async Task<PaymentWalletDTO> GetPaymentWalletbyId(long WalletId)
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.GetPaymentWalletbyId(WalletId);
            return itemInfo;
        }
    }

    public async Task<MainViewRecallInvoiceDTO> RecallInvoiceInformation(long userId, long customerId)
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.RecallInvoiceInformation(userId, customerId);
            return itemInfo;
        }
    }


    public async Task<List<CreateSalesDeliveryDTO>> SalesInformationUsingIDs(List<long> SalesOrderIds)
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.SalesInformationUsingIDs(SalesOrderIds);
            return itemInfo;
        }
    }

    public async Task<MainViewRecallInvoiceDTO> RecallInvoice(long userId, long SalesDeliveryId)
    {
        var items = new List<Item>();
        var data = new List<POSSalesDeliveryLine>();
        using (var dataService = _context.CreateDataService())
        {
            data = await dataService.RecallInvoiceRow(userId, SalesDeliveryId);

        }
        items = await GetItemByItemIDs(data.Select(s => s.ItemId).ToList());

        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.RecallInvoice(userId, SalesDeliveryId, items);
            return itemInfo;
        }

    }

    public async Task<List<RecallInvoiceHomeObjDTO>> InvoiceHomePageLanding(long userId)
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.RecallInvoiceInformation(userId);

            if (itemInfo.Count == 0)
            {
                return new List<RecallInvoiceHomeObjDTO>();
            }

            var response = itemInfo.Select((n, Index) => new RecallInvoiceHomeObjDTO
            {
                SL = Index + 1,
                SalesOrderId = n.SalesOrderId,
                SalesInvoice = n.SalesOrderCode,
                OrderDate = n.OrderDate.ToString("dd-MM-yyyy"),
                Quantity = n.TotalQuantity,
                CashAmount = n.CashPayment ?? 0,
                SalesAmount = n.NetAmount,
                CustomerName = n.CustomerName
            }).ToList();

            return response;
        }
    }

    public async Task<POSSalesDeliveryHeader> GetPOSSalesDeliveryHeader(string InvoiceCode)
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.GetPOSSalesDeliveryHeader(InvoiceCode);
            return itemInfo;
        }
    }

    public async Task<string> InvoiceCodeGenerate(long CounterId, string CounterCode)
    {
        DateTime dateTime = DateTime.UtcNow.BD();
        var year = (dateTime.Year % 100).ToString();
        var month = dateTime.Month > 9 ? (dateTime.Month).ToString() : ("0" + (dateTime.Month).ToString());
        var day = dateTime.Day > 9 ? (dateTime.Day).ToString() : ("0" + (dateTime.Day).ToString());
        var hour = dateTime.Hour > 9 ? (dateTime.Hour).ToString() : ("0" + (dateTime.Hour).ToString());
        var minute = dateTime.Minute > 9 ? (dateTime.Minute).ToString() : ("0" + (dateTime.Minute).ToString());
        var second = dateTime.Second > 9 ? (dateTime.Second).ToString() : ("0" + (dateTime.Second).ToString());
        var milliSecond = dateTime.Millisecond > 9 ? (dateTime.Millisecond).ToString() : ("0" + (dateTime.Millisecond).ToString());
        var GeneratedCode = "000000";
        try
        {
            if (await _connectionCheck.IsServerConnectionAvailable() == true)
            {
                using (var dataService = _context.CreateSQLDataService())
                {

                    var TotalCount = dataService.GetSQLServerSalesDeliveryHeaderInfo(CounterId);
                    TotalCount++;
                    var TotalCountStr = TotalCount.ToString();
                    var countLen = TotalCountStr.Length;

                    var subZeros = GeneratedCode.Substring(0, 5 - countLen);

                    //GeneratedCode = CounterCode + year + month + day + milliSecond + subZeros + TotalCountStr;

                    GeneratedCode = CounterCode + year + month + day + hour+ minute+ second + subZeros + TotalCountStr;

                }
            }
            else
            {
                using (var dataService = _context.CreateDataService())
                {
                    var TotalCount = dataService.GetPOSSalesDeliveryHeaderInfo(CounterId);
                    TotalCount++;
                    var TotalCountStr = TotalCount.ToString();
                    var countLen = TotalCountStr.Length;

                    var subZeros = GeneratedCode.Substring(0, 5 - countLen);
                    //GeneratedCode = CounterCode + year + month + day + milliSecond + subZeros + TotalCountStr;
                    GeneratedCode = CounterCode + year + month + day + hour + minute + second + subZeros + TotalCountStr;
                }
            }

        }
        catch
        {

            using (var dataService = _context.CreateDataService())
            {
                var TotalCount = dataService.GetPOSSalesDeliveryHeaderInfo(CounterId);
                TotalCount++;
                //if (TotalCount == 0)
                //    TotalCount = 1;
                var TotalCountStr = TotalCount.ToString();
                var countLen = TotalCountStr.Length;

                var subZeros = GeneratedCode.Substring(0, 5 - countLen);
                //var subZeros = GeneratedCode[..(7 - countLen)];
                //GeneratedCode = CounterCode + year + subZeros + TotalCountStr;
                GeneratedCode = CounterCode + year + month + day + second + subZeros + TotalCountStr;

                //GeneratedCode = CounterCode + year + month + day + hour + minute+ second;

            }
        }
        return GeneratedCode;
    }



    public void RemoveDuplicateItems()
    {
        using (var dataService = _context.CreateDataService())
        {
            dataService.RemoveDuplicateItems();
        }
    }

    public async Task<Item> GetItemByItemID(long Id)
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.GetItemAsync(Id);
            return itemInfo;
        }
    }



    public async Task<List<tblPointOfferRow>> GetPointsOfferRowsByItemIds(List<long> itemIds)
    {
        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.GetPointsOfferRowsByItemIds(itemIds);
            return itemInfo;
        }
    }



    public async Task<Item> GetStockQtyCheckItemByItemID(long Id, decimal salesRate)
    {



        var itemInfo = new Item();
        var settings = new TblSettings();
        using (var dataService = _context.CreateDataService())
        {
            settings = await dataService.GetSettings();

        }
        if (await _connectionCheck.IsServerConnectionAvailable() == true)
        {
            try
            {
                using (var dataService = _context.CreateSQLDataService())
                {
                    itemInfo = await dataService.GetSQLStockQtyCheckItemByItemID(Id, salesRate, settings.intAccountId, settings.intBranchId, settings.intWarehouseId);

                }
            }
            catch
            {

                using (var dataService = _context.CreateDataService())
                {
                    itemInfo = await dataService.GetStockQtyCheckItemByItemID(Id, salesRate);

                }

            }

        }
        else
        {
            using (var dataService = _context.CreateDataService())
            {
                itemInfo = await dataService.GetStockQtyCheckItemByItemID(Id, salesRate);

            }

        }
        return itemInfo;




    }



    public async Task<List<PaymentWalletDTO>> GetPaymentWalletbyIds(List<long> WalletIds)
    {

        using (var dataService = _context.CreateDataService())
        {
            var itemInfo = await dataService.GetPaymentWalletbyIds(WalletIds);
            return itemInfo;
        }
    }
    public async Task<MessageHelper> DeleteSessionData()
    {

        using (var dataService = _context.CreateDataService())
        {
            var msg = await dataService.DeleteSessionData();
            return msg;
        }
    }
    public async Task DeleteDataLog()
    {
        using (var dataService = _context.CreateDataService())
        {
            await dataService.DeleteDataLog();
        }
    }


}
