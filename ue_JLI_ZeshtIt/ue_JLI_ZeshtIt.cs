using CSI.FullTrust.IDM;
using CSI.MG;
using CSI.Production.APS;
using Infor.DocumentManagement.ICP;
using Mongoose.IDO;
using Mongoose.IDO.DataAccess;
using Mongoose.IDO.Protocol;
using Mongoose.MGCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_ZeshtIt.Properties;

namespace ue_JLI_ZeshtIt
{
    public class ue_JLI_ZeshtIt : CSIExtensionClassBase
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
               
            }
        }

        [IDOMethod(MethodFlags.RequiresTransaction, "infobar")]
        public int ue_JLI_DocTrackAttachToIDM(string fromFolder,
                                              string toFolder, 
                                              int? recordCap,
                                              short? attachToIDM,
                                              string entityName,
                                              string entityType,
                                              string processType,
                                              ref string infobar)
        {
            if (recordCap < 1)
                recordCap = 1;
            if (attachToIDM != 1)
                attachToIDM = 0;

            if (string.IsNullOrEmpty(fromFolder) || string.IsNullOrEmpty(toFolder))
            {
                infobar = "Input values missing (fromFolder,toFolder).";
                return 0;
            }
            if (attachToIDM == 1 && (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(processType)) )
            {
                infobar = "Input values missing (entityName,entityType,processType).";
                return 0;
            }

            string errMsg = string.Empty;
            string logicalFolderNameFrom = fromFolder;
            string logicalFolderName_Archive = toFolder;

            // Get list of files from method
            DataTable files = ue_JLI_GetFileList(logicalFolderNameFrom, ref infobar);

            if (!string.IsNullOrEmpty(infobar))
            {
                infobar = "Error while getting file list: " + infobar;
                return 0;
            }

            if (files == null || files.Rows.Count == 0)
            {
                infobar = "No files found.";
                return 0;
            }

            string serverFrom = string.Empty;
            string folderTemplateFrom = string.Empty;
            string accessDepthFrom = string.Empty;
            string fileSpecFrom = string.Empty;

            string serverTo = string.Empty;
            string folderTemplateTo = string.Empty;
            string accessDepthTo = string.Empty;
            string fileSpecTo = string.Empty;

            string fileName = string.Empty;
            string fileDesc = string.Empty;
            int moved = 0;
            byte[] fileContent = null;
            string parsedFileSpec = string.Empty;

            string filter = string.Empty;
            string coNum = string.Empty;
            string rmaNum = string.Empty;
            string loadNo = string.Empty;
            string VendNum = string.Empty;            
            string ReceiptNum = string.Empty;            
            string PONum = string.Empty;
            string idoName = string.Empty;
            string propertyList = string.Empty;

            // Create instance of your file server extension class
            FileServerExtension fileServer = new FileServerExtension();

            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderNameFrom, ref serverFrom, ref folderTemplateFrom, ref accessDepthFrom, ref infobar);
            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName_Archive, ref serverTo, ref folderTemplateTo, ref accessDepthTo, ref infobar);
            if (!string.IsNullOrEmpty(infobar) || string.IsNullOrEmpty(serverFrom) || string.IsNullOrEmpty(serverTo))
            {
                infobar = "Invalid Input Values (fromFolder,toFolder).";
                return 0;
            }        
            

            try
            {
                foreach (DataRow row in files.Rows)
                {
                    fileName = row["DerFileName"]?.ToString();

                    if(processType == "CustomerOrder")
                    {
                        if (fileName.Length >= 10)
                            coNum = fileName.Substring(0, 10);
                        else
                            continue;                        
                        fileDesc = coNum + " CustomerOrder Doc Track File";
                        idoName = "SLCos";
                        propertyList = "CustNum,CoNum,CustPo";
                        filter = string.Format("CoNum = '{0}' ", coNum);
                    }
                    else if (processType == "RMA")
                    {
                        //R000001149-20191210
                        if (fileName.Length >= 10)
                            rmaNum = fileName.Substring(0, 10);
                        else
                            continue;
                        fileDesc = rmaNum + " RMA Doc Track File";
                        idoName = "SLRmas";
                        propertyList = "RmaNum";
                        filter = string.Format("RmaNum = '{0}' ", rmaNum);
                    }
                    else if (processType == "BOL_Signed")
                    {
                        //496861-20251110-0001
                        if (fileName.IndexOf("-") > 0)
                        {
                            loadNo = fileName.Substring(0, (fileName.IndexOf("-")) );
                        }
                        else if (fileName.Length >= 6)
                            loadNo = fileName.Substring(0, 6);
                        else
                            continue;
                        fileDesc = loadNo + " BOL Signed Doc Track File";
                        idoName = "JLI_LoadHdrs";
                        propertyList = "LoadNo";
                        filter = string.Format("LoadNo = '{0}' ", loadNo);
                    }
                    else if (processType == "Comercial Docs")
                    {
                        //496861-20251110-0001
                        if (fileName.IndexOf("-") > 0)
                        {
                            loadNo = fileName.Substring(0, (fileName.IndexOf("-")));
                        }
                        else if (fileName.Length >= 6)
                            loadNo = fileName.Substring(0, 6);
                        else
                            continue;
                        fileDesc = loadNo + " Comercial Doc Track File";
                        idoName = "JLI_LoadHdrsPO";
                        propertyList = "LoadNo";
                        filter = string.Format("LoadNo = '{0}' ", loadNo);
                    }
                    else if (processType == "Comercial Invoice")
                    {
                        //496861-20251110-0001
                        if (fileName.IndexOf("-") > 0)
                        {
                            loadNo = fileName.Substring(0, (fileName.IndexOf("-")));
                        }
                        else if (fileName.Length >= 6)
                            loadNo = fileName.Substring(0, 6);
                        else
                            continue;
                        fileDesc = loadNo + " Comercial Invoice Doc Track File";
                        idoName = "JLI_LoadHdrsPO";
                        propertyList = "LoadNo";
                        filter = string.Format("LoadNo = '{0}' ", loadNo);
                    }
                    else if (processType == "Comercial Packing List")
                    {
                        //496861-20251110-0001
                        if (fileName.IndexOf("-") > 0)
                        {
                            loadNo = fileName.Substring(0, (fileName.IndexOf("-")));
                        }
                        else if (fileName.Length >= 6)
                            loadNo = fileName.Substring(0, 6);
                        else
                            continue;
                        fileDesc = loadNo + " Comercial Packing List Doc Track File";
                        idoName = "JLI_LoadHdrsPO";
                        propertyList = "LoadNo";
                        filter = string.Format("LoadNo = '{0}' ", loadNo);
                    }
                    else if (processType == "PO Loads")
                    {
                        //496861-20251110-0001
                        if (fileName.IndexOf("-") > 0)
                        {
                            loadNo = fileName.Substring(0, (fileName.IndexOf("-")));
                        }
                        else if (fileName.Length >= 6)
                            loadNo = fileName.Substring(0, 6);
                        else
                            continue;
                        fileDesc = loadNo + " Doc Track File";
                        idoName = "JLI_LoadHdrsPO";
                        propertyList = "LoadNo";
                        filter = string.Format("LoadNo = '{0}' ", loadNo);
                    }
                    else if (processType == "PO Receving Doc-MX")
                    {
                        //496861-20251110-0001
                        if (fileName.IndexOf("-") > 0)
                        {
                            loadNo = fileName.Substring(0, (fileName.IndexOf("-")));
                        }
                        else if (fileName.Length >= 6)
                            loadNo = fileName.Substring(0, 6);
                        else
                            continue;
                        fileDesc = loadNo + " PO Receving (MX) Doc Track File";
                        idoName = "JLI_LoadHdrsPO";
                        propertyList = "LoadNo";
                        filter = string.Format("LoadNo = '{0}' ", loadNo);
                    }                    
                    else if (processType == "JLM_PO")
                    {
                        //JLMPO001//PM002656
                        if (fileName.Length >= 8)
                            PONum = fileName.Substring(0, 8);
                        else
                            continue;
                        fileDesc = PONum + " Doc Track File";
                        idoName = "ue_JLI_JLM_PUR_PoHdrs";
                        propertyList = "PoNum";
                        filter = string.Format("PoNum = '{0}' ", PONum);
                    }
                    else if (processType == "JLM_PO_Receipt")
                    {
                        //JLMR0001609
                        if (fileName.Length >= 11)
                            ReceiptNum = fileName.Substring(0, 11);
                        else
                            continue;
                        fileDesc = ReceiptNum + " Doc Track File";
                        idoName = "ue_JLI_JLM_PUR_ReceiptHdrs";
                        propertyList = "ReceiptNum";
                        filter = string.Format("ReceiptNum = '{0}' ", ReceiptNum);
                    }
                    else if (processType == "1_W9_OtherDocs")
                    {
                        //R000023
                        if (fileName.Length >= 7)
                            VendNum = fileName.Substring(0, 7);
                        else
                            continue;
                        fileDesc = VendNum + " Doc Track File";
                        idoName = "SLVendors";
                        propertyList = "VendNum";
                        filter = string.Format("VendNum = '{0}' ", VendNum);
                    }
                    //1_W9_OtherDocs
                    else if (attachToIDM == 1)
                    {
                        infobar = "Logic not implimented for " + processType;
                        return 0;
                    }
                    

                    fileSpecFrom = ue_JLI_GetFileSpec(folderTemplateFrom, fileName, "*.*", accessDepthFrom, true);
                    if(attachToIDM == 1)
                    {
                        fileServer.GetFileContent(fileSpecFrom, serverFrom, logicalFolderNameFrom, ref fileContent, ref parsedFileSpec, ref infobar);                    
                        ue_JLI_AddContentToIDM(entityName, entityType, "JLI", fileName, fileContent, fileDesc, idoName, propertyList, filter, ref errMsg);
                    }

                    if (string.IsNullOrEmpty(errMsg))
                    {                        
                        fileSpecTo = ue_JLI_GetFileSpec(folderTemplateTo, fileName, "*.*", accessDepthTo, true);
                        fileServer.MoveFileServerToServer(ref infobar, ref moved, serverFrom, fileSpecFrom, logicalFolderNameFrom, serverTo, fileSpecTo, logicalFolderName_Archive, 1, 1);
                    }
                    recordCap = recordCap - 1;

                    if (recordCap < 1)
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Processing File: {ex.Message}");
            }

            infobar = filter + "  " + errMsg;

            return 0;
        }
        private DataTable ue_JLI_GetFileList(string logicalFolderName, ref string infobar)
        {
            string fileServer = string.Empty;
            string folderTemplate = string.Empty;
            string accessDepth = string.Empty;
            string getFileAction = "File";
            string errMsg = string.Empty;
            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName, ref fileServer, ref folderTemplate, ref accessDepth, ref errMsg);
            string fileSpec = ue_JLI_GetFileSpec(folderTemplate, "", "*.*", accessDepth, true);
            FileServerExtension fileServerExtension = new FileServerExtension();
            // Assuming GetFileList returns a DataTable
            DataTable files = fileServerExtension.GetFileList(fileSpec, fileServer, logicalFolderName, getFileAction, ref errMsg);

            infobar = errMsg;
            return files;
        }
        private void ue_JLI_GetFileServerInfoByLogicalFolderName(string logicalFolderName, ref string fileServerName, ref string folderTemplate, ref string accessDepth, ref string errMsg)
        {
            using (ApplicationDB appdb = IDORuntime.Context.CreateApplicationDB())
            {
                using (IDbCommand sql = appdb.CreateCommand())
                {
                    try
                    {
                        sql.CommandType = CommandType.StoredProcedure;
                        sql.CommandText = "GetFileServerInfoByLogicalFolderNameSp";

                        appdb.AddCommandParameterWithValue(sql, "LogicalFolderName", logicalFolderName, ParameterDirection.Input).Size = 100;
                        appdb.AddCommandParameterWithValue(sql, "ServerName", fileServerName, ParameterDirection.InputOutput).Size = 100;
                        appdb.AddCommandParameterWithValue(sql, "FolderTemplate", folderTemplate, ParameterDirection.InputOutput).Size = 100;
                        appdb.AddCommandParameterWithValue(sql, "FolderAccessDepth", accessDepth, ParameterDirection.InputOutput).Size = 100;

                        appdb.ExecuteNonQuery(sql);

                        fileServerName = ((IDbDataParameter)sql.Parameters[1]).Value.ToString();
                        folderTemplate = ((IDbDataParameter)sql.Parameters[2]).Value.ToString();
                        accessDepth = ((IDbDataParameter)sql.Parameters[3]).Value.ToString();
                    }
                    catch (Exception ex)
                    {
                        errMsg = ex.Message;
                    }
                }
            }
        }
        private string ue_JLI_GetFileSpec(string folderTemplate, string fileName, string fileExtension, string accessDepth, bool useServerCheck)
        {
            string useServerCheckStr;
            string fileSpec = string.Empty;

            if (useServerCheck)
                useServerCheckStr = "1";
            else
                useServerCheckStr = "0";

            if (fileName.LastIndexOf(@"\") >= 0)
                fileName = fileName.Substring(fileName.LastIndexOf(@"\") + 1).TrimStart('\\');

            fileSpec = folderTemplate.TrimEnd('/') + @"\" + fileName + "|" + fileExtension + "|" + accessDepth + "|" + useServerCheckStr;

            return fileSpec;
        }
        private void ue_JLI_AddContentToIDM(string entityName,
                                            string entityType,
                                            string accountingEntity,
                                            string filename,
                                            byte[] fileContent,
                                            string description,
                                            string idoCollection,
                                            string idoPropertyList,
                                            string idoFilter,
                                            ref string errMsg)
        {

            string refRowPointer = string.Empty;

            // Create fixed attribute list
            List<string> additionalAttributes = new List<string>(new string[] { "Description", "EntityType", "AccountingEntity" });

            // Get Name/Value pairs for IdoPropertyList
            DataTable dt = new DataTable();
            using (DataTableReader reader = new DataTableReader(ue_JLI_CLM_GetIDONameValueData(idoCollection, idoPropertyList, idoFilter,ref errMsg)))
            {
                dt.Load(reader);
            }
            if(!string.IsNullOrEmpty(errMsg)) 
            {
                return; 
            }
            //createLog("", "", 1, dt.Rows.Count.ToString());

            // add rows for attribute values from input parms
            foreach (string attr in additionalAttributes)
            {
                DataRow rw = dt.NewRow();
                rw["Name"] = attr;
                rw["Datatype"] = "String";

                switch (attr)
                {
                    case "EntityType":
                        rw["Value"] = entityType;
                        break;

                    case "Description":
                        rw["Value"] = description;
                        break;

                    case "AccountingEntity":
                        rw["Value"] = accountingEntity;
                        break;

                }

                dt.Rows.Add(rw);
            }

            // assign byte array as resource
            CMResource res = new CMResource(filename, fileContent);

            // define ACL = EntityType
            CMAccessControlList ACLList = new CMAccessControlList(entityType);

            // create search string
            string SearchString = idoFilter.Replace("'", "\"");

            // create IDM item and assign static properties
            CMItem item = new CMItem
            {
                EntityName = entityName,
                Filename = filename,
                AccessControlList = ACLList
            };

            // Set attributes from name/value pair
            foreach (DataRow row in dt.Rows)
            {
                //createLog("", "", 2, row["Name"].ToString() + "---" + row["Value"].ToString());
                switch (row["Name"].ToString())
                {
                    case "RowPointer":
                        refRowPointer = row["Value"].ToString();
                        break;

                    default:
                        item.SetAttributeValue(row["Name"].ToString(), row["Value"].ToString());
                        break;
                }                
            }


            try
            {
                item.Resources.Add(res);
                IIDMControllerFactory oIDMControllerFactory = this.GetService<IIDMControllerFactory>();
                IIDMController trustedIDMController = oIDMControllerFactory.Create();

                if (fileContent.Length < 52428800)
                    trustedIDMController.AddItem(item);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;                
            }



        }
        private DataTable ue_JLI_CLM_GetIDONameValueData(string IDOName, string PropList, string Filter,ref string errMsg)
        {
            // add RowPointer to property list
            PropList += ",RowPointer";

            // convert property list to array
            string[] aryPropList = PropList.Split(',');
            for (int i = 0; i < aryPropList.Length; i++)
            {
                aryPropList[i] = aryPropList[i].Trim();
            }

            // create datatable to store name,value,datatype data
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            dt.Columns.Add("Datatype", typeof(string));

            try
            {
                // retrieve collection
                LoadCollectionRequestData IDOrequest = new LoadCollectionRequestData();
                LoadCollectionResponseData IDOresponse = null;

                GetPropertyInfoResponseData propInfo = new GetPropertyInfoResponseData();

                propInfo = this.Context.Commands.GetPropertyInfo(IDOName);

                IDOrequest.IDOName = IDOName;
                IDOrequest.PropertyList.SetProperties(aryPropList);
                IDOrequest.Filter = Filter;
                IDOrequest.RecordCap = 1;
                IDOresponse = this.Context.Commands.LoadCollection(IDOrequest);

                if(IDOresponse.Items.Count < 0)
                {
                    errMsg = "Details Not found";                    
                }

                // loop through properties & assign values to datatable rows
                foreach (IDOItem ii in IDOresponse.Items)
                {
                    for (int i = 0; i < aryPropList.Length; i++)
                    {
                        DataRow row = dt.NewRow();
                        row["Name"] = aryPropList[i];

                        foreach (PropertyInfo idoprop in propInfo.Properties)
                        {
                            if (idoprop.Name == aryPropList[i])
                            {
                                switch (idoprop.DataType)
                                {
                                    case "NumSortedString":
                                        row["Datatype"] = "String";
                                        row["Value"] = ii.PropertyValues[i].GetValue("").ToString();
                                        break;

                                    case "Date":
                                        row["Datatype"] = idoprop.DataType;
                                        row["Value"] = ii.PropertyValues[i]
                                            .GetValue<DateTime>()
                                            .ToString("yyyy-MM-ddTHH:mm:ssK");
                                        break;

                                    case "GUID":
                                        row["Datatype"] = "String";
                                        row["Value"] = ii.PropertyValues[i].GetValue("").ToString();
                                        break;

                                    default:
                                        row["Datatype"] = idoprop.DataType;
                                        row["Value"] = ii.PropertyValues[i].GetValue("").ToString();
                                        break;
                                }
                            }
                        }

                        dt.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: log error if needed
                // err handling was empty in VB, so kept empty here
            }
            finally
            {
            }

            return dt;
        }

        [IDOMethod(MethodFlags.CustomLoad, "infobar")]
        public DataTable ue_JLI_TestQuery(string query, ref string infobar)
        {
            IDataReader Resultset = null;
            DataTable dt = new DataTable();
            string sqlScript = string.Empty; 
            using (ApplicationDB appdb = IDORuntime.Context.CreateApplicationDB())
            {
                using (IDbCommand sql = appdb.CreateCommand())
                {
                    try
                    {
                        sqlScript = Resources.ue_JLI_TestQuery;
                        sql.CommandType = CommandType.Text;
                        sql.CommandText = sqlScript;
                        appdb.AddCommandParameterWithValue(sql, "query", query);                        
                        Resultset = sql.ExecuteReader();                        
                        dt.Load(Resultset);                        
                    }
                    catch (Exception ex)
                    {
                        infobar = ex.Message;
                    }
                }
            }
            return dt;
        }






























    }
}
