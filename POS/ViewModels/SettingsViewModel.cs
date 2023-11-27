using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;

using POS.Contracts.Services;
using POS.Core.IService;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Helpers;
using POS.Models;
using POS.Services;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace POS.ViewModels;

public class SettingsViewModel : ObservableRecipient
{
    private readonly IRestService _IRestService;
    private readonly IGetService _IGetService;
    public DeviceInfo df { get; } = new DeviceInfo();
    public List<AccountList> accountList { get; set; } = new List<AccountList>();
    public List<BranchList> branchLists { get; set; } = new List<BranchList>();
    public List<OfficeLists> OfficeLists { get; set; } = new List<OfficeLists>();
    public List<WarehouseList> warehouseLists { get; set; } = new List<WarehouseList>();
    public List<CounterList> counterLists { get; set; } = new List<CounterList>();
    public List<BranchList> filterbranchLists { get; set; } = new List<BranchList>();
    public List<OfficeLists> filterOfficeLists { get; set; } = new List<OfficeLists>();
    public List<WarehouseList> filterwarehouseLists { get; set; } = new List<WarehouseList>();
    public List<CounterList> filtercounterLists { get; set; } = new List<CounterList>();

    public AccountList selectedAccount
    {
        get; set;
    }
    public BranchList selectedBranch
    {
        get; set;
    }
    public OfficeLists selectedOffice
    {
        get; set;

    }
    public WarehouseList selectedWarehouse
    {
        get; set;
    }
    public CounterList selectedCounter
    {
        get; set;
    }
    //private string _versionDescription;
    public SettingsViewModel(IRestService IRestService, IGetService iGetService)
    {
        _IRestService = IRestService;
        _IGetService = iGetService; 
    }
    public async void GetDeviceInfo(object parameter)
    {
        
        Constants.DeviceID = "";
        var deviceInformation = new EasClientDeviceInformation();
        Constants.DeviceID = deviceInformation.Id.ToString();
        Constants.DeviceMAC = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                               where nic.OperationalStatus == OperationalStatus.Up
                               select nic.GetPhysicalAddress().ToString()).FirstOrDefault() ?? "";
        df.DeviceId = Constants.DeviceID;
        df.DeviceMac = Constants.DeviceMAC;
        var setting = await _IGetService.GetSettings();
        if(setting != null)
        {
            AppSettings.APPURL = setting.AppUrl;
            AppSettings.SQLServerConnectionString = setting.SqlServerConnString;
            var dt =  await _IRestService.GetAllForPreDDL(AppSettings.APPURL);
            df.DeviceId = setting.DeviceId;
            df.DeviceMac = setting.MacAddress;
            df.SqlServerConnString = setting.SqlServerConnString;
            df.IsSync = setting.IsSync;
            df.OflineConnection = setting.OflineConnection;

            if (dt.accountList != null)
            {
                accountList.AddRange(dt.accountList);
                branchLists.AddRange(dt.branchList);
                OfficeLists.AddRange(dt.officeList);
                warehouseLists.AddRange(dt.warehouseList);
                counterLists.AddRange(dt.counterList);
          
                selectedAccount = accountList.Where(w => w.AccountId == setting.intAccountId).FirstOrDefault();
                selectedBranch = branchLists.Where(w => w.BranchId == setting.intBranchId).FirstOrDefault();
                selectedOffice = OfficeLists.Where(w => w.OfficeId == setting.intOfficeId).FirstOrDefault();
                selectedWarehouse = warehouseLists.Where(w => w.WarehouseId == setting.intWarehouseId).FirstOrDefault();
                selectedCounter = counterLists.Where(w => w.CounterId == setting.intCounterId).FirstOrDefault();
                if (selectedAccount != null && selectedBranch != null && selectedOffice != null && selectedWarehouse != null && selectedCounter != null)
                {
                    filterbranchLists = new List<BranchList>();
                    filterbranchLists.Add(new BranchList { AccountId = selectedAccount.AccountId, BranchId = 0, BranchName = "ALL" });
                    filterbranchLists.AddRange(branchLists.Where(x => x.AccountId == selectedAccount.AccountId).ToList());
                    filterOfficeLists = new List<OfficeLists>();
                    filterOfficeLists.Add(new OfficeLists { AccountId = selectedAccount.AccountId, BranchId = selectedBranch.BranchId, OfficeId = 0, OfficeName = "ALL" });
                    filterOfficeLists.AddRange(OfficeLists.Where(x => x.BranchId == selectedBranch.BranchId && x.AccountId == selectedBranch.AccountId).ToList());
                    filterwarehouseLists = new List<WarehouseList>();
                    filterwarehouseLists.Add(new WarehouseList { AccountId = selectedAccount.AccountId, BranchId = selectedBranch.BranchId, OfficeId = selectedOffice.OfficeId, WarehouseId = 0, WarehouseName = "ALL" });
                    filterwarehouseLists.AddRange(warehouseLists.Where(x => x.OfficeId == selectedOffice.OfficeId && x.AccountId == selectedOffice.AccountId && x.BranchId == selectedOffice.BranchId).ToList());
                    filterwarehouseLists = new List<WarehouseList>();
                    filterwarehouseLists.Add(new WarehouseList { AccountId = selectedAccount.AccountId, BranchId = selectedBranch.BranchId, OfficeId = selectedOffice.OfficeId, WarehouseId = 0, WarehouseName = "ALL" });
                    filterwarehouseLists.AddRange(warehouseLists.Where(x => x.OfficeId == selectedOffice.OfficeId && x.AccountId == selectedOffice.AccountId && x.BranchId == selectedOffice.BranchId).ToList());
                }
            }


        }
    }
    public async void GetAccountDDL(string route)
    {
        AllDDl dt = new AllDDl();       
        branchLists = new List<BranchList>();
        accountList = new List<AccountList>();
 
        OfficeLists=new List<OfficeLists>();
        warehouseLists =new List<WarehouseList>();
        counterLists =new List<CounterList>();
        dt = await _IRestService.GetAllDDL(route);
        if (dt.accountList != null)
        {
            dt.accountList = dt.accountList.Where(w => !accountList.Select(s => s.AccountId).Contains(w.AccountId)).ToList();
            dt.branchList = dt.branchList.Where(w => !branchLists.Select(s => s.BranchId).Contains(w.BranchId)).ToList();
            dt.officeList = dt.officeList.Where(w => !OfficeLists.Select(s => s.OfficeId).Contains(w.OfficeId)).ToList();
            dt.warehouseList = dt.warehouseList.Where(w => !warehouseLists.Select(s => s.WarehouseId).Contains(w.WarehouseId)).ToList();
            dt.counterList = dt.counterList.Where(w => !counterLists.Select(s => s.CounterId).Contains(w.CounterId)).ToList();
            accountList.AddRange(dt.accountList);
            branchLists.AddRange(dt.branchList);
            OfficeLists.AddRange(dt.officeList);
            warehouseLists.AddRange(dt.warehouseList);
            counterLists.AddRange(dt.counterList);


            filterbranchLists = branchLists;
            filterOfficeLists = OfficeLists;
            filterwarehouseLists = warehouseLists;
            filtercounterLists = counterLists;
        }
    }

    public void GetBranchDDL()
    {
        if (selectedAccount == null)
        {
            filterbranchLists = new List<BranchList>();
        }
        else
        {
            filterbranchLists = new List<BranchList>();
            filterbranchLists.Add(new BranchList { AccountId = selectedAccount.AccountId, BranchId = 0, BranchName = "ALL" });
            filterbranchLists.AddRange(branchLists.Where(x => x.AccountId == selectedAccount.AccountId).ToList());
        }
    }
    public void GetOfficeDDL()
    {
        if (selectedBranch == null)
        {
            filterOfficeLists = new List<OfficeLists>();
        }
        else
        {
            filterOfficeLists = new List<OfficeLists>();
            filterOfficeLists.Add(new OfficeLists { AccountId = selectedAccount.AccountId, BranchId = selectedBranch.BranchId, OfficeId = 0, OfficeName = "ALL" });
            filterOfficeLists.AddRange(OfficeLists.Where(x => x.BranchId == selectedBranch.BranchId && x.AccountId == selectedBranch.AccountId).ToList());

        }


    }
    public void GetWarehouseDDL()
    {
        if (selectedOffice == null)
        {
            filterwarehouseLists = new List<WarehouseList>();
        }
        else
        {
            filterwarehouseLists = new List<WarehouseList>();
            filterwarehouseLists.Add(new WarehouseList { AccountId = selectedAccount.AccountId, BranchId = selectedBranch.BranchId, OfficeId = selectedOffice.OfficeId, WarehouseId = 0, WarehouseName = "ALL" });
            filterwarehouseLists.AddRange(warehouseLists.Where(x => x.OfficeId == selectedOffice.OfficeId && x.AccountId == selectedOffice.AccountId && x.BranchId == selectedOffice.BranchId).ToList());
        }

    }
    public void GetCounterDDL()
    {
        if (selectedWarehouse == null)
        {
            filtercounterLists = new List<CounterList>();
        }
        else
            filtercounterLists = counterLists.Where(x => x.WarehouseId == selectedWarehouse.WarehouseId && x.AccountId == selectedWarehouse.AccountId && x.BranchId == selectedWarehouse.BranchId && x.OfficeId == selectedWarehouse.OfficeId).ToList();
    }
    public async Task<MessageHelper> CreateSetting(TblSettings settings)
    {
        try
        {
            var msg = await _IRestService.CreateSetting(settings);
            return msg;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
