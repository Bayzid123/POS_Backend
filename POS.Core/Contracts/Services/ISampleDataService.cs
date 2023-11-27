using POS.Core.Models;
using POS.Core.ViewModels;

namespace POS.Core.Contracts.Services;

// Remove this class once your pages/features are using your data.
public interface ISampleDataService
{
    Task<IEnumerable<SampleOrder>> GetGridDataAsync();
    Task<IList<SampleOrderDetail>> GetGridItemAsync();
    Task<IList<SampleCustomer>> GetSampleCustomerAsync();
    Task<IList<OfficeList>> GetOfficeListAsync();
}
