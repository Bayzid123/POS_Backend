using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.ViewModels.MainViewModelDTO;
public class CreateSalesDeliveryDTO
{
    public POSSalesDeliveryHeader pOSSalesDeliveryHeader
    {
        set;
        get;
    }

    public List<POSSalesDeliveryLine> pOSSalesDeliveryLine
    {
        set;get;
    }

    //public SalesWallet salesWallet
    //{
    //    set;get;
    //}
    public List<POSSalesPayment> pOSSalesPayments
    {
        set;get;
    }
    public bool IsOnline
    {
        set; get;
    }
}


