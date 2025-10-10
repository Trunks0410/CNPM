using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PROJECT_CK
{
    public class ConnectDB
    {
        public static string Connect()
        {
             string serverName = ".";
             string databaseName = "QuanLyXeMuaBanXeMay";
             string sqlLogin ="anhkiet_login";

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.DataSource = serverName;
            builder.InitialCatalog = databaseName;
            builder.UserID = sqlLogin;              
            builder.Password = "anhkiet123";            
            builder.PersistSecurityInfo = false;

            return builder.ConnectionString;
        }
    
    }
}
