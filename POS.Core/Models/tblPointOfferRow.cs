using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("tblPointOfferRow")]
public class tblPointOfferRow
{
    [Key]
    public long intPointOfferRowId
    {
        get; set;
    }
    public long intPointOfferId
    {
        get; set;
    }
    public long intItemId
    {
        get; set;
    }
    public string strItemName
    {
        get; set;
    }
    public bool isActive
    {
        get; set;
    }
}
