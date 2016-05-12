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
    public class ProcessElectricityConsumptionService
    {
        public static DataTable GetReportData(string organizationId, string startDate, string endDate)
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

//            string mySql = @"select A.LevelCode,A.Name,SUM(case when B.FormulaValue is not null then B.FormulaValue when C.FormulaValue is not null then C.FormulaValue else 0 end) as FormulaValue,
//                                SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end) as DenominatorValue,
//                                (case when SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end)=0 then 0 
//                                else SUM(case when B.FormulaValue is not null then B.FormulaValue when C.FormulaValue is not null then C.FormulaValue else 0 end)
//                                /SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end) end) as ElectricityConsumption
//                                from rc_Report R inner join report_CustomizationContrast A on R.KeyID=A.KeyID left join [{0}].[dbo].[HistoryMainMachineFormulaValue] B
//                                on (A.OrganizationID=B.OrganizationID and A.VariableId=B.VariableID) left join [{0}].[dbo].[HistoryFormulaValue] C
//                                on (A.OrganizationID=C.OrganizationID and A.VariableId=C.VariableID)
//                                where (B.vDate>=@startDate
//                                and B.vDate<=@endDate
//                                or B.vDate is null)
//                                and (C.vDate>=@startDate
//                                and C.vDate<=@endDate
//                                or C.vDate is null)
//								and R.OrganizationID=@organizationId
//								and R.ReportId='ProcessElectricityConsumption'
//                                group by A.LevelCode,A.Name
//                                order by LevelCode";
            string mySql = @"select A.LevelCode,A.Name,SUM(case when B.FormulaValue is not null then B.FormulaValue when C.FormulaValue is not null then C.FormulaValue else 0 end) as FormulaValue,
                                SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end) as DenominatorValue,
                                (case when SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end)=0 then 0 
                                else SUM(case when B.FormulaValue is not null then B.FormulaValue when C.FormulaValue is not null then C.FormulaValue else 0 end)
                                /SUM(case when B.DenominatorValue is not null then B.DenominatorValue when C.DenominatorValue is not null then C.DenominatorValue else 0 end) end) as ElectricityConsumption
                                from  rc_Report R inner join report_CustomizationContrast A on R.KeyID=A.KeyID
                                left join [{0}].[dbo].[HistoryMainMachineFormulaValue] B
                                on (A.OrganizationID=B.OrganizationID and A.VariableId=B.VariableID and B.vDate>='2015-07-23 17:18:15.000' and B.vDate<='2015-07-24 17:18:15.000') 
                                left join [{0}].[dbo].[HistoryFormulaValue] C
                                on (A.OrganizationID=C.OrganizationID and A.VariableId=C.VariableID and C.vDate>='2015-07-23 17:18:15.000' and C.vDate<='2015-07-24 17:18:15.000')
                                where R.OrganizationID=@organizationId and R.ReportId='ProcessElectricityConsumption'
                                group by A.LevelCode,A.Name
                                order by LevelCode";
            SqlParameter[] parameters = { new SqlParameter("startDate", startDate), new SqlParameter("endDate", endDate),
                                        new SqlParameter("organizationId",organizationId)};
            DataTable result = dataFactory.Query(string.Format(mySql, meterDatabaseName), parameters);
            string ammeterSql = @"select sum(A043Energy) as A043Energy,sum(A044Energy) as A044Energy,sum(A045Energy) as A045Energy,sum(A046Energy) as A046Energy,
                                    sum(A047Energy) as A047Energy,sum(A048Energy) as A048Energy
                                    from [{0}].[dbo].[HistoryAmmeterIncrement] A
                                    where A.vDate>=@startDate
                                    and A.vDate<=@endDate";
            SqlParameter[] ammeterParameters = { new SqlParameter("startDate", startDate), new SqlParameter("endDate", endDate) };
            DataTable ammeterTable = dataFactory.Query(string.Format(ammeterSql, meterDatabaseName), ammeterParameters);
            if (ammeterTable.Rows.Count == 1)
            {
                result.Rows[4]["FormulaValue"] = ammeterTable.Rows[0]["A046Energy"];//1#原料磨
                result.Rows[5]["FormulaValue"] = ammeterTable.Rows[0]["A047Energy"];//2#原料磨
                result.Rows[6]["FormulaValue"] = ammeterTable.Rows[0]["A045Energy"];//烧成窑头
                result.Rows[7]["FormulaValue"] = ammeterTable.Rows[0]["A043Energy"];//1#水泥磨
                result.Rows[8]["FormulaValue"] = ammeterTable.Rows[0]["A044Energy"];//2#水泥磨
            }
            return result;
        }

        public static DataTable GetData(string organizationId, string startDate, string endDate)
        {
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
            string mySql = @"select A.LevelCode,A.Name,sum(ISNULL(B.TotalPeakValleyFlatB,0)) as FormulaValue,A.Denominator as DenominatorName
                                from (select B.* from rc_Report A,report_CustomizationContrast B 
		                                where A.KeyID=B.keyID and A.OrganizationID=@organizationId and A.ReportId='ProcessElectricityConsumption') A
                                left join (select B.* from tz_Balance A,balance_Energy B 
			                                where A.BalanceId=B.KeyID and A.StaticsCycle='day' and A.OrganizationID='zc_zcshn_zhuzhouc_zhuzhouf' and A.TimeStamp>=@startDate and A.TimeStamp<=@endDate) B
                                on A.OrganizationID=B.OrganizationID and A.VariableId+'_ElectricityQuantity'=B.VariableId
                                group by A.OrganizationID,A.VariableId,A.Name,A.Denominator,A.LevelCode
                                order by A.LevelCode";
            SqlParameter[] parameters = { new SqlParameter("startDate", startDate), new SqlParameter("endDate", endDate),
                                        new SqlParameter("organizationId",organizationId)};
            DataTable result = dataFactory.Query(string.Format(mySql, meterDatabaseName), parameters);
            IDictionary<string, decimal> materialDict = new Dictionary<string, decimal>();
            StringBuilder materialBuilder = new StringBuilder();
            foreach (DataRow dr in result.Rows)
            {
                string label = dr["DenominatorName"].ToString().Trim();
                if (materialDict.Keys.Contains(label) ||label=="")
                {
                    continue;
                }
                else
                {
                    materialDict.Add(label, 0);
                    materialBuilder.Append("sum(" + label + ") as " + label);
                    materialBuilder.Append(",");
                }
            }
            materialBuilder.Remove(materialBuilder.Length - 1, 1);
            string materialSql = @"select  {0}
                                    from [{1}].[dbo].[HistoryDCSIncrement] A
                                    where convert(varchar(10),A.vDate,20)>=@startDate
                                    and convert(varchar(10),A.vDate,20)<=@endDate";
            SqlParameter[] materialParameter = { new SqlParameter("startDate", startDate), 
                                             new SqlParameter("endDate", endDate)};
            DataTable materialTable = dataFactory.Query(string.Format(materialSql, materialBuilder.ToString(), meterDatabaseName),
                materialParameter);
            foreach (DataColumn dc in materialTable.Columns)
            {
                string t_label = dc.ColumnName;
                if (materialDict.Keys.Contains(t_label))
                {
                    materialDict[t_label] =Convert.ToDecimal(materialTable.Rows[0][t_label]);
                }
            }
            DataColumn denominatorColumn = new DataColumn("DenominatorValue", typeof(decimal));
            denominatorColumn.DefaultValue = 0;
            DataColumn electricityConsumptionColumn = new DataColumn("ElectricityConsumption", typeof(decimal));
            electricityConsumptionColumn.DefaultValue = 0;
            result.Columns.Add(denominatorColumn);
            result.Columns.Add(electricityConsumptionColumn);
            foreach (DataRow dr in result.Rows)
            {
                string t_label = dr["DenominatorName"].ToString().Trim();
                if (materialDict.Keys.Contains(t_label))
                {
                    dr["DenominatorValue"] = materialDict[t_label];
                    dr["ElectricityConsumption"] = materialDict[t_label] == 0 ? 0 : Convert.ToDecimal(dr["FormulaValue"]) / materialDict[t_label];
                }
            }
            result.Columns.Remove("DenominatorName");
            return result;
        }
    }
}
