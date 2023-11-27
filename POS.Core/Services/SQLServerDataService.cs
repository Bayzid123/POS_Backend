
namespace POS.Core;

public class SQLServerDataService : DataServiceBase
{
    public SQLServerDataService(string connectionString)
        : base(new SQLServerDb(connectionString))
    {
         
    }
}
