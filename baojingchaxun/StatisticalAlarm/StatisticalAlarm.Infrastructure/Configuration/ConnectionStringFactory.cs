using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalAlarm.Infrastructure.Configuration
{
    public static class ConnectionStringFactory
    {
        //private static string _connString = "Data Source=QH-20140814XCYI;Initial Catalog=NXJC;Integrated Security=True";
        //private static string _connString = "Data Source=DEC-WINSVR12;Initial Catalog=NXJC_DEVELOP;User Id=sa; Password = jsh123+";
        //private static string _connString = "Data Source=ADMIN-PC;Initial Catalog=NXJC_DEVELOP;User Id=sa; Password = 123456";
        //public static string NXJCConnectionString { get { return _connString; } }
        public static string NXJCConnectionString { get { return ConfigurationManager.ConnectionStrings["ConnNXJC"].ToString(); } }
    }
}
