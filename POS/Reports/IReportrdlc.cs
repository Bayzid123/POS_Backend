using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Reports;
public interface IReportrdlc
{
    string GetReportPDF(int branchId, string dataSetName, DataTable dt, string path, string reportPath, string reportName);
    byte[] GetReportXLSX(int branchId, string dataSetName, DataTable dt, string path, string reportPath, string reportName);
    byte[] GetReportXLSX(int branchId, string dataSetName, DataTable dt, string path, string reportPath, string reportName, Dictionary<string, object> reportParameters = null);
    string GetReportView(int branchId, string dataSetName, DataTable dt, string path, string reportPath, string reportName);
    string GetReportView(int branchId, string dataSetName, DataTable dt, string path, string reportPath, string reportName, Dictionary<string, object> reportParameters = null);
    string GetReportView(int branchId, List<string> dataSetList, List<DataTable> dtList, string path, string reportPath, string reportName, Dictionary<string, object> reportParameters = null);
    byte[] GetReportXLSX(int branchId, List<string> dataSetList, List<DataTable> dtList, string path, string reportPath, string reportName, Dictionary<string, object> reportParameters = null);
}