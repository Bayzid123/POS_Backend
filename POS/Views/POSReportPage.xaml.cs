using Microsoft.UI.Xaml.Controls;

using POS.ViewModels;

namespace POS.Views;

public sealed partial class POSReportPage : Page
{
    public POSReportViewModel ViewModel
    {
        get;
    }

    public POSReportPage()
    {
        ViewModel = App.GetService<POSReportViewModel>();
        InitializeComponent();
    }
}
