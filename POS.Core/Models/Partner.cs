using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("Partner")]
public class Partner
{
    [Key]
    //[Required]
    public long PartnerId
    {
        get; set;
    }
    //[MaxLength(200)]
    public string PartnerName
    {
        get; set;
    }
    //[MaxLength(50)]
    public string PartnerCode
    {
        get; set;
    }
    //[MaxLength(100)]
    public string NID
    {
        get; set;
    }
    //[Required]
    public long PartnerTypeId
    {
        get; set;
    }
    //[MaxLength(50)]
    public string PartnerTypeName
    {
        get; set;
    }
    public long? TaggedEmployeeId
    {
        get; set;
    }
    //[MaxLength(100)]
    public string TaggedEmployeeName
    {
        get; set;
    }
    //[MaxLength(300)]
    public string Address
    {
        get; set;
    }
    //[MaxLength(100)]
    public string City
    {
        get; set;
    }
    //[MaxLength(100)]
    public string Email
    {
        get; set;
    }
    //[MaxLength(50)]
    public string MobileNo
    {
        get; set;
    }
    public long? AccountId
    {
        get; set;
    }
    public long? BranchId
    {
        get; set;
    }
    public decimal? AdvanceBalance
    {
        get; set;
    }
    public decimal? CreditLimit
    {
        get; set;
    }
    public long? ActionById
    {
        get; set;
    }
    //[MaxLength(100)]
    public string ActionByName
    {
        get; set;
    }
    public DateTime? ActionTime
    {
        get; set;
    }
    public bool? isActive
    {
        get; set;
    }
    //[MaxLength(50)]
    public string OtherContactNumber
    {
        get; set;
    }
    //[MaxLength(200)]
    public string OtherContactName
    {
        get; set;
    }
    //[Required]
    public decimal PartnerBalance
    {
        get; set;
    }
    public long? PartnerGroupId
    {
        get; set;
    }
    //[MaxLength(200)]
    public string PartnerGroupName
    {
        get; set;
    }
    public long? PriceTypeId
    {
        get; set;
    }
    //[MaxLength(500)]
    public string PriceTypeName
    {
        get; set;
    }
    //[MaxLength(250)]
    public string BinNumber
    {
        get; set;
    }
    //[Required]
    public bool IsForeign
    {
        get; set;
    }
    //[Required]
    public long TerritoryId
    {
        get; set;
    }
    //[Required]
    public long DistrictId
    {
        get; set;
    }
    //[Required]
    public long ThanaId
    {
        get; set;
    }
    //public bool IsSync
    //{
    //    get; set;
    //}

    public decimal Points
    {
        set;get;
    }
    public decimal PointsAmount
    {
        set;get;
    }
}
