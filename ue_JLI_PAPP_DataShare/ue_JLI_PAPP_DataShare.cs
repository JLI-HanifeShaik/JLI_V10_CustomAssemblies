using Infor.DocumentManagement.ICP;
using Mongoose.Core.Extensions;
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

namespace ue_JLI_PAPP_DataShare
{
    [IDOExtensionClass("ue_JLI_PAPP_DataShare")]
    public class ue_JLI_PAPP_DataShare : ExtensionClassBase
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

        [IDOMethod(MethodFlags.None, "infobar")]
        public int ue_JLI_QCIssuesDataPull(ref string infobar)
        {
            string errMsg = string.Empty;
            string logicalFolderName = string.Empty;
            ue_JLI_GetlogicalFolderName(ref logicalFolderName);

            // Get list of files from method
            DataTable files = ue_JLI_GetFileList(logicalFolderName, ref infobar);            

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

            string serverName = string.Empty;
            string folderTemplate = string.Empty;
            string accessDepth = string.Empty;
            string fileSpec = string.Empty;
            string fileName = string.Empty;
            string fileContent = string.Empty;
            string parsedFileSpec = string.Empty;
            var ue_JLI_PAPP_QCIssuesDT = ue_JLI_CreateDataTable("ue_JLI_PAPP_QCIssues", "CreatedBy,UpdatedBy,CreateDate,RecordDate,RowPointer,NoteExistsFlag,InWorkflow,SerialNo,QC_date,whse,QC_usercode,QC_name,status,QC_loc,QC_defect,QC_source,QC_part,QC_rate,Uf_total_time_of_scan,Uf_InspectionArea,appStartTime,appEndTime");
            
            // Create instance of your file server extension class
            FileServerExtension fileServer = new FileServerExtension();

            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName, ref serverName, ref folderTemplate, ref accessDepth, ref infobar);            

