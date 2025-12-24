using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ue_JLI_JLM_OpenPoReport
{
    public class ue_JLI_JLM_OpenPoReport : IDOExtensionClass
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
        public DataTable ue_JLI_JLM_Rpt_OpenPO(string inpSite = null)
        {

            //createLog("ue_JLI_JLM_OpenPoReport", "ue_JLI_JLM_Rpt_OpenPO", 56, "Method Call");
            if (string.IsNullOrEmpty(inpSite)) { inpSite = null; }

            string str = $@"

Declare @pSite         SiteType = '{inpSite}'

Declare @ResultSet Table(
RecordType			Nvarchar(5),
ID					int,
GL_Account1			nvarchar(30),
GL_Account2			nvarchar(30),
GL_Account3			nvarchar(30),
GL_Account4			nvarchar(30),
GL_Account5			nvarchar(30),
GL_Account6			nvarchar(30),
GL_Account7			nvarchar(30),
GL_Account8			nvarchar(30),
GL_Account9			nvarchar(30),
GL_Account10		nvarchar(30),
OpenPO_Count		Int,
JLM					Nvarchar(8),
GL_Account			Nvarchar(30),
GL_AccountDesc		Nvarchar(60),
PoNum				Nvarchar(20),
ReqDate				Nvarchar(20),
OrderDate			Nvarchar(20),
Vendor				Nvarchar(20),
VendName			Nvarchar(60),
POOpen				Nvarchar(20),
PORcved				Nvarchar(20),
OpenBal				Nvarchar(20),
POInvoiced			Nvarchar(100),
POCredits			Nvarchar(20),
POPayB				Nvarchar(20)
)


Declare @PoReceivedQty Table(
PoNum			Nvarchar(20),
GL_Account		nvarchar(30),
InvNum			Nvarchar(30),
OrderQty		decimal(12,2),
ReceivedQty		decimal(12,2)
)

Insert Into @PoReceivedQty(PoNum,GL_Account,InvNum,ReceivedQty,OrderQty)
Select 
 ReceiptHdr.po_num
,PoHdr.JLM_JL_account_num
,InvNum.InvNum
,Sum(ReceiptItem.received_qty)
,OrderQty.OrderQty
From ue_JLI_JLM_PUR_PoHdr_mst PoHdr
Inner Join ue_JLI_JLM_PUR_ReceiptHdr_mst ReceiptHdr 
On ReceiptHdr.po_num = PoHdr.po_num
Inner Join ue_JLI_JLM_PUR_ReceiptItem_mst ReceiptItem
On ReceiptItem.receipt_num = ReceiptHdr.receipt_num
Outer Apply(Select SUM(qty) As OrderQty From ue_JLI_JLM_PUR_PoItem_mst Where po_num = PoHdr.po_num) OrderQty
Outer Apply(Select Top 1 inv_num As InvNum From ue_JLI_JLM_PUR_ReceiptHdr_mst Where po_num = PoHdr.po_num) InvNum
Group By ReceiptHdr.po_num,OrderQty.OrderQty,InvNum.InvNum,PoHdr.JLM_JL_account_num



Declare @GL_AccountsTemp Table(
Seq			int,
GL_Account  nvarchar(30)
)
Insert Into @GL_AccountsTemp(GL_Account,Seq)
Select GL_Account,ROW_NUMBER() OVER (ORDER BY GL_Account) AS rn
From (SELECT DISTINCT PoHdr.JLM_JL_account_num AS GL_Account 
	  FROM ue_JLI_JLM_PUR_PoHdr_mst PoHdr Inner Join @PoReceivedQty PRQ On PRQ.PoNum = PoHdr.po_num
	  WHERE PRQ.OrderQty <> PRQ.ReceivedQty) GL_Account

Declare @GL_Accounts Table(
ID int Identity(1,1),
GL_Account1 nvarchar(30),
GL_Account2 nvarchar(30),
GL_Account3 nvarchar(30),
GL_Account4 nvarchar(30),
GL_Account5 nvarchar(30),
GL_Account6 nvarchar(30),
GL_Account7 nvarchar(30),
GL_Account8 nvarchar(30),
GL_Account9 nvarchar(30),
GL_Account10 nvarchar(30)
)

Declare 
 @GL_Account	nvarchar(30)
,@rn			int
,@count			int = 1
,@RowCount		int = 1
,@OpenBal		int


DECLARE cursor_QtyAllocJobItem CURSOR LOCAL static FOR 
	Select GL_Account,seq From @GL_AccountsTemp Order By Seq
OPEN cursor_QtyAllocJobItem;
FETCH NEXT FROM cursor_QtyAllocJobItem INTO @GL_Account,@rn
WHILE @@FETCH_STATUS = 0
    BEGIN
		------------------------------------------------	
		Select @OpenBal = Sum(OrderQty-ReceivedQty) From @PoReceivedQty Where GL_Account = @GL_Account

		If @count = 1
			Insert Into @GL_Accounts(GL_Account1)Select Concat('JLM',@rn,'/',Cast(@OpenBal As nvarchar(5)) )
		Else If @count = 2
			Update @GL_Accounts Set GL_Account2 = Concat('JLM',Cast(@rn As nvarchar(5)),'/',Cast(@OpenBal As nvarchar(5)) ) Where ID = @RowCount
		Else If @count = 3
			Update @GL_Accounts Set GL_Account3 = Concat('JLM',Cast(@rn As nvarchar(5)),'/',Cast(@OpenBal As nvarchar(5)) ) Where ID = @RowCount
		Else If @count = 4
			Update @GL_Accounts Set GL_Account4 = Concat('JLM',Cast(@rn As nvarchar(5)),'/',Cast(@OpenBal As nvarchar(5)) ) Where ID = @RowCount
		Else If @count = 5
			Update @GL_Accounts Set GL_Account5 = Concat('JLM',Cast(@rn As nvarchar(5)),'/',Cast(@OpenBal As nvarchar(5)) ) Where ID = @RowCount
 		Else If @count = 6
			Update @GL_Accounts Set GL_Account6 = Concat('JLM',Cast(@rn As nvarchar(5)),'/',Cast(@OpenBal As nvarchar(5)) ) Where ID = @RowCount
 		Else If @count = 7
			Update @GL_Accounts Set GL_Account7 = Concat('JLM',Cast(@rn As nvarchar(5)),'/',Cast(@OpenBal As nvarchar(5)) ) Where ID = @RowCount
 		Else If @count = 8
			Update @GL_Accounts Set GL_Account8 = Concat('JLM',Cast(@rn As nvarchar(5)),'/',Cast(@OpenBal As nvarchar(5)) ) Where ID = @RowCount
 		Else If @count = 9
			Update @GL_Accounts Set GL_Account9 = Concat('JLM',Cast(@rn As nvarchar(5)),'/',Cast(@OpenBal As nvarchar(5)) ) Where ID = @RowCount
 		Else If @count = 10
			Update @GL_Accounts Set GL_Account10 = Concat('JLM',Cast(@rn As nvarchar(5)),'/',Cast(@OpenBal As nvarchar(5)) ) Where ID = @RowCount

		Set @count = @count + 1

		If @count = 10
		Begin
			Set @count = 1
			Set @RowCount = @RowCount + 1
		End
		------------------------------------------------
        FETCH NEXT FROM cursor_QtyAllocJobItem INTO @GL_Account,@rn
    END;
CLOSE cursor_QtyAllocJobItem;
DEALLOCATE cursor_QtyAllocJobItem;

Insert Into @ResultSet(RecordType,ID,GL_Account1,GL_Account2,GL_Account3,GL_Account4,GL_Account5,GL_Account6,GL_Account7,GL_Account8,GL_Account9,GL_Account10,OpenPO_Count)
Select 'R1' As RecordType,ID,GL_Account1,GL_Account2,GL_Account3,GL_Account4,GL_Account5,GL_Account6,GL_Account7,GL_Account8,GL_Account9,GL_Account10,OpenPO_Count
From @GL_Accounts
Outer Apply(Select Count(1) As OpenPO_Count From ue_JLI_JLM_PUR_PoHdr_mst PoHdr Inner Join @PoReceivedQty PoReceivedQty
On PoReceivedQty.PoNum = PoHdr.po_num And PoReceivedQty.OrderQty <> PoReceivedQty.ReceivedQty ) OpenPO_Count--Where stat = 'O'





Insert Into @ResultSet(RecordType,JLM,GL_Account,GL_AccountDesc,PoNum,ReqDate,OrderDate,Vendor,VendName,POOpen,PORcved,OpenBal,POInvoiced,POCredits,POPayB)
Select 
 'R2' As RecordType
,'JLM'+Cast(GLAT.Seq As nvarchar(5)) As JLM
,JLM_JL_account_num As GL_Account
,UDTV.Description
,po_num As PoNum
,Cast(PoHdr.due_date As Date) As ReqDate
,Cast(order_date As Date) As OrderDate
,vend_num As Vendor
,Vend.name
,PoReceivedQty.OrderQty As POOpen
,PoReceivedQty.ReceivedQty As PORcved
,PoReceivedQty.OrderQty - PoReceivedQty.ReceivedQty As OpenBal
,PoReceivedQty.InvNum As POInvoiced
,Null As POCredits
,Cast(POPayB.balance As decimal(20,2)) As POPayB
From ue_JLI_JLM_PUR_PoHdr_mst PoHdr
Inner Join @PoReceivedQty PoReceivedQty
On PoReceivedQty.PoNum = PoHdr.po_num And PoReceivedQty.OrderQty <> PoReceivedQty.ReceivedQty
Left Join ue_JLI_JLM_PUR_Vendor_mst Vend
On Vend.JLM_vend_num = PoHdr.vend_num
Left Join UserDefinedTypeValues UDTV
On UDTV.TypeName = 'JL JLM Purchase Tacna GL Accounts'
And UDTV.Value = PoHdr.JLM_JL_account_num
Inner Join @GL_AccountsTemp GLAT
On GLAT.GL_Account = PoHdr.JLM_JL_account_num 
Outer Apply(Select Sum(PaymentInv.balance) balance From ue_JLI_JLM_PUR_PaymentInv_mst PaymentInv Where PaymentInv.po_num = PoHdr.po_num) POPayB
Order By JLM_JL_account_num,Cast(PoHdr.due_date As Date),po_num

Insert Into @ResultSet(RecordType,ID,GL_Account1,GL_Account2,GL_Account3,GL_Account4,GL_Account5,GL_Account6,GL_Account7,GL_Account8,GL_Account9,GL_Account10,OpenPO_Count)
Select 'R3' As RecordType,ID,GL_Account1,GL_Account2,GL_Account3,GL_Account4,GL_Account5,GL_Account6,GL_Account7,GL_Account8,GL_Account9,GL_Account10,OpenPO_Count
From @GL_Accounts
Outer Apply(Select Count(1) As OpenPO_Count From ue_JLI_JLM_PUR_PoHdr_mst PoHdr Inner Join @PoReceivedQty PoReceivedQty
On PoReceivedQty.PoNum = PoHdr.po_num And PoReceivedQty.OrderQty <> PoReceivedQty.ReceivedQty ) OpenPO_Count--Where stat = 'O'


Select 
 RecordType		
,ID				
,GL_Account1		
,GL_Account2		
,GL_Account3		
,GL_Account4		
,GL_Account5		
,GL_Account6		
,GL_Account7		
,GL_Account8		
,GL_Account9		
,GL_Account10	
,OpenPO_Count	
,JLM				
,GL_Account		
,GL_AccountDesc	
,PoNum			
,ReqDate			
,OrderDate		
,Vendor			
,VendName		
,POOpen			
,PORcved			
,OpenBal		
,POInvoiced		
,POCredits		
,POPayB
,SumOfOpenBalByDueDate.SumOfOpenBal As SumOfOpenBalByDueDate
,SumOfOpenBalByGLAcct.SumOfOpenBal As SumOfOpenBalByGLAcct
,SumOf.SumOfOpenBal As SumOfOpenBal
,SumOf.SumOfPOCredits As SumOfPOCredits
,SumOf.SumOfPOPayB As SumOfPOPayB
From @ResultSet RS
Outer Apply(Select Sum(Cast(R.OpenBal As decimal(12,2))) As SumOfOpenBal From @ResultSet R Where R.JLM = RS.JLM And R.ReqDate = RS.ReqDate) SumOfOpenBalByDueDate
Outer Apply(Select Sum(Cast(R.OpenBal As decimal(12,2))) As SumOfOpenBal From @ResultSet R Where R.JLM = RS.JLM) SumOfOpenBalByGLAcct
Outer Apply(Select Sum(Cast(R.OpenBal As decimal(12,2))) As SumOfOpenBal
				  ,Sum(Cast(R.POCredits As decimal(12,2))) As SumOfPOCredits
				  ,Sum(Cast(R.POPayB As decimal(12,2))) As SumOfPOPayB From @ResultSet R) SumOf
Order By RecordType,ID,JLM,Cast(ReqDate As Date),PoNum


 
";
            DataTable dt_Resultset = new DataTable();

            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = str;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    dt_Resultset.Load(Resultset);
                    if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                    {
                        return dt_Resultset;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_JLM_OpenPoReport", "ue_JLI_JLM_Rpt_OpenPO", 56, "ex - " + ex.Message);
                return null;
            }



        }

    }
}
