using Mongoose.IDO.DataAccess;
using Mongoose.IDO.Protocol;
using Mongoose.IDO;
using Mongoose.MGCore;
using System;
using System.Collections.Generic;
using System.Data;
using Infor.DocumentManagement.ICP;
using CSI.FullTrust.IDM;
using CSI.MG;
using System.Text;
using System.Linq;
using System.Globalization;
using ue_JLI_EDIFileDrop.Properties;

namespace ue_JLI_EDIFileDrop
{
    [IDOExtensionClass("ue_JLI_EDIFileDrop")]
    public class ue_JLI_EDIFileDrop : CSIExtensionClassBase
    {
        public interface IIDOExtensionClass
        {
            void SetContext(Mongoose.IDO.IIDOExtensionClassContext context);
        }

        [IDOMethod(MethodFlags.RequiresTransaction, "infobar")]
        public int ue_JLI_CustomerEDIPOPdfAttachedToIDM(ref string infobar)
        {
            
            string errMsg = string.Empty;
            string logicalFolderNameFrom = string.Empty;
            string logicalFolderName_Archive = string.Empty;
            string fileExtension = ".pdf";
            DataTable filesList = new DataTable();
            filesList.Columns.Add("ID", typeof(int));
            filesList.Columns.Add("FileName", typeof(string));
            filesList.Columns.Add("CustNum", typeof(string));
            filesList.Columns.Add("CoNum", typeof(string));

            logicalFolderNameFrom = "1_PRD_JLI_EDICustomerPOs";
            logicalFolderName_Archive = "1_PRD_JLI_EDICustomerPOsARC";

            // Get list of files from method
            DataTable files = ue_JLI_GetFileList(logicalFolderNameFrom, fileExtension, ref infobar);

            if (!string.IsNullOrEmpty(infobar))
            {
                infobar = "Error while getting file list: " + infobar;
                ue_JLI_CustomerPoCheckNotify(filesList, 0, 0);
                return 0;
            }

            if (files == null || files.Rows.Count == 0)
            {
                infobar = "No files found.";
                ue_JLI_CustomerPoCheckNotify(filesList, 0, 0);
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
            string fileNameWithOrderNum = string.Empty;
            string custNum = string.Empty;
            string poNum = string.Empty;
            string ordDate = string.Empty;
            string coNum = string.Empty;
            int moved = 0;
            int deleted = 0;
            int id = 0;
            int totalFiles = 0;
            short processed = 0;
            byte[] fileContent = null;
            string parsedFileSpecTo = string.Empty;
            string filter = string.Empty;
            LoadCollectionResponseData loadResponse;
            // Create instance of your file server extension class
            FileServerExtension fileServer = new FileServerExtension();            

            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderNameFrom, ref serverFrom, ref folderTemplateFrom, ref accessDepthFrom, ref infobar);

            try
            {
                totalFiles = files.Rows.Count;

                foreach (DataRow row in files.Rows)
                {
                    id++;

                    fileName = row["DerFileName"]?.ToString();
                    if (fileName.Contains("DUP") || fileName.Contains("-S"))
                        continue;

                    ue_JLI_GetPoNumFromFileName(fileName, ref custNum, ref poNum, ref ordDate);

                    fileSpecFrom = ue_JLI_GetFileSpec(folderTemplateFrom, fileName, ".pdf", accessDepthFrom, true);

                    filter = string.Format("CustNum = '{0}' And CustPo = '{1}' ", custNum, poNum);
                    loadResponse = new LoadCollectionResponseData();
                    loadResponse = this.Context.Commands.LoadCollection("SLCos", "CoNum", filter, string.Empty, -1);

                    if (loadResponse.Items.Count > 0)
                    {
                        processed = 1;                        

                        if (loadResponse.Items.Count == 1)
                        {
                            coNum = loadResponse[0, "CoNum"].GetValue<string>();
                            filter = string.Format("CoNum = '{0}' ", coNum);

                            DataRow filesListRow = filesList.NewRow();
                            filesListRow["ID"] = id;
                            filesListRow["FileName"] = fileName;
                            filesListRow["CustNum"] = custNum;
                            filesListRow["CoNum"] = coNum;
                            filesList.Rows.Add(filesListRow);

                            fileNameWithOrderNum = custNum + "-" + poNum + "-" + ordDate + "-" + coNum + ".pdf";

                            fileServer.GetFileContent(fileSpecFrom, serverFrom, logicalFolderNameFrom, ref fileContent, ref parsedFileSpecTo, ref infobar);
                            ue_JLI_AddContentToIDM("CS_SalesOrder", "InforSalesOrder", "JLI", fileNameWithOrderNum, fileContent, "CS_SalesOrder", "SLCos", "CustNum,CoNum,CustPo", filter, ref errMsg);
                            
                            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName_Archive, ref serverTo, ref folderTemplateTo, ref accessDepthTo, ref infobar);
                            fileSpecTo = ue_JLI_GetFileSpec(folderTemplateTo, fileNameWithOrderNum, ".pdf", accessDepthTo, true);
                            fileServer.MoveFileServerToServer(ref infobar, ref moved, serverFrom, fileSpecFrom, logicalFolderNameFrom, serverTo, fileSpecTo, logicalFolderName_Archive, 1, 1);
                            
                        }
                        else
                        {
                            if (custNum == "C001379")
                            {
                                for (int j = 0; j < loadResponse.Items.Count; j++)
                                {
                                    coNum = loadResponse[j, "CoNum"].GetValue<string>();
                                    filter = string.Format("CoNum = '{0}' ", coNum);

                                    DataRow filesListRow = filesList.NewRow();
                                    filesListRow["ID"] = id;
                                    filesListRow["FileName"] = fileName;
                                    filesListRow["CustNum"] = custNum;
                                    filesListRow["CoNum"] = coNum;
                                    filesList.Rows.Add(filesListRow);

                                    fileNameWithOrderNum = custNum + "-" + poNum + "-" + ordDate + "-" + coNum + "-Mult" + ".pdf";

                                    fileServer.GetFileContent(fileSpecFrom, serverFrom, logicalFolderNameFrom, ref fileContent, ref parsedFileSpecTo, ref infobar);
                                    ue_JLI_AddContentToIDM("CS_SalesOrder", "InforSalesOrder", "JLI", fileNameWithOrderNum, fileContent, "CS_SalesOrder", "SLCos", "CustNum,CoNum,CustPo", filter, ref errMsg);

                                    ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName_Archive, ref serverTo, ref folderTemplateTo, ref accessDepthTo, ref infobar);
                                    fileSpecTo = ue_JLI_GetFileSpec(folderTemplateTo, fileNameWithOrderNum, ".pdf", accessDepthTo, true);
                                    fileServer.MoveFileServerToServer(ref infobar, ref moved, serverFrom, fileSpecFrom, logicalFolderNameFrom, serverTo, fileSpecTo, logicalFolderName_Archive);
                                }

                                fileServer.DeleteFromFileServer(serverFrom, fileSpecFrom, logicalFolderNameFrom, ref deleted, ref infobar);

                            }
                            else
                            {
                                coNum = loadResponse[0, "CoNum"].GetValue<string>();
                                filter = string.Format("CoNum = '{0}' ", coNum);

                                DataRow filesListRow = filesList.NewRow();
                                filesListRow["ID"] = id;
                                filesListRow["FileName"] = fileName;
                                filesListRow["CustNum"] = custNum;
                                filesListRow["CoNum"] = coNum;
                                filesList.Rows.Add(filesListRow);

                                fileNameWithOrderNum = custNum + "-" + poNum + "-" + ordDate + "-" + coNum + "-DUP" + ".pdf";

                                fileSpecTo = ue_JLI_GetFileSpec(folderTemplateFrom, fileNameWithOrderNum, ".pdf", accessDepthFrom, true);
                                fileServer.MoveFileServerToServer(ref infobar, ref moved, serverFrom, fileSpecFrom, logicalFolderNameFrom, serverFrom, fileSpecTo, logicalFolderNameFrom, 1, 1);
                            }
                        }
                    }
                    else
                    {
                        DataRow filesListRow = filesList.NewRow();
                        filesListRow["ID"] = id;
                        filesListRow["FileName"] = fileName;
                        filesListRow["CustNum"] = custNum;
                        filesListRow["CoNum"] = string.Empty;
                        filesList.Rows.Add(filesListRow);
                    }



                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Processing File: {ex.Message}");
            }

