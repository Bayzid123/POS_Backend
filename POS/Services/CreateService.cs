using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.ViewModels.CounterSession;
using POS.Core.ViewModels;
using POS.Core.Models;

namespace POS.Services;
public class CreateService : ICreateService
{
    readonly IDataServiceFactory _context;
    public CreateService(IDataServiceFactory context)
    {
        _context = context;
    }
    public async Task<MessageHelper> CreateCountersSession(CreateCounterSeason create)
    {
        using var dataService = _context.CreateDataService();
        var dt = await dataService.CreatePosCounterSessionAsync(create);
        return dt;
    }
    public async Task CreatedataLog(tblDataLog log)
    {
        using var dataSerice = _context.CreateDataService();
        await  dataSerice.CreatePosLog(log);
    }
    public async Task<MessageHelper> DeleteRecallInvoice(long SalesOrderId,string SalesOrderCode)
    {
        using var dataSourch = _context.CreateDataService();
        var msg = await dataSourch.DeleteRecallInvoice(SalesOrderId, SalesOrderCode);
        return msg;
    }
}
