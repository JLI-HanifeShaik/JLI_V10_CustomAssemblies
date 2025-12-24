using Mongoose.Core.Common;
using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_SwatchReqTemps.Properties;

namespace ue_JLI_SwatchReqTemps
{
    public class ue_JLI_SwatchReqTemps : IDOExtensionClass
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

        public int ue_JLI_SWLivingSpacesImport()
        {
            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_SWLivingSpacesImport;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.ExecuteNonQuery();


                    //dt_Resultset.Load(Resultset);
                    //if (dt_Resultset == null && dt_Resultset.Rows.Count < 1)
                    //{
                    //    return 0;
                    //}
                    //UpdateCollectionResponseData oResponseData;
                    //UpdateCollectionRequestData oRequestData;
                    //IDOUpdateItem oUpdateItem;
                    //foreach (DataRow row in dt_Resultset.Rows)
                    //{
                    //    string recordType = row["RecordType"].ToString();
                    //    string partner = row["partner"].ToString();
                    //    string sword_num = row["sword_num"].ToString();
                    //    int sword_line = Convert.ToInt16(row["sword_line"].ToString());
                    //    string cust_num = row["cust_num"].ToString();
                    //    string status = row["status"].ToString();
                    //    string po_num = row["po_num"].ToString();
                    //    DateTime po_date = Convert.ToDateTime(row["po_date"].ToString());
                    //    DateTime rcvd_Date = Convert.ToDateTime(row["rcvd_Date"].ToString());
                    //    string ord_type = row["ord_type"].ToString();
                    //    string ref_num = row["ref_num"].ToString();
                    //    string shipto_name = row["shipto_name"].ToString();
                    //    string shipto_addr = row["shipto_addr"].ToString();
                    //    string shipto_city = row["shipto_city"].ToString();
                    //    string shipto_state = row["shipto_state"].ToString();
                    //    string shipto_zip = row["shipto_zip"].ToString();
                    //    string shipto_email = row["shipto_email"].ToString();
                    //    int qty = Convert.ToInt16(row["qty"].ToString());
                    //    string um = row["um"].ToString();
                    //    double price = Convert.ToDouble(row["price"].ToString());
                    //    string sku = row["sku"].ToString();
                    //    string description = row["description"].ToString();

                    //    oRequestData = new UpdateCollectionRequestData();
                    //    oResponseData = new UpdateCollectionResponseData();
                    //    oUpdateItem = new IDOUpdateItem(UpdateAction.Insert);

                    //    if (recordType == "H")
                    //    {
                    //        oRequestData = new UpdateCollectionRequestData("JLI_SWReqHdrs");
                    //        oUpdateItem.Properties.Add("Partner", partner);
                    //        oUpdateItem.Properties.Add("CustNum", cust_num);
                    //        oUpdateItem.Properties.Add("Status", status);
                    //        oUpdateItem.Properties.Add("PoNum", po_num);
                    //        oUpdateItem.Properties.Add("PoDate", po_date.ToString());
                    //        oUpdateItem.Properties.Add("RcvdDate", rcvd_Date.ToString());
                    //        oUpdateItem.Properties.Add("OrdType", ord_type);
                    //        oUpdateItem.Properties.Add("RefNum", ref_num);
                    //        oUpdateItem.Properties.Add("ShiptoName", shipto_name);
                    //        oUpdateItem.Properties.Add("ShiptoAddr", shipto_addr);
                    //        oUpdateItem.Properties.Add("ShiptoCity", shipto_city);
                    //        oUpdateItem.Properties.Add("ShiptoState", shipto_state);
                    //        oUpdateItem.Properties.Add("ShiptoEmail", shipto_email);
                    //        oRequestData.Items.Add(oUpdateItem);
                    //        oResponseData = Context.Commands.UpdateCollection(oRequestData);
                    //    }
                    //    else if (recordType == "D")
                    //    {
                    //        oRequestData = new UpdateCollectionRequestData("JLI_SWReqItems");
                    //        oUpdateItem.Properties.Add("Partner", partner);
                    //        oUpdateItem.Properties.Add("SwordNum", sword_num);
                    //        oUpdateItem.Properties.Add("SwordLine", sword_line.ToString());
                    //        oUpdateItem.Properties.Add("PoNum", po_num);
                    //        oUpdateItem.Properties.Add("PoDate", po_date.ToString());
                    //        oUpdateItem.Properties.Add("Qty", qty.ToString());
                    //        oUpdateItem.Properties.Add("Um", um);
                    //        oUpdateItem.Properties.Add("Price", price.ToString());
                    //        oUpdateItem.Properties.Add("Sku", sku);
                    //        oUpdateItem.Properties.Add("Description", description);
                    //        oRequestData.Items.Add(oUpdateItem);
                    //        oResponseData = Context.Commands.UpdateCollection(oRequestData);
                    //    }

                    //};
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_SwatchReqTemps", "ue_JLI_SWLivingSpacesImport", 190, "ex - " + ex.Message);
            }
            return 0;








        }

        
    }
}
