using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("ItemWarehouseBalance")]
public class ItemWarehouseBalance
{
    [Key]
    public long ItemWarehouseBalanceId
    {
        get; set;
    }
    public long WarehouseId
    {
        get; set;
    }
    public long ItemId
    {
        get; set;
    }
    public decimal CurrentStock { get; set; } = 0;
}
