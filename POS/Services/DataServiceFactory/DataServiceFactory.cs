

using POS.Core;
using POS.Core.IService;

namespace POS.Services;

public class DataServiceFactory : IDataServiceFactory
{
    private static Random _random = new Random(0);

    public IDataService CreateDataService()
    {
        if (AppSettings.Current.IsRandomErrorsEnabled)
        {
            if (_random.Next(20) == 0)
            {
                throw new InvalidOperationException("Random error simulation");
            }
        }

        switch (AppSettings.Current.DataProvider)
        {
            case DataProviderType.SQLite:
                return new SQLiteDataService(AppSettings.Current.SQLiteConnectionString);

            case DataProviderType.SQLServer:
                return new SQLServerDataService(AppSettings.SQLServerConnectionString);

            default:
                throw new NotImplementedException();
        }
    }

    public IDataService CreateSQLDataService()
    {
        if (AppSettings.Current.IsRandomErrorsEnabled)
        {
            if (_random.Next(20) == 0)
            {
                throw new InvalidOperationException("Random error simulation");
            }
        }

        return new SQLServerDataService(AppSettings.SQLServerConnectionString);
    }
}
