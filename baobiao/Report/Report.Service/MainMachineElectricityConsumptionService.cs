using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using SqlServerDataAdapter;
using Report.Infrastructure.Configuration;
namespace Report.Service
{
    public class MainMachineElectricityConsumptionService
    {
        private readonly static ISqlServerDataFactory dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        public static DataTable GetElectricityConsumption(string myOrganizationId, string myYesterdayDate, string myToday)
        {
            string m_ElectricitySql = @"select 
                                A.OrganizationName as OrganizationName,
                                A.MachineName as MachineName, 
                                B.FirstB as ElectricityQuantityYA,
                                B.SecondB as ElectricityQuantityYB,
                                B.ThirdB as ElectricityQuantityYC,
                                C.FirstB as ElectricityQuantityTA,
                                C.SecondB as ElectricityQuantityTB,
                                C.ThirdB as ElectricityQuantityTC,
                                (case when D.FirstB is null or D.FirstB = 0 then null else B.FirstB / D.FirstB end) as ElectricityConsumptionYA,
                                (case when D.SecondB is null or D.SecondB = 0 then null else B.SecondB / D.SecondB end) as ElectricityConsumptionYB,
                                (case when D.ThirdB is null or D.ThirdB = 0 then null else B.ThirdB / D.ThirdB end) as ElectricityConsumptionYC,
                                (case when E.FirstB is null or E.FirstB = 0 then null else C.FirstB / E.FirstB end) as ElectricityConsumptionTA,
                                (case when E.SecondB is null or E.SecondB = 0 then null else C.SecondB / E.SecondB end) as ElectricityConsumptionTB,
                                (case when E.ThirdB is null or E.ThirdB = 0 then null else C.ThirdB / E.ThirdB end) as ElectricityConsumptionTC
                                from 
                                (
                                select H.OrganizationID + I.VariableId as RowKeyId, H.OrganizationID + K.VariableId as DenominatorKeyId, M.Name as OrganizationName, I.VariableId, I.Name as MachineName, I.Denominator, K.VariableId as DenominatorId, K.Name as DenominatorName from tz_formula H, formula_FormulaDetail I, tz_Material J, material_MaterialDetail K, system_Organization L,  system_Organization M
                                where H.Enable = 1
                                and H.State = 0
                                and H.Type = 2
                                and H.KeyID = I.KeyID
                                and I.LevelType = 'MainMachine'
                                and J.Enable = 1
                                and J.State = 0
                                and J.KeyID = K.KeyID
                                and L.OrganizationID = '{0}'
                                and M.OrganizationID like L.OrganizationID + '%'
                                and H.OrganizationID= J.OrganizationID
                                and I.Denominator = K.Formula
                                and H.OrganizationID in (M.OrganizationID)
                                ) A
                                left join 
                                (select N.OrganizationID + N.VariableId as RowKeyId, N.FirstB, N.SecondB, N.ThirdB from tz_Balance M, balance_energy N
                                where M.OrganizationID = '{0}'
                                and M.TimeStamp = '{1}'
                                and M.StaticsCycle = 'day'
                                and M.BalanceId = N.KeyId
                                and N.ValueType = 'ElectricityQuantity') B on A.RowKeyId + '_ElectricityQuantity' = B.RowKeyId
                                left join 
                                (
                                select P.OrganizationID + P.VariableId as RowKeyId, 
                                (case when Q.Shifts = '甲班' then P.CumulantClass when  Q.Shifts = '乙班' then P.CumulantLastClass when Q.Shifts = '丙班' then P.CumulantDay - P.CumulantLastClass - P.CumulantClass end) as FirstB,
                                (case when Q.Shifts = '甲班' then 0 when  Q.Shifts = '乙班' then P.CumulantClass when Q.Shifts = '丙班' then P.CumulantLastClass end) as SecondB,
                                (case when Q.Shifts = '甲班' then 0 when  Q.Shifts = '乙班' then 0 when Q.Shifts = '丙班' then P.CumulantClass end) as ThirdB
                                 from  RealtimeIncrementCumulant P
                                left join system_ShiftDescription Q on Q.OrganizationID = '{0}' and Q.StartTime <=convert(varchar(5),getdate(),8) and Q.EndTime > convert(varchar(5),getdate(),8),
                                system_Organization W, system_Organization V
                                where CONVERT(varchar, P.UpdateDateTime, 23) = '{2}'
                                and W.OrganizationID = '{0}'
                                and V.OrganizationID like W.OrganizationID + '%'
                                and P.OrganizationID in (V.OrganizationID)) C on A.RowKeyId + '_ElectricityQuantity' = C.RowKeyId
                                left join 
                                (select N.OrganizationID + N.VariableId as RowKeyId, N.FirstB, N.SecondB, N.ThirdB from tz_Balance M, balance_energy N
                                where M.OrganizationID = '{0}'
                                and M.TimeStamp = '{1}'
                                and M.StaticsCycle = 'day'
                                and M.BalanceId = N.KeyId
                                and N.ValueType = 'MaterialWeight') D on A.DenominatorKeyId = D.RowKeyId
                                left join 
                                (select P.OrganizationID + P.VariableId as RowKeyId, 
                                (case when Q.Shifts = '甲班' then P.CumulantClass when  Q.Shifts = '乙班' then P.CumulantLastClass when Q.Shifts = '丙班' then P.CumulantDay - P.CumulantLastClass - P.CumulantClass end) as FirstB,
                                (case when Q.Shifts = '甲班' then 0 when  Q.Shifts = '乙班' then P.CumulantClass when Q.Shifts = '丙班' then P.CumulantLastClass end) as SecondB,
                                (case when Q.Shifts = '甲班' then 0 when  Q.Shifts = '乙班' then 0 when Q.Shifts = '丙班' then P.CumulantClass end) as ThirdB
                                 from  RealtimeIncrementCumulant P
                                left join system_ShiftDescription Q on Q.OrganizationID = '{0}' and Q.StartTime <=convert(varchar(5),getdate(),8) and Q.EndTime > convert(varchar(5),getdate(),8),
                                system_Organization W, system_Organization V
                                where CONVERT(varchar, P.UpdateDateTime, 23) = '{2}' 
                                and W.OrganizationID = '{0}'
                                and V.OrganizationID like W.OrganizationID + '%'
                                and P.OrganizationID in (V.OrganizationID)) E on A.DenominatorKeyId = E.RowKeyId
                                order by A.OrganizationName, A.MachineName";
            try
            {
                m_ElectricitySql = string.Format(m_ElectricitySql, myOrganizationId, myYesterdayDate, myToday);
                DataTable ElectricityConsumptionTable = dataFactory.Query(m_ElectricitySql);
                return ElectricityConsumptionTable;
            }
            catch
            {
                return null;
            }
            //////////////以下是产量字段
                                //A.DenominatorName as DenominatorName,
                                //D.FirstB as MaterialWeightYA,
                                //D.SecondB as MaterialWeightYB,
                                //D.ThirdB as MaterialWeightYC,
                                //E.FirstB as MaterialWeightTA,
                                //E.SecondB as MaterialWeightTB,
                                //E.ThirdB as MaterialWeightTC,
        }
    }
}
