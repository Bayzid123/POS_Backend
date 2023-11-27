using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS.Contracts.Services;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Core.ViewModels.MainViewModelDTO;
using POS.Models;
using POS.Services.HttpsClient;
using POS.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace POS.Views.SalesInvoice;
public sealed partial class InvoicePage : Page
{
    public SalesInvoiceViewModel ViewModel
    {
        get; set;
    }
    public bool IsLoad = true;


    public InvoicePage()
    {
        try
        {

            ViewModel = App.GetService<SalesInvoiceViewModel>();
            IsLoad = false;
            InitializeComponent();
            if (ViewModel.isInvoiceOnlineSearch == false)
            {
                txtOnline.Text = "Ofline";
                ViewModel.GetSalesInvoice("", true);
            }
            else
            {
                txtOnline.Text = "Online";

                ViewModel.GetSalesInvoiceLiveServer("", true);
            }
            IsLoad = true;

        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void InvoiceView_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var button = sender as Button;
            var item = button.DataContext;
            var salesOrder = item as SalesInvoiceDTO;
            AppSettings.salesOrderId = salesOrder.SalesInvoice;
            var user = await ViewModel.GetUserInformation(AppSettings.UserId);
            if (user.bolIsPOSAdmin == true)
            {
                userName.Text = "";
                UserPassword.Password = "";
                AuthorizeAdmin.Hide();
                myGrid.IsTapEnabled = false;
                InvoiceView view = new InvoiceView();

                myPopup.Child = view;
                view.ParentPopup = myPopup;
                myPopup.IsOpen = true;
                return;
            }
            else
            {
                AppSettings.PermisionValue = 6;
                await AuthorizeAdmin.ShowAsync();
            }


        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private async void InvoicePrint_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var item = button.DataContext;
        var salesOrder = item as SalesInvoiceDTO;
        AppSettings.salesOrderId = salesOrder.SalesInvoice;
        var user = await ViewModel.GetUserInformation(AppSettings.UserId);
        if (user.bolIsPOSAdmin == true)
        {
            AuthorizeAdmin.Hide();
            await Reprint();
        }
        else
        {
            AppSettings.PermisionValue = 5;
            await AuthorizeAdmin.ShowAsync();
        }

    }
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
        var PartnerPoints = (await ViewModel.GetPartnerData(header.CustomerId)).Points;
        var InvoiceModelDTOObj = row.Select((n, index) => new InvoiceModelDTO()
        {
            AccountId = settingInfo.intAccountId,
            AccountName = settingInfo.StrAccountName,
            OfficeName = settingInfo.StrOfficeName,
            Address = settingInfo.StrWareHouseName,
            VatName = "MUSHAK 6.3",
            BinNo = settingInfo.BIN,
            Date = header.ActionTime,
            CounterNo = AppSettings.IsOnline==false ? settingInfo.StrCounterCode: header.SalesForceName,
            InvoiceNo = header.SalesOrderCode,
            SalesPersonName = AppSettings.IsOnline == false ? userInfomation.strEmployeeName : header.ActionByName,
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
            Discount = header.HeaderDiscount + header.DiscoundItemTotalPrice??0,
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
                    if (AppSettings.PermisionValue == (long)PermisionValue.Reprint)
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
                                Content = "Access Denied. Please contact your administrator !",
                                CloseButtonText = "Ok"
                            };
                            containtDialog.XamlRoot = this.Content.XamlRoot;
                            await containtDialog.ShowAsync();
                            return;
                        }
                        if (localLogin != null && localLogin.bolIsPOSAdmin == true)
                        {
                            AuthorizeAdmin.Hide();
                            await Reprint();
                            return;
                        }

                        var auth = await ViewModel.Authorization(user);
                        if (auth == true)
                        {
                            AuthorizeAdmin.Hide();
                            await Reprint();
                        }
                        else
                        {
                            userName.Text = "";
                            UserPassword.Password = "";
                            AuthorizeAdmin.Hide();
                            ContentDialog containtDialog = new ContentDialog()
                            {
                                Title = "LogIn Failed",
                                Content = "Access Denied. Please contact your administrator !",
                                CloseButtonText = "Ok"
                            };
                            containtDialog.XamlRoot = this.Content.XamlRoot;
                            await containtDialog.ShowAsync();
                        }
                    }
                    if (AppSettings.PermisionValue == (long)PermisionValue.InvoiceEdit)
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
                        if (localLogin != null && localLogin.bolIsPOSAdmin == true)
                        {
                            userName.Text = "";
                            UserPassword.Password = "";
                            AuthorizeAdmin.Hide();
                            myGrid.IsTapEnabled = false;
                            InvoiceView view = new InvoiceView();

                            myPopup.Child = view;
                            view.ParentPopup = myPopup;
                            myPopup.IsOpen = true;
                            return;
                        }
                        var auth = await ViewModel.Authorization(user);
                        if (auth == true)
                        {
                            userName.Text = "";
                            UserPassword.Password = "";
                            AuthorizeAdmin.Hide();
                            myGrid.IsTapEnabled = false;
                            InvoiceView view = new InvoiceView();

                            myPopup.Child = view;
                            view.ParentPopup = myPopup;
                            myPopup.IsOpen = true;
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
                        }
                    }

                    if (AppSettings.PermisionValue == (long)PermisionValue.InvoiceSynchonization)
                    {
                        var localLogin = await ViewModel.GetUser(user.UserName, user.Password);
                        if (localLogin != null)
                        {
                            AuthorizeAdmin.Hide();
                            await SynchronousFunction();
                            return;
                        }

                        var auth = await ViewModel.Authorization(user);
                        if (auth == true)
                        {
                            AuthorizeAdmin.Hide();
                            await SynchronousFunction();
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
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
            }
        }
    }

    private async Task Reprint()
    {
        try
        {
            List<POSSalesDeliveryLine> row = new List<POSSalesDeliveryLine>();
            List<POSSalesPaymentDTO> payment = new List<POSSalesPaymentDTO>();
            //var button = sender as Button;
            //var item = button.DataContext;
            //var salesOrder = item as SalesInvoiceDTO;
            var salsesOrderId = AppSettings.salesOrderId;
            var head = await ViewModel.GetPosDeliveryHeader(salsesOrderId, AppSettings.isInvoiceOnlineSearch);
            if (head != null)
            {
                row = await ViewModel.GetPosDeliveryLine(head.SalesOrderId, AppSettings.isInvoiceOnlineSearch);
                payment = await ViewModel.GetSalesPayment(head.SalesOrderId, AppSettings.isInvoiceOnlineSearch);
            }

            CreateSalesDeliveryDTO createSalesDeliveryDTO = new CreateSalesDeliveryDTO();

            var SalesDeliveryHeader = new POSSalesDeliveryHeader()
            {
                SalesOrderId = head.SalesOrderId,
                SalesOrderCode = head.SalesOrderCode,
                CustomerOrderId = head.CustomerOrderId,
                AccountId = head.AccountId,
                AccountName = head.AccountName,
                BranchId = head.BranchId,
                BranchName = head.BranchName,
                CustomerId = head.CustomerId,
                CustomerName = head.CustomerName,
                Phone = head.Phone,
                ChallanNo = head.ChallanNo,
                OrderDate = head.OrderDate,
                DeliveryDate = head.DeliveryDate,
                Remarks = head.Remarks,
                PaymentTypeId = head.PaymentTypeId,
                PaymentTypeName = head.PaymentTypeName,
                TotalQuantity = head.TotalQuantity,
                ItemTotalAmount = head.ItemTotalAmount,
                NetDiscount = head.NetDiscount,
                OthersCost = head.OthersCost,
                NetAmount = head.NetAmount,
                TotalLineDiscount = head.TotalLineDiscount,
                HeaderDiscount = head.HeaderDiscount,
                HeaderDiscountPercentage = head.HeaderDiscountPercentage,
                ReceiveAmount = head.ReceiveAmount,
                PendingAmount = head.PendingAmount,
                ReturnAmount = head.ReturnAmount,
                InterestRate = head.InterestRate,
                NetAmountWithInterest = head.NetAmountWithInterest,
                TotalNoOfInstallment = head.TotalNoOfInstallment,
                InstallmentStartDate = head.InstallmentStartDate,
                InstallmentType = head.InstallmentType,
                AmountPerInstallment = head.AmountPerInstallment,
                SalesForceId = head.SalesForceId,
                SalesForceName = head.SalesForceName,
                ActionById = head.ActionById,
                ActionByName = head.ActionByName,
                ActionTime = head.ActionTime,
                IsPosSales = head.IsPosSales,
                isActive = head.isActive,
                SalesOrReturn = head.SalesOrReturn,
                AdvanceBalanceAdjust = head.AdvanceBalanceAdjust,
                CustomerNetAmount = head.CustomerNetAmount,
                IsComplete = head.IsComplete,
                SalesTypeId = head.SalesTypeId,
                SalesTypeName = head.SalesTypeName,
                SalesOrderRefId = head.SalesOrderRefId,
                Narration = head.Narration,
                SmsTransactionId = head.SmsTransactionId,
                AnonymousAddress = head.AnonymousAddress,
                TotalSd = head.TotalSd,
                TotalVat = head.TotalVat,
                IsBillCreated = head.IsBillCreated,
                DiscoundItemTotalPrice = head.DiscoundItemTotalPrice,
                OfferItemTotal = head.OfferItemTotal,
                WalletId = head.WalletId,
                ComissionPercentage = head.ComissionPercentage,
                isInclusive = head.isInclusive,
                OfficeId = head.OfficeId,
                CustomerPO = head.CustomerPO,
                BillNo = head.BillNo,
                ShippingAddressId = head.ShippingAddressId,
                ShippingAddressName = head.ShippingAddressName,
                ShippingContactPerson = head.ShippingContactPerson,
                IsConfirmed = head.IsConfirmed,
                IsApprove = head.IsApprove,
                ProjectName = head.ProjectName,
                FreeTypeId = head.FreeTypeId,
                FreeTypeName = head.FreeTypeName,
                JobOrderId = head.JobOrderId,
                IsSync = head.IsSync,
                Draft = head.Draft,
                UserId = head.UserId,
                CashPayment = head.CashPayment,
                Points = head.Points,
                CounterId= head.CounterId,
                CounterName= head.CounterName,
                HeaderDiscountId= head.HeaderDiscountId,
                ISExchange = head.ISExchange,
                ISReturn = head.ISReturn       


            };
            var SalesDeliveryLIne = row.Select(n => new POSSalesDeliveryLine
            {
                SalesOrderId = n.SalesOrderId,
                ItemId = n.ItemId,
                ItemName = n.ItemName,
                UomId = n.UomId,
                UomName = n.UomName,
                Quantity = n.Quantity,
                ChangeQuantity = n.ChangeQuantity,
                Price = n.Price,
                TotalAmount = n.TotalAmount,
                LineDiscount = n.LineDiscount,
                NetAmount = n.NetAmount,
                VatPercentage = n.VatPercentage,
                WarrantyExpiredDate = n.WarrantyExpiredDate,
                WarrantyDescription = n.WarrantyDescription,
                WarrantyInMonth = n.WarrantyInMonth,
                HeaderDiscountProportion = n.HeaderDiscountProportion,
                HeaderCostProportion = n.HeaderCostProportion,
                CostPrice = n.CostPrice,
                CostTotal = n.CostTotal,
                AnonymousAddress = n.AnonymousAddress,
                WarehouseId = n.WarehouseId,
                SdPercentage = n.SdPercentage,
                VatAmount = n.VatAmount,
                SdAmount = n.SdAmount,
                DiscountType = n.DiscountType,
                DiscountAmount = n.DiscountAmount,
                OfferItemName = n.OfferItemName,
                OfferItemQty = n.OfferItemQty,
                OfferItemId = n.OfferItemId,
                IsOfferItem = n.IsOfferItem,
                ItemBasePriceInclusive = n.ItemBasePriceInclusive,
                ItemDescription = n.ItemDescription,
                FreeTypeId = n.FreeTypeId,
                FreeTypeName = n.FreeTypeName,
                ItemSerial = n.ItemSerial,
                Batch = n.Batch,
                IsSync = n.IsSync,
                ExchangeReferenceId = n.ExchangeReferenceId,

            }).ToList();
            var posSalesPayment = payment.Select(n => new POSSalesPayment()
            {
                SalesDeliveryId = n.SalesDeliveryId,
                AccountId = n.AccountId,
                BranchId = n.BranchId,
                OfficeId = n.OfficeId,
                WalletId = n.WalletId,
                CollectionAmount = n.CollectionAmount,
                TransactionDate = n.TransactionDate,
                IsActive = n.IsActive,
                ActionById = n.ActionById,
                LastActionDatetime = n.LastActionDatetime,
                ServerDatetime = n.ServerDatetime,
                IsSync = n.IsSync
            }).ToList();

            createSalesDeliveryDTO.pOSSalesDeliveryHeader = SalesDeliveryHeader;
            createSalesDeliveryDTO.pOSSalesDeliveryLine = SalesDeliveryLIne;
            createSalesDeliveryDTO.pOSSalesPayments = posSalesPayment;
            if (head != null)
            {
                var responsePrint = await PrintFunction(createSalesDeliveryDTO);

                if (responsePrint == true)
                {
                    userName.Text = "";
                    UserPassword.Password = "";
                    AuthorizeAdmin.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Print Successfully", "POS");

                }
                else
                {
                    AuthorizeAdmin.Hide();
                    App.GetService<IAppNotificationService>().OnNotificationInvoked("Print Failed", "POS");
                }
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
                if (AppSettings.PermisionValue == (long)PermisionValue.Reprint)
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
                    if (localLogin != null && localLogin.bolIsPOSAdmin == true)
                    {
                        AuthorizeAdmin.Hide();
                        await Reprint();
                        return;
                    }

                    var auth = await ViewModel.Authorization(user);
                    if (auth == true)
                    {
                        AuthorizeAdmin.Hide();
                        await Reprint();
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
                    }
                }
                if (AppSettings.PermisionValue == (long)PermisionValue.InvoiceEdit)
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

                    if (localLogin != null && localLogin.bolIsPOSAdmin == true)
                    {
                        userName.Text = "";
                        UserPassword.Password = "";
                        AuthorizeAdmin.Hide();
                        myGrid.IsTapEnabled = false;
                        InvoiceView view = new InvoiceView();

                        myPopup.Child = view;
                        view.ParentPopup = myPopup;
                        myPopup.IsOpen = true;
                        return;
                    }
                    var auth = await ViewModel.Authorization(user);
                    if (auth == true)
                    {
                        userName.Text = "";
                        UserPassword.Password = "";
                        AuthorizeAdmin.Hide();
                        myGrid.IsTapEnabled = false;
                        InvoiceView view = new InvoiceView();

                        myPopup.Child = view;
                        view.ParentPopup = myPopup;
                        myPopup.IsOpen = true;
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
                    }
                }




                if (AppSettings.PermisionValue == (long)PermisionValue.InvoiceSynchonization)
                {
                    var localLogin = await ViewModel.GetUser(user.UserName, user.Password);
                    if (localLogin != null)
                    {
                        AuthorizeAdmin.Hide();
                        await SynchronousFunction();
                        return;
                    }

                    var auth = await ViewModel.Authorization(user);
                    if (auth == true)
                    {
                        AuthorizeAdmin.Hide();
                        await SynchronousFunction();
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
                    }
                }

            }

        }
        catch (Exception ex)
        {

            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }
    }

    private void btnInvoiceScarch_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
       
            if (btnInvoiceScarch.Text == "")
            {
                if (AppSettings.isInvoiceOnlineSearch == false)
                {
                    ViewModel.GetSalesInvoice("", true);
                    ItemGrid.ItemsSource = ViewModel.invoice;
                }
                else
                {
                    ViewModel.GetSalesInvoiceLiveServer("", true);
                    ItemGrid.ItemsSource = ViewModel.invoice;
                }
            }
            else
            {
               
                if (AppSettings.isInvoiceOnlineSearch == false)
                {
                    ViewModel.GetSalesInvoice("", true);
                }
                else
                {
                    AppSettings.IsOnline = true;
                    ViewModel.GetSalesInvoiceLiveServer(btnInvoiceScarch.Text, true);
                    if(isInvoiceSearchOnlie.IsChecked == false)
                    {
                        AppSettings.IsOnline = false;
                    }
                }
                ViewModel.invoice = new ObservableCollection<SalesInvoiceDTO>(ViewModel.invoice.Where(w => w.SalesInvoice == btnInvoiceScarch.Text).ToList());
                ItemGrid.ItemsSource = ViewModel.invoice;
            }



        }

    }



    private async void synchronous_assynchronous_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        isInvoiceSearchOnlie.IsChecked = false;
        ViewModel.isSyncronize = true;
        ViewModel.GetUnSyncronizedSalesInvoice("", !ViewModel.isSyncronize);

        //selectAll_CheckBoxGrid.Visibility = Visibility.Visible;
        ViewModel.SalesInvoiceForSyncList.Clear();
        ViewModel.SalesInvoiceForSyncList.AddRange(ViewModel.invoice);

        if (ViewModel.SalesInvoiceForSyncList.Count > 0)
        {
            SyncButtonGrid.Visibility = Visibility.Visible;
        }
        else
        {
            SyncButtonGrid.Visibility = Visibility.Collapsed;
        }
        if (AppSettings.IsOnline == false && await ViewModel.ConnectionCheck() == false)
        {
            synchronousButton.Visibility = Visibility.Collapsed;
            txtsynchronous.Visibility = Visibility.Collapsed;
        }
        else
        {
            synchronousButton.Visibility = Visibility.Visible;
            txtsynchronous.Visibility = Visibility.Visible;
        }
    }

    private async void synchronous_assynchronous_CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        ViewModel.isSyncronize = false;
        ViewModel.GetSalesInvoice("", !ViewModel.isSyncronize);

        //selectAll_CheckBoxGrid.Visibility = Visibility.Collapsed;
        ViewModel.SalesInvoiceForSyncList.Clear();
        if (ViewModel.SalesInvoiceForSyncList.Count > 0)
        {
            SyncButtonGrid.Visibility = Visibility.Visible;
        }
        else
        {
            SyncButtonGrid.Visibility = Visibility.Collapsed;
        }
        if (AppSettings.IsOnline == false && await ViewModel.ConnectionCheck() == false)
        {
            synchronousButton.Visibility = Visibility.Collapsed;
            txtsynchronous.Visibility = Visibility.Collapsed;
        }
        else
        {
            synchronousButton.Visibility = Visibility.Visible;
            txtsynchronous.Visibility = Visibility.Visible;
        }
    }

    //private void InvoiceSelectCheckBox_Unchecked(object sender, RoutedEventArgs e)
    //{
    //    var checkbox = sender as CheckBox;
    //    var item = checkbox.DataContext;
    //    var selectedItem = item as SalesInvoiceDTO;

    //    if (selectedItem != null)
    //    {
    //        var selectedChange = ViewModel.invoice.Where(n => n.SalesOrderId == selectedItem.SalesOrderId).FirstOrDefault();
    //        if(selectedChange != null)
    //        {
    //            ViewModel.SalesInvoiceForSyncList.Remove(selectedItem);
    //            if (ViewModel.SalesInvoiceForSyncList.Count > 0)
    //            {
    //                ViewModel.isSyncButtonVisible = false;
    //                SyncButtonGrid.Visibility = Visibility.Visible;
    //            }
    //            else
    //            {
    //                ViewModel.isSyncButtonVisible = true;
    //                SyncButtonGrid.Visibility = Visibility.Collapsed;
    //            }
    //            selectedChange.isSelected = false;
    //        }
    //    }
    //    var name = "My name is Amit";
    //}

    //private void InvoiceSelectCheckBox_Checked(object sender, RoutedEventArgs e)
    //{
    //    var checkbox = sender as CheckBox;
    //    var item = checkbox.DataContext;

    //    var selectedItem = item as SalesInvoiceDTO;

    //    var s = sender.ToString();
    //    if (selectedItem != null)
    //    {
    //        var selectedChange = ViewModel.invoice.Where(n => n.SalesOrderId == selectedItem.SalesOrderId).FirstOrDefault();
    //        if (selectedChange != null)
    //        {
    //            selectedChange.isSelected = true;

    //            var checkIfAlreadyExists = ViewModel.SalesInvoiceForSyncList.Where(n => n.SalesOrderId == selectedChange.SalesOrderId).FirstOrDefault();
    //            if(checkIfAlreadyExists != null)
    //            {
    //                return;
    //            }

    //            ViewModel.SalesInvoiceForSyncList.Add(selectedItem);
    //            if(ViewModel.SalesInvoiceForSyncList.Count > 0)
    //            {
    //                ViewModel.isSyncButtonVisible = false;
    //                SyncButtonGrid.Visibility = Visibility.Visible;
    //            }
    //            else
    //            {
    //                ViewModel.isSyncButtonVisible = true;
    //                SyncButtonGrid.Visibility = Visibility.Collapsed;
    //            }
    //        }
    //    }
    //}

    private async void synchronousButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            AppSettings.PermisionValue = 7;
            await AuthorizeAdmin.ShowAsync();

        }
        catch (Exception ex)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked(ex.Message, "POS");
        }

    }



    private async Task SynchronousFunction()
    {
        var invoiceIds = ViewModel.SalesInvoiceForSyncList.Select(n => n.SalesOrderId).Distinct().ToList();
        var InvoiceInformation = await ViewModel.SalesInformationUsingIDs(invoiceIds);

        var settingInfo = await ViewModel.GetSettingInformation();
        var response = await ViewModel.CreateSalesDeliveryInformationIntoSQLServer(InvoiceInformation, settingInfo);

        //sales invoice refresh korte hobe....
        var removeList = ViewModel.SalesInvoiceForSyncList.Where(n => invoiceIds.Contains(n.SalesOrderId)).ToList();
        foreach (var singleRow in removeList)
        {
            ViewModel.SalesInvoiceForSyncList.Remove(singleRow);
            ViewModel.invoice.Remove(singleRow);
        }
        //sales invoice refresh korte hobe....
        ItemGrid.ItemsSource = ViewModel.invoice;

        userName.Text = "";
        UserPassword.Password = "";
    }


    private void selectAll_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        ViewModel.SalesInvoiceForSyncList.Clear();
        foreach (var singleItem in ViewModel.invoice)
        {
            singleItem.isSelected = true;
            ViewModel.SalesInvoiceForSyncList.Add(singleItem);
        }
        ItemGrid.ItemsSource = ViewModel.invoice;


        var sortedList = ViewModel.SalesInvoiceForSyncList.ToList();
        ViewModel.invoice.Clear();
        ItemGrid.ItemsSource = null;
        ItemGrid.ItemsSource = new ObservableCollection<SalesInvoiceDTO>();
        foreach (var singleList in sortedList)
        {
            ViewModel.invoice.Add(singleList);
        }
        //check if current item already added to the list.......

        ViewModel.invoice = new ObservableCollection<SalesInvoiceDTO>(ViewModel.invoice.OrderByDescending(p => p.Sl));
        ItemGrid.ItemsSource = ViewModel.invoice;

        if (ViewModel.SalesInvoiceForSyncList.Count > 0)
        {
            SyncButtonGrid.Visibility = Visibility.Visible;
        }
        else
        {
            SyncButtonGrid.Visibility = Visibility.Collapsed;
        }
    }

    private void selectAll_CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {

        ViewModel.SalesInvoiceForSyncList.Clear();
        foreach (var singleItem in ViewModel.invoice)
        {
            singleItem.isSelected = false;
            ViewModel.SalesInvoiceForSyncList.Add(singleItem);
        }
        ItemGrid.ItemsSource = ViewModel.invoice;


        var sortedList = ViewModel.SalesInvoiceForSyncList.ToList();
        ViewModel.invoice.Clear();
        ItemGrid.ItemsSource = null;
        ItemGrid.ItemsSource = new ObservableCollection<SalesInvoiceDTO>();
        foreach (var singleList in sortedList)
        {
            ViewModel.invoice.Add(singleList);
        }
        //check if current item already added to the list.......

        ViewModel.invoice = new ObservableCollection<SalesInvoiceDTO>(ViewModel.invoice.OrderByDescending(p => p.Sl));
        ItemGrid.ItemsSource = ViewModel.invoice;

        ViewModel.SalesInvoiceForSyncList.Clear();

        if (ViewModel.SalesInvoiceForSyncList.Count > 0)
        {
            SyncButtonGrid.Visibility = Visibility.Visible;
        }
        else
        {
            SyncButtonGrid.Visibility = Visibility.Collapsed;
        }
    }

    private void isInvoiceSearchOnlie_Checked(object sender, RoutedEventArgs e)
    {
        if (IsLoad == true)
        {
            synchronous_assynchronous_CheckBox.IsChecked = false;
        }
        AppSettings.isInvoiceOnlineSearch = true;

        if (AppSettings.isInvoiceOnlineSearch == false)
        {
            if (IsLoad == true)
            {
                txtOnline.Text = "Offline";
                ViewModel.GetSalesInvoice("", true);
            }

        }
        else
        {
            if (IsLoad == true)
            {
                txtOnline.Text = "Online";
                ViewModel.GetSalesInvoiceLiveServer("", true);
            }

        }
        //ei function niye kaj kora lagbe.................
    }
    private void isInvoiceSearchOnlie_Unchecked(object sender, RoutedEventArgs e)
    {

        AppSettings.isInvoiceOnlineSearch = false;
        if (AppSettings.isInvoiceOnlineSearch == false)
        {
            if (IsLoad == true)
            {
                txtOnline.Text = "Offline";
                ViewModel.GetSalesInvoice("", true);
            }


        }
        else
        {
            if (IsLoad == true)
            {
                ViewModel.GetSalesInvoiceLiveServer("", true);
                txtOnline.Text = "Online";
            }

        }
        //ei function niye kaj kora lagbe..................
    }
}
