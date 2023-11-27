using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection.Metadata;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using POS.Contracts.Services;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.MainViewModelDTO;
using POS.ViewModels;
using POS.Views.LogIn;
using WinRT.Interop;
using POS.Core.ViewModels.CounterSession;
using POS.Core.Helpers;
using POS.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using BarcodeLib;
using System.Diagnostics;
using System.Timers;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualBasic.Logging;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.EntityFrameworkCore;

namespace POS.Views;

public sealed partial class MainPage : Page
{
    private UIElement? _shell = null;
    private System.Timers.Timer _timer;
    private System.Timers.Timer _timer2;
    int i = 0, j = 0;
    public MainViewModel ViewModel
    {
        get;
    }
    long SalesOrderId = 0;

    public MainPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<MainViewModel>();
        LoadDataAsync();
        if (AppSettings.IsOnline == false)
            IsOnlineCheck.Text = "System Is Now Offline.";
        else
            IsOnlineCheck.Text = "";

        _timer2 = new System.Timers.Timer(10 * 1000);
        _timer2.Elapsed += Timer_Elapsed;
        _timer2.AutoReset = true;
        _timer2.Enabled = true;

        _timer = new System.Timers.Timer(1800); // 5 minutes in milliseconds
        _timer.Elapsed += TemporaryHomePageStore;
        _timer.AutoReset = true;
        _timer.Enabled = true;
        btnPrint.IsEnabled = true;

    }

    private void Timer_Elapsed(object sender, EventArgs e) => DispatcherQueue.TryEnqueue(() =>
    {
        try
        {
            if (AppSettings.IsOnline == false)
            {
                IsOnlineCheck.Text = "System Is Now Offline.";


            }
            else
            {
                IsOnlineCheck.Text = "";


            }

        }
        catch
        {
            //throw ex;
        }
    });



    private void TemporaryHomePageStore(object sender, ElapsedEventArgs e)
    {
        try
        {

            if (ViewModel.homePageItemList != null && ViewModel.homePageItemList.Count > 0)
            {
                AppSettings.HomePageRefreshObject = new MainViewRecallInvoiceDTO();
                AppSettings.HomePageRefreshObject.Items = new List<MainViewModelItemDTO>();
                AppSettings.HomePageRefreshObject.PaymentModeInformation = new List<PaymentModeInformation>();
                AppSettings.HomePageRefreshObject.collection = new MyCollection();

                if (ViewModel.homePageItemList != null && ViewModel.homePageItemList.Count > 0)
                {
                    AppSettings.HomePageRefreshObject.Items = ViewModel.homePageItemList.ToList();

                }

                if (ViewModel.itemPaymentModeList != null && ViewModel.itemPaymentModeList.Count > 0)
                {
                    AppSettings.HomePageRefreshObject.PaymentModeInformation = ViewModel.itemPaymentModeList.ToList();

                }

                //if (ViewModel.CollectionObj != null)
                //{
                //    AppSettings.HomePageRefreshObject.collection.TotalBill = ViewModel.CollectionObj?.TotalBill ?? "";
                //    AppSettings.HomePageRefreshObject.collection.NumtotalBill = ViewModel.CollectionObj?.NumtotalBill ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.TotalSD = ViewModel.CollectionObj?.TotalSD ?? "";
                //    AppSettings.HomePageRefreshObject.collection.NumtotalSD = ViewModel.CollectionObj?.NumtotalSD ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.TotalVAT = ViewModel.CollectionObj?.TotalVAT ?? "";
                //    AppSettings.HomePageRefreshObject.collection.NumtotalVAT = ViewModel.CollectionObj?.NumtotalVAT ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.TotalDiscount = ViewModel.CollectionObj?.TotalDiscount ?? "";
                //    AppSettings.HomePageRefreshObject.collection.NumTotalDiscount = ViewModel.CollectionObj?.NumTotalDiscount ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.OtherDiscount = ViewModel.CollectionObj?.OtherDiscount ?? "";
                //    AppSettings.HomePageRefreshObject.collection.NumotherDiscount = ViewModel.CollectionObj?.NumotherDiscount ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.GrandTotal = ViewModel.CollectionObj?.GrandTotal ?? "";
                //    AppSettings.HomePageRefreshObject.collection.NumGrandTotal = ViewModel.CollectionObj?.NumGrandTotal ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.ReceiveAmount = ViewModel.CollectionObj?.ReceiveAmount ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.ChangeAmount = ViewModel.CollectionObj?.ChangeAmount ?? 0.0M;
                //    //AppSettings.HomePageRefreshObject.collection.OtherDiscountType = ViewModel.CollectionObj?.OtherDiscountType ?? 0;
                //    AppSettings.HomePageRefreshObject.collection.NumOtherDiscountPercentage = ViewModel.CollectionObj?.NumOtherDiscountPercentage ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.NumOtherDiscountAmount = ViewModel.CollectionObj?.NumOtherDiscountAmount ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.OtherDiscountId = ViewModel.CollectionObj?.OtherDiscountId ?? 0;
                //    AppSettings.HomePageRefreshObject.collection.CashPayment = ViewModel.CollectionObj?.CashPayment ?? 0.0M;
                //    AppSettings.HomePageRefreshObject.collection.InvoiceNumber = ViewModel.CollectionObj?.InvoiceNumber ?? "";
                //    AppSettings.HomePageRefreshObject.collection.CustomerId = ViewModel.CollectionObj?.CustomerId ?? 0;
                //    AppSettings.HomePageRefreshObject.collection.CustomerName = ViewModel.CollectionObj?.CustomerName ?? "";
                //    AppSettings.HomePageRefreshObject.collection.CustomerCode = ViewModel.CollectionObj?.CustomerCode ?? "";
                //    AppSettings.HomePageRefreshObject.collection.CustomerPoints = ViewModel.CollectionObj?.CustomerPoints ?? "";
                //}
            }
            else
            {
                AppSettings.HomePageRefreshObject = new MainViewRecallInvoiceDTO();
                AppSettings.HomePageRefreshObject.Items = new List<MainViewModelItemDTO>();
                AppSettings.HomePageRefreshObject.PaymentModeInformation = new List<PaymentModeInformation>();
                AppSettings.HomePageRefreshObject.collection = new MyCollection();
            }
        }
        catch
        {

        }
    }


    private async void ClearFields()
    {
        ViewModel.homePageItemList.Clear();

        ViewModel.CollectionObj.NumtotalBill = 0.0M;
        ViewModel.CollectionObj.NumtotalSD = 0.0M;
        ViewModel.CollectionObj.NumtotalVAT = 0.0M;
        ViewModel.CollectionObj.NumTotalDiscount = 0.0M;
        ViewModel.CollectionObj.NumotherDiscount = 0.0M;
        ViewModel.CollectionObj.NumGrandTotal = 0.0M;
        ViewModel.CollectionObj.ReceiveAmount = 0.0M;
        ViewModel.CollectionObj.ChangeAmount = 0.0M;
        ViewModel.CollectionObj.NumOtherDiscountPercentage = 0.0M;
        ViewModel.CollectionObj.CashPayment = 0M;
        ViewModel.CollectionObj.OtherDiscountId = 0;

        //newly added......
        ViewModel.CollectionObj.OtherDiscountId = -1;
        ViewModel.CollectionObj.MaxDiscountAmount = 0.0M;
        ViewModel.CollectionObj.MinDiscountAmount = 0.0M;
        //newly added......

        ViewModel.itemPaymentModeList.Clear();
        //ViewModel.PaymentWalletDTOList.Clear();
        ViewModel.recallInvoiceHomeObjDTOs.Clear();
        NumberBoxDiscountRate.SelectedItem = null;

        ViewModel.SelectedCustomerObj.Id = "";
        ViewModel.SelectedCustomerObj.CustomerName = "";
        ViewModel.SelectedCustomerObj.CustomerAddress = "";

        TextBoxTotalBill.Text = "0.00";
        TextBoxTotalSD.Text = "0.00";
        TextBoxTotalVAT.Text = "0.00";
        TextBoxTotalDiscount.Text = "0.00";
        TextBoxOtherDiscount.Text = "0.00";


        GrandTotal.Text = "0.00";
        NumberBoxDiscountRate.Text = "0.00";
        textBoxCashAmount.Text = "";
        textBlockPaidAmount.Text = "0.00";
        textBlockChangeAmount.Text = "0.00";
        textBlockPrintTotalAmount.Text = "0.00";
        textBoxTotalSelectedItem.Text = "0 Items";

        textPointVale.Text = "0.00";
        txtCustomer.Text = "";
        txtCustomerName.Text = "";

        itemStock.Text = "0.0";
        //itemQty.Text ="0.0";
        //itemSellingRate.Text ="0.0";

        textBoxWalletAmount.Text = "";
        itemBarCode.Text = "";
        InvoiceNote.Text = "";
        var InvoiceNumber = await ViewModel.InvoiceCodeGenerate(0, "0");
        textINVNumber.Text = "INV#" + InvoiceNumber;
        ViewModel.CollectionObj.InvoiceNumber = InvoiceNumber;
        SalesOrderId = 0;
        AppSettings.IsReturn = false;
        txtCustomer.IsEnabled = true;

        if (btnPrint.IsEnabled == false)
            btnPrint.IsEnabled = true;
        ViewModel.OtherDiscountList.Clear();
        var NoneDiscount = new OtherDiscountDTO
        {
            HeaderId = 0,
            OfferName = "None",
            DiscountType = 0,
            StrDiscountType = "None",
            Value = 0.0M,
        };

        ViewModel.OtherDiscountList.Insert(0, NoneDiscount);
        var responseDiscount = await ViewModel.GetOtherDiscountList();
        foreach (var (item, index) in responseDiscount.Select((value, i) => (value, i)))
        {
            ViewModel.OtherDiscountList.Insert(index + 1, item);
        }

        //ViewModel.OtherDiscountList.Add(NoneDiscount);
        //var responseDiscount = await ViewModel.GetOtherDiscountList();
        //ViewModel.OtherDiscountList.AddRange(responseDiscount);

        ViewModel.isRecollRecallInvoice = await ViewModel.InvoiceCheck(AppSettings.UserId);
        if (ViewModel.isRecollRecallInvoice == true)
        {
            grdRecallInvoice.Visibility = Visibility.Collapsed;
            grdRecallInvoiceSecond.Visibility = Visibility.Visible;

        }
        else
        {
            grdRecallInvoice.Visibility = Visibility.Visible;
            grdRecallInvoiceSecond.Visibility = Visibility.Collapsed;
        }

        AppSettings.HomePageRefreshObject = new MainViewRecallInvoiceDTO();
        AppSettings.HomePageRefreshObject.Items = null;
        AppSettings.HomePageRefreshObject.PaymentModeInformation = null;
        AppSettings.HomePageRefreshObject.collection = null;
    }
    private void SetTextFieldValue()
    {
        try
        {
            //collection......
            ViewModel.CollectionObj.NumtotalBill = Math.Round((ViewModel.homePageItemList.Select(n => Convert.ToDecimal(n.Amount)).Sum()), 2);
            ViewModel.CollectionObj.NumtotalSD = Math.Round((ViewModel.homePageItemList.Select(n => Convert.ToDecimal(n.SD)).Sum()), 2);
            ViewModel.CollectionObj.NumtotalVAT = Math.Round((ViewModel.homePageItemList.Select(n => Convert.ToDecimal(n.Vat)).Sum()), 2);
            ViewModel.CollectionObj.NumTotalDiscount = Math.Round((ViewModel.homePageItemList.Select(n => Convert.ToDecimal(n.Discount)).Sum()), 2);







            //ViewModel.CollectionObj.NumotherDiscount = ViewModel.CollectionObj.OtherDiscountType == 1 ? (Math.Round((ViewModel.CollectionObj.NumOtherDiscountAmount), 2)) : (Math.Round(((ViewModel.CollectionObj.NumtotalBill * ViewModel.CollectionObj.NumOtherDiscountPercentage / 100)), 2));
            var grandTotalWithoutOtherDiscount = ViewModel.CollectionObj.NumtotalBill + ViewModel.CollectionObj.NumtotalSD + ViewModel.CollectionObj.NumtotalVAT + ViewModel.CollectionObj.NumTotalDiscount;
            if (grandTotalWithoutOtherDiscount >= ViewModel.CollectionObj.MinDiscountAmount && grandTotalWithoutOtherDiscount <= ViewModel.CollectionObj.MaxDiscountAmount)
            {
                ViewModel.CollectionObj.NumotherDiscount = ViewModel.CollectionObj.OtherDiscountType == 1 ? (Math.Round((ViewModel.CollectionObj.NumOtherDiscountAmount), 2)) : (Math.Round(((ViewModel.CollectionObj.NumtotalBill * ViewModel.CollectionObj.NumOtherDiscountPercentage / 100)), 2));
            }
            else
            {
                ViewModel.CollectionObj.NumotherDiscount = 0.0M;

                //special discount initialize to index == 0
                if(ViewModel.OtherDiscountList.Count > 0)
                    NumberBoxDiscountRate.SelectedIndex = 0;
                //special discount initialize to index == 0
            }
            //else
            //{
            //    ViewModel.CollectionObj.OtherDiscountType = 0;
            //    ViewModel.CollectionObj.NumotherDiscount = 0.0M;
            //    ViewModel.CollectionObj.NumOtherDiscountPercentage = 0.0M;
            //    ViewModel.CollectionObj.NumOtherDiscountAmount = 0.0M;
            //    ViewModel.CollectionObj.OtherDiscountId = 0;

            //    ViewModel.CollectionObj.MaxDiscountAmount = 0.0M;
            //    ViewModel.CollectionObj.MinDiscountAmount = 0.0M;
            //    //App.GetService<IAppNotificationService>().OnNotificationInvoked("$ " + ViewModel.CollectionObj.NumGrandTotal.ToString() + " Amount is not Applicable for this Discount", "POS");
            //}






            ViewModel.CollectionObj.NumGrandTotal = Math.Round((ViewModel.CollectionObj.NumtotalBill + ViewModel.CollectionObj.NumtotalSD + ViewModel.CollectionObj.NumtotalVAT - ViewModel.CollectionObj.NumTotalDiscount - ViewModel.CollectionObj.NumotherDiscount), 0, MidpointRounding.AwayFromZero);


            TextBoxTotalBill.Text = ViewModel.CollectionObj.NumtotalBill.ToString();
            TextBoxTotalSD.Text = ViewModel.CollectionObj.NumtotalSD.ToString();
            TextBoxTotalVAT.Text = ViewModel.CollectionObj.NumtotalVAT.ToString();
            TextBoxTotalDiscount.Text = ViewModel.CollectionObj.NumTotalDiscount.ToString();
            TextBoxOtherDiscount.Text = ViewModel.CollectionObj.NumotherDiscount.ToString();
            GrandTotal.Text = ViewModel.CollectionObj.NumGrandTotal.ToString();
            //collection......
            NumberBoxDiscountRate.Text = ViewModel.CollectionObj.NumOtherDiscountPercentage.ToString();

            var totalAddedAmount = Math.Round((ViewModel.itemPaymentModeList.Select(n => n.numberAmount).Sum() + ViewModel.CollectionObj.CashPayment), 2);
            //textBoxPaidAmount.Text = totalAddedAmount.ToString();
            textBoxCashAmount.Text = ViewModel.CollectionObj.CashPayment.ToString();

            textBlockPaidAmount.Text = totalAddedAmount.ToString();
            var grandTotal = Convert.ToDecimal(GrandTotal.Text);
            //textBlockChangeAmount.Text = (totalAddedAmount - grandTotal) > 0 ? (Math.Round((totalAddedAmount - grandTotal), 2)).ToString() : "0";
            textBlockChangeAmount.Text = (totalAddedAmount - grandTotal).ToString();

            textBlockPrintTotalAmount.Text = ViewModel.CollectionObj.NumGrandTotal.ToString();

            textBoxTotalSelectedItem.Text = ViewModel.homePageItemList.Select(n => Convert.ToDecimal(n.Quantity)).Sum().ToString() + " Items";

            ViewModel.CollectionObj.ReceiveAmount = totalAddedAmount;
            ViewModel.CollectionObj.ChangeAmount = (totalAddedAmount - grandTotal) > 0 ? (totalAddedAmount - grandTotal) : 0;

            textINVNumber.Text = "INV#" + ViewModel.CollectionObj.InvoiceNumber;
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }
    private async void LoadDataAsync()
    {
        ViewModel.isRecollRecallInvoice = false;
        if (ViewModel.PaymentWalletDTOList.Count <= 0)
        {
            var response = await ViewModel.GetPaymentWalletList();
            ViewModel.PaymentWalletDTOList.AddRange(response);
        }

        if (ViewModel.OtherDiscountList.Count <= 0)
        {
            var NoneDiscount = new OtherDiscountDTO
            {
                HeaderId = 0,
                OfferName = "None",
                DiscountType = 0,
                StrDiscountType = "None",
                Value = 0.0M,
            };
            ViewModel.OtherDiscountList.Insert(0, NoneDiscount);
            var responseDiscount = await ViewModel.GetOtherDiscountList();
            foreach (var (item, index) in responseDiscount.Select((value, i) => (value, i)))
            {
                ViewModel.OtherDiscountList.Insert(index + 1, item);
            }
            //ViewModel.OtherDiscountList.Add(NoneDiscount);
            //var responseDiscount = await ViewModel.GetOtherDiscountList();
            //ViewModel.OtherDiscountList.AddRange(responseDiscount);

        }

        var InvoiceNumber = await ViewModel.InvoiceCodeGenerate(0, "0");
        ViewModel.CollectionObj.InvoiceNumber = InvoiceNumber;


        try
        {
            //new code for restoring homepage store information........
            if (AppSettings.HomePageRefreshObject != null)
            {
                if (AppSettings.HomePageRefreshObject.Items != null && AppSettings.HomePageRefreshObject.Items.Count > 0)
                {
                    foreach (var singleItem in AppSettings.HomePageRefreshObject.Items)
                    {
                        ViewModel.homePageItemList.Add(singleItem);
                    }
                }

                if (AppSettings.HomePageRefreshObject.PaymentModeInformation != null && AppSettings.HomePageRefreshObject.PaymentModeInformation.Count > 0)
                {
                    foreach (var singlePay in AppSettings.HomePageRefreshObject.PaymentModeInformation)
                    {
                        ViewModel.itemPaymentModeList.Add(singlePay);
                    }
                }

                if (AppSettings.HomePageRefreshObject.collection != null)
                {
                    //    ViewModel.CollectionObj.TotalBill = AppSettings.HomePageRefreshObject.collection.TotalBill;
                    //    ViewModel.CollectionObj.NumtotalBill = AppSettings.HomePageRefreshObject.collection.NumtotalBill;
                    //    ViewModel.CollectionObj.TotalSD = AppSettings.HomePageRefreshObject.collection.TotalSD;
                    //    ViewModel.CollectionObj.NumtotalSD = AppSettings.HomePageRefreshObject.collection.NumtotalSD;
                    //    ViewModel.CollectionObj.TotalVAT = AppSettings.HomePageRefreshObject.collection.TotalVAT;
                    //    ViewModel.CollectionObj.NumtotalVAT = AppSettings.HomePageRefreshObject.collection.NumtotalVAT;
                    //    ViewModel.CollectionObj.TotalDiscount = AppSettings.HomePageRefreshObject.collection.TotalDiscount;
                    //    ViewModel.CollectionObj.NumTotalDiscount = AppSettings.HomePageRefreshObject.collection.NumTotalDiscount;
                    //    ViewModel.CollectionObj.OtherDiscount = AppSettings.HomePageRefreshObject.collection.OtherDiscount;
                    //    ViewModel.CollectionObj.NumotherDiscount = AppSettings.HomePageRefreshObject.collection.NumotherDiscount;
                    //    ViewModel.CollectionObj.GrandTotal = AppSettings.HomePageRefreshObject.collection.GrandTotal;
                    //    ViewModel.CollectionObj.NumGrandTotal = AppSettings.HomePageRefreshObject.collection.NumGrandTotal;
                    //    ViewModel.CollectionObj.ReceiveAmount = AppSettings.HomePageRefreshObject.collection.ReceiveAmount;
                    //    ViewModel.CollectionObj.ChangeAmount = AppSettings.HomePageRefreshObject.collection.ChangeAmount;
                    //    //ViewModel.CollectionObj.OtherDiscountType = AppSettings.HomePageRefreshObject.collection.OtherDiscountType;
                    //    ViewModel.CollectionObj.NumOtherDiscountPercentage = AppSettings.HomePageRefreshObject.collection.NumOtherDiscountPercentage;
                    //    ViewModel.CollectionObj.NumOtherDiscountAmount = AppSettings.HomePageRefreshObject.collection.NumOtherDiscountAmount;
                    //    ViewModel.CollectionObj.OtherDiscountId = AppSettings.HomePageRefreshObject.collection.OtherDiscountId;
                    //    ViewModel.CollectionObj.CashPayment = AppSettings.HomePageRefreshObject.collection.CashPayment;
                    //    ViewModel.CollectionObj.InvoiceNumber = AppSettings.HomePageRefreshObject.collection.InvoiceNumber;
                    ViewModel.CollectionObj.CustomerId = AppSettings.HomePageRefreshObject.collection.CustomerId;
                    ViewModel.CollectionObj.CustomerName = AppSettings.HomePageRefreshObject.collection.CustomerName;
                    ViewModel.CollectionObj.CustomerCode = AppSettings.HomePageRefreshObject.collection.CustomerCode;
                    ViewModel.CollectionObj.CustomerPoints = AppSettings.HomePageRefreshObject.collection.CustomerPoints;
                    //textBoxCashAmount.Text = AppSettings.HomePageRefreshObject.collection.CashPayment.ToString();
                    textPointVale.Text = ViewModel.CollectionObj.CustomerPoints;
                    txtCustomer.Text = ViewModel.CollectionObj.CustomerCode;
                    txtCustomerName.Text = ViewModel.CollectionObj.CustomerName;
                    //    NumberBoxDiscountRate.SelectedItem = ViewModel.OtherDiscountList.Where(n => n.HeaderId == ViewModel.CollectionObj.OtherDiscountId).FirstOrDefault() ?? null;
                }
            }

            //new code for restroing homepage store information........
        }
        catch
        {

        }


        await Task.Run(() =>
        {
            Thread.Sleep(50);

        });


        SetTextFieldValue();
        await ViewModel.LoadOffice();
        itemBarCode.Focus(FocusState.Pointer);
        ViewModel.isRecollRecallInvoice = await ViewModel.InvoiceCheck(AppSettings.UserId);
        if (ViewModel.isRecollRecallInvoice == true)
        {
            grdRecallInvoice.Visibility = Visibility.Collapsed;
            grdRecallInvoiceSecond.Visibility = Visibility.Visible;

        }
        else
        {
            grdRecallInvoice.Visibility = Visibility.Visible;
            grdRecallInvoiceSecond.Visibility = Visibility.Collapsed;
        }

        btnPrint.IsEnabled = true;


    }

    private void VATRecalculateFunction(decimal SpecialDiscountAmount)
    {
        try
        {
            var totalSumAmount = ViewModel.homePageItemList.Where(n => Convert.ToDecimal(n.Quantity) >= 0).Sum(n => Convert.ToDecimal(n.Amount));
            foreach (var singleHomeItem in ViewModel.homePageItemList)
            {

                if (Convert.ToDecimal(singleHomeItem.Quantity) < 0)
                    continue;

                var currentItemAmount = Convert.ToDecimal(singleHomeItem.Amount);
                var itemSpecialDiscount = (SpecialDiscountAmount * currentItemAmount) / totalSumAmount;

                singleHomeItem.Vat = Math.Round((singleHomeItem.VATPercentage * ((Convert.ToDecimal(singleHomeItem.Quantity) * Convert.ToDecimal(singleHomeItem.SalesRate)) - singleHomeItem.Discount - itemSpecialDiscount)) / 100, 2);
                singleHomeItem.OtherDiscount = itemSpecialDiscount;
            }

            SetTextFieldValue();
        }
        catch
        {

        }
        
    }

    private void btnCustomerDetails(object sender, RoutedEventArgs e)
    {
        //EditDialog.Title = "Customer Details";
        //EditDialog.PrimaryButtonText = "Save";

        //await EditDialog.ShowAsync();
        //

    }

    private async void txtCustomer_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var partnerCode = txtCustomer.Text.Trim().ToString();
                var PartnerInformation = await ViewModel.GetPartnerById(partnerCode);
                if (PartnerInformation == null)
                {
                    textPointVale.Text = "0.00";
                    txtCustomer.Text = "";
                    txtCustomerName.Text = "";

                    App.GetService<IAppNotificationService>().OnNotificationInvoked("No Customer Found !", "POS");
                    return;
                }

                textPointVale.Text = PartnerInformation.Points.ToString();

                ViewModel.SelectedCustomerObj.Id = PartnerInformation.PartnerId.ToString();
                ViewModel.SelectedCustomerObj.CustomerName = PartnerInformation.PartnerName;
                ViewModel.SelectedCustomerObj.CustomerAddress = PartnerInformation.Address;
                txtCustomerName.Text = PartnerInformation.PartnerName;
                itemBarCode.Focus(FocusState.Pointer);


                if (PartnerInformation != null)
                {
                    ViewModel.CollectionObj.CustomerName = PartnerInformation.PartnerName;
                    ViewModel.CollectionObj.CustomerId = PartnerInformation.PartnerId;
                    ViewModel.CollectionObj.CustomerCode = partnerCode;
                }

                //var responsefrom = await ViewModel.FetchWalletInformationFromSQLServerToSQLiteDatabase();
                //var responsefrom = await ViewModel.FetchPromotionRowsFromSQLServerToSQLiteDatabase();
                //var response = await ViewModel.ItemSellingInvoiceSendToSQLDatabase();
                //await ViewModel.ItemSellingInvoiceSendToSQLDatabase();
                //await ViewModel.FetchSpecialDiscountFromSQLServerToSQLiteDatabase();
            }

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void itemBarCode_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (itemBarCode.Text == "")
                {
                    textBoxCashAmount.Text = "";

                    //textBoxCashAmount.Text = (Convert.ToDecimal(GrandTotal.Text)).ToString();
                    textBoxCashAmount.Text = textBoxCashAmount.Text = (Convert.ToDecimal(GrandTotal.Text) - Convert.ToDecimal(ViewModel.itemPaymentModeList.Sum(s => s.numberAmount))) > 0 ?
                  (Convert.ToDecimal(GrandTotal.Text) - Convert.ToDecimal(ViewModel.itemPaymentModeList.Sum(s => s.numberAmount))).ToString() : "0";

                    textBoxCashAmount.Focus(FocusState.Pointer);
                    var caretIndex = textBoxCashAmount.Text.Length;


                    if (caretIndex >= 0)
                    {

                        textBoxCashAmount.SelectionStart = caretIndex;
                    }
                    textBoxCashAmount.SelectAll();
                    return;
                }

                var ItemCode = itemBarCode.Text.Trim().ToString();
                var ItemCodeforPcs = ItemCode;
                var Quantity = 1.0M;

                var barCodeQuantity = 1.0M;

                //item bar code eikhane check hobe........
                if (ItemCode.Length >= 2)
                {
                    if (ItemCode[0] == '9' && ItemCode[1] == '9')
                    {
                        var barcode = ItemCode.Substring(2, 4);
                        Quantity = Convert.ToDecimal(ItemCode.Substring(7, 2));

                        var pointQty = Convert.ToDecimal(ItemCode.Substring(9, 3)) / 1000;
                        pointQty = Math.Round(pointQty, 3);

                        Quantity += pointQty;
                        ItemCode = barcode;
                        barCodeQuantity = Quantity;
                    }
                }
                else
                {
                    itemBarCode.Text = "";
                    return;
                }
                //item bar code eikhane check hobe........

                //var ItemInformation = await ViewModel.GetItemByBarCode(ItemCode);
                var ItemInformation = new Item();
                //var ItemsInformation = await ViewModel.GetItemListByBarCode(ItemCode);
                var ItemsInformation = await ViewModel.GetMultipleSalesPrizeItemListByBarCode(ItemCode);

                if (ItemsInformation.Count == 0 || ItemsInformation == null)
                {

                    App.GetService<IAppNotificationService>().OnNotificationInvoked("BarCode [" + ItemCode + "] Not Found", "POS");
                    itemBarCode.Text = "";
                    return;
                }
                if (ItemsInformation.Count == 1)
                {
                    ItemInformation = ItemsInformation.FirstOrDefault();
                    if (ItemInformation.UomId == 5 || ItemInformation.UomId == 6)
                    {
                        if (ItemCodeforPcs.Length >= 2)
                        {
                            if (ItemCodeforPcs[0] == '9' && ItemCodeforPcs[1] == '9')
                            {
                                var pointQty = Convert.ToDecimal(ItemCodeforPcs.Substring(9, 3));
                                pointQty = Math.Round(pointQty, 3);
                                Quantity = pointQty;
                            }
                        }
                    }
                    itemStock.Text = "Stock: " + ItemInformation.TotalQuantity.ToString() + " " + ItemInformation.UomName.ToString();
                    //itemQty.Text = ItemInformation.TotalQuantity.ToString();
                    //itemSellingRate.Text = ItemInformation.CurrentSellingPrice.ToString();
                    if (ItemInformation.TotalQuantity <= 0 && ItemInformation.IsNegativeSales == false)
                    {
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Out Of Stock !", ItemInformation.ItemName + " [" + ItemCode + "]");
                        itemBarCode.Text = "";
                        return;
                    }
                    //check if current item already added to the list.......
                    var addedCheck = ViewModel.homePageItemList.Where(n => n.ItemId == ItemInformation.ItemId && Convert.ToDecimal(n.SalesRate) == Convert.ToDecimal(ItemInformation.CurrentSellingPrice)).FirstOrDefault();


                    if (addedCheck != null)
                    {
                        if (Convert.ToDecimal(addedCheck?.Quantity) + Quantity > ItemInformation.TotalQuantity && ItemInformation.IsNegativeSales == false)
                        {
                            App.GetService<IAppNotificationService>().OnNotificationInvoked("Out Of Stock !", ItemInformation.ItemName + " [" + ItemCode + "]");
                            itemBarCode.Text = "";
                            return;
                        }

                    }


                    if (addedCheck == null)
                    {
                        var itemDiscount = ItemInformation.MaximumDiscountPercent > 0 ? (Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2)) : (ItemInformation.MaximumDiscountAmount > 0 ? (Math.Round((Quantity * ItemInformation.MaximumDiscountAmount ?? 0), 2)) : 0.0M);

                        var Items = new MainViewModelItemDTO()
                        {
                            SL = ViewModel.homePageItemList.Count + 1,
                            ItemId = ItemInformation.ItemId,
                            ItemName = ItemInformation.ItemName,

                            Quantity = Quantity.ToString(),
                            SalesRate = ItemInformation.CurrentSellingPrice > 0 ? ItemInformation.CurrentSellingPrice.ToString() : "0",

                            SD = Math.Round((ItemInformation.SD * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            Discount = itemDiscount,
                            Vat = Math.Round((ItemInformation.Vat * ((Quantity * ItemInformation.CurrentSellingPrice) - itemDiscount)) / 100, 2),
                            Amount = (Math.Round((Quantity * ItemInformation.CurrentSellingPrice), 2)).ToString(),

                            DiscountPercentage = ItemInformation.MaximumDiscountPercent > 0 ? ItemInformation.MaximumDiscountPercent : 0.0M,
                            SingleDiscountAmount = ItemInformation.MaximumDiscountPercent > 0 ? 0 : ItemInformation.MaximumDiscountAmount ?? 0.0M,
                            SDPercentage = ItemInformation.SD,
                            VATPercentage = ItemInformation.Vat,

                            UMOid = ItemInformation.UomId ?? 0,
                            UMOName = ItemInformation.UomName,

                            BarCode = ItemInformation.Barcode,
                        };
                        ViewModel.homePageItemList.Add(Items);
                    }
                    else
                    {
                        var indexRemove = ViewModel.homePageItemList.IndexOf(addedCheck);
                        var Maxindex = ViewModel.homePageItemList.MaxBy(x => x.SL).SL;
                        var NewQty = Convert.ToDecimal(addedCheck.Quantity) + Quantity;
                        var Items = new MainViewModelItemDTO()
                        {
                            //SL = addedCheck.SL,
                            SL = Maxindex,
                            ItemId = ItemInformation.ItemId,
                            ItemName = ItemInformation.ItemName,

                            Quantity = NewQty.ToString(),
                            SalesRate = ItemInformation.CurrentSellingPrice > 0 ? ItemInformation.CurrentSellingPrice.ToString() : "0",
                            Vat = Math.Round((ItemInformation.Vat * (NewQty * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            SD = Math.Round((ItemInformation.SD * (NewQty * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            //Discount = Math.Round((ItemInformation.MaximumDiscountPercent * (NewQty * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            Discount = ItemInformation.MaximumDiscountPercent > 0 ? (Math.Round((ItemInformation.MaximumDiscountPercent * (NewQty * ItemInformation.CurrentSellingPrice)) / 100, 2)) : (ItemInformation.MaximumDiscountAmount > 0 ? (Math.Round((NewQty * ItemInformation.MaximumDiscountAmount ?? 0), 2)) : 0.0M),
                            Amount = (Math.Round((NewQty * ItemInformation.CurrentSellingPrice), 2)).ToString(),

                            //DiscountPercentage = ItemInformation.MaximumDiscountPercent,
                            DiscountPercentage = ItemInformation.MaximumDiscountPercent > 0 ? ItemInformation.MaximumDiscountPercent : 0.0M,
                            SingleDiscountAmount = ItemInformation.MaximumDiscountPercent > 0 ? 0 : ItemInformation.MaximumDiscountAmount ?? 0.0M,

                            SDPercentage = ItemInformation.SD,
                            VATPercentage = ItemInformation.Vat,

                            UMOid = ItemInformation.UomId ?? 0,
                            UMOName = ItemInformation.UomName,
                            BarCode = ItemInformation.Barcode,
                        };
                        ViewModel.homePageItemList.Remove(addedCheck);
                        ViewModel.homePageItemList.Insert(0, Items);
                        var index = Maxindex;
                        foreach (var item in ViewModel.homePageItemList)
                        {
                            item.SL = index;
                            index--;
                        }
                        //ViewModel.homePageItemList.Add( Items);


                    }
                    //check if current item already added to the list.......


                    ViewModel.homePageItemList = new ObservableCollection<MainViewModelItemDTO>(ViewModel.homePageItemList.OrderByDescending(p => p.SL));
                    ItemGrid.ItemsSource = ViewModel.homePageItemList;



                    SetTextFieldValue();
                }
                else
                {
                    if (ViewModel.MultipleBarCodeItemList.Count > 0)
                    {
                        ViewModel.MultipleBarCodeItemList.Clear();
                    }
                    foreach (var singleItem in ItemsInformation)
                    {
                        ItemInformation = singleItem;
                        Quantity = barCodeQuantity;
                        //newly added section for unit of measure selection (pices or fraction)
                        if (ItemInformation.UomId == 5 || ItemInformation.UomId == 6)
                        {
                            if (ItemCodeforPcs.Length >= 2)
                            {
                                if (ItemCodeforPcs[0] == '9' && ItemCodeforPcs[1] == '9')
                                {
                                    var pointQty = Convert.ToDecimal(ItemCodeforPcs.Substring(9, 3));
                                    pointQty = Math.Round(pointQty, 3);
                                    Quantity = pointQty;
                                }
                            }
                        }
                        //newly added section for unit of measure selection (pices or fraction)





                        var Items = new MainViewModelItemDTO()
                        {
                            SL = ViewModel.MultipleBarCodeItemList.Count + 1,
                            ItemId = ItemInformation.ItemId,
                            ItemName = ItemInformation.ItemName,

                            Quantity = Quantity.ToString(),
                            DatabaseStock = ItemInformation.TotalQuantity.ToString(),
                            SalesRate = ItemInformation.CurrentSellingPrice > 0 ? ItemInformation.CurrentSellingPrice.ToString() : "0",
                            Vat = Math.Round((ItemInformation.Vat * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            SD = Math.Round((ItemInformation.SD * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            //Discount = Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            Discount = ItemInformation.MaximumDiscountPercent > 0 ? (Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2)) : (ItemInformation.MaximumDiscountAmount > 0 ? (Math.Round((Quantity * ItemInformation.MaximumDiscountAmount ?? 0), 2)) : 0.0M),
                            Amount = (Math.Round((Quantity * ItemInformation.CurrentSellingPrice), 2)).ToString(),

                            //DiscountPercentage = ItemInformation.MaximumDiscountPercent,
                            DiscountPercentage = ItemInformation.MaximumDiscountPercent > 0 ? ItemInformation.MaximumDiscountPercent : 0.0M,
                            SingleDiscountAmount = ItemInformation.MaximumDiscountPercent > 0 ? 0 : ItemInformation.MaximumDiscountAmount ?? 0.0M,
                            SDPercentage = ItemInformation.SD,
                            VATPercentage = ItemInformation.Vat,

                            UMOid = ItemInformation.UomId ?? 0,
                            UMOName = ItemInformation.UomName,
                            BarCode = ItemInformation.Barcode,

                            IsNegativeSales = ItemInformation.IsNegativeSales,
                        };

                        ViewModel.MultipleBarCodeItemList.Add(Items);
                    }
                    itemBarCode.Text = "";
                    ItemMultipleBarCode.ItemsSource = ViewModel.MultipleBarCodeItemList;
                    conMultipleBarCode.Title = "Item List";
                    //conMultipleBarCode.Focus(FocusState.Pointer);
                    //ItemMultipleBarCode.Focus(FocusState.Pointer);
                    ItemMultipleBarCode.Focus(FocusState.Programmatic);
                    await conMultipleBarCode.ShowAsync();


                }

                if (ViewModel.PaymentWalletDTOList.Count <= 0)
                {
                    var response = await ViewModel.GetPaymentWalletList();
                    ViewModel.PaymentWalletDTOList.AddRange(response);
                }
                itemBarCode.Text = "";
                ItemGrid.SelectedIndex = 0;


                //newly added code............
                if(ViewModel.CollectionObj.NumotherDiscount > 0)
                {
                    VATRecalculateFunction(ViewModel.CollectionObj.NumotherDiscount);
                    List<MainViewModelItemDTO> sortedList = new List<MainViewModelItemDTO>();
                    sortedList = ViewModel.homePageItemList.Select(n => n).ToList();

                    ItemGrid.ItemsSource = null;
                    ItemGrid.ItemsSource = new ObservableCollection<MainViewModelItemDTO>();
                    foreach (var singleList in sortedList)
                    {
                        ViewModel.homePageItemList.Add(singleList);
                    }

                    ViewModel.homePageItemList = new ObservableCollection<MainViewModelItemDTO>(ViewModel.homePageItemList.OrderByDescending(p => p.SL));
                    ItemGrid.ItemsSource = ViewModel.homePageItemList;
                }
                //newly added code.............
                if(ItemsInformation.Count == 1)
                    itemBarCode.Focus(FocusState.Pointer);
                else
                    ItemMultipleBarCode.Focus(FocusState.Programmatic);
            }


        }
        catch (Exception ex)
        {
            conMultipleBarCode.Hide();
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }


    private async void btnDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {

            var user = await ViewModel.GetUserInformation(AppSettings.UserId);
            if (user.IsItemDelete == true || user.bolIsPOSAdmin == true)
            {
                var button = sender as Button;
                var item = button.DataContext;
                var removeItem = item as MainViewModelItemDTO;
                ViewModel.homePageItemList.Remove(removeItem);
                var json = JsonConvert.SerializeObject(removeItem);

                await ViewModel.DataLog("DeleteItem", json, "DeleteItem");
                SetTextFieldValue();
                //ViewModel.DeleteItemList = new MainViewModelItemDTO();
            }
            else
            {
                AppSettings.PermisionValue = 1;
                var button = sender as Button;
                var item = button.DataContext;
                var removeItem = item as MainViewModelItemDTO;
                ViewModel.DeleteItemList = removeItem;

                await AuthorizeAdmin.ShowAsync();
            }

            //ViewModel.homePageItemList.Remove(removeItem);

            //SetTextFieldValue();

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }

    //print button click
    private async void btnPrint_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            btnPrint.IsEnabled = false;
            Button clickedButton = sender as Button;
            var buttonName = clickedButton.Name;



            var allItemIds = ViewModel.homePageItemList.Select(n => n.ItemId).ToList();
            var allItemInfo = await ViewModel.GetItemByItemIDs(allItemIds);

            var pointsOffer = await ViewModel.GetPointsOfferRowsByItemIds(allItemIds);
            var pointsOfferItemList = pointsOffer.Select(n => n.intItemId).Distinct().ToList();
            var pointSellingPriceAmount = ViewModel.homePageItemList.Where(n => !pointsOfferItemList.Contains(n.ItemId)).Select(n => Convert.ToDecimal(n.Amount)).Sum();

            var customerId = txtCustomer.Text.Trim().ToString();
            var CustomerInfo = await ViewModel.GetPartnerById(customerId);

            var settingInfor = await ViewModel.GetSettings();

            var Paidamount = ViewModel.CollectionObj.CashPayment + ViewModel.itemPaymentModeList.Select(n => n.numberAmount).Sum();
            var GrandTotal = Convert.ToDecimal(ViewModel.CollectionObj.NumGrandTotal);

            var exchangeInvoiceFlag = ViewModel.homePageItemList.Select(n => n.ExchangeReferenceNo).Sum() > 0 ? true : false;

            //change Amount........
            var strChangeAmount = textBlockChangeAmount.Text.ToString();
            var numChangeAmount = -5.0M;
            try
            {
                numChangeAmount = Convert.ToDecimal(strChangeAmount);
            }
            catch
            {
                numChangeAmount = -5.0M;
            }
            //change Amount.......


            if (((Paidamount <= 0 && exchangeInvoiceFlag == false) || (exchangeInvoiceFlag == true && GrandTotal < 0 && numChangeAmount < 0)) && buttonName != "btnHoldInVoice")
            {
                btnPrint.IsEnabled = true;
                textBoxCashAmount.Focus(FocusState.Pointer);
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Receive amount is less than Sale Amount", "POS");
                return;
            }


            if (AppSettings.IsReturn == false && decimal.Parse(textBlockPrintTotalAmount.Text) < 0 && buttonName != "btnHoldInVoice")
            {
                btnPrint.IsEnabled = true;
                itemBarCode.Focus(FocusState.Pointer);
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Negative Sales Not Allowed", "POS");
                return;
            }


            if ((allItemInfo.Count <= 0 || String.IsNullOrEmpty(customerId) || CustomerInfo == null || settingInfor == null || Paidamount < GrandTotal) && buttonName != "btnHoldInVoice")
            {
                if (allItemInfo.Count <= 0)
                {
                    btnPrint.IsEnabled = true;
                    itemBarCode.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Item Information Not Found", "POS");
                }
                else if (CustomerInfo == null || String.IsNullOrEmpty(customerId))
                {
                    btnPrint.IsEnabled = true;
                    txtCustomer.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Customer Information Not Found", "POS");
                }
                else if (settingInfor == null)
                {
                    btnPrint.IsEnabled = true;
                    itemBarCode.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("User Information Not Found", "POS");
                }
                else if (Paidamount < GrandTotal)
                {
                    btnPrint.IsEnabled = true;
                    textBoxCashAmount.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Receive amount is less than Sale Amount", "POS");

                }
                else
                {
                    btnPrint.IsEnabled = true;
                    itemBarCode.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Something Went Wrong", "POS");
                }
                btnPrint.IsEnabled = true;
                return;
            }
            if ((allItemInfo.Count <= 0 || String.IsNullOrEmpty(customerId) || CustomerInfo == null || settingInfor == null) && buttonName == "btnHoldInVoice")
            {
                if (allItemInfo.Count <= 0)
                {
                    btnPrint.IsEnabled = true;
                    itemBarCode.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Item Information Not Found", "POS");
                }
                else if (CustomerInfo == null || String.IsNullOrEmpty(customerId))
                {
                    txtCustomer.Focus(FocusState.Pointer);
                    btnPrint.IsEnabled = true;
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Customer Information Not Found", "POS");
                }
                else if (settingInfor == null)
                {
                    btnPrint.IsEnabled = true;
                    itemBarCode.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("User Information Not Found", "POS");
                }
                else
                {
                    btnPrint.IsEnabled = true;
                    itemBarCode.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Something Went Wrong", "POS");
                }
                btnPrint.IsEnabled = true;
                return;
            }



            var salesorderCode = await ViewModel.InvoiceCodeGenerate(0, "0");

            CreateSalesDeliveryDTO createSalesDeliveryDTO = new CreateSalesDeliveryDTO();
            var SalesDeliveryHeader = new POSSalesDeliveryHeader()
            {
                SalesOrderId = SalesOrderId != 0 ? SalesOrderId : 0,
                //SalesOrderCode = ViewModel.CollectionObj.InvoiceNumber,
                SalesOrderCode = salesorderCode,
                CustomerOrderId = 1,
                AccountId = settingInfor.intAccountId,
                AccountName = "",
                BranchId = settingInfor.intBranchId,
                BranchName = "",
                CustomerId = CustomerInfo.PartnerId,
                CustomerName = CustomerInfo.PartnerName,
                Phone = CustomerInfo.MobileNo,
                ChallanNo = "",
                OrderDate = DateTime.Now.BD(),
                DeliveryDate = DateTime.Now.BD(),
                Remarks = "",
                PaymentTypeId = CustomerInfo.PartnerTypeId,
                PaymentTypeName = CustomerInfo.PartnerTypeName,
                TotalQuantity = ViewModel.homePageItemList.Select(n => Convert.ToDecimal(n.Quantity)).Sum(),
                ItemTotalAmount = ViewModel.CollectionObj.NumtotalBill,
                NetDiscount = ViewModel.CollectionObj.NumTotalDiscount + ViewModel.CollectionObj.NumotherDiscount,
                OthersCost = 0,
                NetAmount = ViewModel.CollectionObj.NumGrandTotal,
                TotalLineDiscount = 0,

                HeaderDiscount = ViewModel.CollectionObj.NumotherDiscount,
                //HeaderDiscountPercentage = ViewModel.CollectionObj.NumOtherDiscountPercentage,
                HeaderDiscountPercentage = ViewModel.CollectionObj.NumotherDiscount > 0 ? (ViewModel.CollectionObj.OtherDiscountType == 1 ? 0.0M : ViewModel.CollectionObj.NumOtherDiscountPercentage) : 0.0M,  //ViewModel.CollectionObj.OtherDiscountType == 1 means amount ........

                ReceiveAmount = ViewModel.CollectionObj.ReceiveAmount,
                PendingAmount = ViewModel.CollectionObj.NumGrandTotal - ViewModel.CollectionObj.ReceiveAmount,
                ReturnAmount = ViewModel.CollectionObj.ReceiveAmount - ViewModel.CollectionObj.NumGrandTotal,
                InterestRate = 0,
                NetAmountWithInterest = 0,
                TotalNoOfInstallment = 0,
                InstallmentStartDate = DateTime.Now.BD(),
                InstallmentType = "",
                AmountPerInstallment = 0,
                SalesForceId = 1,
                SalesForceName = "",
                ActionById = AppSettings.UserId,
                ActionByName = "",
                ActionTime = DateTime.Now.BD(),
                IsPosSales = true,
                isActive = true,
                SalesOrReturn = "Sales",
                AdvanceBalanceAdjust = 0,
                CustomerNetAmount = 0,
                IsComplete = true,
                SalesTypeId = 1,
                SalesTypeName = "",
                SalesOrderRefId = 1,
                Narration = "",
                SmsTransactionId = 1,
                AnonymousAddress = CustomerInfo.Address,
                TotalSd = ViewModel.CollectionObj.NumtotalSD,
                TotalVat = ViewModel.CollectionObj.NumtotalVAT,
                IsBillCreated = true,
                DiscoundItemTotalPrice = ViewModel.homePageItemList.Select(n => n.Discount).Sum(),
                OfferItemTotal = 0,
                WalletId = 0,
                ComissionPercentage = 0,
                isInclusive = true,
                OfficeId = settingInfor.intOfficeId,
                CustomerPO = "",
                BillNo = "",
                ShippingAddressId = 1,
                ShippingAddressName = "",
                ShippingContactPerson = "",
                IsConfirmed = true,
                IsApprove = false,
                ProjectName = "",
                FreeTypeId = 0,
                FreeTypeName = "",
                JobOrderId = 1,
                IsSync = 0,
                Draft = buttonName == "btnHoldInVoice" ? true : false,
                UserId = AppSettings.UserId,
                CashPayment = ViewModel.CollectionObj.CashPayment,
                //Points = buttonName == "btnHoldInVoice" ? 0.0M : (CustomerInfo.PointsAmount > 0 ? (ViewModel.CollectionObj.NumtotalBill / CustomerInfo.PointsAmount) : 0.0M),
                //Points = buttonName == "btnHoldInVoice" ? 0.0M : (CustomerInfo.PointsAmount > 0 ? ((ViewModel.CollectionObj.NumtotalBill - ViewModel.CollectionObj.NumTotalDiscount - ViewModel.CollectionObj.NumotherDiscount) / CustomerInfo.PointsAmount) : 0.0M),
                Points = buttonName == "btnHoldInVoice" ? 0.0M : (CustomerInfo.PointsAmount > 0 ? ((pointSellingPriceAmount - ViewModel.CollectionObj.NumTotalDiscount - ViewModel.CollectionObj.NumotherDiscount) / (CustomerInfo.PointsAmount == 0 ? 1 : CustomerInfo.PointsAmount)) : 0.0M),

                ISExchange = exchangeInvoiceFlag,
                HeaderDiscountId = ViewModel.CollectionObj.NumotherDiscount > 0 ? ViewModel.CollectionObj.OtherDiscountId : 0,
                CounterId = settingInfor.intCounterId,
                CounterName = settingInfor.StrCounterName,
                ISReturn = AppSettings.IsReturn

            };
            var SalesDeliveryLIne = ViewModel.homePageItemList.Select(n => new POSSalesDeliveryLine
            {
                SalesOrderId = 0,
                ItemId = n.ItemId,
                ItemName = n.ItemName,
                UomId = n.UMOid,
                UomName = n.UMOName,
                Quantity = Convert.ToDecimal(n.Quantity),
                ChangeQuantity = 0,
                Price = Math.Round(Convert.ToDecimal(n.SalesRate), 2),
                TotalAmount = Convert.ToDecimal(n.Amount),
                LineDiscount = n.DiscountPercentage > 0 ? n.DiscountPercentage : n.SingleDiscountAmount,
                NetAmount = Convert.ToDecimal(n.Amount),
                VatPercentage = n.VATPercentage,
                WarrantyExpiredDate = DateTime.Now.BD(),
                WarrantyDescription = "",
                WarrantyInMonth = 1,
                HeaderDiscountProportion = 1,
                HeaderCostProportion = 1,
                CostPrice = Convert.ToDecimal(n.Amount),
                CostTotal = Convert.ToDecimal(n.Amount),
                AnonymousAddress = ViewModel.SelectedCustomerObj.CustomerAddress,
                WarehouseId = settingInfor.intWarehouseId,
                SdPercentage = n.SDPercentage,
                VatAmount = n.Vat,
                SdAmount = n.SD,
                DiscountType = n.DiscountPercentage > 0 ? "Percentage" : "Amount",
                DiscountAmount = n.Discount,
                OfferItemName = "",
                OfferItemQty = 0,
                OfferItemId = 0,
                IsOfferItem = false,
                ItemBasePriceInclusive = 1,
                ItemDescription = "",
                FreeTypeId = 0,
                FreeTypeName = "",
                ItemSerial = "",
                Batch = "",
                IsSync = false,
                ExchangeReferenceId = n.ExchangeReferenceNo,
                OtherDiscount = n.OtherDiscount

            }).ToList();
            var posSalesPayment = ViewModel.itemPaymentModeList.Select(n => new POSSalesPayment()
            {
                SalesDeliveryId = 0,
                AccountId = settingInfor.intAccountId,
                BranchId = settingInfor.intBranchId,
                OfficeId = settingInfor.intOfficeId,
                WalletId = n.intWalletId,
                ReferanceNo = n.ReferanceNo,
                CollectionAmount = n.numberAmount,
                TransactionDate = DateTime.Now.BD(),
                IsActive = true,
                ActionById = AppSettings.UserId,
                LastActionDatetime = DateTime.Now.BD(),
                ServerDatetime = DateTime.Now.BD(),
                IsSync = false,

            }).ToList();

            createSalesDeliveryDTO.pOSSalesDeliveryHeader = SalesDeliveryHeader;
            createSalesDeliveryDTO.pOSSalesDeliveryLine = SalesDeliveryLIne;
            createSalesDeliveryDTO.pOSSalesPayments = posSalesPayment;

            var response = await ViewModel.SaveItemIntoSalesDeliveryLines(createSalesDeliveryDTO);

            if (response != null)
            {
                //var json = JsonConvert.SerializeObject(response);
                var msg = "";
                if (buttonName != "btnHoldInVoice")
                {
                    msg = "Print Invoice , Invoice no: [" + response.pOSSalesDeliveryHeader.SalesOrderCode + "]";
                    try
                    {
                        var responsePrint = await PrintFunction(response);
                    }
                    catch
                    {

                    }

                }
                else
                {
                    msg = "Hold Invoice, Invoice no: [" + response.pOSSalesDeliveryHeader.SalesOrderCode + "]";
                }

                //await ViewModel.DataLog(msg, json, msg);
                SalesOrderId = 0;

                App.GetService<IAppNotificationService>().OnNotificationInvoked(buttonName == "btnHoldInVoice" ? "Hold Successfully" : "Invoice Created Successfully", "POS");

                var blockChangeAmount = textBlockChangeAmount.Text;
                var blockPaidAmount = textBlockPaidAmount.Text;
                ClearFields();
                textBlockChangeAmount.Text = blockChangeAmount;
                textBlockPaidAmount.Text = blockPaidAmount;


                //temporary object empty.......
                AppSettings.HomePageRefreshObject.Items = null;
                AppSettings.HomePageRefreshObject.PaymentModeInformation = null;
                AppSettings.HomePageRefreshObject.collection = null;
                btnPrint.IsEnabled = true;
                //temporaty object empty.......
                itemBarCode.Focus(FocusState.Pointer);
            }
            else
            {
                App.GetService<IAppNotificationService>().OnNotificationInvoked("The server is offline. Please contact to the administrator.", "POS");
            }

            btnPrint.IsEnabled = true;
        }
        catch (Exception ex)
        {
            ClearFields();
            btnPrint.IsEnabled = true;
            itemBarCode.Focus(FocusState.Pointer);
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }

    private async void ItemGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        try
        {
            if (e.Column.Header.ToString() == "Qty")
            {
                if (e.EditAction == DataGridEditAction.Commit)
                {
                    var textBox = e.EditingElement as TextBox;
                    var newValue = textBox.Text.ToString();
                    try
                    {
                        var decimalQty = Convert.ToDecimal(newValue);
                        if (decimalQty < 0)
                            newValue = "1";
                    }
                    catch
                    {
                        newValue = "1";
                    }

                    var item = e.Row.DataContext as MainViewModelItemDTO;
                    if (item != null)
                    {
                        if (item.isExchangeEnable == true)
                        {
                            newValue = item.Quantity;
                        }
                    }
                    var Srate = Convert.ToDecimal(item.SalesRate);
                    //stock check.......
                    item.Quantity = newValue;
                    var NewQnty = Convert.ToDecimal(newValue);
                    var itemstockCheck = await ViewModel.GetStockQtyCheckItemByItemID(item.ItemId, Srate);
                    if (NewQnty == 0)
                    {
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Quantity Cann't be Zero", "POS");
                    }
                    if (NewQnty > itemstockCheck.TotalQuantity && itemstockCheck.IsNegativeSales == false)
                    {
                        NewQnty = 0.0M;
                        newValue = "1";
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Out of stock", "POS");
                    }
                    //stock check.......

                    //item.Quantity = newValue;
                    //var NewQnty = Convert.ToDecimal(newValue);

                    var change = ViewModel.homePageItemList.Where(n => n.SL == item.SL).FirstOrDefault();

                    var SalesRate = Convert.ToDecimal(change.SalesRate);
                    var Discount = Convert.ToDecimal(change.Discount);

                    var indexRemove = ViewModel.homePageItemList.IndexOf(change);
                    var itemDiscount = change.DiscountPercentage > 0 ? (Math.Round(((NewQnty * SalesRate) * change.DiscountPercentage) / 100, 2)) : (change.SingleDiscountAmount > 0 ? Math.Round((change.SingleDiscountAmount * NewQnty), 2) : 0.0M);

                    var Items = new MainViewModelItemDTO()
                    {
                        SL = change.SL,
                        ItemId = change.ItemId,
                        ItemName = change.ItemName,
                        Quantity = newValue,
                        SalesRate = change.SalesRate,
                        Vat = Math.Round((change.VATPercentage * ((NewQnty * SalesRate) - itemDiscount)) / 100, 2),
                        SD = Math.Round((change.SDPercentage * (NewQnty * SalesRate)) / 100, 2),
                        Discount = change.DiscountPercentage > 0 ? (Math.Round(((NewQnty * SalesRate) * change.DiscountPercentage) / 100, 2)) : (change.SingleDiscountAmount > 0 ? Math.Round((change.SingleDiscountAmount * NewQnty), 2) : 0.0M),
                        Amount = (Math.Round((NewQnty * SalesRate), 2)).ToString(),

                        VATPercentage = change.VATPercentage,
                        SDPercentage = change.SDPercentage,
                        DiscountPercentage = change.DiscountPercentage,
                        SingleDiscountAmount = change.SingleDiscountAmount,
                        isExchangeEnable = change.isExchangeEnable,
                        UMOid = change.UMOid,
                        UMOName = change.UMOName,
                        BarCode = change.BarCode,
                    };
                    ViewModel.homePageItemList.Remove(change);
                    ViewModel.homePageItemList.Insert(indexRemove, Items);

                    SetTextFieldValue();
                    //}
                }

                //newly added code............
                
                if (ViewModel.CollectionObj.NumotherDiscount > 0)
                {
                    VATRecalculateFunction(ViewModel.CollectionObj.NumotherDiscount);
                    List<MainViewModelItemDTO> sortedList = new List<MainViewModelItemDTO>();
                    sortedList = ViewModel.homePageItemList.Select(n => n).ToList();

                    ItemGrid.ItemsSource = null;
                    ItemGrid.ItemsSource = new ObservableCollection<MainViewModelItemDTO>();
                    foreach (var singleList in sortedList)
                    {
                        ViewModel.homePageItemList.Add(singleList);
                    }

                    ViewModel.homePageItemList = new ObservableCollection<MainViewModelItemDTO>(ViewModel.homePageItemList.OrderByDescending(p => p.SL));
                    ItemGrid.ItemsSource = ViewModel.homePageItemList;
                }
                //newly added code.............
            }

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void btnWalletAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            //var WalleInfo = cmbSpcPromg1.SelectedItem as PaymentWalletDTO;
            //var WalletAmount = Convert.ToDecimal(textBoxWalletAmount.Text);
            //if (WalletAmount <= 0)
            //{
            //    return;
            //}
            //if (WalleInfo != null)
            //{
            //    ViewModel.itemPaymentModeList.Add(new PaymentModeInformation { intWalletId = WalleInfo.intWalletId, strWalletId = WalleInfo.strWalletName, numberAmount = WalletAmount });
            //}

            //var totalAddedAmount = ViewModel.itemPaymentModeList.Select(n => n.numberAmount).Sum();
            //var CashAmount = Convert.ToDecimal(textBoxCashAmount.Text);
            //totalAddedAmount = totalAddedAmount + CashAmount;

            //textBlockPaidAmount.Text = totalAddedAmount.ToString();
            //var grandTotal = Convert.ToDecimal(GrandTotal.Text);

            //textBlockChangeAmount.Text = (totalAddedAmount - grandTotal) > 0 ? (totalAddedAmount - grandTotal).ToString() : "0";
            //textBlockPrintTotalAmount.Text = totalAddedAmount.ToString();
            //textBoxWalletAmount.Text = "";
            //SetTextFieldValue();
            await AddWallateMobileNo.ShowAsync();


        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }


    private async void btnRecallInvoice_Click(object sender, RoutedEventArgs e)
    {
        try
        {

            ViewModel.recallInvoiceHomeObjDTOs.Clear();

            var userId = AppSettings.UserId;

            var information = await ViewModel.InvoiceHomePageLanding(userId);
            if (information.Count == 0)
            {
                conRecallInvoice.Hide();
                App.GetService<IAppNotificationService>().OnNotificationInvoked("No Recall Data Found", "POS");
                return;
            }
            foreach (var singleInfo in information)
            {
                ViewModel.recallInvoiceHomeObjDTOs.Add(singleInfo);
            }

            txtCustomer.Text = "";
            conRecallInvoice.Hide();
            conRecallInvoice.Title = "Invoice";
            conRecallInvoice.Width = 900;
            if (i == 0)
                await conRecallInvoice.ShowAsync();
            ItemGridRecallInvoice.Focus(FocusState.Pointer);
            i = 0;
        }
        catch (Exception ex)
        {

            conRecallInvoice.Hide();
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }
    private async void btnCloseSession(object sender, RoutedEventArgs e)
    {
        await ConfirmDialog.ShowAsync();

        //SessionCloseDialog.Title = "Session Close";
        //await SessionCloseDialog.ShowAsync();



    }
    //hold function call (F9)
    private async void KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            args.Handled = true;
            var buttonName = "btnHoldInVoice";
            var settingInfor = await ViewModel.GetSettings();
            var SalesDeliveryLIne = ViewModel.homePageItemList.Select(n => new POSSalesDeliveryLine
            {
                SalesOrderId = 0,
                ItemId = n.ItemId,
                ItemName = n.ItemName,
                UomId = n.UMOid,
                UomName = n.UMOName,
                Quantity = Convert.ToDecimal(n.Quantity),
                ChangeQuantity = 0,
                Price = Convert.ToDecimal(n.SalesRate),
                TotalAmount = Convert.ToDecimal(n.Amount),
                LineDiscount = n.DiscountPercentage > 0 ? n.DiscountPercentage : n.SingleDiscountAmount,
                NetAmount = Convert.ToDecimal(n.Amount),
                VatPercentage = n.VATPercentage,
                WarrantyExpiredDate = DateTime.Now.BD(),
                WarrantyDescription = "",
                WarrantyInMonth = 1,
                HeaderDiscountProportion = 1,
                HeaderCostProportion = 1,
                CostPrice = Convert.ToDecimal(n.Amount),
                CostTotal = Convert.ToDecimal(n.Amount),
                AnonymousAddress = ViewModel.SelectedCustomerObj.CustomerAddress,
                WarehouseId = settingInfor.intWarehouseId,
                SdPercentage = n.SDPercentage,
                VatAmount = n.Vat,
                SdAmount = n.SD,
                DiscountType = n.DiscountPercentage > 0 ? "Percentage" : "Amount",
                DiscountAmount = n.Discount,
                OfferItemName = "",
                OfferItemQty = 0,
                OfferItemId = 0,
                IsOfferItem = false,
                ItemBasePriceInclusive = 1,
                ItemDescription = "",
                FreeTypeId = 0,
                FreeTypeName = "",
                ItemSerial = "",
                Batch = "",
                IsSync = false,
                ExchangeReferenceId = n.ExchangeReferenceNo,
                OtherDiscount = n.OtherDiscount
            }).ToList();
            var allItemIds = ViewModel.homePageItemList.Select(n => n.ItemId).ToList();

            if (SalesDeliveryLIne.Count > 0)
            {
                var allItemInfo = await ViewModel.GetItemByItemIDs(allItemIds);
                var customerId = txtCustomer.Text.Trim().ToString();

                var CustomerInfo = await ViewModel.GetPartnerById(customerId);

                if (allItemInfo.Count <= 0 || String.IsNullOrEmpty(customerId) || CustomerInfo == null || settingInfor == null)
                {
                    if (allItemInfo.Count <= 0)
                    {
                        itemBarCode.Focus(FocusState.Pointer);
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Item Information Not Found", "POS");
                    }
                    else if (CustomerInfo == null || String.IsNullOrEmpty(customerId))
                    {
                        txtCustomer.Focus(FocusState.Pointer);
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Customer Information Not Found", "POS");
                    }
                    else
                    {
                        itemBarCode.Focus(FocusState.Pointer);
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("User Information Not Found", "POS");
                    }
                    return;
                }
                if (allItemInfo.Count > 0)
                {


                    var exchangeInvoiceFlag = ViewModel.homePageItemList.Select(n => n.ExchangeReferenceNo).Sum() > 0 ? true : false;



                    CreateSalesDeliveryDTO createSalesDeliveryDTO = new CreateSalesDeliveryDTO();
                    var SalesDeliveryHeader = new POSSalesDeliveryHeader()
                    {
                        SalesOrderId = SalesOrderId != 0 ? SalesOrderId : 0,
                        SalesOrderCode = ViewModel.CollectionObj.InvoiceNumber,
                        //SalesOrderCode = salesorderCode,
                        CustomerOrderId = 1,
                        AccountId = settingInfor.intAccountId,
                        AccountName = "",
                        BranchId = settingInfor.intBranchId,
                        BranchName = "",
                        CustomerId = CustomerInfo.PartnerId,
                        CustomerName = CustomerInfo.PartnerName,
                        Phone = CustomerInfo.MobileNo,
                        ChallanNo = "",
                        OrderDate = DateTime.Now.BD(),
                        DeliveryDate = DateTime.Now.BD(),
                        Remarks = "",
                        PaymentTypeId = CustomerInfo.PartnerTypeId,
                        PaymentTypeName = CustomerInfo.PartnerTypeName,
                        TotalQuantity = ViewModel.homePageItemList.Select(n => Convert.ToDecimal(n.Quantity)).Sum(),
                        ItemTotalAmount = ViewModel.CollectionObj.NumtotalBill,
                        NetDiscount = ViewModel.CollectionObj.NumTotalDiscount + ViewModel.CollectionObj.NumotherDiscount,
                        OthersCost = 0,
                        NetAmount = ViewModel.CollectionObj.NumGrandTotal,
                        TotalLineDiscount = 0,


                        HeaderDiscount = ViewModel.CollectionObj.NumotherDiscount,
                        //HeaderDiscountPercentage = ViewModel.CollectionObj.OtherDiscountType == 1 ? 0.0M : ViewModel.CollectionObj.NumOtherDiscountPercentage,  //ViewModel.CollectionObj.OtherDiscountType == 1 means amount........
                        HeaderDiscountPercentage = ViewModel.CollectionObj.NumotherDiscount > 0 ? (ViewModel.CollectionObj.OtherDiscountType == 1 ? 0.0M : ViewModel.CollectionObj.NumOtherDiscountPercentage) : 0.0M,  //ViewModel.CollectionObj.OtherDiscountType == 1 means amount ........

                        ReceiveAmount = ViewModel.CollectionObj.ReceiveAmount,
                        PendingAmount = ViewModel.CollectionObj.NumGrandTotal - ViewModel.CollectionObj.ReceiveAmount,
                        ReturnAmount = ViewModel.CollectionObj.ReceiveAmount - ViewModel.CollectionObj.NumGrandTotal,
                        InterestRate = 0,
                        NetAmountWithInterest = 0,
                        TotalNoOfInstallment = 0,
                        InstallmentStartDate = DateTime.Now.BD(),
                        InstallmentType = "",
                        AmountPerInstallment = 0,
                        SalesForceId = 1,
                        SalesForceName = "",
                        ActionById = AppSettings.UserId,
                        ActionByName = "",
                        ActionTime = DateTime.Now.BD(),
                        IsPosSales = true,
                        isActive = true,
                        SalesOrReturn = "Sales",
                        AdvanceBalanceAdjust = 0,
                        CustomerNetAmount = 0,
                        IsComplete = true,
                        SalesTypeId = 1,
                        SalesTypeName = "",
                        SalesOrderRefId = 1,
                        Narration = "",
                        SmsTransactionId = 1,
                        AnonymousAddress = CustomerInfo.Address,
                        TotalSd = ViewModel.CollectionObj.NumtotalSD,
                        TotalVat = ViewModel.CollectionObj.NumtotalVAT,
                        IsBillCreated = true,
                        DiscoundItemTotalPrice = ViewModel.homePageItemList.Select(n => n.Discount).Sum(),
                        OfferItemTotal = 0,
                        WalletId = 0,
                        ComissionPercentage = 0,
                        isInclusive = true,
                        OfficeId = settingInfor.intOfficeId,
                        CustomerPO = "",
                        BillNo = "",
                        ShippingAddressId = 1,
                        ShippingAddressName = "",
                        ShippingContactPerson = "",
                        IsConfirmed = true,
                        IsApprove = false,
                        ProjectName = "",
                        FreeTypeId = 0,
                        FreeTypeName = "",
                        JobOrderId = 1,
                        IsSync = 0,
                        Draft = buttonName == "btnHoldInVoice" ? true : false,
                        UserId = AppSettings.UserId,
                        CashPayment = ViewModel.CollectionObj.CashPayment,
                        Points = 0.0M,

                        ISExchange = exchangeInvoiceFlag,
                        //HeaderDiscountId = ViewModel.CollectionObj.OtherDiscountId,
                        HeaderDiscountId = ViewModel.CollectionObj.NumotherDiscount > 0 ? ViewModel.CollectionObj.OtherDiscountId : 0,
                        CounterId = settingInfor.intCounterId,
                        CounterName = settingInfor.StrCounterName,
                        ISReturn = AppSettings.IsReturn
                    };

                    var posSalesPayment = ViewModel.itemPaymentModeList.Select(n => new POSSalesPayment()
                    {
                        SalesDeliveryId = 0,
                        AccountId = settingInfor.intAccountId,
                        BranchId = settingInfor.intBranchId,
                        OfficeId = settingInfor.intOfficeId,
                        WalletId = n.intWalletId,
                        CollectionAmount = n.numberAmount,
                        TransactionDate = DateTime.Now.BD(),
                        IsActive = true,
                        ReferanceNo = n.ReferanceNo,
                        ActionById = AppSettings.UserId,
                        LastActionDatetime = DateTime.Now.BD(),
                        ServerDatetime = DateTime.Now.BD(),
                        IsSync = false,

                    }).ToList();

                    createSalesDeliveryDTO.pOSSalesDeliveryHeader = SalesDeliveryHeader;
                    createSalesDeliveryDTO.pOSSalesDeliveryLine = SalesDeliveryLIne;
                    createSalesDeliveryDTO.pOSSalesPayments = posSalesPayment;

                    var salesorderCode = await ViewModel.InvoiceCodeGenerate(0, "0");
                    createSalesDeliveryDTO.pOSSalesDeliveryHeader.SalesOrderCode = salesorderCode;
                    var response = await ViewModel.SaveItemIntoSalesDeliveryLines(createSalesDeliveryDTO);

                    if (response != null)
                    {
                        //var json = JsonConvert.SerializeObject(createSalesDeliveryDTO);
                        //await ViewModel.DataLog("Print Invoice using (F9) Invoice no:" + "[" + createSalesDeliveryDTO.pOSSalesDeliveryHeader.SalesOrderCode + "]", json, "Print Invoice");
                        SalesOrderId = 0;
                        App.GetService<IAppNotificationService>().OnNotificationInvoked(buttonName == "btnHoldInVoice" ? "Hold Successfully" : "Invoice Created Successfully", "POS");

                        ClearFields();
                        itemBarCode.Focus(FocusState.Pointer);
                    }




                    //temporary object empty.......
                    AppSettings.HomePageRefreshObject.Items = null;
                    AppSettings.HomePageRefreshObject.PaymentModeInformation = null;
                    AppSettings.HomePageRefreshObject.collection = null;
                    //temporaty object empty.......
                }

            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void textBoxCashAmount_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (textBoxCashAmount.Text.Length >= 6)
                {
                    ContentDialog con = new ContentDialog()
                    {
                        Title = "POS Amount",
                        Content = " Are You want to paid [" + textBoxCashAmount.Text + "]",
                        PrimaryButtonText = "Ok",
                    };
                    con.XamlRoot = this.Content.XamlRoot;
                    await con.ShowAsync();
                    //App.GetService<IAppNotificationService>().OnNotificationInvoked("", "POS");
                }
                if (textBoxCashAmount.Text.Length >= 7)
                {
                    textBoxCashAmount.Text = "0";
                    textBoxCashAmount.Focus(FocusState.Pointer);
                    var caretIndex = textBoxCashAmount.Text.Length;
                    if (caretIndex >= 0)
                    {
                        textBoxCashAmount.SelectionStart = caretIndex;
                    }
                    textBoxCashAmount.SelectAll();
                    SetTextFieldValue();
                    return;
                }
                var CashAmount = Convert.ToDecimal(textBoxCashAmount.Text);
                ViewModel.CollectionObj.CashPayment = CashAmount;

                SetTextFieldValue();
                btnPrint.Focus(FocusState.Pointer);

            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void KeyboardAccelerator_Invoked_RecallInvoice(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            conRecallInvoice.Hide();
            ViewModel.recallInvoiceHomeObjDTOs.Clear();
            conRecallInvoice.Title = "Invoice";
            var userId = AppSettings.UserId;

            var information = await ViewModel.InvoiceHomePageLanding(userId);
            if (information.Count == 0)
            {
                App.GetService<IAppNotificationService>().OnNotificationInvoked("No Recall Data Found", "POS");
                return;
            }
            foreach (var singleInfo in information)
            {
                ViewModel.recallInvoiceHomeObjDTOs.Add(singleInfo);
            }
            conRecallInvoice.Hide();
            await conRecallInvoice.ShowAsync();

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void btnRecallInvoiceView_Click(object sender, RoutedEventArgs e)
    {
        Button clickedButton = sender as Button;
        var item = clickedButton.DataContext;
        var viewRow = item as RecallInvoiceHomeObjDTO;
        if (SalesOrderId != viewRow.SalesOrderId)
        {
            var dt = await ViewModel.DeleteRecallInvoice(viewRow.SalesOrderId, viewRow.SalesInvoice);

            if (dt.StatusCode == 200)
            {
                ViewModel.recallInvoiceHomeObjDTOs.Remove(viewRow);
                ItemGridRecallInvoice.ItemsSource = ViewModel.recallInvoiceHomeObjDTOs;
            }

            ViewModel.isRecollRecallInvoice = await ViewModel.InvoiceCheck(AppSettings.UserId);
            if (ViewModel.isRecollRecallInvoice == true)
            {
                grdRecallInvoice.Visibility = Visibility.Collapsed;
                grdRecallInvoiceSecond.Visibility = Visibility.Visible;

            }
            else
            {
                grdRecallInvoice.Visibility = Visibility.Visible;
                grdRecallInvoiceSecond.Visibility = Visibility.Collapsed;
            }
        }
    }








    private async void btnExchangeView_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var button = sender as Button;
            var item = button.DataContext;
            txtCustomer.IsEnabled = false;
            if (item == null)
            {
                App.GetService<IAppNotificationService>().OnNotificationInvoked("No Item Found", "POS");
                return;
            }
            if (chkReturn.IsChecked == true)
            {
                AppSettings.IsReturn = true;
                txtCustomer.IsEnabled = false;
            }
            var itemInformation = item as MainViewModelItemDTO;
            var itemList = ViewModel.homePageItemList.ToList();
            //var checkIfAlreadyExists = itemList.Where(n => n.ItemId == itemInformation.ItemId && n.SalesRate == itemInformation.SalesRate).FirstOrDefault();
            //if (checkIfAlreadyExists != null)
            //{
            //    var newQuantity = Convert.ToDecimal(checkIfAlreadyExists.Quantity) - 1;
            //    var salesRate = Convert.ToDecimal(checkIfAlreadyExists.SalesRate);
            //    itemInformation.Id = itemList.Select(n => n.Quantity).Count() + 1;
            //    itemInformation.Quantity = (Math.Round(newQuantity, 2)).ToString();
            //    itemInformation.Vat = Math.Round((itemInformation.VATPercentage * (newQuantity * salesRate)) / 100, 2);
            //    itemInformation.SD = Math.Round((itemInformation.SDPercentage * (newQuantity * salesRate)) / 100, 2);
            //    //itemInformation.Discount = Math.Round((checkIfAlreadyExists.DiscountPercentage * ((-1) * newQuantity * salesRate)) / 100, 2);
            //    itemInformation.Discount = checkIfAlreadyExists.DiscountPercentage > 0 ? (Math.Round(((newQuantity * salesRate) * checkIfAlreadyExists.DiscountPercentage) / 100, 2)) : (checkIfAlreadyExists.SingleDiscountAmount > 0 ? Math.Round((checkIfAlreadyExists.SingleDiscountAmount * newQuantity), 2) : 0.0M);

            //    itemInformation.Amount = (Math.Round((newQuantity * salesRate), 2)).ToString();
            //    itemInformation.BarCode = checkIfAlreadyExists.BarCode;

            //    itemInformation.isExchangeEnable = itemInformation.isExchangeEnable;
            //    itemInformation.ExchangeReferenceNo = itemInformation.ExchangeReferenceNo;

            //    var index = ViewModel.homePageItemList.IndexOf(checkIfAlreadyExists);
            //    ViewModel.homePageItemList.Remove(checkIfAlreadyExists);
            //    ViewModel.homePageItemList.Add(itemInformation);
            //}
            //else
            //{
            var newQuantity = 0M;
            var checkQty = 0M;
            decimal.TryParse(itemInformation.ExcQty, out checkQty);
            if (checkQty <= 0)
            {
                newQuantity = -1 * Convert.ToDecimal(itemInformation.Quantity);
            }
            else
            {
                if (checkQty >= Convert.ToDecimal(itemInformation.Quantity) || itemInformation.ExcQty == null)
                {
                    newQuantity = -1 * Convert.ToDecimal(itemInformation.Quantity);

                }
                else
                {
                    newQuantity = -1 * Convert.ToDecimal(itemInformation.ExcQty);
                }
            }

            itemInformation.Id = itemList.Select(n => n.Quantity).Count() + 1;
            var salesRate = Convert.ToDecimal(itemInformation.SalesRate) ;
            itemInformation.Quantity = newQuantity.ToString();
            itemInformation.SL = ViewModel.homePageItemList.Count + 1;
            //itemInformation.Quantity = itemInformation.Quantity-(Math.Round(newQuantity, 2)).ToString();
           
            itemInformation.Discount = itemInformation.DiscountPercentage > 0 ? (Math.Round(((newQuantity * salesRate) * itemInformation.DiscountPercentage) / 100, 2)) : (itemInformation.SingleDiscountAmount > 0 ? Math.Round((itemInformation.SingleDiscountAmount * newQuantity), 2) : 0.0M);

            var otherDiscount = Convert.ToDecimal(itemInformation.OtherDiscount);
          
            itemInformation.Amount = (Math.Round((newQuantity * salesRate)+ (itemInformation.Discount+ otherDiscount), 2)).ToString();
            itemInformation.Vat = Math.Round((itemInformation.VATPercentage * ((newQuantity * salesRate)+(itemInformation.Discount + otherDiscount))) / 100, 2);
            itemInformation.SD = Math.Round((itemInformation.SDPercentage * ((newQuantity * salesRate)- (itemInformation.Discount + otherDiscount))) / 100, 2);
            itemInformation.BarCode = itemInformation.BarCode;
            itemInformation.isExchangeEnable = itemInformation.isExchangeEnable;
            itemInformation.ExchangeReferenceNo = itemInformation.ExchangeReferenceNo;

            ViewModel.homePageItemList.Add(itemInformation);
            //}

            var sortedList = ViewModel.homePageItemList.OrderByDescending(p => p.SL).ToList();
            ViewModel.homePageItemList.Clear();

            foreach (var singleList in sortedList)
            {
                ViewModel.homePageItemList.Add(singleList);
            }
            ItemGrid.ItemsSource = ViewModel.homePageItemList;

            ViewModel.ExchangeItemInformationList.Remove(itemInformation);

            ItemGridExchange.ItemsSource = ViewModel.ExchangeItemInformationList;

            var json = JsonConvert.SerializeObject(itemInformation);
            await ViewModel.DataLog("ExchangeItem", json, "ExchangeItem");
            SetTextFieldValue();

            ItemGrid.SelectedIndex = 0;



            //
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }

    private async void btnExchange_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var user = await ViewModel.GetUserInformation(AppSettings.UserId);
            if (user.IsExchange == true || user.bolIsPOSAdmin == true)
            {
                AuthorizeAdmin.Hide();
                conExchange.Title = "Exchange";
                await conExchange.ShowAsync();
                ViewModel.ExchangeItemInformationList.Clear();
            }
            else
            {
                AppSettings.PermisionValue = 3;
                AuthorizeAdmin.Hide();
                await AuthorizeAdmin.ShowAsync();
            }


            //done ,,, this section is only for getting new windows to work with..............
            //conExchange.Title = "Exchange";
            //await conExchange.ShowAsync();
            //ViewModel.ExchangeItemInformationList.Clear();

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }

    private async void ExchangeInvoiceItemSearchBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var ItemCode = ExchangeInvoiceItemSearchBox.Text;

                var ItemInformation = await ViewModel.GetItemByBarCode(ItemCode);

                if (ItemInformation == null)
                {
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Item Not Found having barcode [" + ItemCode + "]", "POS");
                    return;
                }
                var addedCheck = ViewModel.ExchangeItemInformationList.Where(n => n.ItemId == ItemInformation.ItemId).FirstOrDefault();
                if (addedCheck == null)
                {
                    var Items = new MainViewModelItemDTO()
                    {
                        SL = ViewModel.ExchangeItemInformationList.Count + 1,
                        ItemId = ItemInformation.ItemId,
                        ItemName = ItemInformation.ItemName,

                        Quantity = "1",
                        SalesRate = ItemInformation.CurrentSellingPrice > 0 ? ItemInformation.CurrentSellingPrice.ToString() : "0",
                        Vat = Math.Round((ItemInformation.Vat * (1 * ItemInformation.CurrentSellingPrice)) / 100, 2),
                        SD = Math.Round((ItemInformation.SD * (1 * ItemInformation.CurrentSellingPrice)) / 100, 2),
                        //Discount = Math.Round((ItemInformation.MaximumDiscountPercent * (1 * ItemInformation.CurrentSellingPrice)) / 100,2),
                        Discount = ItemInformation.MaximumDiscountPercent > 0 ? (Math.Round((ItemInformation.MaximumDiscountPercent * (1 * ItemInformation.CurrentSellingPrice)) / 100, 2)) : (ItemInformation.MaximumDiscountAmount > 0 ? (Math.Round((1 * ItemInformation.MaximumDiscountAmount ?? 0), 2)) : 0.0M),
                        Amount = (Math.Round((1 * ItemInformation.CurrentSellingPrice), 2)).ToString(),
                        BarCode = ItemInformation.Barcode,

                        DiscountPercentage = ItemInformation.MaximumDiscountPercent,
                        SDPercentage = ItemInformation.SD,
                        VATPercentage = ItemInformation.Vat,

                        UMOid = ItemInformation.UomId ?? 0,
                        UMOName = ItemInformation.UomName,
                    };
                    ViewModel.ExchangeItemInformationList.Add(Items);
                }
                else
                {
                    var indexRemove = ViewModel.ExchangeItemInformationList.IndexOf(addedCheck);
                    var NewQty = Convert.ToDecimal(addedCheck.Quantity) + 1;
                    var Items = new MainViewModelItemDTO()
                    {
                        SL = addedCheck.SL,
                        ItemId = ItemInformation.ItemId,
                        ItemName = ItemInformation.ItemName,

                        Quantity = NewQty.ToString(),
                        SalesRate = ItemInformation.CurrentSellingPrice > 0 ? ItemInformation.CurrentSellingPrice.ToString() : "0",
                        Vat = (ItemInformation.Vat * (NewQty * ItemInformation.CurrentSellingPrice)) / 100,
                        SD = (ItemInformation.SD * (NewQty * ItemInformation.CurrentSellingPrice)) / 100,
                        //Discount = (ItemInformation.MaximumDiscountPercent * (NewQty * ItemInformation.CurrentSellingPrice)) / 100,
                        Discount = addedCheck.VATPercentage > 0 ? (Math.Round(((NewQty * ItemInformation.CurrentSellingPrice) * addedCheck.DiscountPercentage) / 100, 2)) : (addedCheck.SingleDiscountAmount > 0 ? Math.Round((addedCheck.SingleDiscountAmount * NewQty), 2) : 0.0M),
                        Amount = ((NewQty * ItemInformation.CurrentSellingPrice)).ToString(),
                        BarCode = ItemInformation.Barcode,

                        DiscountPercentage = ItemInformation.MaximumDiscountPercent,
                        SDPercentage = ItemInformation.SD,
                        VATPercentage = ItemInformation.Vat,

                        UMOid = ItemInformation.UomId ?? 0,
                        UMOName = ItemInformation.UomName,
                    };
                    ViewModel.ExchangeItemInformationList.Remove(addedCheck);
                    ViewModel.ExchangeItemInformationList.Insert(indexRemove, Items);
                }
                //check if current item already added to the list.......
                ItemGridExchange.ItemsSource = ViewModel.ExchangeItemInformationList;
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }

    private async void ExchangeInvoiceSearchBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var UserId = AppSettings.UserId;
                var InvoiceCode = ExchangeInvoiceSearchBox.Text;
                //var headerInfo = await ViewModel.GetPOSSalesDeliveryHeader(InvoiceCode);
                //if (headerInfo == null)
                //{
                //    App.GetService<IAppNotificationService>().OnNotificationInvoked("Invoice Not Found", "POS");
                //    return;
                //}
                //var response = await ViewModel.RecallInvoice(UserId, headerInfo.SalesOrderId);
                var response = await ViewModel.GetSalesDeliveryInformationFromSQLServer(InvoiceCode);
                if (response != null)
                {
                    if (response.Items.Count > 0)
                    {
                        if (ViewModel.ExchangeItemInformationList != null)
                        {
                            ViewModel.ExchangeItemInformationList.Clear();
                        }

                        var itemIds = response.Items.Select(n => n.ItemId).ToList();
                        var itemInformatins = await ViewModel.GetItemByItemIDs(itemIds);
                        var itemData = ViewModel.homePageItemList.Where(w => itemIds.Contains(w.ItemId)).ToList();

                        itemData = itemData.Select(f => { f.Qty = Math.Abs(Convert.ToDecimal(f.Quantity)); return f; }).ToList();
                        foreach (var singleInfo in response.Items.Where(w => w.isExchangeEnable != false))
                        {
                            if (itemData.Where(w => w.ItemId == singleInfo.ItemId
                            && Convert.ToDecimal(w.Qty) == Math.Abs(Convert.ToDecimal(singleInfo.Quantity))).Count() > 0)
                            {

                            }
                            else
                            {
                                singleInfo.BarCode = itemInformatins.Where(n => n.ItemId == singleInfo.ItemId).Select(n => n.Barcode).FirstOrDefault() ?? "";
                                singleInfo.SL = ViewModel.ExchangeItemInformationList.Count + 1;
                                singleInfo.Quantity = singleInfo.Quantity;
                                singleInfo.Vat = singleInfo.Vat;
                                singleInfo.SD = singleInfo.SD;
                                singleInfo.Discount = singleInfo.Discount;
                                singleInfo.Amount = singleInfo.Amount;
                                singleInfo.OtherDiscount = singleInfo.OtherDiscount;
                                ViewModel.ExchangeItemInformationList.Add(singleInfo);
                            }


                        }
                        //ItemGridExchange.ItemsSource = null;
                        ItemGridExchange.ItemsSource = new ObservableCollection<MainViewModelItemDTO>(ViewModel.ExchangeItemInformationList);

                        var customerId = Convert.ToInt64(response.collection.CustomerId);
                        var customerInfo = await ViewModel.GetCustomerbyCustomerId(customerId);

                        txtCustomer.Text = customerInfo.MobileNo;
                        txtCustomerName.Text = customerInfo.PartnerName;
                        textPointVale.Text = Convert.ToString(customerInfo.Points);

                        ViewModel.CollectionObj.CustomerCode = customerInfo.MobileNo;
                        ViewModel.CollectionObj.CustomerName = customerInfo.PartnerName;
                        ViewModel.CollectionObj.CustomerPoints = Convert.ToString(customerInfo.Points);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }

    private void conExchange_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            SetTextFieldValue();
            conExchange.Hide();
            ExchangeInvoiceSearchBox.Text = "";
            ExchangeInvoiceItemSearchBox.Text = "";

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }







    private void KeyboardAccelerator_Invoked_Barcode(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        itemBarCode.Focus(FocusState.Pointer);
        itemBarCode.Text = "";
    }
    private void KeyboardAccelerator_Invoked_Cust(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        txtCustomer.Focus(FocusState.Pointer);
        txtCustomer.Text = "";
    }

    //print button alt + P
    private async void KeyboardAccelerator_Invoked_Print(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            args.Handled = true;
            var buttonName = "Print";



            var allItemIds = ViewModel.homePageItemList.Select(n => n.ItemId).ToList();
            var allItemInfo = await ViewModel.GetItemByItemIDs(allItemIds);

            var pointsOffer = await ViewModel.GetPointsOfferRowsByItemIds(allItemIds);
            var pointsOfferItemList = pointsOffer.Select(n => n.intItemId).Distinct().ToList();
            var pointSellingPriceAmount = ViewModel.homePageItemList.Where(n => !pointsOfferItemList.Contains(n.ItemId)).Select(n => Convert.ToDecimal(n.Amount)).Sum();

            var customerId = txtCustomer.Text.Trim().ToString();
            var CustomerInfo = await ViewModel.GetPartnerById(customerId);

            var settingInfor = await ViewModel.GetSettings();

            var Paidamount = ViewModel.CollectionObj.CashPayment + ViewModel.itemPaymentModeList.Select(n => n.numberAmount).Sum();
            var GrandTotal = Convert.ToDecimal(ViewModel.CollectionObj.NumGrandTotal);

            //if (Paidamount <= 0)
            //{
            //    App.GetService<IAppNotificationService>().OnNotificationInvoked("Receive amount is less than Sale Amount", "POS");
            //    return;
            //}

            var exchangeInvoiceFlag = ViewModel.homePageItemList.Select(n => n.ExchangeReferenceNo).Sum() > 0 ? true : false;

            //change Amount........
            var strChangeAmount = textBlockChangeAmount.Text.ToString();
            var numChangeAmount = -5.0M;
            try
            {
                numChangeAmount = Convert.ToDecimal(strChangeAmount);
            }
            catch
            {
                numChangeAmount = -5.0M;
            }
            //change Amount.......

            if (((Paidamount <= 0 && exchangeInvoiceFlag == false) || (exchangeInvoiceFlag == true && GrandTotal < 0 && numChangeAmount < 0)) && buttonName != "btnHoldInVoice")
            {
                textBoxCashAmount.Focus(FocusState.Pointer);
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Receive amount is less than Sale Amount", "POS");
                return;
            }
            if (AppSettings.IsReturn == false && decimal.Parse(textBlockPrintTotalAmount.Text) < 0 && buttonName != "btnHoldInVoice")
            {
                btnPrint.IsEnabled = true;
                textBoxCashAmount.Focus(FocusState.Pointer);
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Negative Sales Not Allowed", "POS");
                return;
            }

            if (allItemInfo.Count <= 0 || String.IsNullOrEmpty(customerId) || CustomerInfo == null || settingInfor == null || Paidamount < GrandTotal)
            {
                if (allItemInfo.Count <= 0)
                {
                    itemBarCode.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Item Information Not Found", "POS");
                }
                else if (CustomerInfo == null || String.IsNullOrEmpty(customerId))
                {
                    txtCustomer.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Customer Information Not Found", "POS");
                }
                else if (Paidamount < GrandTotal)
                {
                    textBoxCashAmount.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Receive amount is less than Sale Amount", "POS");
                }
                else
                {
                    itemBarCode.Focus(FocusState.Pointer);
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Something Went Wrong", "POS");
                }
                return;
            }

            //var exchangeInvoiceFlag = ViewModel.homePageItemList.Select(n => n.ExchangeReferenceNo).Sum() > 0 ? true : false;

            var salesorderCode = await ViewModel.InvoiceCodeGenerate(0, "0");

            CreateSalesDeliveryDTO createSalesDeliveryDTO = new CreateSalesDeliveryDTO();
            var SalesDeliveryHeader = new POSSalesDeliveryHeader()
            {
                SalesOrderId = SalesOrderId != 0 ? SalesOrderId : 0,
                //SalesOrderCode = ViewModel.CollectionObj.InvoiceNumber,
                SalesOrderCode = salesorderCode,
                CustomerOrderId = 1,
                AccountId = settingInfor.intAccountId,
                AccountName = "",
                BranchId = settingInfor.intBranchId,
                BranchName = "",
                CustomerId = CustomerInfo.PartnerId,
                CustomerName = CustomerInfo.PartnerName,
                Phone = CustomerInfo.MobileNo,
                ChallanNo = "",
                OrderDate = DateTime.Now.BD(),
                DeliveryDate = DateTime.Now.BD(),
                Remarks = "",
                PaymentTypeId = CustomerInfo.PartnerTypeId,
                PaymentTypeName = CustomerInfo.PartnerTypeName,
                TotalQuantity = ViewModel.homePageItemList.Select(n => Convert.ToDecimal(n.Quantity)).Sum(),
                ItemTotalAmount = ViewModel.CollectionObj.NumtotalBill,
                NetDiscount = ViewModel.CollectionObj.NumTotalDiscount + ViewModel.CollectionObj.NumotherDiscount,
                OthersCost = 0,
                NetAmount = ViewModel.CollectionObj.NumGrandTotal,
                TotalLineDiscount = 0,


                HeaderDiscount = ViewModel.CollectionObj.NumotherDiscount,
                //HeaderDiscountPercentage = ViewModel.CollectionObj.NumOtherDiscountPercentage,
                //HeaderDiscountPercentage = ViewModel.CollectionObj.OtherDiscountType == 1 ? 0.0M : ViewModel.CollectionObj.NumOtherDiscountPercentage,  //ViewModel.CollectionObj.OtherDiscountType == 1 means amount........

                HeaderDiscountPercentage = ViewModel.CollectionObj.NumotherDiscount > 0 ? (ViewModel.CollectionObj.OtherDiscountType == 1 ? 0.0M : ViewModel.CollectionObj.NumOtherDiscountPercentage) : 0.0M,  //ViewModel.CollectionObj.OtherDiscountType == 1 means amount ........

                ReceiveAmount = ViewModel.CollectionObj.ReceiveAmount,
                PendingAmount = ViewModel.CollectionObj.NumGrandTotal - ViewModel.CollectionObj.ReceiveAmount,
                ReturnAmount = ViewModel.CollectionObj.ReceiveAmount - ViewModel.CollectionObj.NumGrandTotal,
                InterestRate = 0,
                NetAmountWithInterest = 0,
                TotalNoOfInstallment = 0,
                InstallmentStartDate = DateTime.Now.BD(),
                InstallmentType = "",
                AmountPerInstallment = 0,
                SalesForceId = 1,
                SalesForceName = "",
                ActionById = AppSettings.UserId,
                ActionByName = "",
                ActionTime = DateTime.Now.BD(),
                IsPosSales = true,
                isActive = true,
                SalesOrReturn = "Sales",
                AdvanceBalanceAdjust = 0,
                CustomerNetAmount = 0,
                IsComplete = true,
                SalesTypeId = 1,
                SalesTypeName = "",
                SalesOrderRefId = 1,
                Narration = "",
                SmsTransactionId = 1,
                AnonymousAddress = CustomerInfo.Address,
                TotalSd = ViewModel.CollectionObj.NumtotalSD,
                TotalVat = ViewModel.CollectionObj.NumtotalVAT,
                IsBillCreated = true,
                DiscoundItemTotalPrice = ViewModel.homePageItemList.Select(n => n.Discount).Sum(),
                OfferItemTotal = 0,
                WalletId = 0,
                ComissionPercentage = 0,
                isInclusive = true,
                OfficeId = settingInfor.intOfficeId,
                CustomerPO = "",
                BillNo = "",
                ShippingAddressId = 1,
                ShippingAddressName = "",
                ShippingContactPerson = "",
                IsConfirmed = true,
                IsApprove = false,
                ProjectName = "",
                FreeTypeId = 0,
                FreeTypeName = "",
                JobOrderId = 1,
                IsSync = 0,
                Draft = buttonName == "btnHoldInVoice" ? true : false,
                UserId = AppSettings.UserId,
                CashPayment = ViewModel.CollectionObj.CashPayment,
                //Points = CustomerInfo.PointsAmount > 0 ? (ViewModel.CollectionObj.NumtotalBill / CustomerInfo.PointsAmount) : 0.0M,
                //Points = CustomerInfo.PointsAmount > 0 ? ((ViewModel.CollectionObj.NumtotalBill - ViewModel.CollectionObj.NumTotalDiscount - ViewModel.CollectionObj.NumotherDiscount) / CustomerInfo.PointsAmount) : 0.0M,
                Points = buttonName == "btnHoldInVoice" ? 0.0M : (CustomerInfo.PointsAmount > 0 ? ((pointSellingPriceAmount - ViewModel.CollectionObj.NumTotalDiscount - ViewModel.CollectionObj.NumotherDiscount) / (CustomerInfo.PointsAmount == 0 ? 1 : CustomerInfo.PointsAmount)) : 0.0M),
                ISExchange = exchangeInvoiceFlag,
                //HeaderDiscountId = ViewModel.CollectionObj.OtherDiscountId,
                HeaderDiscountId = ViewModel.CollectionObj.NumotherDiscount > 0 ? ViewModel.CollectionObj.OtherDiscountId : 0,
                CounterId = settingInfor.intCounterId,
                CounterName = settingInfor.StrCounterName,
                ISReturn = AppSettings.IsReturn
            };
            var SalesDeliveryLIne = ViewModel.homePageItemList.Select(n => new POSSalesDeliveryLine
            {
                SalesOrderId = 0,
                ItemId = n.ItemId,
                ItemName = n.ItemName,
                UomId = n.UMOid,
                UomName = n.UMOName,
                Quantity = Convert.ToDecimal(n.Quantity),
                ChangeQuantity = 0,
                Price = Convert.ToDecimal(n.SalesRate),
                TotalAmount = Convert.ToDecimal(n.Amount),
                LineDiscount = n.DiscountPercentage > 0 ? n.DiscountPercentage : n.SingleDiscountAmount,
                NetAmount = Convert.ToDecimal(n.Amount),
                VatPercentage = n.VATPercentage,
                WarrantyExpiredDate = DateTime.Now.BD(),
                WarrantyDescription = "",
                WarrantyInMonth = 1,
                HeaderDiscountProportion = 1,
                HeaderCostProportion = 1,
                CostPrice = Convert.ToDecimal(n.Amount),
                CostTotal = Convert.ToDecimal(n.Amount),
                AnonymousAddress = ViewModel.SelectedCustomerObj.CustomerAddress,
                WarehouseId = settingInfor.intWarehouseId,
                SdPercentage = n.SDPercentage,
                VatAmount = n.Vat,
                SdAmount = n.SD,
                DiscountType = "",
                DiscountAmount = n.Discount,
                OfferItemName = "",
                OfferItemQty = 0,
                OfferItemId = 0,
                IsOfferItem = false,
                ItemBasePriceInclusive = 1,
                ItemDescription = n.DiscountPercentage > 0 ? "Percentage" : "Amount",
                FreeTypeId = 0,
                FreeTypeName = "",
                ItemSerial = "",
                Batch = "",
                IsSync = false,
                ExchangeReferenceId = n.ExchangeReferenceNo,
                OtherDiscount = n.OtherDiscount
            }).ToList();
            var posSalesPayment = ViewModel.itemPaymentModeList.Select(n => new POSSalesPayment()
            {
                SalesDeliveryId = 0,
                AccountId = settingInfor.intAccountId,
                BranchId = settingInfor.intBranchId,
                OfficeId = settingInfor.intOfficeId,
                WalletId = n.intWalletId,
                CollectionAmount = n.numberAmount,
                TransactionDate = DateTime.Now.BD(),
                ReferanceNo = n.ReferanceNo,
                IsActive = true,
                ActionById = AppSettings.UserId,
                LastActionDatetime = DateTime.Now.BD(),
                ServerDatetime = DateTime.Now.BD(),
                IsSync = false,

            }).ToList();

            createSalesDeliveryDTO.pOSSalesDeliveryHeader = SalesDeliveryHeader;
            createSalesDeliveryDTO.pOSSalesDeliveryLine = SalesDeliveryLIne;
            createSalesDeliveryDTO.pOSSalesPayments = posSalesPayment;

            var response = await ViewModel.SaveItemIntoSalesDeliveryLines(createSalesDeliveryDTO);

            if (response != null)
            {
                //var json = JsonConvert.SerializeObject(response);
                // await ViewModel.DataLog("Print Invoice using (alt + P) Invoice no:" + "[" + response.pOSSalesDeliveryHeader.SalesOrderCode + "]", json, "Print Invoice");
                try
                {
                    var responsePrint = await PrintFunction(response);
                }
                catch
                {

                }


                SalesOrderId = 0;
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Invoice Created Successfully", "POS");
                var blockChangeAmount = textBlockChangeAmount.Text;
                var blockPaidAmount = textBlockPaidAmount.Text;
                ClearFields();
                textBlockChangeAmount.Text = blockChangeAmount;
                textBlockPaidAmount.Text = blockPaidAmount;
                itemBarCode.Focus(FocusState.Pointer);

                //temporary object empty.......
                AppSettings.HomePageRefreshObject.Items = null;
                AppSettings.HomePageRefreshObject.PaymentModeInformation = null;
                AppSettings.HomePageRefreshObject.collection = null;
                //temporaty object empty.......
            }
            else
            {
                App.GetService<IAppNotificationService>().OnNotificationInvoked("The server is offline. Please contact to the administrator.", "POS");
            }


        }
        catch (Exception ex)
        {
            ClearFields();
            itemBarCode.Focus(FocusState.Pointer);
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }
    private async void KeyboardAccelerator_Invoked_Quantity(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {


            POSMainGrid.Focus(FocusState.Pointer);
            EditQtyDialog.Title = " ";
            EditQtyDialog.Hide();

            await EditQtyDialog.ShowAsync();

            ItemQty.Focus(FocusState.Pointer);

        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void ItemQty_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var newValue = ItemQty.Text;
                try
                {
                    var decimalQnty = Convert.ToDecimal(newValue);
                    if (decimalQnty < 0)
                        newValue = "1";
                }
                catch
                {
                    newValue = "1";
                }

                var change = ViewModel.homePageItemList.FirstOrDefault();
                var Srate = Convert.ToDecimal(change.SalesRate);
                var NewQnty = Convert.ToDecimal(newValue);
                var itemstockCheck = await ViewModel.GetStockQtyCheckItemByItemID(change.ItemId, Srate);
                if (change.isExchangeEnable == true)
                {
                    newValue = change.Quantity;
                    NewQnty = Convert.ToDecimal(newValue);
                }
                else
                {
                    if (NewQnty > itemstockCheck.TotalQuantity && itemstockCheck.IsNegativeSales == false)
                    {
                        NewQnty = 0.0M;
                        newValue = "0";
                        EditQtyDialog.Hide();
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Out of stock", "POS");
                    }
                }
                var SalesRate = Convert.ToDecimal(change.SalesRate);
                var Discount = Convert.ToDecimal(change.Discount);

                var indexRemove = ViewModel.homePageItemList.IndexOf(change);
                var itemDiscount = change.DiscountPercentage > 0 ? (Math.Round(((NewQnty * SalesRate) * change.DiscountPercentage) / 100, 2)) : (change.SingleDiscountAmount > 0 ? Math.Round((change.SingleDiscountAmount * NewQnty), 2) : 0.0M);

                var Items = new MainViewModelItemDTO()
                {
                    SL = change.SL,
                    ItemId = change.ItemId,
                    ItemName = change.ItemName,
                    Quantity = newValue,
                    SalesRate = change.SalesRate,
                    Vat = Math.Round((change.VATPercentage * ((NewQnty * SalesRate) - itemDiscount)) / 100, 2),
                    SD = Math.Round((change.SDPercentage * (NewQnty * SalesRate)) / 100, 2),
                    //Discount = ((NewQnty * SalesRate) * change.DiscountPercentage) / 100,
                    Discount = change.DiscountPercentage > 0 ? (Math.Round(((NewQnty * SalesRate) * change.DiscountPercentage) / 100, 2)) : (change.SingleDiscountAmount > 0 ? Math.Round((change.SingleDiscountAmount * NewQnty), 2) : 0.0M),
                    Amount = (Math.Round((NewQnty * SalesRate), 2)).ToString(),
                    BarCode = change.BarCode,
                    isExchangeEnable = change.isExchangeEnable,
                    VATPercentage = change.VATPercentage,
                    SDPercentage = change.SDPercentage,
                    DiscountPercentage = change.DiscountPercentage,
                    SingleDiscountAmount = change.SingleDiscountAmount,
                    UMOid = change.UMOid,
                    UMOName = change.UMOName,
                };
                ViewModel.homePageItemList.Remove(change);
                ViewModel.homePageItemList.Insert(indexRemove, Items);
                ItemGrid.ItemsSource = ViewModel.homePageItemList;
                EditQtyDialog.Hide();
                ItemQty.Text = "";
                itemBarCode.Focus(FocusState.Pointer);


                //newly added code............
                if (ViewModel.CollectionObj.NumotherDiscount > 0)
                {
                    VATRecalculateFunction(ViewModel.CollectionObj.NumotherDiscount);
                    List<MainViewModelItemDTO> sortedList = new List<MainViewModelItemDTO>();
                    sortedList = ViewModel.homePageItemList.Select(n => n).ToList();

                    ItemGrid.ItemsSource = null;
                    ItemGrid.ItemsSource = new ObservableCollection<MainViewModelItemDTO>();
                    foreach (var singleList in sortedList)
                    {
                        ViewModel.homePageItemList.Add(singleList);
                    }

                    ViewModel.homePageItemList = new ObservableCollection<MainViewModelItemDTO>(ViewModel.homePageItemList.OrderByDescending(p => p.SL));
                    ItemGrid.ItemsSource = ViewModel.homePageItemList;
                }
                //newly added code.............


            }
            SetTextFieldValue();

            ItemGrid.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void EditQtyDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        EditQtyDialog.Hide();
        itemBarCode.Focus(FocusState.Pointer);
        itemBarCode.Text = "";
    }










    private void ItemMultipleBarCode_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Up)
            {
                e.Handled = true;
                if (ItemMultipleBarCode.SelectedIndex > 0)
                {
                    ItemMultipleBarCode.SelectedIndex--;
                }
            }
            else if (e.Key == Windows.System.VirtualKey.Down)
            {
                e.Handled = true;
                if (ItemMultipleBarCode.SelectedIndex < ViewModel.MultipleBarCodeItemList.Count - 1)
                {
                    ItemMultipleBarCode.SelectedIndex++;
                }
            }

            else if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var selectedItem = ItemMultipleBarCode.SelectedItem;


                //check if current item already added to the list.......
                var itemInformation = selectedItem as MainViewModelItemDTO;
                if (itemInformation == null)
                {
                    return;
                }

                if (Convert.ToDecimal(itemInformation?.DatabaseStock) <= 0 && itemInformation?.IsNegativeSales == false)
                {
                    conMultipleBarCode.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Out Of Stock !", itemInformation.ItemName + " [" + itemInformation.BarCode + "]");
                    return;
                }
                var itemList = ViewModel.homePageItemList.ToList();
                var checkIfAlreadyExists = itemList.Where(n => n.ItemId == itemInformation.ItemId && n.SalesRate == itemInformation.SalesRate).FirstOrDefault();
                if (checkIfAlreadyExists != null)
                {
                    var newQuantity = Convert.ToDecimal(checkIfAlreadyExists.Quantity) + Convert.ToDecimal(itemInformation.Quantity);
                    var salesRate = Convert.ToDecimal(itemInformation.SalesRate);
                    itemInformation.Id = itemList.Select(n => n.Quantity).Count() + 1;
                    itemInformation.SL = checkIfAlreadyExists.SL;
                    itemInformation.Quantity = newQuantity.ToString();

                    itemInformation.SD = Math.Round((itemInformation.SDPercentage * (newQuantity * salesRate)) / 100, 2);
                    //itemInformation.Discount = Math.Round((checkIfAlreadyExists.DiscountPercentage * (newQuantity * salesRate)) / 100, 2);
                    itemInformation.Discount = checkIfAlreadyExists.DiscountPercentage > 0 ? (Math.Round(((newQuantity * salesRate) * checkIfAlreadyExists.DiscountPercentage) / 100, 2)) : (checkIfAlreadyExists.SingleDiscountAmount > 0 ? Math.Round((checkIfAlreadyExists.SingleDiscountAmount * newQuantity), 2) : 0.0M);
                    itemInformation.Vat = Math.Round((itemInformation.VATPercentage * ((newQuantity * salesRate) - itemInformation.Discount)) / 100, 2);

                    itemInformation.Amount = (Math.Round((newQuantity * salesRate), 2)).ToString();
                    itemInformation.BarCode = checkIfAlreadyExists.BarCode;


                    itemInformation.VATPercentage = checkIfAlreadyExists.VATPercentage;
                    itemInformation.SDPercentage = checkIfAlreadyExists.SDPercentage;
                    itemInformation.DiscountPercentage = checkIfAlreadyExists.DiscountPercentage;
                    itemInformation.SingleDiscountAmount = checkIfAlreadyExists.SingleDiscountAmount;

                    itemInformation.UMOid = checkIfAlreadyExists.UMOid;
                    itemInformation.UMOName = checkIfAlreadyExists.UMOName;

                    var index = ViewModel.homePageItemList.IndexOf(checkIfAlreadyExists);
                    ViewModel.homePageItemList.Remove(checkIfAlreadyExists);
                    //ViewModel.homePageItemList.Add(itemInformation);

                    ViewModel.homePageItemList.Insert(0, itemInformation);
                    var indexSl = itemList.Count();
                    foreach (var item in ViewModel.homePageItemList)
                    {
                        item.SL = indexSl;
                        indexSl--;
                    }
                }
                else
                {

                    var newQuantity = Convert.ToDecimal(itemInformation.Quantity);
                    var salesRate = Convert.ToDecimal(itemInformation.SalesRate);
                    itemInformation.SL = ViewModel.homePageItemList.Count + 1;
                    itemInformation.Quantity = newQuantity.ToString();
                    itemInformation.Vat = Math.Round((itemInformation.VATPercentage * (newQuantity * salesRate)) / 100, 2);
                    itemInformation.SD = Math.Round((itemInformation.SDPercentage * (newQuantity * salesRate)) / 100, 2);
                    //itemInformation.Discount = Math.Round((itemInformation.DiscountPercentage * (newQuantity * salesRate)) / 100, 2);
                    itemInformation.Discount = itemInformation.DiscountPercentage > 0 ? (Math.Round(((newQuantity * salesRate) * itemInformation.DiscountPercentage) / 100, 2)) : (itemInformation.SingleDiscountAmount > 0 ? Math.Round((itemInformation.SingleDiscountAmount * newQuantity), 2) : 0.0M);
                    itemInformation.Amount = (Math.Round((newQuantity * salesRate), 2)).ToString();
                    itemInformation.BarCode = itemInformation.BarCode;

                    itemInformation.VATPercentage = itemInformation.VATPercentage;
                    itemInformation.SDPercentage = itemInformation.SDPercentage;
                    itemInformation.DiscountPercentage = itemInformation.DiscountPercentage;
                    itemInformation.SingleDiscountAmount = itemInformation.SingleDiscountAmount;

                    itemInformation.UMOid = itemInformation.UMOid;
                    itemInformation.UMOName = itemInformation.UMOName;

                    ViewModel.homePageItemList.Add(itemInformation);
                }


                itemStock.Text = "Stock: " + itemInformation.DatabaseStock.ToString() + " " + itemInformation.UMOName.ToString();

                var sortedList = ViewModel.homePageItemList.OrderByDescending(p => p.SL).ToList();
                ViewModel.homePageItemList.Clear();


                ItemGrid.ItemsSource = null;
                ItemGrid.ItemsSource = new ObservableCollection<MainViewModelItemDTO>();
                foreach (var singleList in sortedList)
                {
                    ViewModel.homePageItemList.Add(singleList);
                }
                //check if current item already added to the list.......

                ViewModel.homePageItemList = new ObservableCollection<MainViewModelItemDTO>(ViewModel.homePageItemList.OrderByDescending(p => p.SL));
                ItemGrid.ItemsSource = ViewModel.homePageItemList;

                SetTextFieldValue();

                ItemGrid.SelectedIndex = 0;
                itemBarCode.Text = "~";
                conMultipleBarCode.Hide();
                Thread.Sleep(500);
                itemBarCode.Focus(FocusState.Pointer);
                itemBarCode.Text = "~";

            }
        }
        catch (Exception ex)
        {
            conMultipleBarCode.Hide();

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }

    private async void ContentDialogItemBarCodeSearchbox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var Quantity = 1.0M;
                var barCodeQuantity = 1.0M;
                var ItemCode = ContentDialogItemBarCodeSearchbox.Text;
                if (ItemCode.Length < 3)
                    return;

                if (ItemCode.Length >= 2)
                {
                    if (ItemCode[0] == '9' && ItemCode[1] == '9')
                    {
                        var barcode = ItemCode.Substring(2, 4);
                        Quantity = Convert.ToDecimal(ItemCode.Substring(7, 2));

                        var pointQty = Convert.ToDecimal(ItemCode.Substring(9, 3)) / 1000;
                        pointQty = Math.Round(pointQty, 3);

                        Quantity += pointQty;
                        ItemCode = barcode;
                        barCodeQuantity = Quantity;
                    }
                }
                else
                {
                    return;
                }

                var ItemCodeforPcs = ItemCode;

                var ItemInformation = new Item();
                var ItemsInformation = await ViewModel.GetItemListByBarCode(ItemCode);

                if (ViewModel.MultipleBarCodeItemList.Count > 0)
                {
                    ViewModel.MultipleBarCodeItemList.Clear();
                }
                foreach (var singleItem in ItemsInformation)
                {
                    ItemInformation = singleItem;

                    Quantity = barCodeQuantity;
                    //multiple bar code (unit of measure pices/ pices 99)
                    if (ItemInformation.UomId == 5 || ItemInformation.UomId == 6)
                    {
                        if (ItemCodeforPcs.Length >= 2)
                        {
                            if (ItemCodeforPcs[0] == '9' && ItemCodeforPcs[1] == '9')
                            {
                                var pointQty = Convert.ToDecimal(ItemCodeforPcs.Substring(9, 3));
                                pointQty = Math.Round(pointQty, 3);
                                Quantity = pointQty;
                            }
                        }
                    }
                    //multiple bar code (unit of measure pices/pices 99)



                    var Items = new MainViewModelItemDTO()
                    {
                        SL = ViewModel.MultipleBarCodeItemList.Count + 1,
                        ItemId = ItemInformation.ItemId,
                        ItemName = ItemInformation.ItemName,

                        Quantity = Quantity.ToString(),
                        DatabaseStock = ItemInformation.TotalQuantity.ToString(),
                        SalesRate = ItemInformation.CurrentSellingPrice > 0 ? ItemInformation.CurrentSellingPrice.ToString() : "0",
                        Vat = Math.Round((ItemInformation.Vat * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                        SD = Math.Round((ItemInformation.SD * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                        //Discount = Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                        Discount = ItemInformation.MaximumDiscountPercent > 0 ? (Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2)) : (ItemInformation.MaximumDiscountAmount > 0 ? (Math.Round((Quantity * ItemInformation.MaximumDiscountAmount ?? 0), 2)) : 0.0M),
                        Amount = (Math.Round((Quantity * ItemInformation.CurrentSellingPrice), 2)).ToString(),
                        BarCode = singleItem.Barcode,

                        DiscountPercentage = ItemInformation.MaximumDiscountPercent > 0 ? ItemInformation.MaximumDiscountPercent : 0.0M,
                        SingleDiscountAmount = ItemInformation.MaximumDiscountPercent > 0 ? 0.0M : ItemInformation.MaximumDiscountAmount ?? 0,
                        SDPercentage = ItemInformation.SD,
                        VATPercentage = ItemInformation.Vat,

                        UMOid = ItemInformation.UomId ?? 0,
                        UMOName = ItemInformation.UomName,
                        IsNegativeSales = ItemInformation.IsNegativeSales,
                    };

                    ViewModel.MultipleBarCodeItemList.Add(Items);
                }
                itemBarCode.Text = "";
                ItemMultipleBarCode.ItemsSource = ViewModel.MultipleBarCodeItemList;

                ContentDialogItemBarCodeSearchbox.Text = "";
                ItemMultipleBarCode.Focus(FocusState.Programmatic);

            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }


    }

    private async void ContentDialogItemNameSearchBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    var Quantity = 1.0M;
                    var ItemCode = ContentDialogItemNameSearchBox.Text;
                    if (ItemCode.Length < 3)
                        return;

                    var ItemInformation = new Item();
                    var ItemsInformation = await ViewModel.GetItemListByItemName(ItemCode);

                    if (ViewModel.MultipleBarCodeItemList.Count > 0)
                    {
                        ViewModel.MultipleBarCodeItemList.Clear();
                    }
                    foreach (var singleItem in ItemsInformation)
                    {
                        ItemInformation = singleItem;
                        var Items = new MainViewModelItemDTO()
                        {
                            SL = ViewModel.MultipleBarCodeItemList.Count + 1,
                            ItemId = ItemInformation.ItemId,
                            ItemName = ItemInformation.ItemName,

                            Quantity = Quantity.ToString(),
                            DatabaseStock = ItemInformation.TotalQuantity.ToString(),
                            SalesRate = ItemInformation.CurrentSellingPrice > 0 ? ItemInformation.CurrentSellingPrice.ToString() : "0",
                            Vat = Math.Round((ItemInformation.Vat * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            SD = Math.Round((ItemInformation.SD * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            //Discount = Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            Discount = ItemInformation.MaximumDiscountPercent > 0 ? (Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2)) : (ItemInformation.MaximumDiscountAmount > 0 ? (Math.Round((Quantity * ItemInformation.MaximumDiscountAmount ?? 0), 2)) : 0.0M),
                            Amount = (Math.Round((Quantity * ItemInformation.CurrentSellingPrice), 2)).ToString(),

                            BarCode = singleItem.Barcode,

                            DiscountPercentage = ItemInformation.MaximumDiscountPercent > 0 ? ItemInformation.MaximumDiscountPercent : 0.0M,
                            SingleDiscountAmount = ItemInformation.MaximumDiscountPercent > 0 ? 0.0M : ItemInformation.MaximumDiscountAmount ?? 0,

                            SDPercentage = ItemInformation.SD,
                            VATPercentage = ItemInformation.Vat,

                            UMOid = ItemInformation.UomId ?? 0,
                            UMOName = ItemInformation.UomName,
                            IsNegativeSales = ItemInformation.IsNegativeSales,
                        };

                        ViewModel.MultipleBarCodeItemList.Add(Items);
                    }
                    itemBarCode.Text = "";
                    ItemMultipleBarCode.ItemsSource = ViewModel.MultipleBarCodeItemList;
                    ContentDialogItemNameSearchBox.Text = "";
                    ItemMultipleBarCode.Focus(FocusState.Programmatic);

                }
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void ItemMultiple_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Up)
            {
                e.Handled = true;
                if (ItemMultiple.SelectedIndex > 0)
                {
                    ItemMultiple.SelectedIndex--;
                }
            }
            else if (e.Key == Windows.System.VirtualKey.Down)
            {
                e.Handled = true;
                if (ItemMultiple.SelectedIndex < ViewModel.MultipleBarCodeItemList.Count - 1)
                {
                    ItemMultiple.SelectedIndex++;
                }
            }

            else if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var selectedItem = ItemMultiple.SelectedItem;


                //check if current item already added to the list.......
                var itemInformation = selectedItem as MainViewModelItemDTO;
                if (itemInformation != null)
                {
                    //conMultipleItem.Hide();
                    //itemBarCode.Focus(FocusState.Pointer);
                    //itemBarCode.Text = "~";
                    //return;

                    if (Convert.ToDecimal(itemInformation.DatabaseStock) <= 0 && itemInformation.IsNegativeSales == false)
                    {
                        conMultipleItem.Hide();
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Out Of Stock !", itemInformation.ItemName + " [" + itemInformation.BarCode + "]");
                        return;
                    }
                    var itemList = ViewModel.homePageItemList.ToList();
                    var checkIfAlreadyExists = itemList.Where(n => n.ItemId == itemInformation.ItemId && n.SalesRate == itemInformation.SalesRate).FirstOrDefault();
                    if (checkIfAlreadyExists != null)
                    {
                        var newQuantity = Convert.ToDecimal(checkIfAlreadyExists.Quantity) + Convert.ToDecimal(itemInformation.Quantity);
                        var salesRate = Convert.ToDecimal(itemInformation.SalesRate);
                        itemInformation.Id = itemList.Select(n => n.Quantity).Count() + 1;
                        itemInformation.SL = checkIfAlreadyExists.SL;
                        itemInformation.Quantity = newQuantity.ToString();
                        itemInformation.Vat = Math.Round((itemInformation.VATPercentage * (newQuantity * salesRate)) / 100, 2);

                        //itemInformation.Discount = Math.Round((checkIfAlreadyExists.DiscountPercentage * (newQuantity * salesRate)) / 100, 2);
                        itemInformation.Discount = checkIfAlreadyExists.DiscountPercentage > 0 ? (Math.Round(((newQuantity * salesRate) * checkIfAlreadyExists.DiscountPercentage) / 100, 2)) : (checkIfAlreadyExists.SingleDiscountAmount > 0 ? Math.Round((checkIfAlreadyExists.SingleDiscountAmount * newQuantity), 2) : 0.0M);
                        itemInformation.SD = Math.Round((itemInformation.SDPercentage * ((newQuantity * salesRate) - itemInformation.Discount)) / 100, 2);

                        itemInformation.Amount = (Math.Round((newQuantity * salesRate), 2)).ToString();
                        itemInformation.BarCode = checkIfAlreadyExists.BarCode;


                        itemInformation.VATPercentage = checkIfAlreadyExists.VATPercentage;
                        itemInformation.SDPercentage = checkIfAlreadyExists.SDPercentage;
                        itemInformation.DiscountPercentage = checkIfAlreadyExists.DiscountPercentage;
                        itemInformation.SingleDiscountAmount = checkIfAlreadyExists.SingleDiscountAmount;

                        itemInformation.UMOid = checkIfAlreadyExists.UMOid;
                        itemInformation.UMOName = checkIfAlreadyExists.UMOName;

                        var index = ViewModel.homePageItemList.IndexOf(checkIfAlreadyExists);
                        ViewModel.homePageItemList.Remove(checkIfAlreadyExists);
                        ViewModel.homePageItemList.Insert(0, itemInformation);
                        var indexSl = itemList.Count();
                        foreach (var item in ViewModel.homePageItemList)
                        {
                            item.SL = indexSl;
                            indexSl--;
                        }
                    }
                    else
                    {

                        var newQuantity = Convert.ToDecimal(itemInformation.Quantity);
                        var salesRate = Convert.ToDecimal(itemInformation.SalesRate);
                        itemInformation.SL = ViewModel.homePageItemList.Count + 1;
                        itemInformation.Quantity = newQuantity.ToString();
                        itemInformation.Vat = Math.Round((itemInformation.VATPercentage * (newQuantity * salesRate)) / 100, 2);
                        itemInformation.SD = Math.Round((itemInformation.SDPercentage * (newQuantity * salesRate)) / 100, 2);
                        //itemInformation.Discount = Math.Round((itemInformation.DiscountPercentage * (newQuantity * salesRate)) / 100, 2);
                        itemInformation.Discount = itemInformation.DiscountPercentage > 0 ? (Math.Round(((newQuantity * salesRate) * itemInformation.DiscountPercentage) / 100, 2)) : (itemInformation.SingleDiscountAmount > 0 ? Math.Round((itemInformation.SingleDiscountAmount * newQuantity), 2) : 0.0M);
                        itemInformation.Amount = (Math.Round((newQuantity * salesRate), 2)).ToString();
                        itemInformation.BarCode = itemInformation.BarCode;

                        itemInformation.VATPercentage = itemInformation.VATPercentage;
                        itemInformation.SDPercentage = itemInformation.SDPercentage;
                        itemInformation.DiscountPercentage = itemInformation.DiscountPercentage;
                        itemInformation.SingleDiscountAmount = itemInformation.SingleDiscountAmount;

                        itemInformation.UMOid = itemInformation.UMOid;
                        itemInformation.UMOName = itemInformation.UMOName;

                        ViewModel.homePageItemList.Add(itemInformation);
                    }


                    itemStock.Text = "Stock: " + itemInformation.DatabaseStock.ToString() + " " + itemInformation.UMOName.ToString();

                    var sortedList = ViewModel.homePageItemList.OrderByDescending(p => p.SL).ToList();
                    ViewModel.homePageItemList.Clear();


                    ItemGrid.ItemsSource = null;
                    ItemGrid.ItemsSource = new ObservableCollection<MainViewModelItemDTO>();
                    foreach (var singleList in sortedList)
                    {
                        ViewModel.homePageItemList.Add(singleList);
                    }
                    //check if current item already added to the list.......

                    ViewModel.homePageItemList = new ObservableCollection<MainViewModelItemDTO>(ViewModel.homePageItemList.OrderByDescending(p => p.SL));
                    ItemGrid.ItemsSource = ViewModel.homePageItemList;

                    SetTextFieldValue();
                    Thread.Sleep(10);
                    ItemGrid.SelectedIndex = 0;
                    itemBarCode.Text = "~";
                    conMultipleItem.Hide();
                    itemBarCode.Focus(FocusState.Pointer);
                    itemBarCode.Text = "~";

                }



                //newly added code............
                if (ViewModel.CollectionObj.NumotherDiscount > 0)
                {
                    VATRecalculateFunction(ViewModel.CollectionObj.NumotherDiscount);
                    List<MainViewModelItemDTO> sortedList = new List<MainViewModelItemDTO>();
                    sortedList = ViewModel.homePageItemList.Select(n => n).ToList();

                    ItemGrid.ItemsSource = null;
                    ItemGrid.ItemsSource = new ObservableCollection<MainViewModelItemDTO>();
                    foreach (var singleList in sortedList)
                    {
                        ViewModel.homePageItemList.Add(singleList);
                    }

                    ViewModel.homePageItemList = new ObservableCollection<MainViewModelItemDTO>(ViewModel.homePageItemList.OrderByDescending(p => p.SL));
                    ItemGrid.ItemsSource = ViewModel.homePageItemList;
                }
                //newly added code.............



            }
        }
        catch (Exception ex)
        {
            conMultipleItem.Hide();
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }


    private async void ContentDialogItemBarCode_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (ContentDialogItemBarCode.Text == "")
                {
                    itemBarCode.Text = "";
                    ContentDialogItemBarCode.Focus(FocusState.Pointer);
                    return;
                }
                var Quantity = 1.0M;
                var barCodeQuantity = 1.0M;
                var ItemCode = ContentDialogItemBarCode.Text;
                if (ItemCode.Length < 3)
                    return;

                if (ItemCode.Length >= 2)
                {
                    if (ItemCode[0] == '9' && ItemCode[1] == '9')
                    {
                        var barcode = ItemCode.Substring(2, 4);
                        Quantity = Convert.ToDecimal(ItemCode.Substring(7, 2));

                        var pointQty = Convert.ToDecimal(ItemCode.Substring(9, 3)) / 1000;
                        pointQty = Math.Round(pointQty, 3);

                        Quantity += pointQty;
                        ItemCode = barcode;
                        barCodeQuantity = Quantity;
                    }
                }
                else
                {
                    return;
                }

                var ItemCodeforPcs = ItemCode;

                var ItemInformation = new Item();
                var ItemsInformation = await ViewModel.GetItemListByBarCode(ItemCode);

                if (ViewModel.MultipleBarCodeItemList.Count > 0)
                {
                    ViewModel.MultipleBarCodeItemList.Clear();
                }
                foreach (var singleItem in ItemsInformation)
                {
                    ItemInformation = singleItem;

                    Quantity = barCodeQuantity;

                    ///////////bar code unit of measure id.............................(pices / pice added)
                    if (ItemInformation.UomId == 5 || ItemInformation.UomId == 6)
                    {
                        if (ItemCodeforPcs.Length >= 2)
                        {
                            if (ItemCodeforPcs[0] == '9' && ItemCodeforPcs[1] == '9')
                            {
                                var pointQty = Convert.ToDecimal(ItemCodeforPcs.Substring(9, 3));
                                pointQty = Math.Round(pointQty, 3);
                                Quantity = pointQty;
                            }
                        }
                    }
                    ///////////bar code unit of measure id.............................(pices / pice added)


                    var Items = new MainViewModelItemDTO()
                    {
                        SL = ViewModel.MultipleBarCodeItemList.Count + 1,
                        ItemId = ItemInformation.ItemId,
                        ItemName = ItemInformation.ItemName,

                        Quantity = Quantity.ToString(),
                        DatabaseStock = ItemInformation.TotalQuantity.ToString(),
                        SalesRate = ItemInformation.CurrentSellingPrice > 0 ? ItemInformation.CurrentSellingPrice.ToString() : "0",
                        Vat = Math.Round((ItemInformation.Vat * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                        SD = Math.Round((ItemInformation.SD * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                        //Discount = Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                        Discount = ItemInformation.MaximumDiscountPercent > 0 ? (Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2)) : (ItemInformation.MaximumDiscountAmount > 0 ? (Math.Round((Quantity * ItemInformation.MaximumDiscountAmount ?? 0), 2)) : 0.0M),
                        Amount = (Math.Round((Quantity * ItemInformation.CurrentSellingPrice), 2)).ToString(),
                        BarCode = singleItem.Barcode,

                        DiscountPercentage = ItemInformation.MaximumDiscountPercent > 0 ? ItemInformation.MaximumDiscountPercent : 0.0M,
                        SingleDiscountAmount = ItemInformation.MaximumDiscountPercent > 0 ? 0.0M : ItemInformation.MaximumDiscountAmount ?? 0,
                        SDPercentage = ItemInformation.SD,
                        VATPercentage = ItemInformation.Vat,

                        UMOid = ItemInformation.UomId ?? 0,
                        UMOName = ItemInformation.UomName,
                        IsNegativeSales = ItemInformation.IsNegativeSales,
                    };

                    ViewModel.MultipleBarCodeItemList.Add(Items);
                }
                itemBarCode.Text = "";
                ItemMultiple.ItemsSource = ViewModel.MultipleBarCodeItemList;

                //ContentDialogItemBarCode.Text = "";
                ItemMultiple.Focus(FocusState.Programmatic);

            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }


    }

    private async void ContentDialogItemName_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    if (ContentDialogItemName.Text == "")
                    {
                        conMultipleItem.Hide();
                        itemBarCode.Text = "";
                        itemBarCode.Focus(FocusState.Pointer);
                        return;
                    }
                    var Quantity = 1.0M;
                    var ItemCode = ContentDialogItemName.Text;
                    if (ItemCode.Length < 3)
                        return;

                    var ItemInformation = new Item();
                    var ItemsInformation = await ViewModel.GetItemListByItemName(ItemCode);

                    if (ViewModel.MultipleBarCodeItemList.Count > 0)
                    {
                        ViewModel.MultipleBarCodeItemList.Clear();
                    }
                    foreach (var singleItem in ItemsInformation)
                    {
                        ItemInformation = singleItem;
                        var Items = new MainViewModelItemDTO()
                        {
                            SL = ViewModel.MultipleBarCodeItemList.Count + 1,
                            ItemId = ItemInformation.ItemId,
                            ItemName = ItemInformation.ItemName,

                            Quantity = Quantity.ToString(),
                            DatabaseStock = ItemInformation.TotalQuantity.ToString(),
                            SalesRate = ItemInformation.CurrentSellingPrice > 0 ? ItemInformation.CurrentSellingPrice.ToString() : "0",
                            Vat = Math.Round((ItemInformation.Vat * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            SD = Math.Round((ItemInformation.SD * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            //Discount = Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2),
                            Discount = ItemInformation.MaximumDiscountPercent > 0 ? (Math.Round((ItemInformation.MaximumDiscountPercent * (Quantity * ItemInformation.CurrentSellingPrice)) / 100, 2)) : (ItemInformation.MaximumDiscountAmount > 0 ? (Math.Round((Quantity * ItemInformation.MaximumDiscountAmount ?? 0), 2)) : 0.0M),
                            Amount = (Math.Round((Quantity * ItemInformation.CurrentSellingPrice), 2)).ToString(),

                            BarCode = singleItem.Barcode,

                            DiscountPercentage = ItemInformation.MaximumDiscountPercent > 0 ? ItemInformation.MaximumDiscountPercent : 0.0M,
                            SingleDiscountAmount = ItemInformation.MaximumDiscountPercent > 0 ? 0.0M : ItemInformation.MaximumDiscountAmount ?? 0,

                            SDPercentage = ItemInformation.SD,
                            VATPercentage = ItemInformation.Vat,

                            UMOid = ItemInformation.UomId ?? 0,
                            UMOName = ItemInformation.UomName,
                            IsNegativeSales = ItemInformation.IsNegativeSales,
                        };

                        ViewModel.MultipleBarCodeItemList.Add(Items);
                    }
                    itemBarCode.Text = "";
                    ItemMultiple.ItemsSource = ViewModel.MultipleBarCodeItemList;
                    //ContentDialogItemName.Text = "";
                    ItemMultiple.Focus(FocusState.Pointer);

                }
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void KeyboardAccelerator_Invoked_ItemSearch(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            ViewModel.MultipleBarCodeItemList.Clear();
            conMultipleItem.Focus(FocusState.Pointer);
            conMultipleItem.Title = " ";
            conMultipleItem.Hide();
            ContentDialogItemBarCode.Text = "";
            ContentDialogItemName.Text = "";

            await conMultipleItem.ShowAsync();

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void KeyboardAccelerator_Invoked_Exchange(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            var user = await ViewModel.GetUserInformation(AppSettings.UserId);
            if (user.IsExchange == true)
            {

                conExchange.Hide();
                conExchange.Title = "Exchange";
                await conExchange.ShowAsync();
                ViewModel.ExchangeItemInformationList.Clear();
            }
            else
            {
                AppSettings.PermisionValue = 3;
                AuthorizeAdmin.Hide();
                await AuthorizeAdmin.ShowAsync();
            }


        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void KeyboardAccelerator_Invoked_FocusDisCountBox(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;
        try
        {
            ViewModel.OtherDiscountList.Clear();
            var NoneDiscount = new OtherDiscountDTO
            {
                HeaderId = 0,
                OfferName = "None",
                DiscountType = 0,
                StrDiscountType = "None",
                Value = 0.0M,
            };
            ViewModel.OtherDiscountList.Insert(0, NoneDiscount);
            var responseDiscount = await ViewModel.GetOtherDiscountList();
            foreach (var (item, index) in responseDiscount.Select((value, i) => (value, i)))
            {
                ViewModel.OtherDiscountList.Insert(index + 1, item);
            }

            var user = await ViewModel.GetUserInformation(AppSettings.UserId);
            if (user.IsSpecialDiscount == true)
            {
                AuthorizeAdmin.Hide();
                conExchange.Hide();
                conMultipleBarCode.Hide();
                EditQtyDialog.Hide();
                conRecallInvoice.Hide();
                conExchange.Hide();
                NumberBoxDiscountRate.IsEnabled = true;
                NumberBoxDiscountRate.Focus(FocusState.Programmatic);
                NumberBoxDiscountRate.IsDropDownOpen = true;
                NumberBoxDiscountRate.Text = "";

            }
            else
            {
                AppSettings.PermisionValue = 2;
                conExchange.Hide();
                conMultipleBarCode.Hide();
                EditQtyDialog.Hide();
                conRecallInvoice.Hide();
                conExchange.Hide();
                AuthorizeAdmin.Hide();
                await AuthorizeAdmin.ShowAsync();
                //NumberBoxDiscountRate.Text = "";
                //NumberBoxDiscountRate.Focus(FocusState.Pointer);
            }

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void KeyboardAccelerator_Invoked_FocusCashAmountBox(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            conExchange.Hide();
            conMultipleBarCode.Hide();
            EditQtyDialog.Hide();
            conRecallInvoice.Hide();
            conExchange.Hide();
            textBoxCashAmount.Text = "";

            textBoxCashAmount.Text = textBoxCashAmount.Text = (Convert.ToDecimal(GrandTotal.Text) - Convert.ToDecimal(ViewModel.itemPaymentModeList.Sum(s => s.numberAmount))) > 0 ?
                  (Convert.ToDecimal(GrandTotal.Text) - Convert.ToDecimal(ViewModel.itemPaymentModeList.Sum(s => s.numberAmount))).ToString() : "0";
            textBoxCashAmount.Focus(FocusState.Pointer);
            var caretIndex = textBoxCashAmount.Text.Length;


            if (caretIndex >= 0)
            {
                textBoxCashAmount.SelectionStart = caretIndex;
            }
            textBoxCashAmount.SelectAll();

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void KeyboardAccelerator_Invoked_FocusPaymentTypeDDL(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            conExchange.Hide();
            conMultipleBarCode.Hide();
            EditQtyDialog.Hide();
            conRecallInvoice.Hide();
            conExchange.Hide();

            textBoxCashAmount.Text = "0.0";

            cmbSpcPromg1.Focus(FocusState.Programmatic);
            cmbSpcPromg1.IsDropDownOpen = true;
            args.Handled = true;
            //textBoxWalletAmount.Text = "";
            //textBoxWalletAmount.Focus(FocusState.Pointer);
            //cmbSpcPromg1.SelectedIndex = 0;

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void ConfirmDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
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
            ClearFields();
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }
    }

    private async void ConfirmDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            ConfirmDialog.Hide();
            var dt = await ViewModel.GetCounterSessionDetails();
            if (dt != null)
            {
                ClosingCash.Value = (double)dt.Select(x => x.ClosingCash).FirstOrDefault();
                ClosingNote.Text = dt.Select(x => x.ClosingNote).FirstOrDefault();
                foreach (var item in dt)
                {
                    switch (item.CuerrencyName)
                    {
                        case nameof(BDT1):
                            numBDT1.Value = item.CurrencyClosingCount;
                            break;
                        case nameof(BDT2):
                            numBDT2.Value = item.CurrencyClosingCount;
                            break;
                        case nameof(BDT5):
                            numBDT5.Value = item.CurrencyClosingCount;
                            break;
                        case nameof(BDT10):
                            numBDT10.Value = item.CurrencyClosingCount;
                            break;
                        case nameof(BDT20):
                            numBDT20.Value = item.CurrencyClosingCount;
                            break;
                        case nameof(BDT50):
                            numBDT50.Value = item.CurrencyClosingCount;
                            break;
                        case nameof(BDT100):
                            numBDT100.Value = item.CurrencyClosingCount;
                            break;
                        case nameof(BDT200):
                            numBDT200.Value = item.CurrencyClosingCount;
                            break;
                        case nameof(BDT500):
                            numBDT500.Value = item.CurrencyClosingCount;
                            break;
                        case nameof(BDT1000):
                            numBDT1000.Value = item.CurrencyClosingCount;
                            break;
                        default:
                            // Handle case where item.CurrencyName does not match any of the above cases
                            break;
                    }
                }
            }
            SessionCloseDialog.Title = "Session Close";
            await SessionCloseDialog.ShowAsync();
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Settings");
        }

    }

    private async void KeyboardAccelerator_Invoked_Session(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            ConfirmDialog.Hide();
            await ConfirmDialog.ShowAsync();
        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Session");
        }

    }

    private async void SessionCloseDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            DateTime currentDate = DateTime.Now.BD();
            CounterSessionDTO cs = new CounterSessionDTO()
            {
                //ClosingDatetime = currentDate,
                ClosingCash = (decimal)ClosingCash.Value,
                ClosingNote = ClosingNote.Text,
                //MFSAmountCollection = Convert.ToDecimal(MFS.Text),
                //CashAmountCollection = Convert.ToDecimal(CashAmount.Text),
                ActionById = AppSettings.UserId,
            };
            List<CounterSessionDetailsDTO> counterSessionDetails = new List<CounterSessionDetailsDTO>();
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT1), CurrencyClosingCount = (long)numBDT1.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT2), CurrencyClosingCount = (long)numBDT2.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT5), CurrencyClosingCount = (long)numBDT5.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT10), CurrencyClosingCount = (long)numBDT10.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT20), CurrencyClosingCount = (long)numBDT20.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT50), CurrencyClosingCount = (long)numBDT50.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT100), CurrencyClosingCount = (long)numBDT100.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT200), CurrencyClosingCount = (long)numBDT200.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT500), CurrencyClosingCount = (long)numBDT500.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT1000), CurrencyClosingCount = (long)numBDT1000.Value });

            CreateCounterSeason create = new CreateCounterSeason()
            {
                Session = cs,
                SessionDetails = counterSessionDetails,
            };

            var msg = await ViewModel.CloseCounterSession(create);
            if (msg.StatusCode == 200)
            {
                SessionCloseDialog.Hide();

                ContentDialog containtDialog = new ContentDialog()
                {
                    Title = "Counter Session",
                    Content = "Session Drafted",
                    CloseButtonText = "Ok"
                };
                containtDialog.XamlRoot = this.Content.XamlRoot;
                containtDialog.CloseButtonClick += containtDialog_CloseButtonClicked;
                await containtDialog.ShowAsync();

            }
        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Session Dialog");
        }
    }
    private void containtDialog_CloseButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            var _navigationService = App.GetService<INavigationService>();
            _navigationService.NavigateTo(typeof(MainViewModel).FullName!);

            App.MainWindow.Activate();
        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Session");
        }
    }



    //print function...........
    private async Task<bool> PrintFunction(CreateSalesDeliveryDTO createSalesDeliveryDTO)
    {
        //var InvoicePrint = App.GetService<IInvoicePrint>();
        var header = createSalesDeliveryDTO.pOSSalesDeliveryHeader;
        var row = createSalesDeliveryDTO.pOSSalesDeliveryLine;
        var payment = createSalesDeliveryDTO.pOSSalesPayments;

        var settingInfo = await ViewModel.GetSettingInformation();
        var userInfomation = await ViewModel.GetUserInformation(AppSettings.UserId);
        var walletInfo = await ViewModel.GetWalletInformationbyId(payment.Select(n => n.WalletId).FirstOrDefault());
        var itemList = await ViewModel.GetItemByItemIDs(row.Select(n => n.ItemId).ToList());
        var walletList = await ViewModel.GetWalletInformationbyIds(payment.Select(n => n.WalletId).ToList());
        var PartnerPoints = (await ViewModel.GetPartner(header.CustomerId)).Points;
        var InvoiceModelDTOObj = row.Select((n, index) => new InvoiceModelDTO()
        {
            AccountId = settingInfo.intAccountId,
            AccountName = settingInfo.StrAccountName,
            OfficeName = settingInfo.StrOfficeName,
            Address = settingInfo.StrOfficeName,
            VatName = "MUSHAK 6.3",
            BinNo = settingInfo.BIN,
            Date = header.OrderDate,
            CounterNo = settingInfo.StrCounterCode,
            InvoiceNo = header.SalesOrderCode,
            SalesPersonName = userInfomation.strEmployeeName,
            CustomerName = header.CustomerName,
            ItemSl = index + 1,
            ItemPrice = (decimal)n.Price,
            ItemQty = (decimal)n.Quantity,

            ItemSD = (decimal)n.SdAmount,
            ItemDiscount = (decimal)n.DiscountAmount,
            TotalPoints = PartnerPoints,
            CurrentPoints = (decimal)header.Points,

            SDAmount = header.TotalSd ?? 0,
            Vat = (decimal)n.VatPercentage,
            ItemVat = (decimal)n.VatAmount,
            BarCode = itemList.Where(w => w.ItemId == n.ItemId).Select(s => s.Barcode).FirstOrDefault(),
            ItemName = n.ItemName,
            ItemAmount = n.TotalAmount,
            TotalAmount = header.ItemTotalAmount,
            Discount = header.NetDiscount,
            VatAmount = header.TotalVat ?? 0,
            SD = header.TotalSd ?? 0,
            PaidAmount = header.ReceiveAmount,
            PaymentMethodTypeId = 0,
            PaymentMethodTypeName = walletInfo == null || payment.Select(r => r.CollectionAmount).Sum() < header.ReceiveAmount ? "CASH" : walletInfo.strWalletName,
            PaymentMethodAmount = walletInfo == null || payment.Select(r => r.CollectionAmount).Sum() < header.ReceiveAmount ? (decimal)header.CashPayment : payment.Where(w => w.WalletId == walletInfo.intWalletId).Select(r => r.CollectionAmount).Sum(),
            OutletName = settingInfo.StrAddress,
            Message = AppSettings.Message,

            changeAmount = header.ReturnAmount ?? 0,
            ExchangeAmount = Math.Round(row.Where(r => r.ExchangeReferenceId > 0).Select(r => (r.NetAmount + r.VatAmount ?? 0.0M - r.DiscountAmount ?? 0.0M)).Sum(), 0, MidpointRounding.AwayFromZero),
            PayableAmount = header.NetAmount,
        }).ToList();

        var itemDataList = InvoiceModelDTOObj.FirstOrDefault();
        var i = 0;
        if ((payment.Select(r => r.CollectionAmount).Sum() >= header.ReceiveAmount))
            i++;
        payment.ForEach(f =>
        {
            if ((payment.Count() >= 0) && i == 0)
            {
                var itemPay = new InvoiceModelDTO();
                itemPay.ItemSl = itemDataList.ItemSl;
                itemPay.PaymentMethodTypeName = walletList.Where(w => w.intWalletId == f.WalletId).FirstOrDefault().strWalletName;
                itemPay.PaymentMethodAmount = payment.Where(w => w.WalletId == f.WalletId).Sum(s => s.CollectionAmount);
                InvoiceModelDTOObj.Add(itemPay);
            }
            i = 0;

        });

        ViewModel.PrintInvoice(InvoiceModelDTOObj);
        return true;
    }

    private async void SessionCloseDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            DateTime currentDate = DateTime.Now.BD();
            CounterSessionDTO cs = new CounterSessionDTO()
            {
                ClosingDatetime = currentDate,
                ClosingCash = (decimal)ClosingCash.Value,
                ClosingNote = ClosingNote.Text,
                //MFSAmountCollection = Convert.ToDecimal(MFS.Text),
                //CashAmountCollection = Convert.ToDecimal(CashAmount.Text),
                ActionById = AppSettings.UserId,
            };
            List<CounterSessionDetailsDTO> counterSessionDetails = new List<CounterSessionDetailsDTO>();
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT1), CurrencyClosingCount = (long)numBDT1.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT2), CurrencyClosingCount = (long)numBDT2.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT5), CurrencyClosingCount = (long)numBDT5.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT10), CurrencyClosingCount = (long)numBDT10.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT20), CurrencyClosingCount = (long)numBDT20.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT50), CurrencyClosingCount = (long)numBDT50.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT100), CurrencyClosingCount = (long)numBDT100.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT200), CurrencyClosingCount = (long)numBDT200.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT500), CurrencyClosingCount = (long)numBDT500.Value });
            counterSessionDetails.Add(new CounterSessionDetailsDTO() { CounterSessionId = 0, ActionById = AppSettings.UserId, CurrencyName = nameof(BDT1000), CurrencyClosingCount = (long)numBDT1000.Value });

            CreateCounterSeason create = new CreateCounterSeason()
            {
                Session = cs,
                SessionDetails = counterSessionDetails,
            };

            var msg = await ViewModel.CloseCounterSession(create);
            if (msg.StatusCode == 200)
            {
                SessionCloseDialog.Hide();

                ContentDialog containtDialog = new ContentDialog()
                {
                    Title = "Counter Session",
                    Content = msg.Message,
                    CloseButtonText = "Ok"
                };
                containtDialog.XamlRoot = this.Content.XamlRoot;
                containtDialog.CloseButtonClick += containtDialog_Secondery_CloseButtonClicked;
                await containtDialog.ShowAsync();

            }
        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "Session");
        }
    }
    private void containtDialog_Secondery_CloseButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var _navigationService = App.GetService<INavigationService>();
        _navigationService.NavigateTo(typeof(OutletViewModel).FullName!);

        App.MainWindow.Activate();
    }
    private void PaymentDelete_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var button = sender as Button;
            var item = button.DataContext;
            var removeItem = item as PaymentModeInformation;

            ViewModel.itemPaymentModeList.Remove(removeItem);

            SetTextFieldValue();
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void textBoxWalletAmount_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                //var WalleInfo = cmbSpcPromg1.SelectedItem as PaymentWalletDTO;
                //var WalletAmount = Convert.ToDecimal(textBoxWalletAmount.Text);
                //if (WalletAmount <= 0)
                //{
                //    return;
                //}
                //if (WalleInfo != null)
                //{
                //    var dt = ViewModel.itemPaymentModeList.Where(x => x.intWalletId == WalleInfo.intWalletId && x.strWalletId == WalleInfo.strWalletName).FirstOrDefault();
                //    if (dt != null)
                //    {
                //        var amount = dt.numberAmount + WalletAmount;
                //        ViewModel.itemPaymentModeList.Remove(dt);
                //        ViewModel.itemPaymentModeList.Add(new PaymentModeInformation { intWalletId = WalleInfo.intWalletId, strWalletId = WalleInfo.strWalletName, numberAmount = amount });

                //    }
                //    else
                //        ViewModel.itemPaymentModeList.Add(new PaymentModeInformation { intWalletId = WalleInfo.intWalletId, strWalletId = WalleInfo.strWalletName, numberAmount = WalletAmount });
                //}

                //var totalAddedAmount = ViewModel.itemPaymentModeList.Select(n => n.numberAmount).Sum();
                //var CashAmount = Convert.ToDecimal(textBoxCashAmount.Text);
                //totalAddedAmount = totalAddedAmount + CashAmount;
                //textBlockPaidAmount.Text = totalAddedAmount.ToString();
                //var grandTotal = Convert.ToDecimal(GrandTotal.Text);

                //textBlockChangeAmount.Text = (totalAddedAmount - grandTotal) > 0 ? (totalAddedAmount - grandTotal).ToString() : "0";
                //textBlockPrintTotalAmount.Text = totalAddedAmount.ToString();
                //textBoxWalletAmount.Text = "";
                //SetTextFieldValue();
                //

                var WalleInfo = cmbSpcPromg1.SelectedItem as PaymentWalletDTO;
                var WalletAmount = Convert.ToDecimal(textBoxWalletAmount.Text);
                if (WalletAmount <= 0 || ViewModel.CollectionObj.NumGrandTotal < WalletAmount)
                {
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Can't Accept More than GrandTotal", "POS");
                    return;
                }

                await AddWallateMobileNo.ShowAsync();

                textBoxCashAmount.Text = textBoxCashAmount.Text = (Convert.ToDecimal(GrandTotal.Text) - Convert.ToDecimal(ViewModel.itemPaymentModeList.Sum(s => s.numberAmount))) > 0 ?
                  (Convert.ToDecimal(GrandTotal.Text) - Convert.ToDecimal(ViewModel.itemPaymentModeList.Sum(s => s.numberAmount))).ToString() : "0";
                textBoxCashAmount.Focus(FocusState.Pointer);
                textBoxCashAmount.SelectAll();

            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }


    //print function...........


    private async void KeyboardAccelerator_Invoked_Reset(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            var user = await ViewModel.GetUserInformation(AppSettings.UserId);
            if (user.IsItemDelete == true || user.bolIsPOSAdmin == true)
            {

                await ViewModel.DataLog("Sales Page Clear", "", "Sales Page Clear");

                ClearFields();
            }
            else
            {
                AppSettings.PermisionValue = 4;
                AuthorizeAdmin.Hide();
                await AuthorizeAdmin.ShowAsync();
            }




        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void AuthorizeLogin_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            LoginModel user = new LoginModel()
            {
                UserName = userName.Text,
                Password = UserPassword.Password,

            };
            if (user != null)
            {
                if (AppSettings.PermisionValue == (long)PermisionValue.ItemDelete)
                {
                    await DeleteItemPermision(user);
                }
                if (AppSettings.PermisionValue == (long)PermisionValue.SpecialDiscount)
                {
                    await SpecialDiscount(user);

                }
                if (AppSettings.PermisionValue == (long)PermisionValue.Exchange)
                {
                    await ExchangeAuthorization(user);
                }
                if (AppSettings.PermisionValue == (long)PermisionValue.Reset)
                {
                    await ResetAll(user);
                }
            }

        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }


    private async void Authorize_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            try
            {
                LoginModel user = new LoginModel()
                {
                    UserName = userName.Text,
                    Password = UserPassword.Password,

                };
                if (user != null)
                {
                    if (AppSettings.PermisionValue == (long)PermisionValue.ItemDelete)
                    {
                        await DeleteItemPermision(user);
                    }
                    if (AppSettings.PermisionValue == (long)PermisionValue.SpecialDiscount)
                    {
                        await SpecialDiscount(user);
                    }
                    if (AppSettings.PermisionValue == (long)PermisionValue.Exchange)
                    {
                        await ExchangeAuthorization(user);
                    }
                    if (AppSettings.PermisionValue == (long)PermisionValue.Reset)
                    {
                        await ResetAll(user);
                    }
                }

            }
            catch (Exception ex)
            {

                App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
            }
        }
    }
    private async Task ResetAll(LoginModel user)
    {
        var localLogin = await ViewModel.GetUser(user.UserName, user.Password);
        if (AppSettings.IsOnline == false && localLogin == null)
        {
            userName.Text = "";
            UserPassword.Password = "";
            ViewModel.DeleteItemList = new MainViewModelItemDTO();
            AuthorizeAdmin.Hide();
            ContentDialog containtDialog = new ContentDialog()
            {
                Title = "LogIn Failed",
                Content = "Access Denied.Please contact your administrator !",
                CloseButtonText = "Ok"
            };
            containtDialog.XamlRoot = this.Content.XamlRoot;
            await containtDialog.ShowAsync();
            return;
        }
        if (localLogin != null && (localLogin.IsItemDelete == true || localLogin.bolIsPOSAdmin == true))
        {
            AuthorizeAdmin.Hide();
            userName.Text = "";
            UserPassword.Password = "";
            await ViewModel.DataLog("Sales Page Clear", "", "Sales Page Clear");
            ClearFields();
            return;
        }
        var auth = await ViewModel.Authorization(user);
        if (auth == true)
        {
            AuthorizeAdmin.Hide();
            userName.Text = "";
            UserPassword.Password = "";
            await ViewModel.DataLog("Sales Page Clear", "", "Sales Page Clear");
            ClearFields();
            return;
        }
        else
        {
            userName.Text = "";
            UserPassword.Password = "";
            ViewModel.DeleteItemList = new MainViewModelItemDTO();
            AuthorizeAdmin.Hide();
            ContentDialog containtDialog = new ContentDialog()
            {
                Title = "LogIn Failed",
                Content = "Access Denied.Please contact your administrator !",
                CloseButtonText = "Ok"
            };
            containtDialog.XamlRoot = this.Content.XamlRoot;
            await containtDialog.ShowAsync();
            return;
        }
    }
    private async Task ExchangeAuthorization(LoginModel user)
    {
        var localLogin = await ViewModel.GetUser(user.UserName, user.Password);
        if (AppSettings.IsOnline == false && localLogin == null)
        {
            userName.Text = "";
            UserPassword.Password = "";
            AuthorizeAdmin.Hide();
            ContentDialog containtDialog = new ContentDialog()
            {
                Title = "LogIn Failed",
                Content = "Access Denied.Please contact your administrator !",
                CloseButtonText = "Ok"
            };
            containtDialog.XamlRoot = this.Content.XamlRoot;
            await containtDialog.ShowAsync();
            return;
        }
        if (localLogin != null && (localLogin.IsExchange == true || localLogin.bolIsPOSAdmin == true))
        {
            userName.Text = "";
            UserPassword.Password = "";
            AuthorizeAdmin.Hide();
            conExchange.Title = "Exchange";
            await conExchange.ShowAsync();
            ViewModel.ExchangeItemInformationList.Clear();
            return;
        }
        var auth = await ViewModel.Authorization(user);
        if (auth == true)
        {
            userName.Text = "";
            UserPassword.Password = "";
            AuthorizeAdmin.Hide();
            conExchange.Title = "Exchange";
            await conExchange.ShowAsync();
            ViewModel.ExchangeItemInformationList.Clear();
            return;
        }
        else
        {
            userName.Text = "";
            UserPassword.Password = "";
            AuthorizeAdmin.Hide();
            ContentDialog containtDialog = new ContentDialog()
            {
                Title = "LogIn Failed",
                Content = "Access Denied.Please contact your administrator !",
                CloseButtonText = "Ok"
            };
            containtDialog.XamlRoot = this.Content.XamlRoot;
            await containtDialog.ShowAsync();
            return;
        }
    }
    private async Task SpecialDiscount(LoginModel user)
    {
        var localLogin = await ViewModel.GetUser(user.UserName, user.Password);
        if (AppSettings.IsOnline == false && localLogin == null)
        {
            userName.Text = "";
            UserPassword.Password = "";
            NumberBoxDiscountRate.IsEnabled = false;
            AuthorizeAdmin.Hide();
            ContentDialog containtDialog = new ContentDialog()
            {
                Title = "LogIn Failed",
                Content = "Access Denied.Please contact your administrator !",
                CloseButtonText = "Ok"
            };
            containtDialog.XamlRoot = this.Content.XamlRoot;
            await containtDialog.ShowAsync();
            return;
        }
        if (localLogin != null && (localLogin.IsSpecialDiscount == true || localLogin.bolIsPOSAdmin == true))
        {
            AuthorizeAdmin.Hide();
            conExchange.Hide();
            conMultipleBarCode.Hide();
            EditQtyDialog.Hide();
            conRecallInvoice.Hide();
            conExchange.Hide();
            userName.Text = "";
            UserPassword.Password = "";
            NumberBoxDiscountRate.IsEnabled = true;
            NumberBoxDiscountRate.Text = "";
            NumberBoxDiscountRate.Focus(FocusState.Programmatic);
            NumberBoxDiscountRate.IsDropDownOpen = true;
            return;
        }
        var auth = await ViewModel.Authorization(user);
        if (auth == true)
        {
            AuthorizeAdmin.Hide();
            conExchange.Hide();
            conMultipleBarCode.Hide();
            EditQtyDialog.Hide();
            conRecallInvoice.Hide();
            conExchange.Hide();
            userName.Text = "";
            UserPassword.Password = "";
            NumberBoxDiscountRate.IsEnabled = true;
            NumberBoxDiscountRate.Text = "";
            NumberBoxDiscountRate.Focus(FocusState.Programmatic);
            NumberBoxDiscountRate.IsDropDownOpen = true;
        }
        else
        {
            userName.Text = "";
            UserPassword.Password = "";
            NumberBoxDiscountRate.IsEnabled = false;
            AuthorizeAdmin.Hide();
            ContentDialog containtDialog = new ContentDialog()
            {
                Title = "LogIn Failed",
                Content = "Access Denied.Please contact your administrator !",
                CloseButtonText = "Ok"
            };
            containtDialog.XamlRoot = this.Content.XamlRoot;
            await containtDialog.ShowAsync();
        }
    }
    private async Task DeleteItemPermision(LoginModel user)
    {

        var localLogin = await ViewModel.GetUser(user.UserName, user.Password);
        if (AppSettings.IsOnline == false && localLogin == null)
        {
            userName.Text = "";
            UserPassword.Password = "";
            ViewModel.DeleteItemList = new MainViewModelItemDTO();
            AuthorizeAdmin.Hide();
            ContentDialog containtDialog = new ContentDialog()
            {
                Title = "LogIn Failed",
                Content = "Access Denied.Please contact your administrator !",
                CloseButtonText = "Ok"
            };
            containtDialog.XamlRoot = this.Content.XamlRoot;
            await containtDialog.ShowAsync();
            return;
        }
        if (localLogin != null && (localLogin.IsItemDelete == true || localLogin.bolIsPOSAdmin == true))
        {
            AuthorizeAdmin.Hide();
            var removeItem = ViewModel.DeleteItemList;
            var json = JsonConvert.SerializeObject(removeItem);
            await ViewModel.DataLog("DeleteItem [" + removeItem.BarCode + "]", json, "DeleteItem");
            ViewModel.homePageItemList.Remove(removeItem);
            SetTextFieldValue();
            userName.Text = "";
            UserPassword.Password = "";
            ViewModel.DeleteItemList = new MainViewModelItemDTO();
            return;
        }
        var auth = await ViewModel.Authorization(user);
        if (auth == true)
        {
            AuthorizeAdmin.Hide();
            var removeItem = ViewModel.DeleteItemList;
            var json = JsonConvert.SerializeObject(removeItem);
            await ViewModel.DataLog("DeleteItem [" + removeItem.BarCode + "]", json, "DeleteItem");
            ViewModel.homePageItemList.Remove(removeItem);
            SetTextFieldValue();
            userName.Text = "";
            UserPassword.Password = "";
            ViewModel.DeleteItemList = new MainViewModelItemDTO();
            return;
        }
        else
        {
            userName.Text = "";
            UserPassword.Password = "";
            ViewModel.DeleteItemList = new MainViewModelItemDTO();
            AuthorizeAdmin.Hide();
            ContentDialog containtDialog = new ContentDialog()
            {
                Title = "LogIn Failed",
                Content = "Access Denied.Please contact your administrator !",
                CloseButtonText = "Ok"
            };
            containtDialog.XamlRoot = this.Content.XamlRoot;
            await containtDialog.ShowAsync();
            return;
        }

    }

    private async void CustomerAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ContentCustomerAdd.ShowAsync();
        }
        catch (Exception)
        {

            throw;
        }
    }

    private async void ContentCustomerAdd_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            if (AppSettings.IsOnline == false && await ViewModel.ConnectionCheck() == false)
            {
                ContentCustomerAdd.Hide();
                App.GetService<IAppNotificationService>().OnNotificationInvoked("The server is offline. Please contact to the administrator.", "LogIn");
                return;
            }
            if (txtCustomerMobileAdd.Text.Count() == 11 && txtCustomerNameAdd.Text != "")
            {


                var setting = await ViewModel.GetSettings();
                var pt = await ViewModel.GetPartner();
                List<Partner> part = new List<Partner>();
                var par = new Partner()
                {

                    AccountId = setting.intAccountId,
                    ActionById = AppSettings.UserId,
                    Address = "",
                    ActionTime = DateTime.Now,
                    BinNumber = "",
                    BranchId = setting.intBranchId,
                    ActionByName = "",
                    AdvanceBalance = 0,
                    MobileNo = txtCustomerMobileAdd.Text,
                    PartnerName = txtCustomerNameAdd.Text,
                    //IsSync = false,
                    PartnerTypeId = 1,
                    PartnerTypeName = "Customer",
                    City = "",
                    CreditLimit = 0,
                    DistrictId = 0,
                    Email = "",
                    isActive = true,
                    IsForeign = false,
                    Points = 0,
                    NID = "",
                    PartnerCode = txtCustomerMobileAdd.Text,
                    ThanaId = 0,
                    PartnerGroupId = 0,
                    PartnerBalance = 0,
                    PointsAmount = pt.PointsAmount,
                };
                part.Add(par);
                var dt = await ViewModel.CreatePartner(part);
                if (dt.FirstOrDefault() != null)
                {
                    txtCustomer.Text = dt.FirstOrDefault().MobileNo;
                    txtCustomerName.Text = dt.FirstOrDefault().PartnerName;
                    textPointVale.Text = dt.FirstOrDefault().Points.ToString();
                    txtCustomerMobileAdd.Text = "";
                    txtCustomerNameAdd.Text = "";
                    ContentCustomerAdd.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Create Successfully", "POS");
                }
                else
                {
                    ContentCustomerAdd.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Partner Create Failed Or Already Exists.", "POS");
                }
                itemBarCode.Focus(FocusState.Pointer);
            }
            else
            {
                if (txtCustomerMobileAdd.Text.Count() != 11)
                {
                    itemBarCode.Focus(FocusState.Pointer);
                    ContentCustomerAdd.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Invalid Mobile No !", "Customer Create Failed");

                }

                if (txtCustomerNameAdd.Text == "")
                {
                    itemBarCode.Focus(FocusState.Pointer);
                    ContentCustomerAdd.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Customer Name Requried !", "Customer Create Failed");

                }

            }
        }
        catch (Exception ex)
        {
            itemBarCode.Focus(FocusState.Pointer);
            ContentCustomerAdd.Hide();
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }
    private void NumberBoxDiscountRate_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {

            if (e.Key == Windows.System.VirtualKey.Enter)
            {

                //new code start here.......
                var selectedItem = NumberBoxDiscountRate.SelectedItem as OtherDiscountDTO;
                //new code end here.........


                var otherDiscountAmount = selectedItem.Value;

                var grandTotal = 0.0M;
                try
                {
                    grandTotal = ViewModel.CollectionObj.NumtotalBill + ViewModel.CollectionObj.NumtotalSD + ViewModel.CollectionObj.NumtotalVAT - ViewModel.CollectionObj.NumTotalDiscount;
                    //grandTotal = Convert.ToDecimal(ViewModel.CollectionObj.NumGrandTotal);
                }
                catch
                {
                    grandTotal = 0.0M;
                }

                if (grandTotal >= selectedItem.MinAmount && grandTotal <= selectedItem.MaxAmount)
                {
                    ViewModel.CollectionObj.OtherDiscountType = selectedItem.DiscountType;
                    ViewModel.CollectionObj.NumotherDiscount = selectedItem.DiscountType == 1 ? selectedItem.Value : ((ViewModel.CollectionObj.NumtotalBill * selectedItem.Value) / 100);
                    ViewModel.CollectionObj.NumOtherDiscountPercentage = selectedItem.DiscountType == 1 ? 0.0M : selectedItem.Value;
                    ViewModel.CollectionObj.NumOtherDiscountAmount = selectedItem.DiscountType == 1 ? selectedItem.Value : 0.0M;
                    ViewModel.CollectionObj.OtherDiscountId = selectedItem.HeaderId;

                    ViewModel.CollectionObj.MaxDiscountAmount = selectedItem.MaxAmount;
                    ViewModel.CollectionObj.MinDiscountAmount = selectedItem.MinAmount;
                }

                else
                {
                    if (ViewModel.CollectionObj.OtherDiscountType != 0)
                    {
                        ViewModel.CollectionObj.OtherDiscountType = 0;
                        ViewModel.CollectionObj.NumotherDiscount = 0.0M;
                        ViewModel.CollectionObj.NumOtherDiscountPercentage = 0.0M;
                        ViewModel.CollectionObj.NumOtherDiscountAmount = 0.0M;
                        ViewModel.CollectionObj.OtherDiscountId = 0;

                        ViewModel.CollectionObj.MaxDiscountAmount = 0.0M;
                        ViewModel.CollectionObj.MinDiscountAmount = 0.0M;

                        App.GetService<IAppNotificationService>().OnNotificationInvoked("" + grandTotal.ToString() + " Amount is not Applicable for this Discount", "POS");

                    }
                    else if (selectedItem.HeaderId == 0)
                    {
                        ViewModel.CollectionObj.OtherDiscountType = 0;
                        ViewModel.CollectionObj.NumotherDiscount = 0.0M;
                        ViewModel.CollectionObj.NumOtherDiscountPercentage = 0.0M;
                        ViewModel.CollectionObj.NumOtherDiscountAmount = 0.0M;
                        ViewModel.CollectionObj.OtherDiscountId = 0;

                        ViewModel.CollectionObj.MaxDiscountAmount = 0.0M;
                        ViewModel.CollectionObj.MinDiscountAmount = 0.0M;
                    }
                }

                //ViewModel.CollectionObj.OtherDiscountType = selectedItem.DiscountType;
                //ViewModel.CollectionObj.NumotherDiscount = selectedItem.DiscountType == 1 ? selectedItem.Value : ((ViewModel.CollectionObj.NumtotalBill * selectedItem.Value) / 100);
                //ViewModel.CollectionObj.NumOtherDiscountPercentage = selectedItem.DiscountType == 1 ? 0.0M : selectedItem.Value;
                //ViewModel.CollectionObj.NumOtherDiscountAmount = selectedItem.DiscountType == 1 ? selectedItem.Value : 0.0M;
                //ViewModel.CollectionObj.OtherDiscountId = selectedItem.HeaderId;
                NumberBoxDiscountRate.IsEnabled = false;

                SetTextFieldValue();
                InvoiceNote.Focus(FocusState.Pointer);


                //new Code for VAT Recalculate Function.......(start)
                VATRecalculateFunction(ViewModel.CollectionObj.NumotherDiscount);
                List<MainViewModelItemDTO> sortedList = new List<MainViewModelItemDTO>();
                sortedList = ViewModel.homePageItemList.Select(n => n).ToList();

                ItemGrid.ItemsSource = null;
                ItemGrid.ItemsSource = new ObservableCollection<MainViewModelItemDTO>();
                foreach (var singleList in sortedList)
                {
                    ViewModel.homePageItemList.Add(singleList);
                }

                ViewModel.homePageItemList = new ObservableCollection<MainViewModelItemDTO>(ViewModel.homePageItemList.OrderByDescending(p => p.SL));
                ItemGrid.ItemsSource = ViewModel.homePageItemList;
                //new Code for VAT Recalculate Fucntion.......(end)

            }


        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }



    private void NumberBoxDiscountRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            var selectedItem = NumberBoxDiscountRate.SelectedItem as OtherDiscountDTO;
            if (selectedItem != null)
            {
                var otherDiscountAmount = selectedItem.Value;

                var grandTotal = 0.0M;
                try
                {
                    grandTotal = ViewModel.CollectionObj.NumtotalBill + ViewModel.CollectionObj.NumtotalSD + ViewModel.CollectionObj.NumtotalVAT - ViewModel.CollectionObj.NumTotalDiscount;
                }
                catch
                {
                    grandTotal = 0.0M;
                }
                ViewModel.CollectionObj.OtherDiscountType = selectedItem.DiscountType;
                if (grandTotal >= selectedItem.MinAmount && grandTotal <= selectedItem.MaxAmount)
                {
                    ViewModel.CollectionObj.OtherDiscountType = selectedItem.DiscountType;
                    ViewModel.CollectionObj.NumotherDiscount = selectedItem.DiscountType == 1 ? selectedItem.Value : ((ViewModel.CollectionObj.NumtotalBill * selectedItem.Value) / 100);
                    ViewModel.CollectionObj.NumOtherDiscountPercentage = selectedItem.DiscountType == 1 ? 0.0M : selectedItem.Value;
                    ViewModel.CollectionObj.NumOtherDiscountAmount = selectedItem.DiscountType == 1 ? selectedItem.Value : 0.0M;
                    ViewModel.CollectionObj.OtherDiscountId = selectedItem.HeaderId;

                    ViewModel.CollectionObj.MaxDiscountAmount = selectedItem.MaxAmount;
                    ViewModel.CollectionObj.MinDiscountAmount = selectedItem.MinAmount;
                }

                else
                {
                    if (ViewModel.CollectionObj.OtherDiscountType != 0)
                    {

                        ViewModel.CollectionObj.OtherDiscountType = 0;
                        ViewModel.CollectionObj.NumotherDiscount = 0.0M;
                        ViewModel.CollectionObj.NumOtherDiscountPercentage = 0.0M;
                        ViewModel.CollectionObj.NumOtherDiscountAmount = 0.0M;
                        ViewModel.CollectionObj.OtherDiscountId = 0;

                        ViewModel.CollectionObj.MaxDiscountAmount = 0.0M;
                        ViewModel.CollectionObj.MinDiscountAmount = 0.0M;
                        conRecallInvoice.Hide();
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("" + grandTotal.ToString() + " Amount is not Applicable for this Discount", "POS");

                    }
                    else if(selectedItem.HeaderId == 0)
                    {
                        ViewModel.CollectionObj.OtherDiscountType = 0;
                        ViewModel.CollectionObj.NumotherDiscount = 0.0M;
                        ViewModel.CollectionObj.NumOtherDiscountPercentage = 0.0M;
                        ViewModel.CollectionObj.NumOtherDiscountAmount = 0.0M;
                        ViewModel.CollectionObj.OtherDiscountId = 0;

                        ViewModel.CollectionObj.MaxDiscountAmount = 0.0M;
                        ViewModel.CollectionObj.MinDiscountAmount = 0.0M;
                    }
                }

                //ViewModel.CollectionObj.OtherDiscountType = selectedItem.DiscountType;
                //ViewModel.CollectionObj.NumotherDiscount = selectedItem.DiscountType == 1 ? selectedItem.Value : ((ViewModel.CollectionObj.NumtotalBill * selectedItem.Value) / 100);
                //ViewModel.CollectionObj.NumOtherDiscountPercentage = selectedItem.DiscountType == 1 ? 0.0M : selectedItem.Value;
                //ViewModel.CollectionObj.NumOtherDiscountAmount = selectedItem.DiscountType == 1 ? selectedItem.Value : 0.0M;
                //ViewModel.CollectionObj.OtherDiscountId = selectedItem.HeaderId;



                NumberBoxDiscountRate.IsEnabled = false;

                SetTextFieldValue();
                InvoiceNote.Focus(FocusState.Pointer);



                //textBoxCashAmount.Focus(FocusState.Programmatic);

                //new Code for VAT Recalculate Function.......(start)
                VATRecalculateFunction(ViewModel.CollectionObj.NumotherDiscount);
                List<MainViewModelItemDTO> sortedList = new List<MainViewModelItemDTO>();
                sortedList = ViewModel.homePageItemList.Select(n => n).ToList();
                
                ItemGrid.ItemsSource = null;
                ItemGrid.ItemsSource = new ObservableCollection<MainViewModelItemDTO>();
                foreach (var singleList in sortedList)
                {
                    ViewModel.homePageItemList.Add(singleList);
                }
                ViewModel.homePageItemList = new ObservableCollection<MainViewModelItemDTO>(ViewModel.homePageItemList.OrderByDescending(p => p.SL));
                ItemGrid.ItemsSource = ViewModel.homePageItemList;
                //new Code for VAT Recalculate Fucntion.......(end)
            }

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void AddWallateMobileNo_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            if (WallateReferance.Text != " " && WallateReferance.Text != "")
            {
                if (textBoxCashAmount.Text == "")
                {
                    textBoxCashAmount.Text = "0.0";
                }
                var WalleInfo = cmbSpcPromg1.SelectedItem as PaymentWalletDTO;
                var WalletAmount = Convert.ToDecimal(textBoxWalletAmount.Text);
                var totalAmount = Convert.ToDecimal(textBoxCashAmount.Text) + ViewModel.itemPaymentModeList.Sum(x => x.numberAmount) + WalletAmount;
                if (WalletAmount <= 0 || ViewModel.CollectionObj.NumGrandTotal < totalAmount)
                {
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Can't Accept More than GrandTotal", "POS");
                    return;
                }
                if (WalleInfo != null)
                {
                    //var dt = ViewModel.itemPaymentModeList.Where(x => x.intWalletId == WalleInfo.intWalletId && x.strWalletId == WalleInfo.strWalletName).FirstOrDefault();
                    //if (dt != null)
                    //{
                    //    //var amount = dt.numberAmount + WalletAmount;
                    //    var amount = WalletAmount;
                    //    ViewModel.itemPaymentModeList.Remove(dt);
                    //    ViewModel.itemPaymentModeList.Add(new PaymentModeInformation { intWalletId = WalleInfo.intWalletId, strWalletId = WalleInfo.strWalletName, numberAmount = amount, ReferanceNo = WallateReferance.Text });

                    //}
                    //else
                    ViewModel.itemPaymentModeList.Add(new PaymentModeInformation { intWalletId = WalleInfo.intWalletId, strWalletId = WalleInfo.strWalletName, numberAmount = WalletAmount, ReferanceNo = WallateReferance.Text });
                }

                var totalAddedAmount = ViewModel.itemPaymentModeList.Select(n => n.numberAmount).Sum();

                var CashAmount = Convert.ToDecimal(textBoxCashAmount.Text);
                totalAddedAmount = totalAddedAmount + CashAmount;
                textBlockPaidAmount.Text = totalAddedAmount.ToString();
                var grandTotal = Convert.ToDecimal(GrandTotal.Text);

                textBlockChangeAmount.Text = (totalAddedAmount - grandTotal) > 0 ? (totalAddedAmount - grandTotal).ToString() : "0";
                textBlockPrintTotalAmount.Text = totalAddedAmount.ToString();
                WallateReferance.Text = "";
                AddWallateMobileNo.Hide();
                textBoxWalletAmount.Text = "";
                textBoxCashAmount.Focus(FocusState.Pointer);
                SetTextFieldValue();
                textBoxCashAmount.SelectAll();
            }

        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void WallateReferance_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter && WallateReferance.Text != "" && WallateReferance.Text != " ")
            {
                if (textBoxCashAmount.Text == "")
                {
                    textBoxCashAmount.Text = "0.0";
                }
                var WalleInfo = cmbSpcPromg1.SelectedItem as PaymentWalletDTO;
                var WalletAmount = 0.00M;
                if (textBoxWalletAmount.Text != "")
                    WalletAmount = Convert.ToDecimal(textBoxWalletAmount.Text);
                var totalAmount = Convert.ToDecimal(textBoxCashAmount.Text) + ViewModel.itemPaymentModeList.Sum(x => x.numberAmount) + WalletAmount;
                if (WalletAmount <= 0 || ViewModel.CollectionObj.NumGrandTotal < totalAmount)
                {
                    AddWallateMobileNo.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Can't Accept More than GrandTotal", "POS");
                    return;
                }
                if (WalleInfo != null)
                {
                    //var dt = ViewModel.itemPaymentModeList.Where(x => x.intWalletId == WalleInfo.intWalletId && x.strWalletId == WalleInfo.strWalletName).FirstOrDefault();
                    //if (dt != null)
                    //{
                    //    //var amount = dt.numberAmount + WalletAmount;
                    //    var amount = WalletAmount;
                    //    ViewModel.itemPaymentModeList.Remove(dt);
                    //    ViewModel.itemPaymentModeList.Add(new PaymentModeInformation { intWalletId = WalleInfo.intWalletId, strWalletId = WalleInfo.strWalletName, numberAmount = amount, ReferanceNo = WallateReferance.Text });

                    //}
                    //else
                    ViewModel.itemPaymentModeList.Add(new PaymentModeInformation { intWalletId = WalleInfo.intWalletId, strWalletId = WalleInfo.strWalletName, numberAmount = WalletAmount, ReferanceNo = WallateReferance.Text });
                }

                var totalAddedAmount = ViewModel.itemPaymentModeList.Select(n => n.numberAmount).Sum();

                var CashAmount = Convert.ToDecimal(textBoxCashAmount.Text);
                totalAddedAmount = totalAddedAmount + CashAmount;
                textBlockPaidAmount.Text = totalAddedAmount.ToString();
                var grandTotal = Convert.ToDecimal(GrandTotal.Text);

                //textBlockChangeAmount.Text = (grandTotal-totalAddedAmount ) > 0 ? (grandTotal-totalAddedAmount).ToString() : "0";


                textBlockPrintTotalAmount.Text = totalAddedAmount.ToString();
                WallateReferance.Text = "";
                AddWallateMobileNo.Hide();
                textBoxWalletAmount.Text = "";
                textBoxCashAmount.Focus(FocusState.Pointer);
                SetTextFieldValue();
                // textBoxCashAmount.Text = (Convert.ToDecimal(GrandTotal.Text) - Convert.ToDecimal(ViewModel.itemPaymentModeList.Sum(s => s.numberAmount))) > 0 ?
                //(Convert.ToDecimal(GrandTotal.Text) - Convert.ToDecimal(ViewModel.itemPaymentModeList.Sum(s => s.numberAmount))).ToString() : "0";
                textBoxCashAmount.SelectAll();

            }
        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void KeyboardAccelerator_Invoked_CustomerCreate(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            conExchange.Hide();
            conMultipleBarCode.Hide();
            EditQtyDialog.Hide();
            conRecallInvoice.Hide();
            conExchange.Hide();
            AuthorizeAdmin.Hide();
            ContentCustomerAdd.Hide();
            await ContentCustomerAdd.ShowAsync();
        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POSt");
        }
    }

    private void KeyboardAccelerator_Invoked_OnlyPrint(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            //args.Handled=true;
            if (j == 0)
            {
                j = 1;
                ViewModel.OnlyPrint();
            }
            else
            {
                j = 0;
            }



        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void ItemGridRecallInvoice_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var selectedItem = ItemGridRecallInvoice.SelectedItem;

            var viewRow = selectedItem as RecallInvoiceHomeObjDTO;
            try
            {
                long UserId = 0;

                try
                {
                    UserId = AppSettings.UserId;
                    SalesOrderId = viewRow.SalesOrderId;
                    if (UserId == 0 || SalesOrderId == 0)
                    {
                        conRecallInvoice.Hide();
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("UserId or Customer Id not Found", "POS");
                        return;
                    }
                }
                catch
                {
                    conRecallInvoice.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("UserId or Customer Id not Found", "POS");
                    return;
                }

                var response = await ViewModel.RecallInvoice(UserId, SalesOrderId);

                if (response == null)
                {
                    conRecallInvoice.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("No Data Found", "POS");
                    return;
                }
                var serialNumber = response.Items.Count;

                ViewModel.homePageItemList.Clear();
                foreach (var single in response.Items)
                {
                    single.SL = serialNumber;
                    ViewModel.homePageItemList.Add(single);
                    serialNumber--;
                }

                ViewModel.itemPaymentModeList.Clear();
                foreach (var singlepayment in response.PaymentModeInformation)
                {
                    ViewModel.itemPaymentModeList.Add(singlepayment);
                }

                ViewModel.CollectionObj = response.collection;

                if (ViewModel.PaymentWalletDTOList.Count <= 0)
                {
                    var Paymentresponse = await ViewModel.GetPaymentWalletList();
                    ViewModel.PaymentWalletDTOList.AddRange(Paymentresponse);
                }

                SetTextFieldValue();

                textPointVale.Text = response.collection.CustomerPoints;
                txtCustomer.Text = response.collection.CustomerCode;
                txtCustomerName.Text = response.collection.CustomerName;

                //selected discount setup...
                NumberBoxDiscountRate.SelectedItem = ViewModel.OtherDiscountList.Where(n => n.HeaderId == ViewModel.CollectionObj.OtherDiscountId).FirstOrDefault();
                //selected discount setup...
                AppSettings.IsReturn = response.collection.IsReturn;
                if (response.collection.IsReturn == true)
                {
                    txtCustomer.IsEnabled = false;
                }
                ViewModel.recallInvoiceHomeObjDTOs.Clear();
                i = 1;
                conRecallInvoice.Hide();
                itemBarCode.Focus(FocusState.Pointer);
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Recalled Invoice Get Successfully", "POS");

            }
            catch (Exception ex)
            {
                conRecallInvoice.Hide();
                App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
            }
            ItemGrid.SelectedIndex = 0;
        }
    }

    private async void txtCustomerMobileAdd_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (AppSettings.IsOnline == false && await ViewModel.ConnectionCheck() == false)
                {
                    ContentCustomerAdd.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("The server is offline. Please contact to the administrator.", "LogIn");
                    return;
                }
                if (txtCustomerMobileAdd.Text.Count() == 11 && txtCustomerNameAdd.Text != "")
                {


                    var setting = await ViewModel.GetSettings();
                    var pt = await ViewModel.GetPartner();
                    List<Partner> part = new List<Partner>();
                    var par = new Partner()
                    {

                        AccountId = setting.intAccountId,
                        ActionById = AppSettings.UserId,
                        Address = "",
                        ActionTime = DateTime.Now,
                        BinNumber = "",
                        BranchId = setting.intBranchId,
                        ActionByName = "",
                        AdvanceBalance = 0,
                        MobileNo = txtCustomerMobileAdd.Text,
                        PartnerName = txtCustomerNameAdd.Text,
                        //IsSync = false,
                        PartnerTypeId = 1,
                        PartnerTypeName = "Customer",
                        City = "",
                        CreditLimit = 0,
                        DistrictId = 0,
                        Email = "",
                        isActive = true,
                        IsForeign = false,
                        Points = 0,
                        NID = "",
                        PartnerCode = txtCustomerMobileAdd.Text,
                        ThanaId = 0,
                        PartnerGroupId = 0,
                        PartnerBalance = 0,
                        PointsAmount = pt.PointsAmount,
                    };
                    part.Add(par);
                    var dt = await ViewModel.CreatePartner(part);
                    if (dt.FirstOrDefault() != null)
                    {
                        txtCustomer.Text = dt.FirstOrDefault().MobileNo;
                        txtCustomerName.Text = dt.FirstOrDefault().PartnerName;
                        textPointVale.Text = dt.FirstOrDefault().Points.ToString();
                        txtCustomerMobileAdd.Text = "";
                        txtCustomerNameAdd.Text = "";
                        ContentCustomerAdd.Hide();
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Create Successfully", "POS");
                    }
                    else
                    {
                        ContentCustomerAdd.Hide();
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Partner Create Failed Or Already Exists.", "POS");
                    }
                }
                else
                {
                    if (txtCustomerMobileAdd.Text.Count() != 11)
                    {
                        ContentCustomerAdd.Hide();
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Invalid Mobile No !", "Customer Create Failed");
                    }

                    if (txtCustomerName.Text == "")
                    {
                        ContentCustomerAdd.Hide();
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Customer Name Requried !", "Customer Create Failed");
                    }

                }
                itemBarCode.Focus(FocusState.Pointer);
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void textBoxWalletAmount_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (!int.TryParse(textBoxWalletAmount.Text, out _) && textBoxWalletAmount.Text.Length > 0)
        {
            var caretIndex = textBoxWalletAmount.SelectionStart - 1;
            if (caretIndex >= 0)
            {
                textBoxWalletAmount.Text = textBoxWalletAmount.Text.Remove(caretIndex, 1);
                textBoxWalletAmount.SelectionStart = caretIndex;
            }
        }
    }

    private void textBoxCashAmount_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (!int.TryParse(textBoxCashAmount.Text, out _) && textBoxCashAmount.Text.Length > 0)
        {
            var caretIndex = textBoxCashAmount.SelectionStart - 1;
            if (caretIndex >= 0)
            {
                textBoxCashAmount.Text = textBoxCashAmount.Text.Remove(caretIndex, 1);
                textBoxCashAmount.SelectionStart = caretIndex;
            }
        }
    }

    private void WallateReferance_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {

        if (!int.TryParse(WallateReferance.Text, out _) && WallateReferance.Text.Length > 0)
        {
            var caretIndex = WallateReferance.SelectionStart - 1;
            if (caretIndex >= 0)
            {
                WallateReferance.Text = WallateReferance.Text.Remove(caretIndex, 1);
                WallateReferance.SelectionStart = caretIndex;
            }
        }
    }

    private void txtCustomerNameAdd_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            txtCustomerMobileAdd.Focus(FocusState.Pointer);
        }
    }

    private void txtCustomerMobileAdd_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (!int.TryParse(txtCustomerMobileAdd.Text, out _) && txtCustomerMobileAdd.Text.Length > 0)
        {
            var caretIndex = txtCustomerMobileAdd.SelectionStart - 1;
            if (caretIndex >= 0)
            {
                txtCustomerMobileAdd.Text = txtCustomerMobileAdd.Text.Remove(caretIndex, 1);
                txtCustomerMobileAdd.SelectionStart = caretIndex;
            }
        }
    }

    private async void KeyboardAccelerator_Invoked_DataProcessing(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        try
        {
            POSMainGrid.Visibility = Visibility.Collapsed;
            MainGridProcess.Visibility = Visibility.Visible;
            txtDataProcessing.Visibility = Visibility.Collapsed;


        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void cmbSpcPromg1_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        textBoxCashAmount.Text = "0";
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            textBoxWalletAmount.Focus(FocusState.Pointer);
        }
    }

    private void txtCustomerMobileAdd_TextChanging_1(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (!int.TryParse(txtCustomerMobileAdd.Text, out _) && txtCustomerMobileAdd.Text.Length > 0)
        {
            var caretIndex = txtCustomerMobileAdd.SelectionStart - 1;
            if (caretIndex >= 0)
            {
                txtCustomerMobileAdd.Text = txtCustomerMobileAdd.Text.Remove(caretIndex, 1);
                txtCustomerMobileAdd.SelectionStart = caretIndex;
            }
        }
    }

    private void InvoiceNote_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            textBoxCashAmount.Text = (Convert.ToDecimal(textBlockChangeAmount.Text) * -1).ToString();
            textBoxCashAmount.Focus(FocusState.Pointer);
            var caretIndex = textBoxCashAmount.Text.Length;


            if (caretIndex >= 0)
            {

                textBoxCashAmount.SelectionStart = caretIndex;
            }

        }

    }

    private void cmbSpcPromg1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        textBoxCashAmount.Text = "0";
        //textBoxWalletAmount.Text = "0";
        //textBoxWalletAmount.Focus(FocusState.Pointer);
        //var caretIndex = textBoxWalletAmount.Text.Length;


        //if (caretIndex >= 0)
        //{

        //    textBoxWalletAmount.SelectionStart = caretIndex;
        //}


    }

    private async void btnDataProcessing_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            btnDataProcessing.IsEnabled = false;
            txtDataProcessing.Visibility = Visibility.Visible;
            AppSettings.IsSync = true;
            var data = await ViewModel.DataProcessing();
            POSMainGrid.Visibility = Visibility.Visible;
            MainGridProcess.Visibility = Visibility.Collapsed;
            txtDataProcessing.Visibility = Visibility.Collapsed;
            btnDataProcessing.IsEnabled = true;
            AppSettings.IsSync = false;
        }
        catch (Exception ex)
        {
            AppSettings.IsSync = false;
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }

    //private void KeyboardAccelerator_Invoked_Escape(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    //{
    //    conMultipleItem.Hide();
    //    itemBarCode.Focus(FocusState.Pointer);
    //}
}
