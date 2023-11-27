using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
public class UnitOfMeasure
{
    [Key]
    [Required]
    public long UomId
    {
        get; set;
    }
    [MaxLength(100)]
    public string UomName
    {
        get; set;
    }
    [MaxLength(100)]
    public string UomDescription
    {
        get; set;
    }
    public long? AccountId
    {
        get; set;
    }
    public bool? isActive
    {
        get; set;
    }
    public bool IsSync
    {
        get; set;
    }
}
