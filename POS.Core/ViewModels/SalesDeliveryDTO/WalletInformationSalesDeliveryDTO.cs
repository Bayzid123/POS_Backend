using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels.SalesDeliveryDTO;
public class WalletInformationSalesDeliveryDTO
{
   public long walletId {set;get;}
    public decimal collectionAmount {set;get;}
    public string ReferanceNo
    {
        get; set;
    }
}
