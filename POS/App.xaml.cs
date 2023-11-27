using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

using POS.Activation;
using POS.Contracts.Services;
using POS.Core;
using POS.Core.Contracts.Services;
using POS.Core.Services;
using POS.Helpers;
using POS.Models;
using POS.Notifications;
using POS.Reports;
using POS.Services;
using POS.Services.HttpsClient;
using POS.ViewModels;
using POS.Views;
using POS.Views.LogIn;
using POS.Views.SalesInvoice;

using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;

namespace POS;

public partial class App : Application
{

    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();
    public App()
    {
        InitializeComponent();


        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();
            services.AddSingleton<IDataServiceFactory, DataServiceFactory>();
            //services.AddSingleton<ISettingsService, SettingsService>();
            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();


            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<ISampleDataService, SampleDataService>();
            services.AddSingleton<IFileService, FileService>();


            services.AddTransient<IRestService, RestService>();
            services.AddTransient<IConnectionCheck, ConnectionCheck>();
            services.AddTransient<IHttpsClientHandlerService, HttpsClientHandlerService>();
            services.AddTransient<IGetService, GetService>();
            services.AddTransient<ICreateService, CreateService>();
            services.AddTransient<IMasterDataRestService, MasterDataRestService>();
            services.AddTransient<IInvoicePrint, InvoicePrint>();
            services.AddTransient<IReportrdlc, Reportrdlc>();
            //services.AddTransient<SQLiteDb>();
            // Views and ViewModels
            services.AddTransient<ChangePasswordViewModel>();
            services.AddTransient<ChangePasswordPage>();
            services.AddTransient<AboutViewModel>();
            services.AddTransient<AboutPage>();
            services.AddTransient<OutletViewModel>();
            services.AddTransient<OutletPage>();
            services.AddTransient<SettingsViewModel>();

            services.AddTransient<SettingsPage>();
            services.AddTransient<DataGridViewModel>();
            services.AddTransient<DataGridPage>();
            services.AddTransient<POSReportViewModel>();
            services.AddTransient<POSReportPage>();
            services.AddTransient<POSConfigViewModel>();
            services.AddTransient<POSConfigPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<LogInPage>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<InvoicePage>();
            services.AddTransient<SalesInvoiceViewModel>();
            services.AddTransient<InvoiceViewModel>();

            services.AddTransient<IMasterDataSQLRestService, MasterDataSQLRestService>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        this.UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true;

        // TODO: Log and handle exceptions as appropriate.
        App.GetService<IAppNotificationService>().OnNotificationInvoked(e.Message, "Notification");


        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        await EnsureLogDbAsync();
        await EnsureDatabaseAsync();

        Constants.DeviceID = "";
        var deviceInformation = new EasClientDeviceInformation();
        Constants.DeviceID = deviceInformation.Id.ToString();
        Constants.DeviceMAC = (
        from nic in NetworkInterface.GetAllNetworkInterfaces()
        where nic.OperationalStatus == OperationalStatus.Up
        select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

        App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);
    }

    private static async Task EnsureDatabaseAsync()
    {
        await EnsureSQLiteDatabaseAsync();
    }

    private static async Task EnsureSQLiteDatabaseAsync()
    {


        var localFolder = ApplicationData.Current.LocalFolder;
        var databaseFolder = await localFolder.CreateFolderAsync(AppSettings.DatabasePath, CreationCollisionOption.OpenIfExists);

        if (await databaseFolder.TryGetItemAsync(AppSettings.DatabaseName) == null)
        {
            if (await databaseFolder.TryGetItemAsync(AppSettings.DatabasePattern) == null)
            {
                //using (var cli = new WebClient())
                //{
                //var bytes = await Task.Run(() =>  cli.DownloadData(AppSettings.DatabaseUrl));
                var sourceMainFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(AppSettings.DatabaseUrl));
                if (sourceMainFile != null)
                {
                    var streamMain = await sourceMainFile.OpenStreamForReadAsync();
                    var bytes = new byte[(int)streamMain.Length];
                    streamMain.Read(bytes, 0, (int)streamMain.Length);
                    var file = await databaseFolder.CreateFileAsync(AppSettings.DatabasePattern, CreationCollisionOption.ReplaceExisting);
                    using var stream = await file.OpenStreamForWriteAsync();
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
                //}
            }
            //var sourceFile = await databaseFolder.GetFileAsync(AppSettings.DatabasePattern);
            //var targetFile = await databaseFolder.CreateFileAsync(AppSettings.DatabaseName, CreationCollisionOption.ReplaceExisting);
            //await sourceFile.CopyAndReplaceAsync(targetFile);
        }


        var reportFolder = await localFolder.CreateFolderAsync(AppSettings.ReportPath, CreationCollisionOption.OpenIfExists);

        if (await reportFolder.TryGetItemAsync(AppSettings.ReportPath) == null)
        {
            await reportFolder.DeleteAsync();
            reportFolder = await localFolder.CreateFolderAsync(AppSettings.ReportPath, CreationCollisionOption.OpenIfExists);

            var sourceLogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/RDLC/rptInvoice.rdlc"));
            var targetLogFile = await reportFolder.CreateFileAsync(sourceLogFile.Name, CreationCollisionOption.ReplaceExisting);
            await sourceLogFile.CopyAndReplaceAsync(targetLogFile);
             sourceLogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/RDLC/rptInvoiceRePrint.rdlc"));
             targetLogFile = await reportFolder.CreateFileAsync(sourceLogFile.Name, CreationCollisionOption.ReplaceExisting);
            await sourceLogFile.CopyAndReplaceAsync(targetLogFile);

            sourceLogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/RDLC/rptPrint.rdlc"));
             targetLogFile = await reportFolder.CreateFileAsync(sourceLogFile.Name, CreationCollisionOption.ReplaceExisting);
            await sourceLogFile.CopyAndReplaceAsync(targetLogFile);

        }
    }
    private static async Task EnsureLogDbAsync()
    {
        var localFolder = ApplicationData.Current.LocalFolder;
        var appLogFolder = await localFolder.CreateFolderAsync(AppSettings.AppLogPath, CreationCollisionOption.OpenIfExists);
        if (await appLogFolder.TryGetItemAsync(AppSettings.AppLogName) == null)
        {
            var sourceLogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AppLog/AppLog.db"));
            var targetLogFile = await appLogFolder.CreateFileAsync(AppSettings.AppLogName, CreationCollisionOption.ReplaceExisting);
            await sourceLogFile.CopyAndReplaceAsync(targetLogFile);
        }
    }
}
