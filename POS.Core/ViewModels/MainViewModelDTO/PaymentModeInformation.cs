using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels.MainViewModelDTO;
public class PaymentModeInformation
{
    public long intWalletId {set;get;}
    public string strWalletId {set; get;}
    public string ReferanceNo {set; get;}
    public decimal numberAmount {set;get;}
}
