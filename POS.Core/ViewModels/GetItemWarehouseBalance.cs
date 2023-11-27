using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels;
public class GetItemWarehouseBalance
{
    public long ItemBalanceWarehouseId
    {
        get; set;
    }
    public long WarehouseId
    {
        get; set;
    }
    public long Value
    {
        get; set;
    }
    public decimal Quantity
    {
        get; set;
    }
}
