using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS.Contracts.Services;
using POS.Core.Helpers;
using POS.Core.Models;
using POS.Core.ViewModels.CounterSession;
using POS.Services;
using POS.ViewModels;
using Windows.System;
using Windows.UI.Input.Preview.Injection;

namespace POS.Views;

public sealed partial class OutletPage : Page
{
    //private readonly INavigationService _navigationService;
    public OutletViewModel ViewModel
    {
        get;
    }

    public OutletPage()
    {
        ViewModel = App.GetService<OutletViewModel>();

        ViewModel.OnNavigatedTo("");
        if (ViewModel.officeList.Count == 0)
        {
            Thread.Sleep(2000);
        }

        InitializeComponent();
        ViewModel.Status = false;
        MainGridProcess.Visibility = Visibility.Collapsed;
    }

    private async void btnSessionOpen(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ViewModel.officeList.FirstOrDefault().selectedCounter == null)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked("Counter not found !", "Counter Session");
            return;
        }
        SessionOpenDialog.Title = "Session Opening";
        var CounterSessionId = ViewModel.officeList.Select(x => x.CounterSessionId).FirstOrDefault();
        if (CounterSessionId > 0)
        {
            var dt = await ViewModel.GetCounterSessionDetails(CounterSessionId);
            OpeningCash.Value = (double)dt.Select(x => x.OpeningCash).FirstOrDefault();
            OpeningNote.Text = dt.Select(x => x.OpeningNote).FirstOrDefault();
            foreach (var item in dt)
            {
                switch (item.CuerrencyName)
                {
                    case nameof(BDT1):
                        numBDT1.Value = item.CurrencyOpeningCount;
                        break;
                    case nameof(BDT2):
                        numBDT2.Value = item.CurrencyOpeningCount;
                        break;
                    case nameof(BDT5):
                        numBDT5.Value = item.CurrencyOpeningCount;
                        break;
                    case nameof(BDT10):
                        numBDT10.Value = item.CurrencyOpeningCount;
                        break;
                    case nameof(BDT20):
                        numBDT20.Value = item.CurrencyOpeningCount;
                        break;
                    case nameof(BDT50):
                        numBDT50.Value = item.CurrencyOpeningCount;
                        break;
                    case nameof(BDT100):
                        numBDT100.Value = item.CurrencyOpeningCount;
                        break;
                    case nameof(BDT200):
                        numBDT200.Value = item.CurrencyOpeningCount;
                        break;
                    case nameof(BDT500):
                        numBDT500.Value = item.CurrencyOpeningCount;
                        break;
                    case nameof(BDT1000):
                        numBDT1000.Value = item.CurrencyOpeningCount;
                        break;
                    default:
                        // Handle case where item.CurrencyName does not match any of the above cases
                        break;
                }
            }
        }
        await SessionOpenDialog.ShowAsync();
        
    }

    private async void SessionOpenDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (AppSettings.IsOnline == false)
        {
            SessionOpenDialog.Hide();
            App.GetService<IAppNotificationService>().OnNotificationInvoked("System is Offline! Can't Open Session Now .", "Counter Session");
            return;
        }
        DateTime currentDate = DateTime.Now.BD();
        var dt = ViewModel.officeList.Where(x => x.selectedCounter != null).FirstOrDefault().selectedCounter;
        CounterSessionDTO cs = new CounterSessionDTO()
        {
            AccountId = dt.AccountId,
            BranchId = dt.BranchId,
            OfficeId = dt.OfficeId,
            CounterId = dt.CounterId,
            CounterCode = dt.CounterName,
            IsSync = false,
            StartDatetime = currentDate,
            OpeningCash = (decimal)OpeningCash.Value,
            OpeningNote = OpeningNote.Text,
            ActionById = AppSettings.UserId,
            ServerDatetime = currentDate,
        };
        List<CounterSessionDetailsDTO> counterSessionDetails = new List<CounterSessionDetailsDTO>();
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT1), CurrencyOpeningCount = (long)numBDT1.Value, IsSync = false });
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT2), CurrencyOpeningCount = (long)numBDT2.Value, IsSync = false });
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT5), CurrencyOpeningCount = (long)numBDT5.Value, IsSync = false });
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT10), CurrencyOpeningCount = (long)numBDT10.Value, IsSync = false });
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT20), CurrencyOpeningCount = (long)numBDT20.Value, IsSync = false });
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT50), CurrencyOpeningCount = (long)numBDT50.Value, IsSync = false });
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT100), CurrencyOpeningCount = (long)numBDT100.Value, IsSync = false });
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT200), CurrencyOpeningCount = (long)numBDT200.Value, IsSync = false });
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT500), CurrencyOpeningCount = (long)numBDT500.Value, IsSync = false });
        counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, AccountId = dt.AccountId, ActionById = AppSettings.UserId, BranchId = dt.BranchId, CounterId = dt.CounterId, CurrencyName = nameof(BDT1000), CurrencyOpeningCount = (long)numBDT1000.Value, IsSync = false });

        CreateCounterSeason create = new CreateCounterSeason()
        {
            Session = cs,
            SessionDetails = counterSessionDetails,
        };
        try
        {
            MainGrid.Visibility = Visibility.Collapsed;
            MainGridProcess.Visibility = Visibility.Visible;
            ViewModel.Status = true;
            AppSettings.IsSync=true;
            var msg = await ViewModel.createCounterSession(create);
            AppSettings.IsSync = false;
            ViewModel.Status = false;
            MainGrid.Visibility = Visibility.Visible;
            MainGridProcess.Visibility = Visibility.Collapsed;
            if (msg.StatusCode == 200)
            {
                SessionOpenDialog.Hide();

                ContentDialog containtDialog = new ContentDialog()
                {
                    Title = "Counter Session",
                    Content = msg.Message,
                    CloseButtonText = "Ok"
                };
                containtDialog.XamlRoot = this.Content.XamlRoot;
                containtDialog.CloseButtonClick += containtDialog_CloseButtonClicked;
                await containtDialog.ShowAsync();
                
            }
        }
        catch (Exception ex)
        {
            AppSettings.IsSync = false;
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Counter Session");
        }

    }

    private void containtDialog_CloseButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var _navigationService = App.GetService<INavigationService>();
        _navigationService.NavigateTo(typeof(MainViewModel).FullName!);

        App.MainWindow.Activate();
    }
}
