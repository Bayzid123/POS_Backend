using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace POS.Models;
public class BooleanValueConverterForVisibility : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool isVisible = !(bool)value;
        //bool isVisible = true;
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    } 

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        Visibility visibility = (Visibility)value;
        return (visibility == Visibility.Visible);
    } 
}
