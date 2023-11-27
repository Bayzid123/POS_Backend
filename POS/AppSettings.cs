using Windows.Storage;
using Windows.ApplicationModel;
using POS.Services;
using POS.Core.ViewModels.MainViewModelDTO;
namespace POS;

public class AppSettings
{
    const string DB_NAME = "POS";
    const string DB_VERSION = "1.01";
    const string DB_BASEURL = "ms-appx:///Assets/POSDB/POS.db";

    static AppSettings()
    {
        Current = new AppSettings();
    }

    public static AppSettings Current
    {
        get;
    }
    public static MainViewRecallInvoiceDTO MainViewRecallInvoice
    {
        get; set;
    }
    public static bool IsAdmin = false;
    public static bool IsReturn = false;
    public static bool IsSync = false;
    public static bool IsOnline = true;
    public static bool isInvoiceOnlineSearch = true;
    public static long PermisionValue;
    public static string APPURL = "";
    public static string Message = "";
    public static string salesOrderId;
    public static long UserId;
    public static long ServerUserId;

    public static string Counter;
    public static string POSUserName;


    public static string JWT_Token="";
    public static readonly string AppLogPath = "AppLog";
    public static readonly string AppLogName = $"AppLog.1.0.db";
    public static readonly string AppLogFileName = Path.Combine(AppLogPath, AppLogName);

    public readonly string AppLogConnectionString = $"Data Source={AppLogFileName}";
    public static readonly string ReportPath = "Reports";
    public static readonly string DatabasePath = "Database";
    public static readonly string DatabaseName = $"{DB_NAME}.{DB_VERSION}.db";
    public static readonly string DatabasePattern = $"{DB_NAME}.{DB_VERSION}.db";
    public static readonly string DatabaseFileName = Path.Combine(DatabasePath, DatabaseName);
    public static readonly string DatabasePatternFileName = Path.Combine(DatabasePath, DatabasePattern);
    public static readonly string DatabaseUrl = $"{DB_BASEURL}";
    public static string SQLServerConnectionString = "Data Source = 10.209.99.244; Initial Catalog = SME; User ID = isukisespts3vapp8dt; Password=wsa0str1vpo@8d5ws;Connect Timeout = 30; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;";
    public readonly string SQLiteConnectionString = $"Data Source={DatabaseFileName}";

    public static readonly string AppSecret = "7061747323313233";

    public ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

    public string Version
    {
        get
        {
            var ver = Package.Current.Id.Version;
            return $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
        }
    }

    public string DbVersion => DB_VERSION;

    public string UserName
    {
        get => GetSettingsValue("UserName", default(string));
        set => LocalSettings.Values["UserName"] = value;
    }
    public string APP_URL
    {
        get; set;
    }

    public string WindowsHelloPublicKeyHint
    {
        get => GetSettingsValue("WindowsHelloPublicKeyHint", default(string));
        set => LocalSettings.Values["WindowsHelloPublicKeyHint"] = value;
    }

    public DataProviderType DataProvider
    {
        get => (DataProviderType)GetSettingsValue("DataProvider", (int)DataProviderType.SQLite);
        set => LocalSettings.Values["DataProvider"] = (int)value;
    }

    //public string SQLServerConnectionString
    //{
    //    get => GetSettingsValue("SQLServerConnectionString", @"Data Source=.\SQLExpress;Initial Catalog=VanArsdelDb;Integrated Security=SSPI");
    //    set => SetSettingsValue("SQLServerConnectionString", value);
    //}

    public bool IsRandomErrorsEnabled
    {
        get => GetSettingsValue("IsRandomErrorsEnabled", false);
        set => LocalSettings.Values["IsRandomErrorsEnabled"] = value;
    }

    private TResult GetSettingsValue<TResult>(string name, TResult defaultValue)
    {
        try
        {
            if (!LocalSettings.Values.ContainsKey(name))
            {
                LocalSettings.Values[name] = defaultValue;
            }
            return (TResult)LocalSettings.Values[name];
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            return defaultValue;
        }
    }
    private void SetSettingsValue(string name, object value)
    {
        LocalSettings.Values[name] = value;
    }





    public static MainViewRecallInvoiceDTO HomePageRefreshObject;
    //{
    //    get; set;
    //} = new MainViewRecallInvoiceDTO();


}
