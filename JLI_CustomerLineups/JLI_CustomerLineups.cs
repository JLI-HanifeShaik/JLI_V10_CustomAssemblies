using JLI_CustomerLineups.Properties;
using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
    
namespace JLI_CustomerLineups
{
    [IDOExtensionClass("JLI_CustomerLineups")]
    public class JLI_CustomerLineups : IDOExtensionClass
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
        public int ue_JLI_insertAvailCollec(string custNum , string item)
        {
            if (item.Contains("-"))
                return 0;
            string query = string.Empty;
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.JLI_insertAvailCollec;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,custNum,item);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //createLog("JLI_CustomerLineups", "JLI_getItemAccessoriesInfo", 190, "ex - " + ex.Message);
            }
            return 0;        
        }
        public int ue_JLI_deleteAvailCollec(string custNum, string item)
        {
            UpdateCollectionResponseData oResponseData;
            UpdateCollectionRequestData oRequestData;
            IDOUpdateItem oUpdateItem;
            LoadCollectionRequestData loadRequest = new LoadCollectionRequestData();
            LoadCollectionResponseData loadResponse = new LoadCollectionResponseData();
            string strFilter = String.Empty;

            strFilter = String.Format("CustNum = '{0}' And Item = '{1}' And RecordType = 'S' ", custNum, item);
            loadRequest.IDOName = "ue_JLI_AvailableCollections";
            loadRequest.Filter = strFilter;
            loadRequest.PropertyList.Add("CustNum");
            loadRequest.PropertyList.Add("Item");
            loadResponse = Context.Commands.LoadCollection(loadRequest);

            if(loadResponse.Items.Count > 0)
            {
                oResponseData = new UpdateCollectionResponseData();
                oRequestData = new UpdateCollectionRequestData("ue_JLI_AvailableCollections"); // IDO Name
                oUpdateItem = new IDOUpdateItem(UpdateAction.Delete, loadResponse.Items[0].ItemID); // Insert Or Update Or Delete
                oRequestData.Items.Add(oUpdateItem);
                oResponseData = Context.Commands.UpdateCollection(oRequestData);
            }

            return 0;          


        }
        public int ue_JLI_insertCustLineupFabETAsData(string custNum, string item, int custItemSeq)
        {
            UpdateCollectionResponseData oResponseData;
            UpdateCollectionRequestData oRequestData;
            IDOUpdateItem oUpdateItem;

            LoadCollectionRequestData loadRequest = new LoadCollectionRequestData();
            LoadCollectionResponseData loadResponse = new LoadCollectionResponseData();
            string strFilter = string.Empty;
            string accessory = string.Empty;
            string optionItem = string.Empty;
            string qtyOnHand = string.Empty;

            strFilter = $"item = '{item}'";
            loadRequest.IDOName = "JLI_ItemAccessories2";
            loadRequest.Filter = strFilter;
            loadRequest.PropertyList.Add("accessory");
            loadRequest.PropertyList.Add("OptionItem");
            loadRequest.RecordCap = 0;
            loadResponse = Context.Commands.LoadCollection(loadRequest);

            if (loadResponse.Items.Count > 0)
            {
                for (int i = 0; i < loadResponse.Items.Count; i++)
                {
                    accessory = loadResponse[i,"accessory"].Value;
                    optionItem = loadResponse[i,"OptionItem"].Value;

                    if ((accessory == "BASE FABRIC" || accessory.Substring(0, 1) == "P") && accessory != "PARTS")
                    {
                        ue_JLI_getItemQtyOnHand(optionItem, ref qtyOnHand);
                        qtyOnHand = string.IsNullOrEmpty(qtyOnHand) ? "0" : qtyOnHand;

                        oResponseData = new UpdateCollectionResponseData();
                        oRequestData = new UpdateCollectionRequestData("ue_JLI_CustLineupFabETAs");
                        oUpdateItem = new IDOUpdateItem(UpdateAction.Insert);
                        oUpdateItem.Properties.Add("CustNum", custNum);
                        oUpdateItem.Properties.Add("CustItemSeq", custItemSeq.ToString());
                        oUpdateItem.Properties.Add("Item", item);
                        oUpdateItem.Properties.Add("FabricCode", optionItem);
                        if (Convert.ToDouble(qtyOnHand) > 0)
                        {
                            oUpdateItem.Properties.Add("Stock", "In-stock");
                            oUpdateItem.Properties.Add("Qty", qtyOnHand);
                        }

                        oRequestData.Items.Add(oUpdateItem);
                        oResponseData = Context.Commands.UpdateCollection(oRequestData);
                    }
                }
            }


            return 0;

        }
        void ue_JLI_getItemQtyOnHand(string item, ref string qtyOnHand)
        {
            LoadCollectionRequestData loadRequest = new LoadCollectionRequestData();
            LoadCollectionResponseData loadResponse = new LoadCollectionResponseData();
            string strFilter = $"Item = '{item}'";

            loadRequest.IDOName = "SLItems";
            loadRequest.Filter = strFilter;
            loadRequest.PropertyList.Add("Item");
            loadRequest.PropertyList.Add("Stat");
            loadRequest.PropertyList.Add("iwvQtyOnHand");
            loadRequest.RecordCap = 1;
            loadResponse = Context.Commands.LoadCollection(loadRequest);

            if (loadResponse.Items.Count > 0)
            {
                qtyOnHand = loadResponse[0,"iwvQtyOnHand"].Value;
            }
        }
        public DataTable ue_JLI_getItemAccessoriesInfo(string item)
        {
            //createLog("JLI_CustomerLineups", "JLI_getItemAccessoriesInfo", 190, "item - " + item);
            // Define temp result set structure
            DataTable tempResultSet = new DataTable();
            tempResultSet.Columns.Add("Accessory", typeof(string));
            tempResultSet.Columns.Add("OptionItem", typeof(string));
            tempResultSet.Columns.Add("OptionItemDesc", typeof(string));
            tempResultSet.Columns.Add("Qty", typeof(string));

            string query = string.Empty;

            DataTable dt_Resultset = new DataTable();

            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.JLI_getItemAccessoriesInfo;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,item);
                    sqlCommand.CommandType = CommandType.Text;
                    IDataReader Resultset = sqlCommand.ExecuteReader(); 
                    dt_Resultset.Load(Resultset);

                    if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                        return dt_Resultset;
                    else
                        return tempResultSet;
                }
            }
            catch (Exception ex)
            {
                //createLog("JLI_CustomerLineups", "JLI_getItemAccessoriesInfo", 190, "ex - " + ex.Message);
            }

            return dt_Resultset;

        }
        public DataTable ue_JLI_CLM_GetFabGradePriceInfo(string custNum, string item, int? custItemSeq, short? priceBook)
        {           

            DataTable tempResultSet = new DataTable();
            tempResultSet.Columns.Add("FabricGrade", typeof(string));
            tempResultSet.Columns.Add("Price", typeof(string));
            tempResultSet.Columns.Add("EffectDate", typeof(string));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.JLI_GetFabGradePriceInfo;
                   IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,custNum,item,custItemSeq,priceBook);
                    sqlCommand.CommandType = CommandType.Text;
                    //db.AddCommandParameterWithValue(sqlCommand, "@CustNum", custNum);
                    //db.AddCommandParameterWithValue(sqlCommand, "@Item", item);
                    //db.AddCommandParameterWithValue(sqlCommand, "@CustItemSeq", custItemSeq);
                    //db.AddCommandParameterWithValue(sqlCommand, "@PriceBook", priceBook);
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    dt_Resultset.Load(Resultset);

                    if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                        return dt_Resultset;
                    else
                        return tempResultSet;
                }
            }
            catch (Exception ex)
            {
                //createLog("JLI_CustomerLineups", "ue_JLI_CLM_GetFabGradePriceInfo", 190, "ex - " + ex.Message);
            }

            return dt_Resultset;
        }




    }
}
