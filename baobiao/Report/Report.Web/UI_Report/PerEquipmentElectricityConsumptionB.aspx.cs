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
    public partial class PerEquipmentElectricityConsumptionB : WebStyleBaseForEnergy.webStyleBase
    {
        private const string REPORT_TEMPLATE_PATH = "\\ReportHeaderTemplate\\PerEquipmentElectricityConsumptionB.xml";
        private static DataTable myDataTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();

            ////////////////////调试用,自定义的数据授权
#if DEBUG
            List<string> m_DataValidIdItems = new List<string>() { "zc_zcshn_zhuzhouc_zhuzhouf" };
            AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
            this.OrganisationTree_ProductionLine.PageName = "PerEquipmentElectricityConsumptionB.aspx";                                     //向web用户控件传递当前调用的页面名称
            this.OrganisationTree_ProductionLine.LeveDepth = 5;

            if (!IsPostBack)
            {

            }

            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                //ExportFile("xls", "导出报表1.xls");
                string[] m_TagData = new string[] { "10月份", "报表类型:日报表", "汇总人:某某某", "审批人:某某某" };
                string m_HtmlData = StatisticalReportHelper.CreateExportHtmlTable(mFileRootPath +
                    REPORT_TEMPLATE_PATH, myDataTable, m_TagData);
                StatisticalReportHelper.ExportExcelFile("xls", "低压设备单机电耗统计报表.xls", m_HtmlData);
            }
        }

        [WebMethod]
        public static string GetReportData(string organizationId, string startDate, string endDate)
        {
            myDataTable = PerEquipmentElectricityConsumptionServiceB.GetEquipmentElectricityConsumption(organizationId, startDate, endDate);
            string m_UserInfoJson = StatisticalReportHelper.ReadReportHeaderFile(mFileRootPath +
                REPORT_TEMPLATE_PATH, myDataTable);
            return m_UserInfoJson;
        }
    }
}