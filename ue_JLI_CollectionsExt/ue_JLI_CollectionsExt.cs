using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Resources;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_CollectionsExt.Properties;

namespace ue_JLI_CollectionsExt
{
    public class ue_JLI_CollectionsExt : IDOExtensionClass
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
        public int ue_JLI_AvailCollDataInsertUpdateDelete(string collectionID,
                                                        string processType,
                                                        string newCollectionGroup,
                                                        string oldCollectionGroup,
                                                        string newStatus,
                                                        string oldStatus,
                                                        string newPriceBook,
                                                        string oldPriceBook,
                                                        string newExclusive,
                                                        string oldExclusive)
        {
            
            string query = string.Empty;
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_AvailCollDataInsertUpdateDelete;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query, collectionID, newCollectionGroup, oldCollectionGroup, newStatus, oldStatus, newPriceBook, oldPriceBook, newExclusive, oldExclusive, processType);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_CollectionsExt", "ue_JLI_AvailCollDataInsertUpdateDelete", 139, ex.Message);
            }


            return 0;
        }






    }
}
