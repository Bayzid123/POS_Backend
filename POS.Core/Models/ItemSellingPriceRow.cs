using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("ItemSellingPriceRow")]
public class ItemSellingPriceRow
{
    [Key]
    
    //[Required]
    public long RowId
    {
        get; set;
    }
    //[Required]
    public long HeaderId
    {
        get; set;
    }
    //[Required]
    public long ItemId
    {
        get; set;
    }
    //[MaxLength(50)]
    //[Required]
    public string ItemCode
    {
        get; set;
    }
    //[Required]
    public decimal OldPrice
    {
        get; set;
    }
    //[Required]
    public decimal NewPrice
    {
        get; set;
    }
    //[Required]
    public decimal Qty
    {
        get; set;
    }
    //[Required]
    public bool IsActive
    {
        get; set;
    }
    //[Required]
    public long ActionById
    {
        get; set;
    }
    //[Required]
    public DateTime LastActionDatetime
    {
        get; set;
    }
}
