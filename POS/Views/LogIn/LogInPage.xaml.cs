using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using POS.Activation;
using POS.Contracts.Services;
using POS.Core.Helpers;
using POS.Models;
using POS.Services;
using POS.Services.HttpsClient;
using POS.ViewModels;
using Windows.Devices.Enumeration;

namespace POS.Views.LogIn;

public sealed partial class LogInPage : Page
{
    private readonly INavigationService _navigationService;
    private UIElement? _shell = null;
    private readonly IConnectionCheck _connection;
    private readonly IGetService _iGetService;
    public LoginViewModel ViewModel
    {
        get;
    }
    public LogInPage(LoginViewModel viewModel, INavigationService navigationService, IConnectionCheck connection, IGetService iGetService)
    {
        ViewModel = viewModel;
        _navigationService = navigationService;
        this.InitializeComponent();
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleBarText.Text = "iBOS POS";
        _connection = connection;
        _iGetService = iGetService;
    }
    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";

        AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
    }
    private async void LogIn(object sender, KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AppSettings.IsOnline = true;
                if (IsOffline.IsChecked == true)
                {
                    AppSettings.IsOnline = false;
                }
                if (userName.Text.ToUpper() != "ADMIN")
                {

                    if (IsOffline.IsChecked == false && await _connection.IsServerConnectionAvailable() == false)
                    {
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("The server is offline. Please contact to the administrator.", "LogIn");
                        return;
                    }
                    else
                    {
                        AppSettings.IsSync = true;
                    }
                }
                else
                {
                    AppSettings.IsSync = true;
                }
                LoginModel dt = new LoginModel();
                dt.UserName = userName.Text;
                dt.Password = UserPassword.Password;
                var isAdminstration = await ViewModel.AdminLogIn(dt);
                AppSettings.IsOnline = true;
                if (IsOffline.IsChecked == true)
                {
                    AppSettings.IsOnline = false;
                }
                else
                {
                    if (await _connection.IsServerConnectionAvailable() == true)
                    {
                        await ViewModel.ItemSellingInvoiceSendToSQLDatabaseFromLogIn();
                    }
                }
                if (isAdminstration)
                {
                    AppSettings.IsAdmin = true;
                    _shell = App.GetService<ShellPage>();
                    App.MainWindow.Content = _shell ?? new Frame();

                    _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!, "");

                    App.MainWindow.Activate();
                }
                else
                {
                    DateTime CurrentDateTime = DateTime.Now.BD();
                    var isLoginToday = await ViewModel.GetUser(dt.UserName, dt.Password);
                    if (isLoginToday != null && isLoginToday.LastDateTime.Date >= CurrentDateTime.Date.AddDays(-5) && isLoginToday.strUserJWTToken.Trim() != "")
                    {
                        AppSettings.JWT_Token = isLoginToday.strUserJWTToken;
                        AppSettings.UserId = isLoginToday.intUserID;
                        AppSettings.IsAdmin = false;
                        var setting = await _iGetService.GetSettings();
                        AppSettings.APPURL = setting.AppUrl;
                        AppSettings.SQLServerConnectionString = setting.SqlServerConnString;
                        AppSettings.Counter = setting.StrCounterName;
                        AppSettings.POSUserName = isLoginToday.strEmployeeName;

                        _shell = App.GetService<ShellPage>();
                        App.MainWindow.Content = _shell ?? new Frame();

                        try
                        {
                            var check = await ViewModel.CheckCounterSession();
                            if (check != null)
                            {
                                _navigationService.NavigateTo(typeof(MainViewModel).FullName!, "");

                            }
                            else
                            {
                                _navigationService.NavigateTo(typeof(OutletViewModel).FullName!, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "LogIn");
                        }

                        App.MainWindow.Activate();
                    }
                    else
                    {


                        AppSettings.IsAdmin = false;
                        var setting = await _iGetService.GetSettings();
                        AppSettings.APPURL = setting?.AppUrl;
                        AppSettings.SQLServerConnectionString = setting?.SqlServerConnString;
                        var isConnection = await _connection.IsInternetAvailable();

                        if (isConnection == false)
                        {
                            ContentDialog containtDialog = new ContentDialog()
                            {
                                Title = "LogIn Failed",
                                Content = "Server Connection Is Not Available",
                                CloseButtonText = "Ok"
                            };
                            containtDialog.XamlRoot = this.Content.XamlRoot;
                            await containtDialog.ShowAsync();
                        }
                        else
                        {
                            dt.AccountId = setting.intAccountId;
                            dt.BranchId = setting.intBranchId;
                            dt.OfficeId = setting.intOfficeId;
                            dt.WarehouseId = setting.intWarehouseId;
                            dt.CounterId = setting.intCounterId;
                            var msg = await ViewModel.Login(dt);

                            if (msg.StatusCode != 200)
                            {
                                if (msg.StatusCode == 404)
                                {
                                    ContentDialog containtDialog = new ContentDialog()
                                    {
                                        Title = "LogIn",
                                        Content = "This user has no permission for this counter.",
                                        CloseButtonText = "Ok"
                                    };
                                    containtDialog.XamlRoot = this.Content.XamlRoot;
                                    await containtDialog.ShowAsync();
                                }
                                else
                                {
                                    ContentDialog containtDialog = new ContentDialog()
                                    {
                                        Title = "LogIn",
                                        Content = "LogIn Failed",
                                        CloseButtonText = "Ok"
                                    };
                                    containtDialog.XamlRoot = this.Content.XamlRoot;
                                    await containtDialog.ShowAsync();
                                }

                            }

                            if (msg.StatusCode == 200)
                            {
                                var user = await ViewModel.GetUser(dt.UserName, dt.Password);
                                AppSettings.Counter = setting.StrCounterName;
                                AppSettings.POSUserName = user.strEmployeeName;
                                _shell = App.GetService<ShellPage>();
                                App.MainWindow.Content = _shell ?? new Frame();

                                try
                                {
                                    var check = await ViewModel.CheckCounterSession();
                                    if (check != null)
                                    {
                                        _navigationService.NavigateTo(typeof(MainViewModel).FullName!, "");

                                    }
                                    else
                                    {
                                        _navigationService.NavigateTo(typeof(OutletViewModel).FullName!, "");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "LogIn");
                                }


                                App.MainWindow.Activate();


                            }
                        }
                    }
                }
                await Task.CompletedTask;
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "LogIn");

        }


    }

    private async void ClickLogIn(object sender, RoutedEventArgs e)
    {
        try
        {
            AppSettings.IsOnline = true;
            if (IsOffline.IsChecked == true)
            {
                AppSettings.IsOnline = false;
            }
            if (userName.Text.ToUpper() != "ADMIN")
            {
                if (IsOffline.IsChecked == false && await _connection.IsServerConnectionAvailable() == false)
                {
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("The server is offline. Please contact to the administrator.", "LogIn");
                    return;
                }
                else
                {
                    AppSettings.IsSync = true;
                }
            }
            else
            {
                AppSettings.IsSync = true;
            }
            LoginModel dt = new LoginModel();
            dt.UserName = userName.Text;
            dt.Password = UserPassword.Password;
            if (IsOffline.IsChecked == true)
            {
                AppSettings.IsOnline = false;
            }
            else
            {
                if (await _connection.IsServerConnectionAvailable() == true)
                {
                    await ViewModel.ItemSellingInvoiceSendToSQLDatabaseFromLogIn();
                }
            }
            var isAdminstration = await ViewModel.AdminLogIn(dt);
            if (isAdminstration)
            {
                AppSettings.IsAdmin = true;
                _shell = App.GetService<ShellPage>();
                App.MainWindow.Content = _shell ?? new Frame();

                _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!, "");

                App.MainWindow.Activate();
            }
            else
            {
                DateTime CurrentDateTime = DateTime.Now.BD();
                var isLoginToday = await ViewModel.GetUser(dt.UserName, dt.Password);
                if (isLoginToday != null && isLoginToday.LastDateTime.Date >= CurrentDateTime.Date.AddDays(-5) && isLoginToday.strUserJWTToken.Trim() != "")
                {
                    AppSettings.JWT_Token = isLoginToday.strUserJWTToken;
                    AppSettings.UserId = isLoginToday.intUserID;
                    AppSettings.IsAdmin = false;
                    var setting = await _iGetService.GetSettings();
                    AppSettings.APPURL = setting.AppUrl;
                    AppSettings.SQLServerConnectionString = setting.SqlServerConnString;
                    AppSettings.Counter = setting.StrCounterName;
                    AppSettings.POSUserName = isLoginToday.strEmployeeName;
                    _shell = App.GetService<ShellPage>();
                    App.MainWindow.Content = _shell ?? new Frame();

                    try
                    {
                        var check = await ViewModel.CheckCounterSession();
                        if (check != null)
                        {
                            _navigationService.NavigateTo(typeof(MainViewModel).FullName!, "");

                        }
                        else
                        {
                            _navigationService.NavigateTo(typeof(OutletViewModel).FullName!, "");
                        }
                    }
                    catch (Exception ex)
                    {
                        App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "LogIn");
                    }


                    App.MainWindow.Activate();
                }
                else
                {
                    AppSettings.IsAdmin = false;
                    var setting = await _iGetService.GetSettings();
                    AppSettings.APPURL = setting.AppUrl;
                    AppSettings.SQLServerConnectionString = setting.SqlServerConnString;
                    var isConnection = await _connection.IsInternetAvailable();

                    if (isConnection == false)
                    {
                        ContentDialog containtDialog = new ContentDialog()
                        {
                            Title = "LogIn Failed",
                            Content = "Server Connection Is Not Available",
                            CloseButtonText = "Ok"
                        };
                        containtDialog.XamlRoot = this.Content.XamlRoot;
                        await containtDialog.ShowAsync();
                    }
                    else
                    {
                        dt.AccountId = setting.intAccountId;
                        dt.BranchId = setting.intBranchId;
                        dt.OfficeId = setting.intOfficeId;
                        dt.WarehouseId = setting.intWarehouseId;
                        dt.CounterId = setting.intCounterId;

                        var msg = await ViewModel.Login(dt);

                        if (msg.StatusCode != 200)
                        {
                            if (msg.StatusCode == 404)
                            {
                                ContentDialog containtDialog = new ContentDialog()
                                {
                                    Title = "LogIn",
                                    Content = "This user has no permission for this counter.",
                                    CloseButtonText = "Ok"
                                };
                                containtDialog.XamlRoot = this.Content.XamlRoot;
                                await containtDialog.ShowAsync();
                            }
                            else
                            {
                                ContentDialog containtDialog = new ContentDialog()
                                {
                                    Title = "LogIn",
                                    Content = "LogIn Failed",
                                    CloseButtonText = "Ok"
                                };
                                containtDialog.XamlRoot = this.Content.XamlRoot;
                                await containtDialog.ShowAsync();
                            }
                        }


                        if (msg.StatusCode == 200)
                        {
                            var user = await ViewModel.GetUser(dt.UserName, dt.Password);
                            AppSettings.Counter = setting.StrCounterName;
                            AppSettings.POSUserName = user.strEmployeeName;
                            _shell = App.GetService<ShellPage>();
                            App.MainWindow.Content = _shell ?? new Frame();

                            try
                            {
                                var check = await ViewModel.CheckCounterSession();
                                if (check != null)
                                {
                                    _navigationService.NavigateTo(typeof(MainViewModel).FullName!, "");

                                }
                                else
                                {
                                    _navigationService.NavigateTo(typeof(OutletViewModel).FullName!, "");
                                }
                            }
                            catch (Exception ex)
                            {
                                App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "LogIn");
                            }

                            App.MainWindow.Activate();


                        }
                    }
                }
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "LogIn");

        }
    }

}
