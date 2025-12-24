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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int ue_JLI_DocTrackAttachToIDM(ref string infobar)
        {

            string errMsg = string.Empty;
            string logicalFolderNameFrom = string.Empty;
            string logicalFolderName_Archive = string.Empty;
            logicalFolderNameFrom = "1_TRN_JLI_DOC_Track";
            logicalFolderName_Archive = "1_TRN_JLI_DOC_Track_Archive";

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
            int moved = 0;
            byte[] fileContent = null;
            string parsedFileSpec = string.Empty;

            string filter = string.Empty;
            string coNum = string.Empty;

            // Create instance of your file server extension class
            FileServerExtension fileServer = new FileServerExtension();

            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderNameFrom, ref serverFrom, ref folderTemplateFrom, ref accessDepthFrom, ref infobar);
            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName_Archive, ref serverTo, ref folderTemplateTo, ref accessDepthTo, ref infobar);

            try
            {
                foreach (DataRow row in files.Rows)
                {
                    fileName = row["DerFileName"]?.ToString();
                    if (fileName.Length > 10)
                        coNum = fileName.Substring(0, 10);
                    filter = string.Format("CoNum = '{0}' ", coNum);

                    fileSpecFrom = ue_JLI_GetFileSpec(folderTemplateFrom, fileName, ".pdf", accessDepthFrom, true);
                    fileServer.GetFileContent(fileSpecFrom, serverFrom, logicalFolderNameFrom, ref fileContent, ref parsedFileSpec, ref infobar);
                    //createLog("", "", 3, "GetFileContent " + infobar);
                    ue_JLI_AddContentToIDM("CS_SalesOrder", "CustomerOrder", "JLI", fileName, fileContent, "CustomerOrder", "SLCos", "CustNum,CoNum,CustPo", filter, ref errMsg);
                    
                    fileSpecTo = ue_JLI_GetFileSpec(folderTemplateTo, fileName, ".pdf", accessDepthTo, true);
                    fileServer.MoveFileServerToServer(ref infobar, ref moved, serverFrom, fileSpecFrom, logicalFolderNameFrom, serverTo, fileSpecTo, logicalFolderName_Archive, 1, 1);
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
            string fileSpec = ue_JLI_GetFileSpec(folderTemplate, "", ".pdf", accessDepth, true);
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
            using (DataTableReader reader = new DataTableReader(ue_JLI_CLM_GetIDONameValueData(idoCollection, idoPropertyList, idoFilter)))
            {
                dt.Load(reader);
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
        private DataTable ue_JLI_CLM_GetIDONameValueData(string IDOName, string PropList, string Filter)
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
































    }
}
