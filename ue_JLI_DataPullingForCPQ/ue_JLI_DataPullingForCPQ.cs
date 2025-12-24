using Mongoose.IDO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Resources;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_DataPullingForCPQ.Properties;

namespace ue_JLI_DataPullingForCPQ
{
    [IDOExtensionClass("ue_JLI_DataPullingForCPQ")]
    public class ue_JLI_DataPullingForCPQ : IDOExtensionClass
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
                infobar = ex.Message;
                return resultSet;
            }

        }//ue_JLI_CLM_PullSwiftsellPrice

        [IDOMethod(MethodFlags.CustomLoad, "infobar")]
        public DataTable ue_JLI_CLM_CustItemFabricPrice(ref string infobar)
        {

            DataTable resultSet = new DataTable();
            // Add columns
            resultSet.Columns.Add("Item", typeof(string));
            resultSet.Columns.Add("CustNum", typeof(string));
            resultSet.Columns.Add("FabricGrade", typeof(string));
            resultSet.Columns.Add("Price", typeof(string));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                query = Resources.ue_JLI_CLM_CustItemFabricPrice;
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
                infobar = ex.Message;
                return resultSet;
            }

        }//ue_JLI_CLM_CustItemFabricPrice

        [IDOMethod(MethodFlags.CustomLoad, "infobar")]
        public DataTable ue_JLI_CLM_SyncSytelineOrderStatus(DateTime? startDate, ref string infobar)
        {

            DataTable resultSet = new DataTable();
            // Add columns
            resultSet.Columns.Add("SiteRef", typeof(string));
            resultSet.Columns.Add("CoNum", typeof(string));
            resultSet.Columns.Add("CoLine", typeof(string));
            resultSet.Columns.Add("CustNum", typeof(string));
            resultSet.Columns.Add("Item", typeof(string));
            resultSet.Columns.Add("ShipDate", typeof(string));
            resultSet.Columns.Add("QtyOrdered", typeof(string));
            resultSet.Columns.Add("QtyShipped", typeof(string));
            resultSet.Columns.Add("Stat", typeof(string));
            resultSet.Columns.Add("RecordDate", typeof(string));
            resultSet.Columns.Add("Cancelled", typeof(string));
            resultSet.Columns.Add("CancelledReason", typeof(string));
            resultSet.Columns.Add("EstimatedShipDate", typeof(string));
            resultSet.Columns.Add("InvoiceNumbers", typeof(string));
            resultSet.Columns.Add("LoadNumbers", typeof(string));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                query = Resources.ue_JLI_CLM_SyncSytelineOrderStatus;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    db.AddCommandParameterWithValue(sqlCommand, "pi_StartDate", startDate);
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
                infobar = ex.Message;
                return resultSet;
            }

        }//ue_JLI_CLM_SyncSytelineOrderStatus










    }
}