            ue_JLI_CustomerPoCheckNotify(filesList, totalFiles, processed);
            infobar = "Process done.";

            return 0;
        }
        private DataTable ue_JLI_GetFileList(string logicalFolderName, string fileExtension, ref string infobar)
        {
            string fileServer = string.Empty;
            string folderTemplate = string.Empty;
            string accessDepth = string.Empty;
            string getFileAction = "File";
            string errMsg = string.Empty;
            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName, ref fileServer, ref folderTemplate, ref accessDepth, ref errMsg);
            string fileSpec = ue_JLI_GetFileSpec(folderTemplate, "", fileExtension, accessDepth, true);
            FileServerExtension fileServerExtension = new FileServerExtension();
            // Assuming GetFileList returns a DataTable
            DataTable files = fileServerExtension.GetFileList(fileSpec, fileServer, logicalFolderName, getFileAction, ref errMsg);

            infobar = errMsg;
            return files;
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
        private void ue_JLI_GetPoNumFromFileName(string fileName, ref string custNum,ref string poNum, ref string ordDate)
        {
            // Remove last 4 characters (like ".pdf", ".txt", etc.)
            if (fileName.Length > 4)
                fileName = fileName.Substring(0, fileName.Length - 4);

            // Remove all spaces
            fileName = fileName.Replace(" ", "");

            // Equivalent of SUBSTRING(PName,1,7)
            if (fileName.Length >= 7)
                custNum = fileName.Substring(0, 7); // C# uses 0-based index

            // Equivalent of RIGHT(Pname,8)
            if (fileName.Length >= 8)
                ordDate = fileName.Substring(fileName.Length - 8, 8);

            // Equivalent of SUBSTRING(Pname,9,(LEN(Pname)-8)-CHARINDEX('-',REVERSE(RIGHT(Pname,9))))
            if (fileName.Length > 9)
            {
                poNum = fileName.Replace(custNum + "-", "");
                poNum = poNum.Replace("-" + ordDate, "");

                if (!string.IsNullOrEmpty(poNum) && poNum.EndsWith("-A", StringComparison.OrdinalIgnoreCase))
                    poNum = poNum.Substring(0, poNum.Length - 2);// Remove the last 2 characters
            }

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
                switch (row["Name"].ToString())
                {
                    case "RowPointer":
                        refRowPointer = row["Value"].ToString();
                        break;

                    default:
                        item.SetAttributeValue(row["Name"].ToString(), row["Value"].ToString());
                        break;
                }

                //SearchString = SearchString.Replace(row["Name"].ToString(),"@" + row["Name"].ToString());
            }

