using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.ViewModels.CounterSession;
using POS.Core.ViewModels;
using POS.Core.Models;

namespace POS.Services;
public interface ICreateService
{
    Task<MessageHelper> CreateCountersSession(CreateCounterSeason create);
    Task CreatedataLog(tblDataLog log);
    Task<MessageHelper> DeleteRecallInvoice(long SalesOrderId, string SalesOrderCode);
}