            try
            {
                foreach (DataRow row in files.Rows)
                {
                    fileName = row["DerFileName"]?.ToString();
                    if (!fileName.StartsWith("QC"))
                        continue;
                    
                    fileSpec = ue_JLI_GetFileSpec(folderTemplate, fileName, ".txt", accessDepth, true);
                    fileServer.GetFileContentAsBase64String(fileSpec, serverName, logicalFolderName, ref fileContent, ref parsedFileSpec, ref infobar);                   
                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        byte[] bytes = Convert.FromBase64String(fileContent);
                        fileContent = Encoding.UTF8.GetString(bytes);
                        string[] parts = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string part in parts)
                        {

                            string SerialNo = null;
                            string QC_date = null;
                            string QC_loc = null;
                            string QC_source = null;
                            string QC_defect = null;
                            string QC_part = null;
                            decimal QC_rate = 0;
                            string QC_name = null;
                            string QC_usercode = null;
                            string status = null;
                            string whse = null;
                            string Uf_InspectionArea = null;
                            decimal Uf_total_time_of_scan = 0;
                            string appStartTime = null;
                            string appEndTime = null;
                            string CreateDate = null;
                            string CreatedBy = null;
                            string RecordDate = null;
                            string UpdatedBy = null;
                            int InWorkflow = 0;
                            int NoteExistsFlag = 0;
                            string RowPointer = null;

                            string[] tabParts = part.Split('\t');
                            
                            if (tabParts.Length >= 22) // safety check
                                {
                                    SerialNo = tabParts[0];
                                    QC_date = tabParts[1];
                                    QC_loc = tabParts[2];
                                    QC_source = tabParts[3];
                                    QC_defect = tabParts[4];
                                    QC_part = tabParts[5];
                                    QC_rate = decimal.TryParse(tabParts[6], out var r) ? r : 0;
                                    QC_name = tabParts[7];
                                    QC_usercode = tabParts[8];
                                    status = tabParts[9];
                                    whse = tabParts[10];
                                    Uf_InspectionArea = tabParts[11];
                                    Uf_total_time_of_scan = decimal.TryParse(tabParts[12], out var t) ? t : 0;
                                    appStartTime = tabParts[13];
                                    appEndTime = tabParts[14];
                                    CreateDate = tabParts[15];
                                    CreatedBy = tabParts[16];
                                    RecordDate = tabParts[17];
                                    UpdatedBy = tabParts[18];
                                    InWorkflow = int.TryParse(tabParts[12], out var p) ? p : 0;
                                    NoteExistsFlag = int.TryParse(tabParts[12], out var q) ? q : 0;
                                    RowPointer = tabParts[21];
                                    ue_JLI_PAPP_QCIssuesDT.Rows.Add(CreatedBy, UpdatedBy, CreateDate, RecordDate, RowPointer, NoteExistsFlag, InWorkflow, SerialNo, QC_date, whse, QC_usercode, QC_name, status, QC_loc, QC_defect, QC_source, QC_part, QC_rate, Uf_total_time_of_scan, Uf_InspectionArea, appStartTime, appEndTime);                  
                                }


                        }//foreach (string part in parts)                        

                    }//if (!string.IsNullOrEmpty(fileContent))

                }//foreach (DataRow row in files.Rows)

                createLog("", "", 22, ue_JLI_PAPP_QCIssuesDT.Rows.Count.ToString());
                //Context.Commands.SaveDataTable(ue_JLI_PAPP_QCIssuesDT, true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Processing File: {ex.Message}");
            }

            

            return 0;
        }
        [IDOMethod(MethodFlags.None, "infobar")]
        public int ue_JLI_FedExDataPull(ref string infobar)
        {
            string errMsg = string.Empty;
            string logicalFolderName = string.Empty;
            ue_JLI_GetlogicalFolderName(ref logicalFolderName);

            // Get list of files from method
            DataTable files = ue_JLI_GetFileList(logicalFolderName, ref infobar);

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

            string serverName = string.Empty;
            string folderTemplate = string.Empty;
            string accessDepth = string.Empty;
            string fileSpec = string.Empty;
            string fileName = string.Empty;
            string fileContent = string.Empty;
            string parsedFileSpec = string.Empty;
            var ue_JLI_FedExTracingsDT = ue_JLI_CreateDataTable("ue_JLI_FedExTracing", "CoNum,TrackingNum,Date,Weight,Amount");

            // Create instance of your file server extension class
            FileServerExtension fileServer = new FileServerExtension();

            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName, ref serverName, ref folderTemplate, ref accessDepth, ref infobar);

            try
            {
                foreach (DataRow row in files.Rows)
                {
                    fileName = row["DerFileName"]?.ToString();
                    if (!fileName.StartsWith("FedEx"))
                        continue;

                    fileSpec = ue_JLI_GetFileSpec(folderTemplate, fileName, ".txt", accessDepth, true);
                    fileServer.GetFileContentAsBase64String(fileSpec, serverName, logicalFolderName, ref fileContent, ref parsedFileSpec, ref infobar);
                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        byte[] bytes = Convert.FromBase64String(fileContent);
                        fileContent = Encoding.UTF8.GetString(bytes);
                        string[] parts = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string part in parts)
                        {
                            string CoNum = null;
                            string TrackingNum = null;
                            string _Date;
                            decimal Weight = 0;
                            decimal Amount = 0;

                            string[] tabParts = part.Split('\t');
                            if (tabParts.Length >= 5)
                            {
                                CoNum = tabParts[0];
                                TrackingNum = tabParts[1];
                                _Date = tabParts[2];
                                Weight = decimal.TryParse(tabParts[3], out var r) ? r : 0;
                                Amount = decimal.TryParse(tabParts[4], out var t) ? t : 0;
                                ue_JLI_FedExTracingsDT.Rows.Add(CoNum, TrackingNum, _Date, Weight, Amount);
                            }


                        }//foreach (string part in parts)                        

                    }//if (!string.IsNullOrEmpty(fileContent))

                }//foreach (DataRow row in files.Rows)

                createLog("RowPointer", "", 22, ue_JLI_FedExTracingsDT.Rows.Count.ToString());
                //Context.Commands.SaveDataTable(ue_JLI_FedExTracingsDT, true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Processing File: {ex.Message}");
            }



            return 0;
        }
        private void ue_JLI_GetlogicalFolderName(ref string logicalFolderName)
        {
            LoadCollectionResponseData oResponse;
            string propertyList = "Charfld1,Charfld2,Charfld3";
            string strFilter = "ParmId = 'PowerApps' And ParmKey = 'OutputDirectory' ";

            oResponse = this.Context.Commands.LoadCollection("JLI_CustParms", propertyList, strFilter, "", 0);

            if (oResponse.Items.Count > 0)
            {
                logicalFolderName = oResponse[0, "Charfld1"].Value.Trim();
            }
        }
        private DataTable ue_JLI_GetFileList(string logicalFolderName, ref string infobar)
        {
            string fileServer = string.Empty;
            string folderTemplate = string.Empty;
            string accessDepth = string.Empty;
            string getFileAction = "File";
            string errMsg = string.Empty;
            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName, ref fileServer, ref folderTemplate, ref accessDepth, ref errMsg);
            string fileSpec = ue_JLI_GetFileSpec(folderTemplate, "", ".txt", accessDepth, true);
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
        private DataTable ue_JLI_CreateDataTable(string idoName, string idoProps)
        {
            DataTable dataTable = new DataTable(idoName);
            string[] array = idoProps.Split(',');
            for (int i = 0; i < array.Length; i++)
            {
                dataTable.Columns.Add(new DataColumn(array[i]));
            }

            return dataTable;
        }

    }
}
