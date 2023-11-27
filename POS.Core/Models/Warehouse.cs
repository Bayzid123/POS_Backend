using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("Warehouse")]
public class Warehouse
{
    [Key]
    //[Required]
    public long WarehouseId
    {
        get; set;
    }
    //[MaxLength(50)]
    public string WarehouseCode
    {
        get; set;
    }
    //[MaxLength(150)]
    public string WarehouseName
    {
        get; set;
    }
    //[MaxLength(300)]
    public string WarehouseAddress
    {
        get; set;
    }
    public long? AccountId
    {
        get; set;
    }
    public bool? IsDefaultWH
    {
        get; set;
    }
    public long? ActionBy
    {
        get; set;
    }
    public DateTime? LastActionDate
    {
        get; set;
    }
    public bool? IsActive
    {
        get; set;
    }
    //[Required]
    public long BranchId
    {
        get; set;
    }
    //[Required]
    public long OfficeId
    {
        get; set;
    }
    //[Required]
    public bool isCentral
    {
        get; set;
    }
    //[Required]
    public bool isWastageWH
    {
        get; set;
    }
    public bool IsSync
    {
        get; set;
    }
}
