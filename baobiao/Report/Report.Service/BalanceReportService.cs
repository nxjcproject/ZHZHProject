using Report.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Service
{
    public class BalanceReportService
    {
        public static DataTable GetReportData(string organizationId, string startDate, string endDate, decimal clinkerCoefficient, decimal cementCoefficient)
        {
            //分厂数据库
            string meterDatabaseName = "";
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string myBaseSql = @"select B.MeterDatabase
                                    from system_Organization A,system_Database B
                                    where A.DatabaseID=B.DatabaseID
                                    and A.OrganizationID=@organizationId";
            SqlParameter parameter = new SqlParameter("organizationId", organizationId);
            DataTable baseTable = dataFactory.Query(myBaseSql, parameter);
            if (baseTable.Rows.Count == 0)
            {
                throw new Exception("没有找到该组织机构对应的分厂数据库");
            }
            else
            {
                meterDatabaseName = baseTable.Rows[0]["MeterDatabase"].ToString().Trim();
            }

//            string mySql = @"select E.Name as ProductLineName,A.Name,SUM(case when B.FormulaValue is not null then B.FormulaValue when C.FormulaValue is not null then C.FormulaValue else 0 end) as FormulaValue,
//                                SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end) as DenominatorValue,
//                                (case when SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end)=0 then 0 
//                                else SUM(case when B.FormulaValue is not null then B.FormulaValue when C.FormulaValue is not null then C.FormulaValue else 0 end)
//                                /SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end) end) as ElectricityConsumption
//                                from rc_Report R inner join report_CustomizationContrast A on R.KeyID=A.KeyID left join [{0}].[dbo].[HistoryMainMachineFormulaValue] B
//                                on (A.OrganizationID=B.OrganizationID and A.VariableId=B.VariableID) left join [{0}].[dbo].[HistoryFormulaValue] C
//                                on (A.OrganizationID=C.OrganizationID and A.VariableId=C.VariableID)
//								left join (SELECT B.LevelCode,B.Name FROM rc_Report A,report_CustomizationContrast B 
//											WHERE A.KeyID=B.KeyID AND A.OrganizationID=@organizationId AND A.ReportId='BalanceReport') E
//								on LEFT(A.LevelCode,3)=E.LevelCode
//                                where (B.vDate>=@startDate
//                                and B.vDate<=@endDate
//                                or B.vDate is null)
//                                and (C.vDate>=@startDate
//                                and C.vDate<=@endDate
//                                or C.vDate is null)
//								and R.OrganizationID=@organizationId
//								and R.ReportId='BalanceReport'
//								and LEN(A.LevelCode)<>3
//                                group by A.LevelCode,A.Name,E.Name
//                                order by A.LevelCode";
            string mySql = @"select E.Name as ProductLineName,A.Name,SUM(case when B.FormulaValue is not null then B.FormulaValue when C.FormulaValue is not null then C.FormulaValue else 0 end) as FormulaValue,
                                SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end) as DenominatorValue,
                                (case when SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end)=0 then 0 
                                else SUM(case when B.FormulaValue is not null then B.FormulaValue when C.FormulaValue is not null then C.FormulaValue else 0 end)
                                /SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end) end) as ElectricityConsumption
                                from (select A.* from rc_Report R inner join report_CustomizationContrast A on R.KeyID=A.KeyID where R.OrganizationID=@organizationId and R.ReportId='BalanceReport' and LEN(A.LevelCode)<>3) as A
                                left join [{0}].[dbo].[HistoryMainMachineFormulaValue] B
                                on (A.OrganizationID=B.OrganizationID and A.VariableId=B.VariableID and(B.vDate>=@startDate and B.vDate<=@endDate)) 
                                left join [{0}].[dbo].[HistoryFormulaValue] C
                                on (A.OrganizationID=C.OrganizationID and A.VariableId=C.VariableID and (C.vDate>=@startDate and C.vDate<=@endDate))
                                left join (SELECT B.LevelCode,B.Name FROM rc_Report A,report_CustomizationContrast B 
			                                WHERE A.KeyID=B.KeyID AND A.OrganizationID=@organizationId AND A.ReportId='BalanceReport') E
                                on LEFT(A.LevelCode,3)=E.LevelCode
                                group by A.LevelCode,A.Name,E.Name
                                order by A.LevelCode";
            SqlParameter[] parameters = { new SqlParameter("startDate", startDate), new SqlParameter("endDate", endDate),
                                        new SqlParameter("organizationId",organizationId)};
            DataTable result = dataFactory.Query(string.Format(mySql, meterDatabaseName), parameters);
            
            IList<decimal> eleList = new List<decimal>();
            foreach (DataRow dr in result.Rows)
            {
                eleList.Add(Convert.ToDecimal(dr["FormulaValue"]));
            }
            result.Rows[8]["FormulaValue"] = eleList[0] + eleList[1] + eleList[2];
            result.Rows[9]["FormulaValue"] = eleList[0] + eleList[1] + eleList[2] + eleList[3];
            result.Rows[10]["FormulaValue"] = eleList[0] + eleList[1] + eleList[2] + eleList[4];
            result.Rows[eleList.Count - 1]["FormulaValue"] = eleList[0] + eleList[1] + eleList[2] + eleList[3] + eleList[4]
                + eleList[5] + eleList[11] + eleList[12] + eleList[13] + eleList[14] + eleList[15] + eleList[16] + eleList[17]
                + eleList[18] + eleList[19];
            //---综合---
            //产量
            result.Rows[8]["DenominatorValue"] = result.Rows[2]["DenominatorValue"];
            result.Rows[9]["DenominatorValue"] = result.Rows[3]["DenominatorValue"];
            result.Rows[10]["DenominatorValue"] = result.Rows[4]["DenominatorValue"];
            //电耗
            result.Rows[8]["ElectricityConsumption"] = Convert.ToDecimal(result.Rows[1]["ElectricityConsumption"]) * clinkerCoefficient + Convert.ToDecimal(result.Rows[2]["ElectricityConsumption"]);
            result.Rows[9]["ElectricityConsumption"] = Convert.ToDecimal(result.Rows[8]["ElectricityConsumption"]) * cementCoefficient + Convert.ToDecimal(result.Rows[3]["ElectricityConsumption"]);
            result.Rows[10]["ElectricityConsumption"] = Convert.ToDecimal(result.Rows[8]["ElectricityConsumption"]) * cementCoefficient + Convert.ToDecimal(result.Rows[4]["ElectricityConsumption"]);

            DataColumn col_FormulaValueB = new DataColumn("FormulaValueB", typeof(decimal));
            DataColumn col_DenominatorValueB = new DataColumn("DenominatorValueB", typeof(decimal));
            DataColumn col_ElectricityConsumptionB = new DataColumn("ElectricityConsumptionB", typeof(decimal));
            result.Columns.Add(col_FormulaValueB);
            result.Columns.Add(col_DenominatorValueB);
            result.Columns.Add(col_ElectricityConsumptionB);
            foreach (DataRow dr in result.Rows)
            {
                dr["FormulaValueB"] = dr["FormulaValue"];
                dr["DenominatorValueB"] = dr["DenominatorValue"];
                dr["ElectricityConsumptionB"] = dr["ElectricityConsumption"];
            }
            return result;
        }
    }
}
