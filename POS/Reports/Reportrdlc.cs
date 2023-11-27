using System.Data;
using System.Drawing.Printing;
using Microsoft.Reporting.WinForms;

namespace POS.Reports;
public class Reportrdlc : IReportrdlc
{
    public string GetReportPDF(int branchId, string dataSetName, DataTable dt, string path, string reportPath,
        string reportName)
    {
        var rdlcRv = new ReportViewer();
        rdlcRv.Reset();
        rdlcRv.LocalReport.DataSources.Clear();
        rdlcRv.ProcessingMode = ProcessingMode.Local;
        rdlcRv.LocalReport.ReportPath = reportPath + reportName;
        var rdc = new ReportDataSource(dataSetName, dt);
        rdlcRv.LocalReport.DataSources.Clear();
        rdlcRv.LocalReport.DataSources.Add(rdc);
        rdlcRv.LocalReport.Refresh();
        var bytes = rdlcRv.LocalReport.Render("PDF", "");

        using (var stream = new FileStream(path + reportName + "_" + DateTime.Now.ToString("yyyyMMdd") +
                                           DateTime.Now.ToString("HHmmss") + "_" + branchId + ".pdf",
            FileMode.Create))
            stream.Write(bytes, 0, bytes.Length);
        LocalReportExtensions.PrintToPrinter(rdlcRv.LocalReport);
     

        return path + reportName + "_" + DateTime.Now.ToString("yyyyMMdd") +
               DateTime.Now.ToString("HHmmss") + "_" + branchId + ".pdf";
    }

    public byte[] GetReportXLSX(int branchId, string dataSetName, DataTable dt, string path, string reportPath,
        string reportName) =>
        GetReportXLSX(branchId, dataSetName, dt, path, reportPath, reportName, null);

    public byte[] GetReportXLSX(int branchId, string dataSetName, DataTable dt, string path, string reportPath,
        string reportName, Dictionary<string, object> reportParameters = null)
    {
        var rdlcRv = new ReportViewer();
        rdlcRv.Reset();
        rdlcRv.LocalReport.DataSources.Clear();
        rdlcRv.ProcessingMode = ProcessingMode.Local;
        rdlcRv.LocalReport.ReportPath = reportPath + reportName;
        var rdc = new ReportDataSource(dataSetName, dt);
        var reportParamCollection = new List<ReportParameter>();
        if (reportParameters != null)
        {
            reportParamCollection.AddRange(reportParameters.Select(parameter =>
                new ReportParameter(parameter.Key, parameter.Value?.ToString())));
            rdlcRv.LocalReport.SetParameters(reportParamCollection);
        }

        rdlcRv.LocalReport.DataSources.Clear();
        rdlcRv.LocalReport.DataSources.Add(rdc);
        rdlcRv.LocalReport.Refresh();
        var bytes = rdlcRv.LocalReport.Render("EXCELOPENXML", "");

        return bytes;
    }

    public string GetReportView(int branchId, string dataSetName, DataTable dt, string path, string reportPath,
        string reportName) =>
        GetReportView(branchId, dataSetName, dt, path, reportPath, reportName, null);

    public string GetReportView(int branchId, string dataSetName, DataTable dt, string path, string reportPath,
        string reportName, Dictionary<string, object> reportParameters = null)
    {
        var rdlcRv = new ReportViewer();
        rdlcRv.Reset();
        rdlcRv.LocalReport.DataSources.Clear();
        rdlcRv.ProcessingMode = ProcessingMode.Local;
        rdlcRv.LocalReport.ReportPath = reportPath + reportName;
        var rdc = new ReportDataSource(dataSetName, dt);

        var reportParamCollection = new List<ReportParameter>();
        if (reportParameters != null)
        {
            reportParamCollection.AddRange(reportParameters.Select(parameter =>
                new ReportParameter(parameter.Key, parameter.Value?.ToString())));
            rdlcRv.LocalReport.SetParameters(reportParamCollection);
        }

        rdlcRv.LocalReport.DataSources.Clear();
        rdlcRv.LocalReport.DataSources.Add(rdc);
        rdlcRv.LocalReport.Refresh();
        var bytes = rdlcRv.LocalReport.Render("PDF", "");
        var docBase64 = Convert.ToBase64String(bytes);
        return docBase64;
    }

    public string GetReportView(int branchId, List<string> dataSetList, List<DataTable> dtList, string path, string reportPath,
        string reportName, Dictionary<string, object> reportParameters = null)

    {
        var rdlcRv = new ReportViewer();
        rdlcRv.Reset();
        rdlcRv.LocalReport.DataSources.Clear();
        rdlcRv.ProcessingMode = ProcessingMode.Local;
        rdlcRv.LocalReport.ReportPath = reportPath + reportName;

        var reportParamCollection = new List<ReportParameter>();
        if (reportParameters != null)
        {
            reportParamCollection.AddRange(reportParameters.Select(parameter =>
                new ReportParameter(parameter.Key, parameter.Value?.ToString())));
            rdlcRv.LocalReport.SetParameters(reportParamCollection);
        }
        rdlcRv.LocalReport.DataSources.Clear();
        int i = 0;
        dtList.ForEach(dt =>
        {
            var rdc = new ReportDataSource(dataSetList[i], dt);
            rdlcRv.LocalReport.DataSources.Add(rdc);
            i++;
        });
        rdlcRv.LocalReport.Refresh();
        var bytes = rdlcRv.LocalReport.Render("PDF", "");
        var docBase64 = Convert.ToBase64String(bytes);
        return docBase64;
    }

    public byte[] GetReportXLSX(int branchId, List<string> dataSetList, List<DataTable> dtList, string path, string reportPath,
        string reportName, Dictionary<string, object> reportParameters = null)
    {
        var rdlcRv = new ReportViewer();
        rdlcRv.Reset();
        rdlcRv.LocalReport.DataSources.Clear();
        rdlcRv.ProcessingMode = ProcessingMode.Local;
        rdlcRv.LocalReport.ReportPath = reportPath + reportName;
        // var rdc = new ReportDataSource(dataSetName, dt);

        var reportParamCollection = new List<ReportParameter>();
        if (reportParameters != null)
        {
            reportParamCollection.AddRange(reportParameters.Select(parameter =>
                new ReportParameter(parameter.Key, parameter.Value?.ToString())));
            rdlcRv.LocalReport.SetParameters(reportParamCollection);
        }

        rdlcRv.LocalReport.DataSources.Clear();
        int i = 0;
        dtList.ForEach(dt =>
        {
            var rdc = new ReportDataSource(dataSetList[i], dt);
            rdlcRv.LocalReport.DataSources.Add(rdc);
            i++;
        });
        // rdlcRv.LocalReport.DataSources.Add(rdc);
        rdlcRv.LocalReport.Refresh();
        var bytes = rdlcRv.LocalReport.Render("EXCELOPENXML", "");

        return bytes;
    }
}
