using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarcodeLib;
using POS.Core.ViewModels;
using POS.Reports;
using Windows.Data.Pdf;
using Windows.Storage;

namespace POS.Services;
public class InvoicePrint : IInvoicePrint
{
    private readonly IReportrdlc _Report;
    public InvoicePrint(IReportrdlc Report)
    {
        _Report = Report;
    }
    public void GenerateInvoice(List<InvoiceModelDTO> invoiceList)
    {
        BarCodeImage bcImage = new BarCodeImage();
        byte[] image = bcImage.GetImage(invoiceList.FirstOrDefault().InvoiceNo, false);
        invoiceList = invoiceList.Select(s => { s.BarCodeImage = image; return s; }).ToList();
        var dt = ListExtensions.ToDataTable(invoiceList);
        var data = _Report.GetReportPDF(0, "rptInvoice", dt, ApplicationData.Current.LocalFolder.Path+"/"+AppSettings.ReportPath + "/" , ApplicationData.Current.LocalFolder.Path + "/" + AppSettings.ReportPath + "/", "rptInvoice.rdlc");
    
    }
    public void GenerateReprintInvoice(List<InvoiceModelDTO> invoiceList)
    {
        BarCodeImage bcImage = new BarCodeImage();
        byte[] image = bcImage.GetImage(invoiceList.FirstOrDefault().InvoiceNo, false);
        invoiceList = invoiceList.Select(s => { s.BarCodeImage = image; return s; }).ToList();
        var dt = ListExtensions.ToDataTable(invoiceList);
        var data = _Report.GetReportPDF(0, "rptInvoice", dt, ApplicationData.Current.LocalFolder.Path + "/" + AppSettings.ReportPath + "/", ApplicationData.Current.LocalFolder.Path + "/" + AppSettings.ReportPath + "/", "rptInvoiceRePrint.rdlc");

    }

    public void GenerateOnlyInvoice()
    {
        var dt = new DataTable();
        var data = _Report.GetReportPDF(0, "rptInvoice", dt, ApplicationData.Current.LocalFolder.Path + "/" + AppSettings.ReportPath + "/", ApplicationData.Current.LocalFolder.Path + "/" + AppSettings.ReportPath + "/", "rptPrint.rdlc");

    }
}
