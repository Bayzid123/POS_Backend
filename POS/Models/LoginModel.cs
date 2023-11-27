using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models;
public class LoginModel
{
    public string UserName
    {
        get; set;
    }
    public string Password
    {
        get; set;
    }
    public long AccountId
    {
        get;set;
    }
    public long BranchId
    {
        get;set;
    }
    public long OfficeId
    {
        get; set;
    }
    public long WarehouseId
    {
        get; set;
    }
    public long CounterId
    {
        get;set;
    }
    public long UserId
    {
        get;set;
    }
}
public class DeviceInfo
{
    public string DeviceId
    {
        get; set;
    }
    public string DeviceMac
    {
        get; set;
    }
    public string SqlServerConnString
    {
        get; set;
    }
    public bool OflineConnection
    {
        get; set;
    }
    public bool IsSync
    {
        get; set;
    }
 

}
public class SmeUserInfoDTO
{
    public long UserId
    {
        get; set;
    }
    public long? EmployeeId
    {
        get; set;
    }
    public string EmployeeName
    {
        get; set;
    }
    public string EmployeeDesignation
    {
        get; set;
    }
    public string EmployeeDepartment
    {
        get; set;
    }
    public long? SupervisorId
    {
        get; set;
    }

    public string SupervisorName
    {
        get; set;
    }
    public long? LineManagerId
    {
        get; set;
    }

    public string LineManagerName
    {
        get; set;
    }
    public string FirstName
    {
        get; set;
    }
    public string LastName
    {
        get; set;
    }
    public string UserName
    {
        get; set;
    }
    public string MobileNumber
    {
        get; set;
    }
    public long? AccountId
    {
        get; set;
    }
    public string AccountName
    {
        get; set;
    }
    public string LogoString
    {
        get; set;
    }
    public string AccountEmail
    {
        get; set;
    }
    public string Email
    {
        get; set;
    }
    public bool? isMasterUser
    {
        get; set;
    }
    public bool isBranch
    {
        get; set;
    }
    public bool? IsAutoChallan
    {
        get; set;
    }
    public bool? IsAutoMoneyReceipt
    {
        get; set;
    }
    public bool? IsAutoItemCode
    {
        get; set;
    }
    public bool? IsAutoPartnerCode
    {
        get; set;
    }
    public bool? IsAutoChallanDepent
    {
        get; set;
    }
    public bool? IsAutoItemCodeDepent
    {
        get; set;
    }
    public bool? IsAutoMoneyReceiptDepent
    {
        get; set;
    }
    public bool? IsAutoPartnerCodeDepent
    {
        get; set;
    }
    public string TermsAndConditions
    {
        get; set;
    }
    public string branchList
    {
        get; set;
    }
    public string officeList
    {
        get; set;
    }
    public bool IsPosAdmin
    {
        get; set;
    }
    public bool IsExchange
    {
        get; set;
    }
    public bool IsItemDelete
    {
        get; set;
    }
    public bool IsSpecialDiscount
    {
        get; set;
    }
    //public List<SmeUserBrnachDTO> Branch
    //{
    //    get; set;
    //}
    //public List<SmeActivityPermisionDTO> UserRole
    //{
    //    get; set;
    //}
    public AuthDTO auth
    {
        get; set;
    }
}
public class AuthDTO
{
    public string Token
    {
        get; set;
    }
    public string RefreshToken
    {
        get; set;
    }
    public bool Success
    {
        get; set;
    }
    public int expires_in
    {
        get; set;
    }
    public string ActionTime
    {
        get; set;
    }
    public IEnumerable<string> Errors
    {
        get; set;
    }
}

 