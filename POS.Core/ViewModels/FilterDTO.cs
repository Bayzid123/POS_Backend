namespace POS.Core.ViewModels;

public class AccountList
{
    public long AccountId { get; set; }
    public string AccountName { get; set; }
}
public class BranchList
{
    public long AccountId { get; set; }
    public long BranchId { get; set; }
    public string BranchName { get; set; }
    public string BIN
    {
        get; set;
    }

    
}
public class OfficeLists
{
    public long CounterSessionId
    {
        get; set;
    }
    public long AccountId { get; set; }
    public long BranchId { get; set; }
    public long OfficeId { get; set; }
    public string OfficeName { get; set; }
    public string Address
    {
        get; set;
    }
    
    public List<CounterList> counterLists
    {
        get;set;
    }
    public CounterList selectedCounter
    {
        get;set;
    }
    public string strStatus
    {
        get; set;
    }
     
}
public class WarehouseList
{
    public long AccountId { get; set; }
    public long BranchId { get; set; }
    public long OfficeId { get; set; }
    public long WarehouseId { get; set; }
    public string WarehouseName { get; set; }
}
public class CounterList
{
    public long AccountId { get; set; }
    public long BranchId { get; set; }
    public long OfficeId { get; set; }
    public long WarehouseId { get; set; }
    public long CounterId { get; set; }
    public string CounterName { get; set; }
    public string CounterCode {set;get;}
    public string Message
    {
        set; get;
    }
    
}
public class AllDDl
{
    public List<AccountList> accountList
    {
    get; set; }
    public List<BranchList>branchList
    {
    get; set; }
    public List<OfficeLists> officeList
    {
        get; set;
    }
    public List<WarehouseList> warehouseList
    {
        get;set;
    }
    public List<CounterList> counterList
    {
        get; set;
    }

}
public class OfficeDetails
{
    public List<OfficeLists> officeLists
    {
        get; set;
    }
    public List<CounterList> counterList
    {
        get; set;
    }

}
