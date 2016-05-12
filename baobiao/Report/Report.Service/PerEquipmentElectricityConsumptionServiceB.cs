using Report.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Report.Service
{
    public class PerEquipmentElectricityConsumptionServiceB
    {

        public static DataTable GetEquipmentElectricityConsumption(string organizationId, string startDate, string endDate)
        {
            string meterDatabaseName = "";
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string myBaseSql = @"select B.MeterDatabase
                                    from system_Organization A,system_Database B
                                    where A.DatabaseID=B.DatabaseID
                                    and A.OrganizationID=@organizationId";
            SqlParameter parameter = new SqlParameter("organizationId", organizationId);
            DataTable DBTable = dataFactory.Query(myBaseSql, parameter);
            if (DBTable.Rows.Count == 0)
            {
                throw new Exception("没有找到该组织机构对应的分厂数据库");
            }
            else
            {
                meterDatabaseName = DBTable.Rows[0]["MeterDatabase"].ToString().Trim();
            }
            //主要结构
            string structSql = @"select B.VariableId,B.Name,B.OrganizationID,B.Denominator,B.LoopCode
                                    from rc_Report A,report_CustomizationContrastB B
                                    where A.KeyID=B.KeyID
                                    and A.OrganizationID=@organizationId
                                    and A.ReportId='PerEquipmentElectricityConsumption'
                                    order by B.LevelCode";

            SqlParameter structParameter = new SqlParameter("organizationId", organizationId);
            DataTable structTable = dataFactory.Query(structSql, structParameter);
            //物料数据
            IDictionary<string, decimal> materialDict = new Dictionary<string, decimal>();
            StringBuilder materialBuilder = new StringBuilder();
            foreach (DataRow dr in structTable.Rows)
            {
                string label = dr["Denominator"].ToString().Trim();
                if (materialDict.Keys.Contains(label))
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
            string materialSql = @"select convert(varchar(10),A.vDate,20) as vDate,{0}
                                    from [{1}].[dbo].[HistoryDCSIncrement] A
                                    where convert(varchar(10),A.vDate,20)>=@startDate
                                    and convert(varchar(10),A.vDate,20)<=@endDate
                                    group by convert(varchar(10),A.vDate,20)
                                    order by convert(varchar(10),A.vDate,20)";
            SqlParameter[] materialParameter = { new SqlParameter("startDate", startDate), 
                                             new SqlParameter("endDate", endDate)};
            DataTable materialTable = dataFactory.Query(string.Format(materialSql, materialBuilder.ToString(), meterDatabaseName),
                materialParameter);
            IDictionary<string, Dictionary<string, decimal>> materialValue = new Dictionary<string, Dictionary<string, decimal>>();
            foreach (DataRow dr in materialTable.Rows)
            {
                Dictionary<string, decimal> chlidrenDic = new Dictionary<string, decimal>();
                foreach (DataColumn dc in materialTable.Columns)
                {
                    string t_label = dc.ColumnName;
                    if (t_label != "vDate")
                    {
                        chlidrenDic.Add(t_label, Convert.ToDecimal(dr[t_label]));
                    }
                }
                materialValue.Add(dr["vDate"].ToString().Trim(), chlidrenDic);
            }
         
            //基本数据
            string baseSql = @"select B.TimeStamp,A.Name,B.TotalPeakValleyFlatB,A.VariableId,B.OrganizationID,A.Denominator,A.LoopCode
                                from(select B.* from rc_Report A, report_CustomizationContrastB B where A.KeyID=B.KeyID and A.OrganizationID=@organizationId and A.ReportId='PerEquipmentElectricityConsumption') A,
                                (select A.TimeStamp, B.* from tz_Balance A,balance_Energy B where A.BalanceId=B.KeyId and A.StaticsCycle='day' and A.TimeStamp>=@startDate and A.TimeStamp<=@endDate) B
                                where A.OrganizationID=B.OrganizationID
                                and A.VariableId+'_ElectricityQuantity'=B.VariableId
                                order by B.TimeStamp";
            SqlParameter[] baseParameters ={new SqlParameter("organizationId",organizationId),
                                          new SqlParameter("startDate",startDate),
                                          new SqlParameter("endDate",endDate)};
            DataTable baseTable = dataFactory.Query(baseSql, baseParameters);

            //构造最终表
            DataTable result = new DataTable();
            DataColumn dateColumn = new DataColumn("vDate", typeof(string));
            result.Columns.Add(dateColumn);
            string loopcodestr = "";
            foreach (DataRow dr in structTable.Rows)
            {
                string c_organizationId = dr["OrganizationID"].ToString().Trim();    
                string c_variableId = dr["VariableId"].ToString().Trim();
                DataColumn column;
                if (loopcodestr != "" && loopcodestr != dr["LoopCode"].ToString().Trim())
                {
                    column = new DataColumn(loopcodestr + "Total", typeof(decimal));
                    column.DefaultValue = 0;
                    result.Columns.Add(column);
                }
                loopcodestr = dr["LoopCode"].ToString().Trim();
                column = new DataColumn(c_organizationId + c_variableId, typeof(decimal));
                column.DefaultValue = 0;
                result.Columns.Add(column);
            }
            DataColumn colu = new DataColumn(loopcodestr + "Total", typeof(decimal));
            colu.DefaultValue = 0;
            result.Columns.Add(colu);
            DateTime t_startDate = DateTime.Parse(startDate);
            DateTime t_endDate = DateTime.Parse(endDate);
            DataRow TotalPeakValleyFlatB = result.NewRow();//电量总累计行
            DataRow TotalmaterialValue = result.NewRow();//产量总累计行
            for (; t_startDate <= t_endDate; t_startDate = t_startDate.AddDays(1))
            {
                string currentDate = t_startDate.ToString("yyyy-MM-dd");
                DataRow newRow = result.NewRow();
                newRow["vDate"] = currentDate;
                DataRow[] rows = baseTable.Select("TimeStamp='" + currentDate + "'");
                foreach (DataRow dr in rows)
                {
                    string c_organizationId = dr["OrganizationID"].ToString().Trim();
                    string variableId = dr["VariableId"].ToString().Trim();
                    string materialLabel = dr["Denominator"].ToString().Trim();
                    string LoopCodestr = dr["LoopCode"].ToString().Trim();
                    if (materialValue.Keys.Contains(currentDate))
                    {
                        TotalPeakValleyFlatB[c_organizationId + variableId] = decimal.Parse(TotalPeakValleyFlatB[c_organizationId + variableId].ToString()) + decimal.Parse(dr["TotalPeakValleyFlatB"].ToString());
                        TotalmaterialValue[c_organizationId + variableId] = decimal.Parse(TotalmaterialValue[c_organizationId + variableId].ToString()) + materialValue[currentDate][materialLabel];
                        newRow[c_organizationId + variableId] = materialValue[currentDate][materialLabel] == 0 ? 0 : Convert.ToDecimal(dr["TotalPeakValleyFlatB"]) / materialValue[currentDate][materialLabel];//materialDict[materialLabel];
                        newRow[LoopCodestr + "Total"] = Convert.ToDecimal(newRow[LoopCodestr + "Total"].ToString()) + Convert.ToDecimal(newRow[c_organizationId + variableId].ToString());
                    }
                }
                result.Rows.Add(newRow);         
               
            }
            DataRow total = result.NewRow();//平均电耗行
            total["vDate"] = "平均电耗";
            IDictionary<string, string> DicLoopCodestr = new Dictionary<string, string>();
            foreach (DataRow dr in structTable.Rows)
            {
                DicLoopCodestr.Add(dr["OrganizationID"].ToString().Trim() + dr["VariableId"].ToString().Trim(), dr["LoopCode"].ToString().Trim());
            }
            foreach (DataColumn col in result.Columns)
            {
                string colname = col.ColumnName.ToString();
                if ((col.DataType.Name.ToString().Trim() == "Decimal")&& (!colname.Contains("Total")))
                {
                    var LoopCodestr = DicLoopCodestr[colname];                    
                    total[colname] = decimal.Parse(TotalmaterialValue[col.ColumnName.ToString()].ToString()) == 0 ? 0 : decimal.Parse(TotalPeakValleyFlatB[col.ColumnName.ToString()].ToString()) / decimal.Parse(TotalmaterialValue[col.ColumnName.ToString()].ToString());
                    total[LoopCodestr.ToString() + "Total"] = Convert.ToDecimal(total[LoopCodestr.ToString() + "Total"].ToString()) + Convert.ToDecimal(total[col.ColumnName.ToString()]);                     
                }
            }
            result.Rows.Add(total);     
            return result;
        }
    }
}
