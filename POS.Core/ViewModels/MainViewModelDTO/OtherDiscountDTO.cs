using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels.MainViewModelDTO;
public class OtherDiscountDTO
{
    public long HeaderId {set;get;}
    public string OfferName {set;get;}
    public long DiscountType {set;get;}
    public string StrDiscountType {set;get;}
    public decimal Value {set;get;}

    public decimal MinAmount {set; get;}
    public decimal MaxAmount {set; get;}
}
