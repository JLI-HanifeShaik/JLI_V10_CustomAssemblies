using Mongoose.Core.Common;
using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace JLI_SLCoitems
{
    public class JLI_SLCoitems : IDOExtensionClass
    {
        public interface IIDOExtensionClass
        {
            void SetContext(Mongoose.IDO.IIDOExtensionClassContext context);
        }
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



        public int ue_JLI_UpdateProofReadStatus(string coNum, string item)
        {
            //createLog("JLI_SLCoitems", "ue_JLI_UpdateProofReadStatus", 49, "coNum " + coNum);

   

            string str = $@"
Declare @CoNum		CoNumType = '{coNum}'

Select Distinct 
Case When IsNull(Uf_ProofReadReq,0) = 0 And IsNull(Uf_ProofReadReqStock,0) = 0 And IsNull(Uf_ProofReadReqSpcl,0) = 0 Then 'Cmp'
	 When IsNull(Uf_ProofReadReq,0) = 1 Then 'InP'
	 When IsNull(Uf_ProofReadReqStock,0) = 1 And (IsNull(isStockitem,0) = 1 Or ConfigItemsCount <> TotalItemsCount) Then 'InP'
	 When IsNull(Uf_ProofReadReqSpcl,0) = 1 And (IsNull(isSpclitem,0) = 1  Or ConfigItemsCount <> TotalItemsCount) Then 'InP'
	 Else 'Cmp'
End As ProofReadStatus
From coitem_mst ii 
Inner Join co_mst co On co.co_num = ii.co_num
Inner Join customer_mst cust On cust.cust_num = co.cust_num
Outer Apply (Select Top 1 1 As isStockitem 
			 From coitem_mst coi 
			 Where coi.co_num = co.co_num 
			 And Left(coi.item,2) Not In ('PT','SW') 
			 And CHARINDEX('-',coi.item) > 0
			 ) isStockitem
Outer Apply (Select Top 1 1 As isSpclitem 
			 From coitem_mst coi 
			 Where coi.co_num = co.co_num 
			 And Left(coi.item,2) Not In ('PT','SW')
			 And CHARINDEX('-',coi.item) = 0
			 ) isSpclitem
Outer Apply (Select Count(1) As ConfigItemsCount 
			 From coitem_mst coi 
			 Where coi.co_num = co.co_num 
			 And Left(coi.item,2) Not In ('SW')
			 And coi.Uf_CPQ_Message Like Case When IsNull(coi.Uf_CPQ_EDI,0) = 1 Then 'Success%' Else 'Manually configured by:%' End
			 ) ConfigItemsCount
Outer Apply (Select Count(1) As TotalItemsCount 
			 From coitem_mst coi 
			 Where coi.co_num = co.co_num 
			 And Left(coi.item,2) Not In ('SW')
			 ) TotalItemsCount
Where cust.cust_seq = 0
And co.taken_by <> 'EDI-POC'
And co.co_num = @CoNum

";

            DataTable dt_Resultset = new DataTable();
            string coUf_ProofReadStatus = "";//Cmp,Err,InP,N / A,Rdy

            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = str;  // You already formatted the string above
                    sqlCommand.CommandType = System.Data.CommandType.Text;

                    using (IDataReader Resultset = sqlCommand.ExecuteReader())
                    {
                        dt_Resultset.Load(Resultset);

                        if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                        {
                            coUf_ProofReadStatus = dt_Resultset.Rows[0]["ProofReadStatus"].ToString();
                        }
                        else
                        {
                            coUf_ProofReadStatus = "N/A"; // Or handle it however appropriate
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
            }





            LoadCollectionResponseData loadResponse = new LoadCollectionResponseData();
            LoadCollectionRequestData loadRequest = new LoadCollectionRequestData();
            LoadCollectionResponseData loadResponseCoItems = new LoadCollectionResponseData();
            LoadCollectionRequestData loadRequestCoItems = new LoadCollectionRequestData();
            string strFilter = string.Empty;

            try
            {
                strFilter = string.Format("CoNum = '{0}'", coNum);
                loadRequest.IDOName = "SLCos";
                loadRequest.Filter = strFilter;
                loadRequest.OrderBy = "";
                loadRequest.PropertyList.Add("coUf_ProofReadStatus");
                loadRequest.RecordCap = 1;
                loadRequest.Distinct = false;
                loadResponse = Context.Commands.LoadCollection(loadRequest);
                //createLog("JLI_SLCoitems", "ue_JLI_UpdateProofReadStatus", 78, "Co Count " + loadResponse.Items.Count.ToString());
                if (loadResponse.Items.Count > 0)
                {   
                    UpdateCollectionRequestData updateRequest;
                    IDOUpdateItem updateItem;
                    updateRequest = new UpdateCollectionRequestData("SLCos");
                    updateItem = new IDOUpdateItem(UpdateAction.Update, loadResponse.Items[0].ItemID);
                    updateItem.Properties.Add("coUf_ProofReadStatus", coUf_ProofReadStatus, true);
                    updateRequest.Items.Add(updateItem);
                    this.Context.Commands.UpdateCollection(updateRequest);
                }
            }
            catch (Exception ex)
            {

            }

            return 0;


        }


    }
}
