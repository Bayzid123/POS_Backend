// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using POS.Contracts.Services;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.MainViewModelDTO;
using POS.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views.SalesInvoice;
public sealed partial class InvoiceView : UserControl
{
    public InvoiceViewModel ViewModel
    {
        get; set;
    }

    public SalesInvoiceViewModel SalesInvoiceViewModel
    {
        get; set;
    }
    public Popup ParentPopup
    {
        get; set;
    }
    public InvoiceView()
    {
        try
        {
            if (AppSettings.salesOrderId != null)
            {
                ViewModel = App.GetService<InvoiceViewModel>();
                SalesInvoiceViewModel = App.GetService<SalesInvoiceViewModel>();



                LoadDataAsync();
                this.InitializeComponent();
                txtPhoneNo.Text = ViewModel.head.Phone;
                txtCustomerName.Text = ViewModel.head.CustomerName;
                CashAmount.Text = ViewModel.CashAmountHead == null ? "0" : (ViewModel.head.CashPayment - ViewModel.head.ReturnAmount).ToString();
                InvoiceViewGrid.ItemsSource = ViewModel.rows;
                InvoiceAmountViewGrid.ItemsSource = ViewModel.payment;

            }
            else
                this.InitializeComponent();
        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }
    public void Close()
    {
        if (ParentPopup != null)
        {
            ParentPopup.IsOpen = false;
        }
    }
    private void LoadDataAsync()
    {
        Task.Run(async () =>
        {
            if (AppSettings.isInvoiceOnlineSearch == false)
            {
                var result = await ViewModel.GetPosDeliveryHeader("", false);
                result = await ViewModel.GetPosDeliveryLineItem(ViewModel.head.SalesOrderId, false);
                result = await ViewModel.GetSalesPayment(ViewModel.head.SalesOrderId, false);


            }
            else
            {
                var result = await ViewModel.GetPosDeliveryHeader("", true);
                result = await ViewModel.GetPosDeliveryLineItem(ViewModel.head.SalesOrderId, true);
                result = await ViewModel.GetSalesPayment(ViewModel.head.SalesOrderId, true);
            }

            if (ViewModel.PaymentWalletDTOList.Count <= 0)
            {
                var response = await ViewModel.GetPaymentWalletList();
                ViewModel.PaymentWalletDTOList.AddRange(response);
            }

        });
        Thread.Sleep(1500);


    }
    public void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void btnWalletAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var WalleInfo = cmbSpcPromg1.SelectedItem as PaymentWalletDTO;
            var WalletAmount = Convert.ToDecimal(textBoxWalletAmount.Text);
            if (txtReferance.Text == "" || txtReferance.Text == " ")
            {
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Refarance Requried", "POS");
                return;
            }
            if (WalletAmount <= 0)
            {
                return;
            }
            if (WalleInfo != null)
            {

                //var dt = ViewModel.payment.Where(x => x.WalletId == WalleInfo.intWalletId && x.WalletName == WalleInfo.strWalletName).FirstOrDefault();
                //if (dt != null)
                //{
                //    var amount = dt.CollectionAmount + WalletAmount;
                //    var SalesPaymentId = dt.POSSalesPaymentId;
                //    ViewModel.payment.Remove(dt);
                //    ViewModel.payment.Add(new POSSalesPaymentDTO { WalletId = WalleInfo.intWalletId, WalletName = WalleInfo.strWalletName, CollectionAmount = amount, SalesDeliveryId = ViewModel.head.SalesOrderId, POSSalesPaymentId = SalesPaymentId, ReferanceNo = txtReferance.Text });
                //}
                //else
                ViewModel.payment.Add(new POSSalesPaymentDTO { WalletId = WalleInfo.intWalletId, WalletName = WalleInfo.strWalletName, CollectionAmount = WalletAmount, SalesDeliveryId = ViewModel.head.SalesOrderId, POSSalesPaymentId = 0, ReferanceNo = txtReferance.Text });

                InvoiceAmountViewGrid.ItemsSource = ViewModel.payment;
            }
            txtReferance.Text = "";
            textBoxWalletAmount.Text = "";
            var data = CashAmount.Text.Count();
            if (CashAmount.Text.Count() <= 0)
            {
                CashAmount.Text = "0";
            }
        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void PaymentDelete_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var button = sender as Button;
            var item = button.DataContext;
            var removeItem = item as POSSalesPaymentDTO;

            ViewModel.payment.Remove(removeItem);
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void textBoxWalletAmount_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (txtReferance.Text == "" || txtReferance.Text == " ")
                {
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Refarance Requried", "POS");
                    return;
                }
                var WalleInfo = cmbSpcPromg1.SelectedItem as PaymentWalletDTO;
                var WalletAmount = Convert.ToDecimal(textBoxWalletAmount.Text);
                if (WalletAmount <= 0)
                {
                    return;
                }
                if (WalleInfo != null)
                {

                    //var dt = ViewModel.payment.Where(x => x.WalletId == WalleInfo.intWalletId && x.WalletName == WalleInfo.strWalletName).FirstOrDefault();
                    //if (dt != null)
                    //{
                    //    var amount = dt.CollectionAmount + WalletAmount;
                    //    var SalesPaymentId = dt.POSSalesPaymentId;
                    //    ViewModel.payment.Remove(dt);
                    //    ViewModel.payment.Add(new POSSalesPaymentDTO { WalletId = WalleInfo.intWalletId, WalletName = WalleInfo.strWalletName, CollectionAmount = amount, SalesDeliveryId = ViewModel.head.SalesOrderId, POSSalesPaymentId = SalesPaymentId, ReferanceNo = txtReferance.Text });

                    //}
                    //else
                    ViewModel.payment.Add(new POSSalesPaymentDTO { WalletId = WalleInfo.intWalletId, WalletName = WalleInfo.strWalletName, CollectionAmount = WalletAmount, SalesDeliveryId = ViewModel.head.SalesOrderId, POSSalesPaymentId = 0, ReferanceNo = txtReferance.Text });

                    InvoiceAmountViewGrid.ItemsSource = ViewModel.payment;
                    txtReferance.Text = "";
                    textBoxWalletAmount.Text = "";
                    var data = CashAmount.Text.Count();
                    if (CashAmount.Text.Count() <= 0)
                    {
                        CashAmount.Text = "0";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void EditPayment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var customer = await ViewModel.GetPartnerById(txtPhoneNo.Text);
            if (customer == null)
            {
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Customer Not Found", "POS");
                return;
            }
            if (txtPhoneNo.Text == "")
            {
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Phone no cann't be empty !", "POS");
                return;
            }
            if (CashAmount.Text == "")
            {
                CashAmount.Text = "0";
            }
            decimal totalWalet = 0;
            if (ViewModel.payment.Count > 0)
            {
                var dt = ViewModel.payment.Sum(x => x.CollectionAmount);
                totalWalet = dt;
            }
            var cash = decimal.Parse(CashAmount.Text);
            if ((decimal)TotalAmount.Value == (cash + totalWalet))
            {
                var head = ViewModel.head;
                var payment = ViewModel.payment;
                EditPosSales edit = new EditPosSales();
                List<POSSalesPayment> pOSSalesPayments = new List<POSSalesPayment>();
                edit.head = head;
                foreach (var item in payment)
                {
                    var pa = new POSSalesPayment()
                    {
                        ServerDatetime = DateTime.Now,
                        POSSalesPaymentId = 0,
                        SalesDeliveryId = ViewModel.head.SalesOrderId,
                        AccountId = ViewModel.head.AccountId,
                        ActionById = AppSettings.UserId,
                        BranchId = ViewModel.head.BranchId,
                        CollectionAmount = item.CollectionAmount,
                        WalletId = item.WalletId,
                        ReferanceNo = item.ReferanceNo,
                        IsActive = true,
                        LastActionDatetime = DateTime.Now,
                        IsSync = false,
                        OfficeId = ViewModel.head.OfficeId,
                        TransactionDate = DateTime.Now
                    };
                    pOSSalesPayments.Add(pa);
                }
                if (pOSSalesPayments.Count > 0)
                {
                    edit.Payments = pOSSalesPayments;
                }
                if (AppSettings.IsOnline == false && head.IsSync == 1)
                {
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("This Invoice Synced in Online. It Can't edit in Offline.", "POS");
                    return;
                }
                if (edit.head != null)
                {
                    edit.head.CashPayment = cash;
                    var msg = await ViewModel.EditPosPayment(edit);
                    if (msg.StatusCode == 200 && head.IsSync != 1)
                    {
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Update Successfully", "POS");
                    }
                }
                if (head.IsSync == 1)
                {
                    var msg = await ViewModel.EditPosPaymentSQL(edit);
                    if (msg.StatusCode == 200 && head.IsSync == 1)
                    {
                        App.GetService<IAppNotificationService>().OnNotificationInvoked("Update Successfully", "POS");
                    }
                }
                Close();
            }
            else
            {
                App.GetService<IAppNotificationService>().OnNotificationInvoked("Paid Amount Not Equal To Total Amount", "POS");
            }
        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void txtPhoneNo_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var partnerCode = txtPhoneNo.Text.Trim().ToString();
            var PartnerInformation = await ViewModel.GetPartnerById(partnerCode);
            if (PartnerInformation == null)
            {
                //textPointVale.Text = "0.00";
                txtPhoneNo.Text = "";
                txtCustomerName.Text = "";

                App.GetService<IAppNotificationService>().OnNotificationInvoked("No Customer Found !", "POS");
                return;
            }

            //textPointVale.Text = PartnerInformation.Points.ToString();

            ViewModel.head.CustomerId = PartnerInformation.PartnerId;
            ViewModel.head.CustomerName = PartnerInformation.PartnerName;
            ViewModel.head.Phone = PartnerInformation.MobileNo;
            txtCustomerName.Text = PartnerInformation.PartnerName;

        }
    }

    private void CashAmount_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (!int.TryParse(CashAmount.Text, out _) && CashAmount.Text.Length > 0)
        {
            var caretIndex = CashAmount.SelectionStart - 1;
            if (caretIndex >= 0)
            {
                CashAmount.Text = CashAmount.Text.Remove(caretIndex, 1);
                CashAmount.SelectionStart = caretIndex;
            }
        }
    }

    private void txtReferance_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (!int.TryParse(txtReferance.Text, out _) && txtReferance.Text.Length > 0)
        {
            var caretIndex = txtReferance.SelectionStart - 1;
            if (caretIndex >= 0)
            {
                txtReferance.Text = txtReferance.Text.Remove(caretIndex, 1);
                txtReferance.SelectionStart = caretIndex;
            }
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
}
