using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Core.ViewModels;

public class GetPartnerDTO
{
    public long PartnerId { get; set; }
    public string PartnerName { get; set; }
    public string PartnerCode { get; set; }
    public string Nid { get; set; }
    public decimal? CreditLimit { get; set; }
    public long? PartnerTypeId { get; set; }
    public string PartnerTypeName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Email { get; set; }
    public string MobileNo { get; set; }
    public long? SalesForceId { get; set; }
    public string SalesForceName { get; set; }
    public long? AccountId { get; set; }
    public long? BranchId { get; set; }
    public long? ActionById { get; set; }
    public string ActionByName { get; set; }
    public string OtherContactNumber { get; set; }
    public string OtherContactName { get; set; }
    public long PartnerGroupId { get; set; }
    public string PartnerGroupName { get; set; }
    public long? PriceTypeId { get; set; }
    public string PriceTypeName { get; set; }
    public string BinNumber { get; set; }
    public bool IsForeign { get; set; }
    public long TerritoryId { get; set; }
    public long DistrictId { get; set; }
    public long ThanaId { get; set; }
}
