using Mongoose.Core.Common;
using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_DailyOrderRecieptSummaryReport.Properties;

namespace ue_JLI_DailyOrderRecieptSummaryReport
{
    public class ue_JLI_DailyOrderRecieptSummaryReport : IDOExtensionClass
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

        public int ue_JLI_DailyOrderRecieptSummary(string inpCustNum, string inpDate)
        {
            string yesterdayDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(inpDate))
                yesterdayDate = inpDate;
            string userName =string.Empty;
            if (string.IsNullOrEmpty(userName)) { userName = IDORuntime.Context.UserName; }

            string query = string.Empty;
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_DailyOrderRecieptSummary;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query, yesterdayDate, inpCustNum, userName);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.ExecuteNonQuery();//.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_DailyOrderRecieptSummaryReport", "ue_JLI_Rpt_DailyOrderRecieptSummary", 190, "ex - " + ex.Message);
            }

            ue_JLI_DailyOrderReceiptSummaryNotify(yesterdayDate);

            return 0;
        }
        public DataTable ue_JLI_Rpt_DailyOrderRecieptSummary(string inpCustNum,string inpDate)
        {

            string yesterdayDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(inpDate))
                yesterdayDate = inpDate;

            DataTable dt = new DataTable();

            // Add columns
            dt.Columns.Add("UbCustNum", typeof(string));
            dt.Columns.Add("UbCustName", typeof(string));
            dt.Columns.Add("UbCoNum", typeof(string));
            dt.Columns.Add("UbCoLine", typeof(string));
            dt.Columns.Add("UbCustPo", typeof(string));
            dt.Columns.Add("UbOrderDate", typeof(DateTime));
            dt.Columns.Add("UbNetPrice", typeof(string));   // Formatted as string (currency)
            dt.Columns.Add("UbShipTo", typeof(string));
            dt.Columns.Add("UbItem", typeof(string));
            dt.Columns.Add("UbItemDesc", typeof(string));
            dt.Columns.Add("UbOrderQty", typeof(decimal));
            dt.Columns.Add("UbItemPrice", typeof(string));  // Formatted as string (currency)
            dt.Columns.Add("UbLTDueDate", typeof(DateTime));
            dt.Columns.Add("UbAccessoryDesc", typeof(string));
            dt.Columns.Add("UbAccessory", typeof(string));
            dt.Columns.Add("UbAccessoryQty", typeof(decimal));
            dt.Columns.Add("UbOptionItem", typeof(string));
            dt.Columns.Add("UbOptionItemDesc", typeof(string));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_Rpt_DailyOrderRecieptSummary;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,inpCustNum,yesterdayDate);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    //createLog("ue_JLI_DailyOrderRecieptSummaryReport", "ue_JLI_Rpt_DailyOrderRecieptSummary", 56, "After ExecuteReader Call"); 
                    dt_Resultset.Load(Resultset);
                    if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                        return dt_Resultset;
                    else
                        return dt;

                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_DailyOrderRecieptSummaryReport", "ue_JLI_Rpt_DailyOrderRecieptSummary", 56, "ex - " + ex.Message);
                return dt;
            }



        }
        public int ue_JLI_DailyOrderReceiptSummaryNotify(string inpDate)
        {

            string yesterdayDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(inpDate))
                yesterdayDate = inpDate;            

            string query = string.Empty;
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_DailyOrderReceiptSummaryNotify;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,yesterdayDate);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_DailyOrderRecieptSummaryReport", "ue_JLI_Rpt_DailyOrderRecieptSummary", 190, "ex - " + ex.Message);
            }
            return 0;
        }


    }
}
