using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace POS.Models;
public class StringConcatenationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var prefix = parameter as string;
        if (prefix != null)
        {
            return value.ToString() + " (" + prefix + ")";
        }
        else
        {
            return value;
        }
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
