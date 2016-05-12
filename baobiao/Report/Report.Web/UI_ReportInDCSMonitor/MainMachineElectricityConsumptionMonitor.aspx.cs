using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;

namespace Report.Web.UI_ReportInDCSMonitor
{
    public partial class MainMachineElectricityConsumptionMonitor : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
            }
        }
        [WebMethod]
        public static string GetElectricityConsumption()
        {
            string m_OrganizationId = "zc_zcshn_zhuzhouc_zhuzhouf";             //"zc_nxjc_byc_byf";
            string m_TodayDate = DateTime.Now.ToString("yyyy-MM-dd");
            string m_YesterdayDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            DataTable table = Report.Service.MainMachineElectricityConsumptionService.GetElectricityConsumption(m_OrganizationId, m_YesterdayDate, m_TodayDate);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
    }
}