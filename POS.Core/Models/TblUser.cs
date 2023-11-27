using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace POS.Core.Models;
[Table("TblUser")]
public class TblUser
{
    [Key]
    public long intUserID
    {
        get; set;
    }
    public long ServerUserID
    {
        get; set;
    }
    public string strUserName
    {
        get; set;
    }
    public bool bolIsPOSAdmin
    {
        get; set;
    }
    public string strUserJWTToken
    {
        get; set;
    }
    public long intAccountId
    {
        get; set;
    }
    public string intBusinessUnitId
    {
        get; set;
    }
    public string intOfficeId
    {
        get; set;
    }
    public long intEmpoyeeId
    {
        get; set;
    }
    public string strEmployeeName
    {
        get; set;
    }
    public string strPassword
    {
        get; set;
    }
    public bool IsAdministration
    {
        get; set;
    }
    public DateTime LastDateTime
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
}
[Table("User", Schema = "dbo")]
public class User
{
    public long UserId
    {
        get; set;
    }
    public string UserName
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
    public string Password
    {
        get; set;
    }
    public string ConfirmPassword
    {
        get; set;
    }
    public string Email
    {
        get; set;
    }
    public string OldPassword
    {
        get; set;
    }
    public bool IsMasterUser
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    }
    public DateTime? ActionTime
    {
        get; set;
    }
    public string ActionByName
    {
        get; set;
    }
    public long? ActionById
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
}
