using Report.Service;
using StatisticalReport.Service.StatisticalReportServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Report.Web.UI_Report
{
    public partial class ProcessElectricityConsumption : WebStyleBaseForEnergy.webStyleBase
    {
        private const string REPORT_TEMPLATE_PATH = "\\ReportHeaderTemplate\\ProcessElectricityConsumption.xml";
        private static DataTable m_DataGridDataStruct;
        private static DataTable m_DataGridData = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_zcshn_zhuzhouc_zhuzhouf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");
                //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.PageName = "BalanceReport.aspx";
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }

            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                DataTable myTable = m_DataGridData.Copy();
                myTable.Columns.Remove("LevelCode");
                //ExportFile("xls", "导出报表1.xls");
                string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
                string m_HtmlData = StatisticalReportHelper.CreateExportHtmlTable(mFileRootPath +
                    REPORT_TEMPLATE_PATH, myTable, m_TagData);
                StatisticalReportHelper.ExportExcelFile("xls", "工序能耗总查询报表.xls", m_HtmlData);
            }
        }

        [WebMethod]
        public static string GetData(string organizationId, string startDate, string endDate)
        {
            DataTable table = ProcessElectricityConsumptionService.GetData(organizationId, startDate, endDate);
            m_DataGridData = table;
            m_DataGridDataStruct = table.Clone();
            string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
            return json;
        }

        /// <summary>
        /// 获得报表数据并转换为json
        /// </summary>
        /// <returns>column的json字符串</returns>
        [WebMethod]
        public static string PrintFile(string myDataGridData)
        {
            string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
            string[] m_DataGridDataGroup = EasyUIJsonParser.Utility.JsonPickArray(myDataGridData, "rows");
            m_DataGridData = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(m_DataGridDataGroup, m_DataGridDataStruct);
            string m_HtmlData = StatisticalReportHelper.CreatePrintHtmlTable(mFileRootPath +
                REPORT_TEMPLATE_PATH, m_DataGridData, m_TagData);
            return m_HtmlData;
        }
        [WebMethod]
        public static void InitDataGridData(string myDataGridData)
        {
            string[] m_DataGridDataGroup = EasyUIJsonParser.Utility.JsonPickArray(myDataGridData, "rows");
            m_DataGridData.Clear();
            m_DataGridData = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(m_DataGridDataGroup, m_DataGridDataStruct);
        }
    }
}