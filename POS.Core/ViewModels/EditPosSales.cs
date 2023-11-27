using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.ViewModels;
public class EditPosSales
{
    public POSSalesDeliveryHeader head
    {
        get; set;
    }
    public List<POSSalesPayment> Payments
    {
        get; set;
    }
}
