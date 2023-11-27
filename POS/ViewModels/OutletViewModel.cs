using CommunityToolkit.Mvvm.ComponentModel;
using POS.Contracts.Services;
using POS.Core.Contracts.Services;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.CounterSession;
using POS.Services;
using POS.Services.HttpsClient;

namespace POS.ViewModels;

public class OutletViewModel : ObservableRecipient
{
    private readonly ISampleDataService _sampleDataService;
    private readonly IRestService _restService;
    private readonly IGetService _getService;
    private readonly ICreateService _createService;
    public List<OfficeLists> officeList { get; set; } = new List<OfficeLists>();
    private readonly IMasterDataRestService _ImasterDataRestService;
    private readonly IMasterDataSQLRestService _ImasterDataSQLRestService;
    public OfficeLists selectedOffice
    {
        get; set;
    }
    public CounterList selectedCounter
    {
        get; set;
    }
    public bool Status
    {
        get; set;
    }
    public OutletViewModel(ISampleDataService sampleDataService, IRestService restService, IGetService getService, ICreateService createService, IMasterDataRestService imasterDataRestService, IMasterDataSQLRestService imasterDataSQLRestService)
    {
        _sampleDataService = sampleDataService;
        _restService = restService;
        _getService = getService;
        _createService = createService;
        _ImasterDataRestService = imasterDataRestService; 
        _ImasterDataSQLRestService = imasterDataSQLRestService;
    }
    public async void OnNavigatedTo(object parameter)
    {
        try
        {

            var Id = AppSettings.UserId;
            var user = await _getService.GetUser(Id);
            var office = await _restService.GetOfficeDetails(user.intOfficeId);


            if (office.officeLists != null)
            {
                officeList = office.officeLists;
            }
            if (officeList != null)
            {
                foreach (var item in officeList)
                {

                    var counter = new List<CounterList>();

                    counter.Add(new CounterList() { AccountId = item.AccountId, BranchId = item.BranchId, CounterId = 0, CounterName = "All", OfficeId = item.OfficeId, WarehouseId = 0 });
                    var data = office.counterList.Where(x => x.OfficeId == item.OfficeId).ToList();
                    if (data.Count > 0)
                        counter.AddRange(data);
                    item.counterLists = counter;
                }
                var check = await _getService.GetCounterSession();
                if (check != null)
                {
                    officeList = officeList.Where(x => x.AccountId == check.AccountId && x.BranchId == check.BranchId && x.OfficeId == check.OfficeId).ToList();
                    foreach (var item in officeList)
                    {
                        item.CounterSessionId = check.CounterSessionId;
                        var counter = new CounterList();

                        var data = office.counterList.Where(x => x.CounterId == check.CounterId).FirstOrDefault();
                        counter = data;
                        item.selectedCounter = counter;
                        item.counterLists = new List<CounterList>();
                        item.counterLists.Add(counter);
                        item.strStatus = "Last open session " + check.StartDatetime.ToString("dd MMM yyyy") + " | " + check.StartDatetime.ToString("hh:mm tt");
                    }
                    officeList.ForEach(x => x.counterLists.Where(x => x.CounterId == check.CounterId).ToList());
                }
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Session");
        }

    }
    public async Task<CounterSession> CheckCounterSession()
    {
        return await _getService.GetCounterSession();
    }

    public async Task<MessageHelper> createCounterSession(CreateCounterSeason create)
    {
        try
        {

            var dt = await _createService.CreateCountersSession(create);
            if (dt != null && dt.ReferanceCode == 0)
            {
                var setting = await _getService.GetSettings();

                if (setting.OflineConnection)
                {
                    var items = await _ImasterDataSQLRestService.GetSQLAllItems(setting.intAccountId, setting.intBranchId);
                    var warehouses = await _ImasterDataSQLRestService.GetSQLAllWarehouses(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
                    var ItemSellingPrice = await _ImasterDataSQLRestService.GetSQLAllItemSellingPrices(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
                    var AllPartners = await _ImasterDataSQLRestService.GetSQLAllPartner(setting.intAccountId, setting.intBranchId);
                    var AllItemStock = await _ImasterDataSQLRestService.GetSQLAllItemBalanceWarehouse(setting.intAccountId, setting.intBranchId);
                    var responsefrom = await _ImasterDataSQLRestService.GetWalletInformationfromSQLServer(setting.intAccountId, setting.intBranchId);
                    var Promotion = await _ImasterDataSQLRestService.GetPromotionRowfromSQLServer(setting.intAccountId, setting.intBranchId);
                    var DiscountInfo = await _ImasterDataSQLRestService.GetSpecialDiscountfromSQLServer(setting.intAccountId, setting.intBranchId);
                    await _ImasterDataSQLRestService.GetUserFromSql(setting.intAccountId);
                }
                else
                {
                    var items = await _ImasterDataRestService.GetAllItems(setting.intAccountId, setting.intBranchId);
                    var warehouses = await _ImasterDataRestService.GetWarehouseForPOS(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
                    var ItemSellingPrice = await _ImasterDataRestService.GetUpdatedItemSellingcPriceForPOS(setting.intAccountId, setting.intBranchId, setting.intOfficeId, setting.intWarehouseId);
                    var AllPartners = await _ImasterDataRestService.GetAllPartner(setting.intAccountId, setting.intBranchId);
                }
                
            }
            return dt;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }
    public async Task<List<GetCounterSessionDetailsDTO>> GetCounterSessionDetails(long counterSessionId)
    {
        try
        {
            var dt = await _getService.GetCounterSessionDetails(counterSessionId);
            return dt;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }
}
