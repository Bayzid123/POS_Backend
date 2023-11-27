using Microsoft.UI.Xaml;
using System.Timers;
using POS.Helpers;
using System.Diagnostics;
using POS.Services.HttpsClient;
using POS.ViewModels;
using POS.Core.Models;

namespace POS;

public sealed partial class MainWindow : WindowEx
{
    private System.Timers.Timer _timer;
    private System.Timers.Timer _timer1;
    private System.Timers.Timer _timer2;
    private System.Timers.Timer _timer3;
    private System.Timers.Timer _timer4;
    private System.Timers.Timer _timer5;
    private System.Timers.Timer _timer6;
    public MainViewModel ViewModel
    {
        get;
    }
    public MainWindow()
    {
        InitializeComponent();

        ViewModel = App.GetService<MainViewModel>();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/managerium_logo.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
        _timer6 = new System.Timers.Timer(15 *  1000);
        _timer6.Elapsed += Timer_Elapsed_Network;
        _timer6.AutoReset = true;
        _timer6.Enabled = true;

        _timer = new System.Timers.Timer(30 * 60 * 1000);  
        _timer.Elapsed += Timer_Elapsed;
        _timer.AutoReset = true;
        _timer.Enabled = true;

        _timer1 = new System.Timers.Timer(10* 60 * 1000);

        _timer1.Elapsed += Timer_ElapsedInvoice;
        _timer1.AutoReset = true;
        _timer1.Enabled = true;

        //_timer2 = new System.Timers.Timer(60 * 1000);
        _timer2 = new System.Timers.Timer(25 * 60 * 1000);

        _timer2.Elapsed += Timer_ElapsedSession;
        _timer2.AutoReset = true;
        _timer2.Enabled = true;

        _timer3 = new System.Timers.Timer(35 * 60 * 1000);

        _timer3.Elapsed += Timer_ElapsedPartner;
        _timer3.AutoReset = true;
        _timer3.Enabled = true;

        _timer4 = new System.Timers.Timer(20 * 1000);
        _timer4.Elapsed += Timer_ElapsedPromotion;
        _timer4.AutoReset = true;
        _timer4.Enabled = true;
        _timer5 = new System.Timers.Timer(28 * 60 * 1000);  
        _timer5.Elapsed += Timer_Elapsed_Item;
        _timer5.AutoReset = true;
        _timer5.Enabled = true;
    }


    private async void Timer_Elapsed_Network(object sender, ElapsedEventArgs e)
    {
        try
        {
            //if (ViewModel.IsServerConnectionForLocal() == true)
            //{
            //    AppSettings.IsOnline = true;
            //}
            //else
            //{
            //    AppSettings.IsOnline = false;
            //}

        }
        catch
        {
            //throw ex;
        }


    }
    private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        try
        {
            //if (ViewModel.IsServerConnectionForLocal()==true)
            //{
            //    AppSettings.IsOnline = true;
            //}
            //else
            //{
            //    AppSettings.IsOnline = false;
            //}
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() && AppSettings.IsSync == true && AppSettings.IsOnline == true)
            {   
                await ViewModel.ItemSellingPriceRowChange();
            }
            
               
        }
        catch
        {
            //throw ex;
        }

       
    }
    private async void Timer_Elapsed_Item(object sender, ElapsedEventArgs e)
    {
        try
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() && AppSettings.IsSync == true && AppSettings.IsOnline == true)
            {
                await ViewModel.ItemRowChange();
         
            }
      
        }
        catch
        {
            //throw ex;
        }


    }
    private async void Timer_ElapsedPartner(object sender, ElapsedEventArgs e)
    {
        try
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() && AppSettings.IsSync == true && AppSettings.IsOnline == true)
                await ViewModel.PartnerRowChange();
        }
        catch
        {
            //throw ex;
        }

        // Add your code to run every 5 minutes here
    }
    private async void Timer_ElapsedInvoice(object sender, ElapsedEventArgs e)
    {
        try
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() && AppSettings.IsSync == true && AppSettings.IsOnline == true )
                await ViewModel.ItemSellingInvoiceSendToSQLDatabase();
        }
        catch
        {
            //throw null;
        }
        // Add your code to run every 5 minutes here
    }

    private async void Timer_ElapsedPromotion(object sender, ElapsedEventArgs e)
    {
        try
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() && AppSettings.IsSync == true && AppSettings.IsOnline == true )
            {
               
                await ViewModel.PromotionDiscountFromSQLServerToSQLiteDatabase();
                await ViewModel.UserProcess();

            }
                
        }
        catch
        {
            //throw null;
        }
        // Add your code to run every 5 minutes here
    }
    private async void Timer_ElapsedSession(object sender, ElapsedEventArgs e)
    {
        try
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() && AppSettings.IsSync == true && AppSettings.IsOnline == true)
            {
                var msg = await ViewModel.DeleteSessionData();
                await ViewModel.CreateSqlServerSession();

                await ViewModel.DeleteDataLog();
                var dt = await ViewModel.GetDataLogs();
                if (dt.Count > 0)
                {
                    var newdt = dt.Select(x => new SQLtblDataLog
                    {
                        LogId = x.LogId,
                        AccountId = x.AccountId,
                        BranchId = x.BranchId,
                        OfficeId = x.OfficeId,
                        CounterId = x.CounterId,
                        ServerUserId = x.ServerUserId,
                        strLogMessage = x.strLogMessage,
                        strEntityData = x.strEntityData,
                        LogType = x.LogType,
                        LastActionDateTime = x.LastActionDateTime,
                        ServerDateTime = x.ServerDateTime,
                        IsSync = x.IsSync,
                    }).ToList();
                    await ViewModel.SqlDataLog(newdt);
                }

            }

        }
        catch (Exception)
        {

            //throw null;
        }
    }

    private void startStopButton_Click(object sender, RoutedEventArgs e)
    {

    }
}
