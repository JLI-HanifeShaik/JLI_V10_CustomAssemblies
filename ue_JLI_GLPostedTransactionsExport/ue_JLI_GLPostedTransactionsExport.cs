using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_GLPostedTransactionsExport.Properties;

namespace ue_JLI_GLPostedTransactionsExport
{
    public class ue_JLI_GLPostedTransactionsExport : IDOExtensionClass
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
        public DataTable ue_JLI_CLM_GLPostedTransactionsExport(string startDate, string endDate, Guid sessionID)
        {
            DataTable dt = new DataTable("LedgerData");

            // String columns
            dt.Columns.Add("Acct", typeof(string));
            dt.Columns.Add("AcctUnit1", typeof(string));
            dt.Columns.Add("AcctUnit2", typeof(string));
            dt.Columns.Add("AcctUnit3", typeof(string));
            dt.Columns.Add("AcctUnit4", typeof(string));
            dt.Columns.Add("AnalysisAttribute01", typeof(string));
            dt.Columns.Add("AnalysisAttribute02", typeof(string));
            dt.Columns.Add("AnalysisAttribute03", typeof(string));
            dt.Columns.Add("AnalysisAttribute04", typeof(string));
            dt.Columns.Add("AnalysisAttribute05", typeof(string));
            dt.Columns.Add("AnalysisAttribute06", typeof(string));
            dt.Columns.Add("AnalysisAttribute07", typeof(string));
            dt.Columns.Add("AnalysisAttribute08", typeof(string));
            dt.Columns.Add("AnalysisAttribute09", typeof(string));
            dt.Columns.Add("AnalysisAttribute10", typeof(string));
            dt.Columns.Add("AnalysisAttribute11", typeof(string));
            dt.Columns.Add("AnalysisAttribute12", typeof(string));
            dt.Columns.Add("AnalysisAttribute13", typeof(string));
            dt.Columns.Add("AnalysisAttribute14", typeof(string));
            dt.Columns.Add("AnalysisAttribute15", typeof(string));
            dt.Columns.Add("BankCode", typeof(string));
            dt.Columns.Add("ChaDescription", typeof(string));
            dt.Columns.Add("Consolidated", typeof(string));
            dt.Columns.Add("ControlPrefix", typeof(string));
            dt.Columns.Add("ControlSite", typeof(string));
            dt.Columns.Add("CurrAmtFormat", typeof(string));
            dt.Columns.Add("CurrCode", typeof(string));
            dt.Columns.Add("CurrCstPrcFormat", typeof(string));
            dt.Columns.Add("DocumentNum", typeof(string));
            dt.Columns.Add("FromId", typeof(string));
            dt.Columns.Add("FromSite", typeof(string));
            dt.Columns.Add("Hierarchy", typeof(string));
            dt.Columns.Add("Ref", typeof(string));
            dt.Columns.Add("RefControlPrefix", typeof(string));
            dt.Columns.Add("RefControlSite", typeof(string));
            dt.Columns.Add("RefType", typeof(string));
            dt.Columns.Add("RowPointer", typeof(Guid));
            dt.Columns.Add("Site", typeof(string));
            dt.Columns.Add("UbCHGLVouNum", typeof(string));
            dt.Columns.Add("VendNum", typeof(string));
            dt.Columns.Add("VendNumSource", typeof(string));
            dt.Columns.Add("Voucher", typeof(string));
            dt.Columns.Add("JhType", typeof(string));

            // Date columns
            dt.Columns.Add("CheckDate", typeof(DateTime));
            dt.Columns.Add("TransDate", typeof(DateTime));

            // Decimal columns
            dt.Columns.Add("ControlNumber", typeof(decimal));
            dt.Columns.Add("DomAmount", typeof(decimal));
            dt.Columns.Add("DTransNum", typeof(decimal));
            dt.Columns.Add("ExchRate", typeof(decimal));
            dt.Columns.Add("ForAmount", typeof(decimal));
            dt.Columns.Add("JournalBatchId", typeof(decimal));
            dt.Columns.Add("MatlTransNum", typeof(decimal));
            dt.Columns.Add("RefControlNumber", typeof(decimal));
            dt.Columns.Add("TransNum", typeof(decimal));
            dt.Columns.Add("ProjTransNum", typeof(decimal));

            // Long Integer (Int64)
            dt.Columns.Add("CheckNum", typeof(long));
            dt.Columns.Add("UbCHLineNum", typeof(long));
            dt.Columns.Add("VouchSeq", typeof(long));

            // Short Integer (Int16)
            dt.Columns.Add("ControlYear", typeof(short));
            dt.Columns.Add("RefControlYear", typeof(short));

            // Byte
            dt.Columns.Add("ControlPeriod", typeof(byte));
            dt.Columns.Add("Processed", typeof(byte));
            dt.Columns.Add("RefControlPeriod", typeof(byte));
            dt.Columns.Add("UbCHRubric", typeof(byte));
            dt.Columns.Add("Cancellation", typeof(byte));
            dt.Columns.Add("EnableCancellationPosting", typeof(byte));


            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_CLM_GLPostedTransactionsExport;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,startDate,endDate,sessionID);
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
                createLog("ue_JLI_GLPostedTransactionsExport", "ue_JLI_CLM_GLPostedTransactionsExport", 56, "ex - " + ex.Message);
                return dt;
            }



        }

    }
}
