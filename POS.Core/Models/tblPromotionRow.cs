using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("tblPromotionRow")]
public class tblPromotionRow
{
    [Key]
    public long IntPromotionRowId {set;get;}
    public long IntPromotionId {set;get;}
    public decimal NumOrderValueFrom {set;get;}
    public decimal NumOrderValueTo {set;get;}
    public long IntDiscountTypeId {set;get;}
    public string StrDiscountTypeName {set;get;}
    public long IntTypeId
    {set;get;}
    public long IntItemId {set;get;}
    public string StrItemName {set;get;}
    public decimal NumDiscountValue {set;get;}
    public bool IsActive {set; get;}
}
