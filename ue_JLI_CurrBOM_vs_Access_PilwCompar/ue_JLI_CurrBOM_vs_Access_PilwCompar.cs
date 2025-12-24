using Mongoose.IDO.Protocol;
using Mongoose.IDO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using static Mongoose.Core.Common.QuickKeywordParser;
using Mongoose.IDO.Metadata;
using System.Collections;
using Mongoose.Core.Common;

namespace ue_JLI_CurrBOM_vs_Access_PilwCompar
{
    public class ue_JLI_CurrBOM_vs_Access_PilwCompar : IDOExtensionClass
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
                // Optionally, log the exception or handle it accordingly
            }
        }

        public void JLI_LoadCollectionRequestData(string collectionName, string filter, List<string> propertyList, int recordCap, bool distinct, string orderBy, ref LoadCollectionResponseData loadResponse)
        {
            LoadCollectionRequestData loadRequest = new LoadCollectionRequestData();
            string strFilter = filter;

            loadRequest.IDOName = collectionName;
            loadRequest.Filter = filter;
            loadRequest.OrderBy = orderBy;

            foreach (string propertyName in propertyList)
            {
                loadRequest.PropertyList.Add(propertyName);
            }

            loadRequest.RecordCap = recordCap;
            loadRequest.Distinct = distinct;

            // Load collection of items based on the request
            loadResponse = Context.Commands.LoadCollection(loadRequest);
        }

        public DataTable ue_JLI_CurrBOM_vs_Access_PilwComparison()
        {
            List<dynamic> currBOM_List = new List<dynamic>();

            LoadCollectionResponseData loadResponse = new LoadCollectionResponseData();
            List<string> propertyList = new List<string>();
            string strFilter = string.Empty;
            string item = string.Empty;
            string jobMatl_Item = string.Empty;
            string accessory_Item = string.Empty;


            propertyList.AddRange(new string[] { "Item", "JobMatl_Item", "DerIsMatchedPillow" });
            //strFilter = string.Format("JobMatl_ItemProductCode = '{0}' And Item = '{1}'", "PILW", "S125629RFX");
            strFilter = string.Format("JobMatl_ItemProductCode = 'PILW' And ItemStat = 'A' And ItemProductCode In  ('FGST','FGBD')");
            //collectionName,filter,propertyList,recordCap,distinct,orderBy,loadResponse
            JLI_LoadCollectionRequestData("ue_JLI_CurrBOM_vs_Access_PilwCompar1", strFilter, propertyList, 0, true, "Item", ref loadResponse);
            //createLog("ue_JLI_CurrBOM_vs_Access_PilwCompar", "ue_JLI_CurrBOM_vs_Access_PilwComparison", 84, (loadResponse.Items.Count).ToString());


            if (loadResponse.Items.Count > 0)
            {
                for (int i = 0; i < loadResponse.Items.Count; i++)
                {
                    if (string.IsNullOrEmpty(loadResponse[i, "DerIsMatchedPillow"].Value))
                    {
                        item = loadResponse[i, "Item"].Value;
                        jobMatl_Item = loadResponse[i, "JobMatl_Item"].Value;
                        currBOM_List.Add(new { item = item, jobMatl_Item = jobMatl_Item });
                        //createLog("ue_JLI_CurrBOM_vs_Access_PilwCompar", "ue_JLI_CurrBOM_vs_Access_PilwComparison", 98, item + "---" + jobMatl_Item);
                    }                    
                }
            }

            List<dynamic> accessory_List = new List<dynamic>(); 

            propertyList.Clear();
            propertyList.AddRange(new string[] { "Item", "AccessoryItem", "DerIsMatchedPillow" });
            //strFilter = string.Format("AccessoryProductCode = '{0}' And Item = '{1}'", "PILW", "S125629RFX");
            strFilter = string.Format("AccessoryProductCode = 'PILW' And ItemStat = 'A' And ItemProductCode In  ('FGST','FGBD')");
            //collectionName,filter,propertyList,recordCap,distinct,orderBy,loadResponse
            JLI_LoadCollectionRequestData("ue_JLI_CurrBOM_vs_Access_PilwCompar2", strFilter, propertyList, 0, true, "Item", ref loadResponse);
            ///createLog("ue_JLI_CurrBOM_vs_Access_PilwCompar", "ue_JLI_CurrBOM_vs_Access_PilwComparison", 84, (loadResponse.Items.Count).ToString());
            if (loadResponse.Items.Count > 0)
            {
                for (int i = 0; i < loadResponse.Items.Count; i++)
                {
                    if (string.IsNullOrEmpty(loadResponse[i, "DerIsMatchedPillow"].Value))
                    {
                        item = loadResponse[i, "Item"].Value;
                        accessory_Item = loadResponse[i, "AccessoryItem"].Value;
                        accessory_List.Add(new { item = item, accessory_Item = accessory_Item });
                        //createLog("ue_JLI_CurrBOM_vs_Access_PilwCompar", "ue_JLI_CurrBOM_vs_Access_PilwComparison", 98, item + "---" + jobMatl_Item);
                    }
                }
            }

            DataTable resultSet = new DataTable("MyTable");
            resultSet.Columns.Add("Item1", typeof(string));
            resultSet.Columns.Add("JobMatl_Item", typeof(string));
            resultSet.Columns.Add("Item2", typeof(string));
            resultSet.Columns.Add("AccessoryItem", typeof(string));

            string filterItem = string.Empty;
            bool exsist = false;

            foreach (var bom in currBOM_List)
            {
                exsist = false;
                filterItem = bom.item;

                var filteredList = accessory_List
                .Where(a => a.item == filterItem)
                .ToList();

                // Output the filtered list
                foreach (var accessory in filteredList)
                {
                    exsist = true;
                    resultSet.Rows.Add(bom.item, bom.jobMatl_Item, accessory.item, accessory.accessory_Item);
                }

                if (exsist == false)
                { resultSet.Rows.Add(filterItem, bom.jobMatl_Item, null,null);}
            }

            foreach (var accessory in accessory_List)
            {
                exsist = false;
                filterItem = accessory.item;

                var filteredList = currBOM_List
                .Where(a => a.item == filterItem)
                .ToList();

                // Output the filtered list
                foreach (var bom in filteredList)
                { exsist = true; }

                if (exsist == false)
                { resultSet.Rows.Add(null, null, filterItem, accessory.accessory_Item); }
            }

            return resultSet;

        }//ue_JLI_CurrBOM_vs_Access_PilwCompar



    }
}