            //SearchString = $"[@EntityType=\"{entityType}\" AND {SearchString}]";
            //SearchString = "/" + entityName + SearchString;

            try
            {
                item.Resources.Add(res);
                IIDMControllerFactory oIDMControllerFactory = this.GetService<IIDMControllerFactory>();
                IIDMController trustedIDMController = oIDMControllerFactory.Create();
                //CMItems items = trustedIDMController.SearchItems(SearchString);
                
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
        private void ue_JLI_CustomerPoCheckNotify(DataTable filesList,int totalFiles, short processed)
        {

            string subject = "SQL JOB EDI POCheck";
            String Attachments = string.Empty;
            String BodyFormat = "HTML";
            String UserName = "sa";
            String mailMsg = string.Empty;
            string toEmail = "Ramakrishna.Balimidi@zeshtit.com;Ushan.dalwis@Jonathanlouis.com";
            string toEmail1 = string.Empty;
            string toEmail2 = string.Empty;
            int errors = 0;
            LoadCollectionResponseData oResponse;
            oResponse = this.Context.Commands.LoadCollection("JLI_CustParms", "Charfld1,Charfld2,Charfld3", "ParmId = 'EDI' And ParmKey = 'ImportEmail' ", "", 1);
            if (oResponse.Items.Count > 0)
            {
                toEmail1 = oResponse[0, "Charfld2"].Value.Trim();
                toEmail2 = oResponse[0, "Charfld3"].Value.Trim();
            }
            else { return; }

            string newLine = "\r\n\r\n";   // CHAR(10)+CHAR(13)+CHAR(10)+CHAR(13)            

            errors = filesList.AsEnumerable().Count(r => string.IsNullOrEmpty(r["CoNum"]?.ToString()));

            if (filesList.Rows.Count > 0)
            {
                if (processed == 1)
                    mailMsg = newLine + totalFiles.ToString() + " Files processed";
                else // process == 0
                    mailMsg = newLine + totalFiles.ToString() + " Files to be Processed";
                StringBuilder bodyTable = new StringBuilder();                

                foreach (DataRow row in filesList.Rows)
                {
                    string id = row["ID"]?.ToString() ?? "";
                    string custNum = row["CustNum"]?.ToString() ?? "";
                    string fName = row["FileName"]?.ToString() ?? "";
                    string coNum = row["CoNum"]?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(coNum))
                        continue;

                    bodyTable.Append("<tr>");

                    bodyTable.Append($"|{id}^");
                    bodyTable.Append($"|{custNum}^");
                    bodyTable.Append($"|{fName}^");

                    bodyTable.Append("</tr>");

                }

                // Convert to final HTML table cells as SQL REPLACE does
                string result = bodyTable.ToString();
                result = result.Replace("|", "<td>")
                               .Replace("^", "</td>");

                mailMsg = "<p>" + mailMsg + "</p><br>" + "<table style='width:100%' border='1'>" + "<tr><th>ID</th><th>CustNum</th><th>FileName</th></tr>" + result + "</table>" + "<p><br>" + errors.ToString() + " Orders Not Found</p>";
                toEmail = toEmail1;
            }
            else
            {
                mailMsg = "No Files to be Processed OR Files to be ProcessedError while getting file list";
                toEmail = toEmail2;
            }

            UpdateCollectionRequestData updateColRequestData;
            UpdateCollectionResponseData updateColResponseData;
            IDOUpdateItem idoUpdateItem;

            updateColRequestData = new UpdateCollectionRequestData();
            updateColRequestData.IDOName = "JLI_EmailQueues";
            idoUpdateItem = new IDOUpdateItem();

            idoUpdateItem.Action = UpdateAction.Insert;
            idoUpdateItem.Properties.Add("EmailDoc", 1);
            idoUpdateItem.Properties.Add("email_sub", subject);
            idoUpdateItem.Properties.Add("email_body", mailMsg);
            idoUpdateItem.Properties.Add("email_to", toEmail);
            idoUpdateItem.Properties.Add("submitted_by", UserName);
            idoUpdateItem.Properties.Add("body_format", BodyFormat);
            updateColRequestData.Items.Add(idoUpdateItem);
            try
            {
                updateColResponseData = this.Context.Commands.UpdateCollection(updateColRequestData);
            }
            catch(Exception ex) 
            {

            }


        }

