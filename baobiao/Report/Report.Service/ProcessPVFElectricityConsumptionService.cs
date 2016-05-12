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
    public class ProcessPVFElectricityConsumptionService
    {

        public static DataTable GetData(string organizationId,string startDate,string endDate)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT  E.LevelCode,E.Name,F.*
                                FROM (select A.Name,A.OrganizationID,B.VariableId,B.LevelCode
		                                from tz_Formula A,formula_FormulaDetail B,system_Organization S
		                                where A.KeyID=B.KeyID and A.OrganizationID=S.OrganizationID
		                                and S.LevelCode like (SELECT LevelCode FROM system_Organization WHERE OrganizationID=@organizationId)+'%') E,
	                                  (select D.VariableName, D.OrganizationID,D.VariableId,sum(D.PeakB) as PeakB,sum(D.MorePeakB)as MorePeakB,sum(D.ValleyB) as ValleyB,sum(D.FlatB) as FlatB,sum(D.TotalPeakValleyFlatB) as TotalPeakValleyFlatB
		                                from tz_Balance C,balance_Energy D
		                                where C.BalanceId=D.KeyID and C.OrganizationID=@organizationId
		                                and C.TimeStamp>=@startDate
		                                and C.TimeStamp<=@endDate
		                                and D.ValueType='ElectricityConsumption'
		                                group by D.VariableId,D.OrganizationID,D.VariableName) F
                                WHERE E.VariableId+'_ElectricityConsumption'=F.VariableId
                                AND E.OrganizationID=F.OrganizationID
                                ORDER BY Name,LevelCode";
            SqlParameter[] parameters = { new SqlParameter("organizationId", organizationId),
                                        new SqlParameter("startDate",startDate),
                                        new SqlParameter("endDate",endDate)};
            return dataFactory.Query(mySql,parameters);
        }
    }
}
