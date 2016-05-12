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
    public class ProcessPVFElectricityQuantityService
    {
        public static DataTable GetData(string organizationId, string startDate, string endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT (case when E.LevelCode IS NULL then G.LevelCode else(G.LevelCode+RIGHT(E.LevelCode,LEN(E.LevelCode)-3))end)AS LevelCode,G.Name AS ProductLineName, E.Name, F.*, 
                             (case when F.TotalPeakValleyFlatB is not null and F.TotalPeakValleyFlatB <> 0 then F.PeakB / F.TotalPeakValleyFlatB else 0 end ) as RatioPeakB, 
                             (case when F.TotalPeakValleyFlatB is not null and F.TotalPeakValleyFlatB <> 0 then F.MorePeakB / F.TotalPeakValleyFlatB else 0 end ) as RatioMorePeakB, 
                             (case when F.TotalPeakValleyFlatB is not null and F.TotalPeakValleyFlatB <> 0 then F.ValleyB / F.TotalPeakValleyFlatB else 0 end ) as RatioValleyB, 
                             (case when F.TotalPeakValleyFlatB is not null and F.TotalPeakValleyFlatB <> 0 then F.FlatB / F.TotalPeakValleyFlatB else 0 end ) as RatioFlatB
                                FROM system_Organization G left join
								(select (A.Name+B.Name) as Name,A.OrganizationID,B.VariableId, B.LevelCode
		                                from tz_Formula A,formula_FormulaDetail B,system_Organization S
		                                where A.KeyID=B.KeyID and A.OrganizationID=S.OrganizationID
                                        and A.Type=2 and A.ENABLE='true' and A.State=0
		                                and S.LevelCode like (SELECT LevelCode FROM system_Organization WHERE OrganizationID=@organizationId)+'%') E
										on G.OrganizationID=E.OrganizationID left join
	                                  (select D.OrganizationID,D.VariableId,sum(D.PeakB) as PeakB,sum(D.MorePeakB)as MorePeakB,sum(D.ValleyB) as ValleyB,sum(D.FlatB) as FlatB,sum(D.TotalPeakValleyFlatB) as TotalPeakValleyFlatB
		                                from tz_Balance C,balance_Energy D
		                                where C.BalanceId=D.KeyID and C.OrganizationID=@organizationId
		                                and C.TimeStamp>=@startDate
		                                and C.TimeStamp<=@endDate
		                                and D.ValueType='ElectricityQuantity'
										and C.StaticsCycle='day'
		                                group by D.VariableId,D.OrganizationID) F
                                on E.VariableId+'_ElectricityQuantity'=F.VariableId
                                AND E.OrganizationID=F.OrganizationID
								WHERE G.LevelType<>'Company'
                                AND G.LevelCode like (SELECT LevelCode FROM system_Organization WHERE OrganizationID=@organizationId)+'%'
                                ORDER BY G.LevelCode";
            SqlParameter[] parameters = { new SqlParameter("organizationId", organizationId),
                                        new SqlParameter("startDate",startDate),
                                        new SqlParameter("endDate",endDate)};
            DataTable resultTable = dataFactory.Query(mySql, parameters);
            DataColumn stateColumn = new DataColumn("state", typeof(string));
            resultTable.Columns.Add(stateColumn);

            foreach (DataRow dr in resultTable.Rows)
            {
                if (dr["Name"] is DBNull || dr["Name"].ToString().Trim() == "")
                {
                    dr["Name"] = dr["ProductLineName"].ToString().Trim();
                }
                if (dr["LevelCode"].ToString().Trim().Length == 7)
                {
                    dr["state"] = "closed";
                }
                else
                {
                    dr["state"] = "open";
                }
            }
            return resultTable;
        }
    }
}
