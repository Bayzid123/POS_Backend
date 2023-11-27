using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.ViewModels.MainViewModelDTO;
public class MainViewRecallInvoiceDTO
{
    public List<MainViewModelItemDTO> Items { get; set; }
    public List<PaymentModeInformation> PaymentModeInformation { get; set; }
    public MyCollection collection { get; set; }
}

public class RecallInvoiceDTO 
{
    public List<POSSalesDeliveryHeader> SalesDeliveryList
    {
        get; set;
    }
   
}

public class SalesDatabaseDTO
{
    public POSSalesDeliveryHeader header { get; set;}
    public List<POSSalesDeliveryLine> lines {set;get;}
    public List<POSSalesPayment> payment {set;get;}
}
