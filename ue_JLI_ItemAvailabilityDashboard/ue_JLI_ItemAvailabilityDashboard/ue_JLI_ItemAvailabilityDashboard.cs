using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_ItemAvailabilityDashboard.Properties;

namespace ue_JLI_ItemAvailabilityDashboard
{
    public class ue_JLI_ItemAvailabilityDashboard : IDOExtensionClass
    {

        public void createLog(string Log_ClassName, string Log_MethodName, int Log_LineNumber, string Log_Details)
        {
            try
            {
                if (string.IsNullOrEmpty(Log_Details))
                {
                    Log_Details = "Error Details not Updated";
                }

                UpdateCollectionResponseData oResponseData;
                UpdateCollectionRequestData oRequestData;
                IDOUpdateItem oUpdateItem;

                oResponseData = new UpdateCollectionResponseData();
                oRequestData = new UpdateCollectionRequestData("ue_ZESHT_CustomAssemblyLogs"); // IDO Name
                oUpdateItem = new IDOUpdateItem(UpdateAction.Insert); // Insert Or Update Or Delete

                oUpdateItem.Properties.Add("ClassName", Log_ClassName);
                oUpdateItem.Properties.Add("MethodName", Log_MethodName);
                oUpdateItem.Properties.Add("LineNumber", Log_LineNumber);
                oUpdateItem.Properties.Add("Comments", Log_Details);

                oRequestData.Items.Add(oUpdateItem);
                oResponseData = Context.Commands.UpdateCollection(oRequestData);
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_JLM_CorteSchedReport", "ue_JLI_CLM_Rpt_JLM_CorteSched", 41, ex.Message);
            }
        }
        public interface IIDOExtensionClass
        {
            void SetContext(Mongoose.IDO.IIDOExtensionClassContext context);
        }


        public DataTable ue_JLI_CLM_ItemAvailabilityDashboard(string custNum, string item)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CustNum", typeof(string));
            dt.Columns.Add("Item", typeof(string));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_CLM_ItemAvailabilityDashboard;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query, custNum, item);
                    db.AddCommandParameterWithValue(sqlCommand, "custNum", custNum);
                    db.AddCommandParameterWithValue(sqlCommand, "item", item);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    dt_Resultset.Load(Resultset);
                    if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                        return dt_Resultset;
                    else
                        return dt;

                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_ItemAvailabilityDashboard", "ue_JLI_CLM_ItemAvailabilityDashboard", 56, "ex - " + ex.Message);
                return dt;
            }



        }

        public DataTable ue_JLI_CLM_ItemAvailabilityDashboard2(string custNum, string item)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CustNum", typeof(string));
            dt.Columns.Add("Item", typeof(string));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_CLM_ItemAvailabilityDashboard2;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    db.AddCommandParameterWithValue(sqlCommand, "custNum", custNum);
                    db.AddCommandParameterWithValue(sqlCommand, "item", item);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    dt_Resultset.Load(Resultset);
                    if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                        return dt_Resultset;
                    else
                        return dt;

                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_ItemAvailabilityDashboard", "ue_JLI_CLM_ItemAvailabilityDashboard2", 56, "ex - " + ex.Message);
                return dt;
            }



        }


    }
}