        public DataTable CreateDataTable(string idoName, string idoProps)
        {
            DataTable dataTable = new DataTable(idoName);
            string[] array = idoProps.Split(',');
            for (int i = 0; i < array.Length; i++)
            {
                dataTable.Columns.Add(new DataColumn(array[i]));
            }

            return dataTable;
        }
        private DataTable ue_JLI_GetCustParmsInfo(string ParmId, string ParmKey)
        {
            LoadCollectionResponseData oResponse;
            string propertyList = "Charfld1,Charfld2,Charfld3,Charfld4,Charfld5";
            DataTable dtCustParmsData = CreateDataTable("JLI_CustParms", propertyList);
            string strFilter = string.Format("ParmId = '{0}' And ParmKey = '{1}' ", ParmId, ParmKey);
            oResponse = this.Context.Commands.LoadCollection("JLI_CustParms", propertyList, strFilter, "", 0);
            if (oResponse.Items.Count > 0)
                oResponse.Fill(dtCustParmsData);

            return dtCustParmsData;
        }
        //******//GL0029 - [A/R Posted Transaction Details - EDI Invoice Sent (date)-Elsy]*******\\\\\
        [IDOMethod(MethodFlags.RequiresTransaction, "infobar")]
        public int ue_JLI_ARPostedTranDtlEDIInvSentDateUpdate(ref string infobar)
        {
            
            string logicalFolderNameFrom = string.Empty;
            string logicalFolderName_Archive = string.Empty;
            string filExtension = ".txt";
            DataTable dtCustParmsData = ue_JLI_GetCustParmsInfo("EDI", "OutputDirectory2");

            if(dtCustParmsData.Rows.Count <= 0)
            {
                infobar = "Directory Not found ";
                return 0;
            }

            foreach (DataRow dr in dtCustParmsData.Rows)
            {
                logicalFolderNameFrom = dr.Field<string>("Charfld1");
                logicalFolderName_Archive = dr.Field<string>("Charfld2");
            }
            //logicalFolderNameFrom = "1_PRD_JLI_C3000InvoiceSend";
            //logicalFolderName_Archive = "1_PRD_JLI_C3000InvoiceSendArchive";

            // Get list of files from method
            DataTable files = ue_JLI_GetFileList(logicalFolderNameFrom, filExtension, ref infobar);

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
            string fileContent = string.Empty;
            string parsedFileSpecTo = string.Empty;

            // Create instance of your file server extension class
            FileServerExtension fileServer = new FileServerExtension();

            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderNameFrom, ref serverFrom, ref folderTemplateFrom, ref accessDepthFrom, ref infobar);
            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName_Archive, ref serverTo, ref folderTemplateTo, ref accessDepthTo, ref infobar);

            try
            {
                foreach (DataRow row in files.Rows)
                {
                    string invSendDate = string.Empty;
                    List<string> invList = new List<string>();
                    fileName = row["DerFileName"]?.ToString();
                    fileSpecFrom = ue_JLI_GetFileSpec(folderTemplateFrom, fileName, filExtension, accessDepthFrom, true);

                    fileServer.GetFileContentAsBase64String(fileSpecFrom, serverFrom, logicalFolderNameFrom, ref fileContent, ref parsedFileSpecTo, ref infobar);

                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        byte[] bytes = Convert.FromBase64String(fileContent);
                        fileContent = Encoding.UTF8.GetString(bytes);
                        string[] parts = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in parts)
                        {
                            if (line.StartsWith("810-"))
                                invList.Add(line.Remove(0, 6).Trim());

                            //12/15/2025 9:35:34 AM
                            if (line.StartsWith("Sent : "))
                            {
                                invSendDate = line.Remove(0, 7).Trim();
                                invSendDate = invSendDate.Replace(Environment.NewLine,"").Replace("/","-").Trim() ;
                                //invSendDate = DateTime.ParseExact(invSendDate, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture).ToString();
                                //infobar = invSendDate;
                            }
                        }
                    }
                    foreach (string invNum in invList)
                    {
                        ue_JLI_UpdateEDISendDate(invNum, invSendDate, ref infobar);
                    }
                    fileSpecTo = ue_JLI_GetFileSpec(folderTemplateTo, fileName, filExtension, accessDepthTo, true);
                    fileServer.MoveFileServerToServer(ref infobar, ref moved, serverFrom, fileSpecFrom, logicalFolderNameFrom, serverTo, fileSpecTo, logicalFolderName_Archive, 1, 1);


                }
            }
            catch (Exception ex)
            {
                infobar = ex.Message;
            }


            return 0;
        }
        private void ue_JLI_UpdateEDISendDate(string invNum, string sendDate, ref string errMsg)
        {
            string query = Resources.ue_JLI_ARPostedTranDtlEDIInvSentDateUpdate;
            using (ApplicationDB appdb = IDORuntime.Context.CreateApplicationDB())
            {
                using (IDbCommand sql = appdb.CreateCommand())
                {
                    try
                    {
                        sql.CommandType = CommandType.Text;
                        sql.CommandText = query;
                        appdb.AddCommandParameterWithValue(sql, "InvNum", invNum, ParameterDirection.Input).Size = 20;
                        appdb.AddCommandParameterWithValue(sql, "InvSendDate", sendDate, ParameterDirection.Input);
                        appdb.ExecuteNonQuery(sql);
                    }
                    catch (Exception ex)
                    {
                        errMsg = ex.Message;
                    }
                }
            }
        }



































    }
}
