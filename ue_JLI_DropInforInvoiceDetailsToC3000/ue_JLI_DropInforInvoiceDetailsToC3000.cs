using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using Mongoose.IDO.DataAccess;
using Mongoose.Core.Common;
using Mongoose.MGCore;
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using ue_JLI_DropInforInvoiceDetailsToC3000.Properties;
using Mongoose.Scripting;

namespace ue_JLI_DropInforInvoiceDetailsToC3000
{
    [IDOExtensionClass("ue_JLI_DropInforInvoiceDetailsToC3000")]
    public class ue_JLI_DropInforInvoiceDetailsToC3000 : IDOExtensionClass 
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

        [IDOMethod(MethodFlags.None, "Infobar")]
        public int ue_JLI_DropInvoiceDetailsToC3000(DateTime? inpDate)
        {

            // If inpDate is null, use current date
            inpDate = inpDate ?? DateTime.Now;
            //createLog("ue_JLI_DropInforInvoiceDetailsToC3000", "ue_JLI_DropInvoiceDetailsToC3000", 135, "inpDate " + inpDate.ToString());

            string infobar = null;
            IDataReader dr = null;
            DataTable dt_Resultset = new DataTable();
            string query = string.Empty;

            // Create an AppDB instance for the application database
            using (ApplicationDB appDB = IDORuntime.Context.CreateApplicationDB())
            {
                // Create a new SQL command
                using (IDbCommand cmd = appDB.CreateCommand())
                {
                    query = Resources.ue_JLI_DropInvoiceDetailsToC3000;
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    appDB.AddCommandParameterWithValue(cmd, "inpDate", inpDate.ToString()) ;
                    try
                    {
                        // Execute the command through the framework
                        dr = appDB.ExecuteReader(cmd);
                        dt_Resultset.Load(dr);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }

                }
            }

            if (dt_Resultset.Rows.Count == 0)
                return 0;
            
            bool success = false;
            //createLog("ue_JLI_DropInforInvoiceDetailsToC3000", "ue_JLI_DropInvoiceDetailsToC3000", 135, "dt.Rows.Count " + dt_Resultset.Rows.Count.ToString());
            string fileName = "EDIInv" + (inpDate ?? DateTime.Now).ToString("dd") + ".M2M";
            //createLog("ue_JLI_DropInforInvoiceDetailsToC3000", "ue_JLI_DropInvoiceDetailsToC3000", 135, "fileName " + fileName);

            success = ue_JLI_SaveFileToFileServer(dt_Resultset, fileName, ref infobar);
            
            return 0;

        }
        public bool ue_JLI_SaveFileToFileServer(DataTable dt, string fileName, ref string infobar)
        {
            infobar = null;
            string accessDepth = string.Empty;
            string servername = string.Empty;
            string folderTemplate = string.Empty;
            string logicalFolderName = string.Empty;
            ue_JLI_GetlogicalFolderName(ref logicalFolderName);
            ue_JLI_GetFileServerInfoByLogicalFolderName(logicalFolderName,ref servername, ref folderTemplate, ref accessDepth, ref infobar);
            string fileSpec = ue_JLI_GetFileSpec(folderTemplate, fileName, ".M2M", accessDepth, true);
            bool success = false;
            int saved = 0;
            string base64FileContent = string.Empty;
            //byte[] fileContentBytes = new byte[0];
            int overwrite = 1;
            string fileNameWhenFileSpecIsAPath = "";
            try
            {
                var line = new StringBuilder();

                foreach (DataRow row in dt.Rows)
                {
                    var fields = new List<string>();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string value = row.IsNull(i) ? "" : row[i].ToString().Trim();
                        fields.Add(value);
                    }
                    line.Append(string.Join("\t", fields));
                    line.Append(Environment.NewLine);
                }

                base64FileContent = ue_JLI_TextToBase64String(line.ToString());
                //fileContentBytes = Encoding.ASCII.GetBytes(line.ToString());
                FileServerExtension fileServer = new FileServerExtension();               
                fileServer.SaveFileContentFromBase64String(ref infobar, ref saved, base64FileContent, fileSpec, servername, logicalFolderName, overwrite, fileNameWhenFileSpecIsAPath);
                //createLog("ue_JLI_DropInforInvoiceDetailsToC3000", "ue_JLI_DropInvoiceDetailsToC3000", 135, "After - SaveFile infobar1 " + infobar);
                //fileServer.SaveFileContent(ref infobar, ref saved, fileContentBytes, fileSpec, servername, logicalFolderName, overwrite, fileNameWhenFileSpecIsAPath);                
                //createLog("ue_JLI_DropInforInvoiceDetailsToC3000", "ue_JLI_DropInvoiceDetailsToC3000", 135, "After - SaveFile infobar2 " + infobar);
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_DropInforInvoiceDetailsToC3000", "ue_JLI_DropInvoiceDetailsToC3000", 135, "Exception - "+ ex.Message);
            }

            return success;
        }
        public string ue_JLI_TextToBase64String(string text)
        {
            byte[] textInBytes = System.Text.Encoding.ASCII.GetBytes(text);
            return Convert.ToBase64String(textInBytes);
        }
        public string ue_JLI_GetFileSpec(string folderTemplate, string fileName, string fileExtension, string accessDepth, bool useServerCheck)
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
        public void ue_JLI_GetlogicalFolderName(ref string logicalFolderName)
        {
            LoadCollectionResponseData oResponse;
            string propertyList = "Charfld1,Charfld2,Charfld3";
            string strFilter = "ParmId = 'EDI' And ParmKey = 'OutputDirectory' ";

            oResponse = this.Context.Commands.LoadCollection("JLI_CustParms", propertyList, strFilter, "", 0);

            if (oResponse.Items.Count > 0)
            {
                logicalFolderName = oResponse[0, "Charfld3"].Value.Trim();
            }
        }
        public void ue_JLI_GetFileServerInfoByLogicalFolderName(string logicalFolderName, ref string fileServerName, ref string folderTemplate, ref string accessDepth, ref string errMsg)
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












    }
}
