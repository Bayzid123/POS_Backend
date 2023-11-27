using Microsoft.UI.Xaml.Controls;
using POS.Contracts.Services;
using POS.Core.ViewModels;
using POS.ViewModels;

namespace POS.Views;

public sealed partial class ChangePasswordPage : Page
{
    public ChangePasswordViewModel ViewModel
    {
        get;
    }

    public ChangePasswordPage()
    {
        ViewModel = App.GetService<ChangePasswordViewModel>();
        InitializeComponent();
        GetUser();
    }
    private async void GetUser()
    {
        var dt = await ViewModel.GetUser();
        if (dt != null)
        {
            userName.Text = dt.strUserName;
        }
    }
    private async void ChangePassword_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (OldPassword.Password == "")
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked("Old Password Requried !", "POS");
        }
        if (NewPassword.Password == "")
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked("New Password Requried !", "POS");
        }
        if (ConfirmPassword.Password == "")
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked("Confirm Password Requried !", "POS");
        }
        if (NewPassword.Password != ConfirmPassword.Password)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked("New Password and Confirm Password Need To Be Same !", "POS");
        }
        if(NewPassword.Password.Length < 6)
        {
            App.GetService<IAppNotificationService>().OnNotificationInvoked("you have to enter at least 6 digit!  !", "POS");
        }
        ChangePassword change = new ChangePassword();
        change.UserName = userName.Text;
        change.Password = OldPassword.Password;
        change.NewPassword = NewPassword.Password;
        change.ConfirmPasseord = ConfirmPassword.Password;
        var msg = await ViewModel.ChangePassword(change);
        if (msg.StatusCode == 200)
        {
            OldPassword.Password = "";
            NewPassword.Password = "";
            ConfirmPassword.Password = "";
        }
        App.GetService<IAppNotificationService>().OnNotificationInvoked(msg.Message, "POS");
    }
}
