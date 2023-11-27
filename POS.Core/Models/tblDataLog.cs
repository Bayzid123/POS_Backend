using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("tblDataLog")]
public class tblDataLog
{
    [Key]
    public long LogId
    {
        get; set;
    }
    public long AccountId
    {
        get; set;
    }
    public long BranchId
    {
        get; set;
    }
    public long? OfficeId
    {
        get; set;
    }
    public long? CounterId
    {
        get; set;
    }
    public long? ServerUserId
    {
        get; set;
    }
    public string strLogMessage
    {
        get; set;
    }
    public string strEntityData
    {
        get; set;
    }
    public string LogType
    {
        get; set;
    }
    public DateTime? LastActionDateTime
    {
        get; set;
    }
    public DateTime ServerDateTime { get; set; } = DateTime.Now;
    public int? IsSync
    {
        get; set;
    }
}

[Table("tblDataLog", Schema = "pos")]
public class SQLtblDataLog
{
    [Key]
    public long LogId
    {
        get; set;
    }
    public long AccountId
    {
        get; set;
    }
    public long BranchId
    {
        get; set;
    }
    public long? OfficeId
    {
        get; set;
    }
    public long? CounterId
    {
        get; set;
    }
    public long? ServerUserId
    {
        get; set;
    }
    public string strLogMessage
    {
        get; set;
    }
    public string strEntityData
    {
        get; set;
    }
    public string LogType
    {
        get; set;
    }
    public DateTime? LastActionDateTime
    {
        get; set;
    }
    public DateTime ServerDateTime { get; set; } = DateTime.Now;
    public int? IsSync
    {
        get; set;
    }
}