using POS.Core.IService;

namespace POS.Services;

public interface IDataServiceFactory
{
    IDataService CreateDataService();

    IDataService CreateSQLDataService();
}
