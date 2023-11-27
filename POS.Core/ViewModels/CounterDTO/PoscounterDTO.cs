using System;
namespace POS.Core.ViewModels.CounterDTO;
public class PoscounterDTO
{
    public long CounterId
    {
        get; set;
    }
    public string CounterName
    {
        get; set;
    }
    public string CounterCode
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
    public long OfficeId
    {
        get; set;
    }
    public long WarehouseId
    {
        get; set;
    }
    public DateTime CounterOpeningDate
    {
        get; set;
    }
    public DateTime? CounterClosingDate
    {
        get; set;
    }
    public long ActionById
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    }
    public DateTime LastActionDatetime
    {
        get; set;
    }
    public DateTime ServerDatetime
    {
        get; set;
    }

}
public class CreatePoscounterDTO
{
    public long CounterId
    {
        get; set;
    }
    public string CounterName
    {
        get; set;
    }
    public string CounterCode
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
    public long OfficeId
    {
        get; set;
    }
    public long WarehouseId
    {
        get; set;
    }
    public DateTime CounterOpeningDate
    {
        get; set;
    }
    public long ActionById
    {
        get; set;
    }
}
public class EditPoscounterDTO
{
    public long CounterId
    {
        get; set;
    }
    public string CounterName
    {
        get; set;
    }
    public long AccountId
    {
        get; set;
    }
    public long ActionById
    {
        get; set;
    }
}
public class GetPoscounterDTO
{
    public long Sl
    {
        get; set;
    }
    public long CounterId
    {
        get; set;
    }
    public string CounterName
    {
        get; set;
    }
    public string CounterCode
    {
        get; set;
    }
    public long AccountId
    {
        get; set;
    }
    public string AccountName
    {
        get; set;
    }
    public long BranchId
    {
        get; set;
    }
    public string BranchName
    {
        get; set;
    }
    public long OfficeId
    {
        get; set;
    }
    public string OfficeName
    {
        get; set;
    }
    public long WarehouseId
    {
        get; set;
    }
    public string WarehouseName
    {
        get; set;
    }
    public long ActionById
    {
        get; set;
    }
    public string ActionByName
    {
        get; set;
    }
    public bool IsActive
    {
        get; set;
    }
    public DateTime LastActionDatetime
    {
        get; set;
    }
}

