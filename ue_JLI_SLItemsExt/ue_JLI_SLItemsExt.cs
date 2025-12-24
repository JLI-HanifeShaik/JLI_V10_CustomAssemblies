using Mongoose.Core.Common;
using Mongoose.Core.Extensions;
using Mongoose.IDO;
using Mongoose.IDO.Metadata;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ue_JLI.Trigger;
using ue_JLI_SLItemsExt.Properties;

namespace ue_JLI_SLItemsExt
{
    public class ue_JLI_SLItemsExt : IDOExtensionClass
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

        public int ue_JLI_ItemCollecDataUpdate(string inpItem,
                                               string inpPropertyName,
                                               string inpReasonDate,
                                               string inpNewStatus,
                                               string inpOldStatus,
                                               string inpNewItemLifeCycleStatus,
                                               string inpOldItemLifeCycleStatus)
        {

            DateTime dt;
            string formattedReasonDate;

            if (string.IsNullOrEmpty(inpReasonDate))
            {
                dt = DateTime.Now;
                formattedReasonDate = dt.ToString("yyyy-MM-dd");
            }
            else
            {
                dt = DateTime.ParseExact(inpReasonDate, "yyyyMMdd HH:mm:ss.fff", null);
                formattedReasonDate = dt.ToString("yyyy-MM-dd");
            }

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable(); 
            try
            {
                query = Resources.ue_JLI_ItemCollecDataUpdate;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,inpItem,inpPropertyName, formattedReasonDate, inpNewStatus,inpOldStatus,inpNewItemLifeCycleStatus,inpOldItemLifeCycleStatus);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    dt_Resultset.Load(Resultset);
                } 
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_SLItemsExt", "ue_JLI_ItemCollecAndItemAcsseUpdation", 56, "ex - " + ex.Message);               
            }

            foreach (DataRow row in dt_Resultset.Rows)
            {
                 string Item = row["Item"].ToString();
                string CollecStatus = row["Status"].ToString();
                string CollecDroppedDate = row["DroppedDate"].ToString();
                DateTime currDateTime = DateTime.Now;
                string formattedcurrDateTime = currDateTime.ToString("yyyy-MM-dd");

                LoadCollectionRequestData loadRequest = new LoadCollectionRequestData();
                LoadCollectionResponseData loadResponse;
                string sFilter = string.Format("CollectionID = '{0}' And Stat <> '{1}'", Item, CollecStatus);
                loadRequest.IDOName = "JLI_Collections";
                loadRequest.Filter = sFilter;
                loadRequest.PropertyList.Add("Stat");
                loadRequest.PropertyList.Add("DateDropped");
                loadRequest.RecordCap = 1;
                loadResponse = Context.Commands.LoadCollection(loadRequest);
                //createLog("ue_JLI_SLItemsExt", "ue_JLI_ItemCollecAndItemAcsseUpdation", 56, "JLI_Collections - " + loadResponse.Items.Count);
                if (loadResponse.Items.Count > 0)
                {
                    try
                    {
                        UpdateCollectionRequestData updateColRequestData;
                        UpdateCollectionResponseData updateColResponseData;
                        IDOUpdateItem idoUpdateItem;

                        updateColRequestData = new UpdateCollectionRequestData();
                        updateColRequestData.IDOName = "JLI_Collections";
                        idoUpdateItem = new IDOUpdateItem();

                        idoUpdateItem.Action = UpdateAction.Update;
                        idoUpdateItem.Properties.Add("Stat", CollecStatus);
                        idoUpdateItem.Properties.Add("DateDropped", CollecDroppedDate);
                        idoUpdateItem.ItemID = loadResponse.Items[0].ItemID;
                        updateColRequestData.Items.Add(idoUpdateItem);
                        updateColResponseData = this.Context.Commands.UpdateCollection(updateColRequestData);
                    }
                    catch (Exception ex)
                    {
                        //createLog("ue_JLI_SLItemsExt", "ue_JLI_ItemCollecAndItemAcsseUpdation", 56, "JLI_Collections Ex - ");
                    }
                }

            };




            return 0;
        }


        public int ue_JLI_ItemAcsseDataInsertAndUpdate(string inpItem,
                                                       string inpPropertyName,
                                                       string inpReasonDate,
                                                       string inpNewStatus,
                                                       string inpOldStatus,
                                                       string inpNewItemLifeCycleStatus,
                                                       string inpOldItemLifeCycleStatus,
                                                       string inpNewSchedItemType,
                                                       string inpOldSchedItemType)
        {
            if (inpItem.Contains("-"))
                return 0;


            DateTime dt;
            string formattedReasonDate;           

            if (string.IsNullOrEmpty(inpReasonDate))
            {
                dt = DateTime.Now;
                formattedReasonDate = dt.ToString("yyyy-MM-dd");                
            }
            else
            {
                dt = DateTime.ParseExact(inpReasonDate, "yyyyMMdd HH:mm:ss.fff", null);
                formattedReasonDate = dt.ToString("yyyy-MM-dd");
            }

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    query = Resources.ue_JLI_ItemAcsseDataInsertAndUpdate;
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query,inpItem,inpPropertyName, formattedReasonDate, inpNewStatus,inpOldStatus,inpNewItemLifeCycleStatus,inpOldItemLifeCycleStatus,inpNewSchedItemType,inpOldSchedItemType); 
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    dt_Resultset.Load(Resultset);
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_SLItemsExt", "ue_JLI_ItemCollecAndItemAcsseUpdation", 56, "ex1 - " + ex.Message);
            }
            //createLog("ue_JLI_SLItemsExt", "ue_JLI_ItemCollecAndItemAcsseUpdation", 56, "Count - " + dt_Resultset.Rows.Count);
            try
            {
                foreach (DataRow row in dt_Resultset.Rows)
                {
                    string item = row["Item"].ToString();
                    string processType = row["ProcessType"].ToString();
                    string status = row["Status"].ToString();
                    string droppedDate = row["DroppedDate"].ToString();
                    DateTime currDateTime = DateTime.Now;
                    string formattedcurrDateTime = currDateTime.ToString("yyyy-MM-dd");

                    if (processType == "I")
                    {
                        UpdateCollectionRequestData updateRequest;
                        IDOUpdateItem updateItem;
                        updateRequest = new UpdateCollectionRequestData("ue_JLI_ItemCollecAccessories");
                        updateItem = new IDOUpdateItem(UpdateAction.Insert);
                        updateItem.Properties.Add("Item", item);
                        updateItem.Properties.Add("Stat", "P");
                        updateItem.Properties.Add("DateAdded", formattedcurrDateTime);
                        updateRequest.Items.Add(updateItem);
                        this.Context.Commands.UpdateCollection(updateRequest);                        
                    }
                    else if(processType == "D" || processType == "U")
                    {
                        LoadCollectionRequestData loadRequest = new LoadCollectionRequestData();
                        LoadCollectionResponseData loadResponse;
                        string sFilter = string.Format("Item = '{0}'", item);
                        loadRequest.IDOName = "ue_JLI_ItemCollecAccessories";
                        loadRequest.Filter = sFilter;
                        loadRequest.PropertyList.Add("Stat");
                        loadRequest.PropertyList.Add("DateDropped");
                        loadRequest.RecordCap = 1;
                        loadResponse = Context.Commands.LoadCollection(loadRequest);

                        if (loadResponse.Items.Count > 0)
                        {
                            UpdateCollectionRequestData updateColRequestData;
                            UpdateCollectionResponseData updateColResponseData;
                            IDOUpdateItem idoUpdateItem;

                            updateColRequestData = new UpdateCollectionRequestData();
                            updateColRequestData.IDOName = "ue_JLI_ItemCollecAccessories";
                            idoUpdateItem = new IDOUpdateItem();                           

                            if (processType == "D")
                            {
                                idoUpdateItem.Action = UpdateAction.Delete;
                                idoUpdateItem.ItemID = loadResponse.Items[0].ItemID;
                                updateColRequestData.Items.Add(idoUpdateItem);
                                updateColResponseData = this.Context.Commands.UpdateCollection(updateColRequestData);
                            }
                            else if(processType == "U")
                            {
                                idoUpdateItem.Action = UpdateAction.Update;
                                idoUpdateItem.Properties.Add("Stat", status);
                                idoUpdateItem.Properties.Add("DateDropped", droppedDate);
                                idoUpdateItem.ItemID = loadResponse.Items[0].ItemID;
                                updateColRequestData.Items.Add(idoUpdateItem);
                                updateColResponseData = this.Context.Commands.UpdateCollection(updateColRequestData);
                            }

                        }
                    }                
                
                };
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_SLItemsExt", "ue_JLI_ItemCollecAndItemAcsseUpdation", 56, "ex2 - " + ex.Message);
            }



            return 0;





        }



        //[IDOAddEventHandler(IDOEvent.PreUpdateCollection)]
        private void JLI_SerialItem_mstIUp(object sender, IDOEventArgs args)
        {
            try
            {
                IDOUpdateEventArgs updateArgs = (IDOUpdateEventArgs)args;
                UpdateCollectionRequestData updateRequest = (UpdateCollectionRequestData)args.RequestPayload;

                string UserName = IDORuntime.Context.UserName;

                //We have a Response Object
                if (updateRequest != null)
                {
                    //Per Item
                    for (int i = 0; i < updateRequest.Items.Count; i++)
                    {
                        #region Init
                        var CurrentItem = updateRequest.Items[i];
                        var OriginalItem = new LoadCollectionResponseData();
                        var CurrentItemProperties = CurrentItem.Properties.AsEnumerable().Select(a => a.Name);
                        //Add Properties that need to be loaded if not supplied.
                        var MustLoadProperties = "Item,Stat,itmUf_SchedItemType,itmUf_ItemLifeCycleStatus".Split(',').AsEnumerable();
                        Guid? CurrentRP = null;
                        DateTime? CurrentRD = null;
                        short InsertFlag = 0;
                        short DeleteFlag = 0;
                        short statModified = 0;
                        short schedItemTypeModified = 0;
                        short lifeCycleStatusModified = 0;
                        string processType = string.Empty;
                        string status = string.Empty;
                        string droppedDate = string.Empty;
                        DateTime currDateTime = DateTime.Now;
                        string formattedcurrDateTime = currDateTime.ToString("yyyy-MM-dd");
                        LoadCollectionResponseData loadResponse;


                        if (CurrentItem.Action == UpdateAction.Delete)
                        {
                            DeleteFlag = 1;
                        }
                        else if (CurrentItem.Action == UpdateAction.Update)
                        {
                            string OverrideProperties = string.Join(",", CurrentItemProperties);
                            foreach (var _item in MustLoadProperties)
                            {
                                if (!CurrentItemProperties.Contains(_item))
                                {
                                    OverrideProperties += $",{_item}";
                                }
                            }
                            //Load Original Values
                            OriginalItem = Context.Commands.GetExistingItemData(updateRequest, CurrentItem, ref CurrentRP, ref CurrentRD, "itm", OverrideProperties);
                        }
                        else if (CurrentItem.Action == UpdateAction.Insert)
                        {
                            InsertFlag = 1;
                        }
                        #endregion

                        #region Capture Values
                        string item = null;
                        if (CurrentItemProperties.Contains("Item"))
                        {
                            item = CurrentItem.Properties["Item"].Value;
                            //createLog("", "", 1, "Item " + item);
                        }
                        string stat = null;
                        if (CurrentItemProperties.Contains("Stat"))
                        {
                            stat = CurrentItem.Properties["Stat"].Value;
                            if (CurrentItem.Properties["Stat"].Modified) { statModified = 1; }
                        }
                        string itmUf_ItemLifeCycleStatus = null;
                        if (CurrentItemProperties.Contains("itmUf_ItemLifeCycleStatus"))
                        {
                            itmUf_ItemLifeCycleStatus = CurrentItem.Properties["itmUf_ItemLifeCycleStatus"].Value;
                            if (CurrentItem.Properties["itmUf_ItemLifeCycleStatus"].Modified) { lifeCycleStatusModified = 1; }
                        }
                        string itmUf_SchedItemType = null;
                        if (CurrentItemProperties.Contains("itmUf_SchedItemType"))
                        {
                            itmUf_SchedItemType = CurrentItem.Properties["itmUf_SchedItemType"].Value;
                            if (CurrentItem.Properties["itmUf_SchedItemType"].Modified) { schedItemTypeModified = 1; }
                        }

                        var org_Stat = OriginalItem[0, "Stat"].Value;
                        var org_itmUf_ItemLifeCycleStatus = OriginalItem[0, "itmUf_ItemLifeCycleStatus"].Value;
                        var org_itmUf_SchedItemType = OriginalItem[0, "itmUf_SchedItemType"].Value;                        
                        #endregion

                        if (statModified == 1 || lifeCycleStatusModified == 1 || schedItemTypeModified == 1)
                        {
                            loadResponse = this.Context.Commands.LoadCollection("ue_JLI_ItemCollecAccessories", "Item", string.Format("Item = '{1}'",item), "", 1);
                            if (itmUf_SchedItemType.MGIsNullOrEmpty()) { itmUf_SchedItemType = org_itmUf_SchedItemType; }
                            if (itmUf_ItemLifeCycleStatus.MGIsNullOrEmpty()) { itmUf_ItemLifeCycleStatus = org_itmUf_ItemLifeCycleStatus; }
                            if (stat.MGIsNullOrEmpty()) { stat = org_Stat; }
                            if (itmUf_SchedItemType == "Accessory")
                            {
                                if (loadResponse.Items.Count > 0)
                                {
                                    if (itmUf_ItemLifeCycleStatus.MGIsNullOrEmpty() || itmUf_ItemLifeCycleStatus == "E-Ready")
                                    { if (org_Stat == "A" && stat == "O") { status = "D"; } }
                                    else { if (stat == "O") { status = "D"; } }
                                    //Update
                                }
                                {
                                    if (stat == "A" && (itmUf_ItemLifeCycleStatus.MGIsNullOrEmpty() || itmUf_ItemLifeCycleStatus == "E-Ready"))
                                    {
                                        //Insert
                                    }
                                }                                
                            }
                            else
                            {
                                //Delete
                            }

                        } 




                    }//For Loop
                }//Update Request Found
            }
            catch (Exception e)
            {
                throw new Exception($"PreUpdateCollection - SLItemsExt\r\n{e.Message}");
            }
        }










    }
}
