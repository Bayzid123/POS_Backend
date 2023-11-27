using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using POS.Contracts.Services;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Models;
using POS.ViewModels;
using POS.Views.LogIn;
using WinRT.Interop;
using Microsoft.UI.Xaml;

namespace POS.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    private UIElement? _shell = null;
    public SettingsViewModel ViewModel
    {
        get; set;
    }


    public SettingsPage()
    {
        try
        {
          
            AppSettings.IsAdmin = true;
            ViewModel = App.GetService<SettingsViewModel>();
            ViewModel.GetDeviceInfo("");
            Thread.Sleep(1000);
            InitializeComponent();
            MainGridsProcess.Visibility = Visibility.Collapsed;
            App_Url.Text = AppSettings.APPURL;
            DeviceId.Text = ViewModel.df.DeviceId;
            MacId.Text = ViewModel.df.DeviceMac;
            SQLConn.Password = ViewModel.df.SqlServerConnString;

            cmbBranch.ItemsSource = new List<BranchList>();
            cmbBranch.ItemsSource = ViewModel.filterbranchLists;
            cmbOffice.ItemsSource = new List<OfficeList>();
            cmbOffice.ItemsSource = ViewModel.filterOfficeLists;
            cmbWarehouse.ItemsSource = new List<WarehouseList>();
            cmbWarehouse.ItemsSource = ViewModel.filterwarehouseLists;
            cmbCounter.ItemsSource = new List<CounterList>();
            cmbCounter.ItemsSource = ViewModel.filtercounterLists;
            cmbCounter.SelectedItem = ViewModel.selectedCounter;
            cmbWarehouse.SelectedItem = ViewModel.selectedWarehouse;
            cmbOffice.SelectedItem = ViewModel.selectedOffice;
            cmbBranch.SelectedItem = ViewModel.selectedBranch;
        }
        catch (Exception ex) { App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings"); }
    }
    private void Check_Url(object sender, KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                //Thread.Sleep(1000);
                LoadData();
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }
    }
    private void check(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            btnCheck.IsEnabled = false;
            cmbCounter.IsEnabled= false;
            cmbWarehouse.IsEnabled = false;
            cmbOffice.IsEnabled = false;
            cmbBranch.IsEnabled = false;
            cmbAccount.IsEnabled = false;  
            //Thread.Sleep(1000);
            LoadData();
            Thread.Sleep(1000);
            btnCheck.IsEnabled = true;
            cmbCounter.IsEnabled = true;
            cmbWarehouse.IsEnabled = true;
            cmbOffice.IsEnabled = true;
            cmbBranch.IsEnabled = true;
            cmbAccount.IsEnabled = true;
            cmbAccount.ItemsSource = ViewModel.accountList;

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }


    }

    private void LoadData()
    {
        try
        {
           
            ViewModel.df.IsSync = true;
            ViewModel.df.OflineConnection = true;
            var route = App_Url.Text;
            AppSettings.APPURL = route;
            MainGrids.Visibility = Visibility.Collapsed;
            MainGridsProcess.Visibility = Visibility.Visible;
            ViewModel.GetAccountDDL(route);
            MainGrids.Visibility = Visibility.Visible;
            MainGridsProcess.Visibility = Visibility.Collapsed;
            App_Url.Text = AppSettings.APPURL;
            DeviceId.Text = ViewModel.df.DeviceId;
            MacId.Text = ViewModel.df.DeviceMac;
            SQLConn.Password = ViewModel.df.SqlServerConnString;

            //cmbBranch.ItemsSource = ViewModel.filterbranchLists;
            //cmbOffice.ItemsSource = ViewModel.filterbranchLists;
            //cmbWarehouse.ItemsSource = ViewModel.filterbranchLists;
            //cmbCounter.ItemsSource = ViewModel.filterbranchLists;



        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }

    }

    private void Account_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            cmbBranch.ItemsSource = new List<BranchList>();
            ViewModel.GetBranchDDL();
            cmbBranch.ItemsSource = ViewModel.filterbranchLists;
            cmbBranch.SelectedItem = ViewModel.selectedBranch;
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }

    }

    private void cmbBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            cmbOffice.ItemsSource = new List<OfficeList>();
            ViewModel.GetOfficeDDL();
            cmbOffice.ItemsSource = ViewModel.filterOfficeLists;
            cmbOffice.SelectedItem = ViewModel.selectedOffice;
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }
    }

    private void cmbOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            cmbWarehouse.ItemsSource = new List<WarehouseList>();
            ViewModel.GetWarehouseDDL();
            cmbWarehouse.ItemsSource = ViewModel.filterwarehouseLists;
            cmbWarehouse.SelectedItem = ViewModel.selectedWarehouse;
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }
    }

    private void cmbWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            cmbCounter.ItemsSource = new List<CounterList>();
            ViewModel.GetCounterDDL();
            cmbCounter.ItemsSource = ViewModel.filtercounterLists;
            cmbCounter.SelectedItem = ViewModel.selectedCounter;
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }
    }

    private async void SaveSetting(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            TblSettings settings = new TblSettings()
            {
                AppUrl = App_Url.Text,
                DeviceId = DeviceId.Text,
                MacAddress = MacId.Text,
                intAccountId = ViewModel.selectedAccount.AccountId,
                intBranchId = ViewModel.selectedBranch.BranchId,
                intCounterId = ViewModel.selectedCounter.CounterId,
                StrOfficeName=ViewModel.selectedOffice.OfficeName,
                intOfficeId = ViewModel.selectedOffice.OfficeId,
                intWarehouseId = ViewModel.selectedWarehouse.WarehouseId,
                SqlServerConnString = SQLConn.Password,
                OflineConnection = OffConnection.IsChecked.GetValueOrDefault(),
                IsSync = IsSync.IsChecked.GetValueOrDefault(),

                StrAccountName = ViewModel.selectedAccount.AccountName,
                StrBranchName = ViewModel.selectedBranch.BranchName,
                StrWareHouseName = ViewModel.selectedWarehouse.WarehouseName,
                StrCounterName = ViewModel.selectedCounter.CounterName,
                StrCounterCode = ViewModel.selectedCounter.CounterCode,
                StrAddress = ViewModel.selectedOffice.Address,
                BIN= ViewModel.selectedBranch.BIN,
            };
            var msg = await ViewModel.CreateSetting(settings);
            if (msg != null)
            {
                ContentDialog containtDialog = new ContentDialog()
                {
                    Title = "Settings",
                    Content = msg.Message,
                    CloseButtonText = "Ok"
                };
                containtDialog.XamlRoot = this.Content.XamlRoot;
                await containtDialog.ShowAsync();
                //App.GetService<IAppNotificationService>().Show(msg.Message);
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }

    }



    private void btnLogOut_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            _shell = App.GetService<LogInPage>();
            App.MainWindow.Content = _shell ?? new Frame();


            // Activate the MainWindow.
            App.MainWindow.Activate();
            var windowHandle = WindowNative.GetWindowHandle(App.MainWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            AppSettings.IsAdmin = false;
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }
    }
}
