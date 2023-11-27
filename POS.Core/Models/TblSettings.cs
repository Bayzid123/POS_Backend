using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("TblSettings")]
public class TblSettings
{
    [Key]
    public long intID
    {
        get; set;
    }
    public string AppUrl
    {
        get; set;
    }
    public string DeviceId
    {
        get; set;
    }
    public string MacAddress
    {
        get; set;
    }
    public long intAccountId
    {
        get; set;
    }
    public long intBranchId
    {
        get; set;
    }
    public long intOfficeId
    {
        get; set;
    }
    public long intWarehouseId
    {
        get; set;
    }
    public long intCounterId
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

    public string StrAccountName
    {
        set; get;
    }
    public string StrBranchName
    {
        set; get;
    }
    public string StrOfficeName
    {
        set; get;
    }
    public string StrWareHouseName
    {
        set; get;
    }
    public string StrCounterName
    {
        set; get;
    }
    public string StrCounterCode
    {
        set; get;
    }

    public string StrAddress
    {
        set; get;
    }
    public string BIN
    {
        set; get;
    }
    public string Message
    {
        get; set;
    }
}
