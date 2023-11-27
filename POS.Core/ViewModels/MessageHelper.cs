using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels;
public class MessageHelper
{
    public string Message
    {
        get; set;
    }
    public long StatusCode

    {
        get; set;
    }
    public long ReferanceCode
    {
        get; set;
    }
}
public class ChangePassword
{
    public string UserName
    {
        get; set;
    }
    public string Password

    {
        get; set;
    }
    public string NewPassword
    {
        get; set;
    }
    public string ConfirmPasseord
    {
        get; set;
    }
}
public enum PermisionValue
{
    ItemDelete = 1,
    SpecialDiscount = 2,
    Exchange = 3,
    Reset = 4,
    Reprint = 5,
    InvoiceEdit = 6,
    InvoiceSynchonization = 7,
};
