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
    public class AlarmMonitorService
    {
        public static DataTable GetRealTimeAlarmData(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select top(200) A.AlarmDateTime,B.Name as ProductLineName,C.Name,C.EnergyConsumptionType,C.StandardValue,C.ActualValue
                                from system_TenDaysRealtimeAlarm A,system_Organization B,shift_EnergyConsumptionAlarmLog C
                                where A.OrganizationID=B.OrganizationID
                                and A.KeyId=C.EnergyConsumptionAlarmLogID
                                and A.AlarmType='EnergyConsumption'
                                and B.LevelCode like (
						                                SELECT E.LevelCode
						                                FROM system_Organization E
						                                WHERE E.OrganizationID=@organizationId)+'%'
                                order by A.AlarmDateTime desc";
            SqlParameter parameter = new SqlParameter("organizationId", organizationId);
            DataTable table = dataFactory.Query(mySql, parameter);
            return table;
        }

        public static DataTable GetHistoryAlarmData(string organizationId,string startTime,string endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.StartTime as AlarmDateTime, B.Name as ProductLineName,A.Name,A.EnergyConsumptionType,A.StandardValue,A.ActualValue
                                from shift_EnergyConsumptionAlarmLog A,system_Organization B
                                where A.OrganizationID=B.OrganizationID
                                and (A.StartTime>=@startTime and A.StartTime<=@endTime)
                                and B.LevelCode like (
				                                SELECT E.LevelCode
				                                FROM system_Organization E
				                                WHERE E.OrganizationID=@organizationId)+'%'
                                order by A.StartTime";
            SqlParameter[] parameters ={new SqlParameter("startTime",startTime),
                                      new SqlParameter("endTime",endTime),
                                      new SqlParameter("organizationId",organizationId)};
            DataTable table = dataFactory.Query(mySql, parameters);
            return table;
        }
    }
}
