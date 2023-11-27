using Microsoft.UI.Xaml.Controls;

using POS.ViewModels;

namespace POS.Views;

public sealed partial class POSConfigPage : Page
{
    public POSConfigViewModel ViewModel
    {
        get;
    }

    public POSConfigPage()
    {
        ViewModel = App.GetService<POSConfigViewModel>();
        InitializeComponent();
    }
}
