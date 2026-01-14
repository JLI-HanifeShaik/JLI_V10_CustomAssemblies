using Mongoose.IDO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_PBI_DataShare.Properties;

namespace ue_JLI_PBI_DataShare
{
    [IDOExtensionClass("ue_JLI_PBI_DataShare")]
    public class ue_JLI_PBI_DataShare : IDOExtensionClass
    {

        [IDOMethod(MethodFlags.CustomLoad, "infobar")]
        public DataTable ue_JLI_PBI_CLM_DailyShipSummary(DateTime? shippedDate, ref string infobar)
        {

            DataTable resultSet = new DataTable();
            // Add columns
            resultSet.Columns.Add("Whse", typeof(string));
            resultSet.Columns.Add("LoadCount", typeof(int));
            resultSet.Columns.Add("Pcs", typeof(int));
            resultSet.Columns.Add("LoadValue", typeof(double));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                query = Resources.ue_JLI_PBI_CLM_DailyShipSummary;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    db.AddCommandParameterWithValue(sqlCommand, "ShippedDate", shippedDate);
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

        }//ue_JLI_PBI_CLM_DailyShipSummary

        [IDOMethod(MethodFlags.CustomLoad, "infobar")]
        public DataTable ue_JLI_PBI_CLM_DailyShipByWhse(DateTime? shippedDate, ref string infobar)
        {

            DataTable resultSet = new DataTable();
            // Add columns
            resultSet.Columns.Add("Whse", typeof(string));
            resultSet.Columns.Add("LoadCount", typeof(int));
            resultSet.Columns.Add("Pcs", typeof(int));
            resultSet.Columns.Add("LoadValue", typeof(double));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                query = Resources.ue_JLI_PBI_CLM_DailyShipByWhse;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    db.AddCommandParameterWithValue(sqlCommand, "ShippedDate", shippedDate);
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

        }//ue_JLI_PBI_CLM_DailyShipByWhse

        [IDOMethod(MethodFlags.CustomLoad, "infobar")]
        public DataTable ue_JLI_PBI_CLM_DailyShipByCustNum(DateTime? shippedDate, ref string infobar)
        {

            DataTable resultSet = new DataTable();
            // Add columns
            resultSet.Columns.Add("CustNum", typeof(string));
            resultSet.Columns.Add("LoadCount", typeof(int));
            resultSet.Columns.Add("Pcs", typeof(int));
            resultSet.Columns.Add("LoadValue", typeof(double));
            resultSet.Columns.Add("CustomerName", typeof(string));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                query = Resources.ue_JLI_PBI_CLM_DailyShipByCustNum;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    db.AddCommandParameterWithValue(sqlCommand, "ShippedDate", shippedDate);
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

        }//ue_JLI_PBI_CLM_DailyShipByCustNum


        //HS20260114  |Development Started 
        //Task-57-GL0034 - [Convert JLI_Plannig Details for V9]       
        [IDOMethod(MethodFlags.RequiresTransaction, "infobar")]
        public int ue_JLI_PBI_PlanningDetail(string productCode, ref string infobar)
        {            

            string query = string.Empty;

            try
            {
                query = Resources.ue_JLI_PBI_PlanningDetail;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    db.AddCommandParameterWithValue(sqlCommand, "ProductCode", productCode);
                    sqlCommand.ExecuteNonQuery();                    
                }
            }
            catch (Exception ex)
            {
                infobar = ex.Message;
            }
            return 0;
        }
        [IDOMethod(MethodFlags.RequiresTransaction, "infobar")]
        public int ue_JLI_PBI_PlanningDetail_BGTaskSubmit(ref string infobar)
        {
            string query = string.Empty;

            try
            {
                query = Resources.ue_JLI_PBI_PlanningDetail_BGTaskSubmit;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                infobar = ex.Message;
            }
            return 0;
        }
        //HS20260114 | Development Ended 

        //HS20260114  |Development Started 
        //Task-58-GL0039 - [Convert P2024-63 task to V10]
        //Task-105-P2024-63 [add Order number  and Order Line to JLI_JobMatlUsageByShipTo_mst]
        [IDOMethod(MethodFlags.RequiresTransaction, "infobar")]
        public int ue_JLI_PBI_JobMatlUsageByShipTo(ref string infobar)
        {

            string query = string.Empty;

            try
            {
                query = Resources.ue_JLI_PBI_JobMatlUsageByShipTo;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                infobar = ex.Message;
            }
            return 0;
        }
        //HS20260114 | Development Ended 




    }
}
