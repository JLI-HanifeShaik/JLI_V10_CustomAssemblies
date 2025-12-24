using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_AvailCollecGroups.Properties;
using static ue_JLI_AvailCollecGroups.ue_JLI_AvailCollecGroups;

namespace ue_JLI_AvailCollecGroups
{
    public class ue_JLI_AvailCollecGroups : IDOExtensionClass
    {

        public interface IIDOExtensionClass
        {
            void SetContext(Mongoose.IDO.IIDOExtensionClassContext context);
        }
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
        public int ue_JLI_AvailCollDataInsertAndDelete(string inpCustNum = null,string inpCollectionGroup = null)
        {           
            string query = string.Empty;
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_AvailCollDataInsertAndDelete;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,inpCustNum,inpCollectionGroup);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //createLog("JLI_AvailCollecGroups", "ue_JLI_AvailCollDataInsertAndDelete", 139, ex.Message);
            }


            return 0;
        }
        public int ue_JLI_AvailCollDataDelete(string inpCustNum = null,string inpCollectionGroup = null)
        {
            string query = string.Empty;
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_AvailCollDataDelete;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,inpCustNum,inpCollectionGroup);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //createLog("JLI_AvailCollecGroups", "ue_JLI_AvailCollDataInsertAndDelete", 139, ex.Message);
            }


            return 0;
        }




    }
}
