using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.ViewModels;

namespace POS.Services;
public interface IInvoicePrint
{
    void GenerateInvoice(List<InvoiceModelDTO> invoiceList);
    void GenerateReprintInvoice(List<InvoiceModelDTO> invoiceList);
    void GenerateOnlyInvoice();
}
