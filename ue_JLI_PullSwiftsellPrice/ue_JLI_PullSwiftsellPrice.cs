using Mongoose.IDO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_PullSwiftsellPrice.Properties;

namespace ue_JLI_PullSwiftsellPrice 
{
    [IDOExtensionClass("ue_JLI_PullSwiftsellPrice")]
    public class ue_JLI_PullSwiftsellPrice : IDOExtensionClass
    {
        [IDOMethod(MethodFlags.CustomLoad, "infobar")]
        public DataTable ue_JLI_CLM_PullSwiftsellPrice(ref string infobar)
        {

            DataTable resultSet = new DataTable();
            // Add columns
            resultSet.Columns.Add("CustNum", typeof(string));
            resultSet.Columns.Add("Item", typeof(string));
            resultSet.Columns.Add("FabricGrade", typeof(string));
            resultSet.Columns.Add("Price", typeof(string));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                query = Resources.ue_JLI_CLM_PullSwiftsellPrice;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    dt_Resultset.Load(Resultset);
                    if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                        return dt_Resultset;
                    else
                        return resultSet;
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_DailyOrderRecieptSummaryReport", "ue_JLI_Rpt_DailyOrderRecieptSummary", 56, "ex - " + ex.Message);
                infobar = ex.Message;
                return resultSet;
            }



        }
















    }
}
