using Mongoose.Core.Common;
using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ue_JLI_JPWebPrintReport
{
    public class ue_JLI_JPWebPrintReport : IDOExtensionClass
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
        public DataTable ue_JLI_Rpt_JPWebPrint(string inpEmpNum = null,string inpWorkDate = null, string inpSite = null)
        {

            //createLog("ue_JLI_DailyOrderRecieptSummaryReport", "ue_JLI_Rpt_DailyOrderRecieptSummary", 56, "Method Call");
            if (string.IsNullOrEmpty(inpEmpNum)) { inpEmpNum = null; }
            if (string.IsNullOrEmpty(inpSite)) { inpSite = null; }

            string str = $@"
Declare 
 @inpEmpNum		nvarchar(20) = '{inpEmpNum}'
,@inpWorkDate   nvarchar(15) = '{inpWorkDate}'
,@pSite         SiteType = '{inpSite}'

 
 Declare 
 @EmpNum		nvarchar(20)
,@EmpName		nvarchar(200)
,@WorkDate		nvarchar(20)
,@TicketType	nvarchar(20)
,@RawScan		nvarchar(50)
,@ScanDate		nvarchar(20)
,@ActualRate	nvarchar(20)
,@Qty			nvarchar(20)
,@Total			nvarchar(20)
,@RowNum		int

,@rn			int = 1
,@OldTicketType	nvarchar(20)
,@SubTotal		Decimal(10,2) = 0
,@GrandTotal	Decimal(10,2) = 0
,@SubQtyTotal	Int = 0

Declare @TmpResultSet Table(
 ID				Int Identity(1,1)
,Group_ID		Int 
,EmpNum			nvarchar(20)
,EmpName		nvarchar(200)
,WorkDate		nvarchar(20)

,TicketType		nvarchar(20)
,RawScan		nvarchar(50)
,ScanDate		nvarchar(20)
,ActualRate		nvarchar(20)
,Qty			nvarchar(20)
)

Declare @ResultSet Table(
 ID				Int Identity(1,1)
,Group_ID		Int 
,EmpNum			nvarchar(20)
,EmpName		nvarchar(200)
,WorkDate		nvarchar(20)

,TicketType1	nvarchar(20)
,RawScan1		nvarchar(50)
,ScanDate1		nvarchar(20)
,ActualRate1	nvarchar(20)
,Qty1			nvarchar(20)

,TicketType2	nvarchar(20)
,RawScan2		nvarchar(50)
,ScanDate2		nvarchar(20)
,ActualRate2	nvarchar(20)
,Qty2			nvarchar(20)

,TicketType3	nvarchar(20)
,RawScan3		nvarchar(50)
,ScanDate3		nvarchar(20)
,ActualRate3	nvarchar(20)
,Qty3			nvarchar(20)
)


DECLARE cursor_TmpReportDate CURSOR FOR
SELECT 
    JPS.emp_num AS EmpNum,
    Emp.name AS EmpName,
    CAST(work_date AS date) AS WorkDate,
    CASE 
    WHEN type = 'P' THEN 'Piece'
    WHEN type = 'O' THEN 'Other'
    WHEN type = 'S' THEN 'Special'
    WHEN type = 'T' THEN 'Time'
    ELSE ''
    END AS TicketType,
    CASE 
    WHEN type = 'P' THEN raw_scan
    WHEN type IN ('O','S') THEN time_desc
    WHEN type = 'T' THEN time_type + '-' + time_desc
    ELSE ''
    END AS RawScan,
    CAST(scan_date AS date) AS ScanDate,
    ISNULL(actual_rate, 0) AS ActualRate,
    CASE 
    WHEN type IN ('P','O','S') THEN ISNULL(qty, 0)
    WHEN type = 'T' THEN ISNULL(time_qty, 0)
    ELSE 0
    END AS Qty,
    CASE 
    WHEN type IN ('P','O','S') THEN ISNULL(qty, 0) * ISNULL(actual_rate, 0)
    WHEN type = 'T' THEN ISNULL(time_qty, 0) * ISNULL(actual_rate, 0)
    ELSE 0
    END AS Total,
    ROW_NUMBER() OVER (PARTITION BY JPS.emp_num ORDER BY JPS.type) AS rn
FROM ue_JLI_JPScans JPS
INNER JOIN employee_mst Emp ON Emp.emp_num = JPS.emp_num
--WHERE JPS.emp_num = 'M000013' 
--AND CAST(work_date AS date) = '2025-04-11'
WHERE JPS.emp_num = @inpEmpNum
AND CAST(work_date AS date) = @inpWorkDate

OPEN cursor_TmpReportDate;
FETCH NEXT FROM cursor_TmpReportDate INTO @EmpNum,@EmpName,@WorkDate,@TicketType,@RawScan,@ScanDate,@ActualRate,@Qty,@Total,@RowNum
WHILE @@FETCH_STATUS = 0
    BEGIN
		-----------------------------------------------------------------
		If @RowNum = 1 Set @OldTicketType = @TicketType

		Set @GrandTotal = @GrandTotal + Cast(@Total As decimal(10,2))

		If @OldTicketType = @TicketType
		Begin
			Select @SubTotal = @SubTotal + Cast(@Total As decimal(10,2)),@SubQtyTotal = @SubQtyTotal + Cast(@Qty As Int)			
		End
		Else If @OldTicketType <> @TicketType
		Begin
			Insert Into @TmpResultSet (EmpNum,EmpName,WorkDate,TicketType,RawScan,ScanDate,ActualRate,Qty)
			Values(@EmpNum,@EmpName,@WorkDate,'','',Concat(@OldTicketType,' ','Total:'),Cast(@SubTotal As nvarchar(12)),Cast(@SubQtyTotal As nvarchar(12))),
				  (@EmpNum,@EmpName,@WorkDate,'','','','',''),
				  (@EmpNum,@EmpName,@WorkDate,'','SCAN DATA','SCAN DATE','RATE','QTY')
			Select @OldTicketType = @TicketType, @SubTotal = 0, @SubQtyTotal = 0
			Select @SubTotal = @SubTotal + Cast(@Total As decimal(10,2)),@SubQtyTotal = @SubQtyTotal + Cast(@Qty As Int)
		End

		If @rn = 1
		Begin
			Insert Into @TmpResultSet (EmpNum,EmpName,WorkDate,TicketType,RawScan,ScanDate,ActualRate,Qty)
			Select @EmpNum,@EmpName,@WorkDate,@TicketType,@RawScan,@ScanDate,@ActualRate,@Qty
		End
		Else If @rn = 2
		Begin
			Insert Into @TmpResultSet (EmpNum,EmpName,WorkDate,TicketType,RawScan,ScanDate,ActualRate,Qty)
			Select @EmpNum,@EmpName,@WorkDate,@TicketType,@RawScan,@ScanDate,@ActualRate,@Qty
		End
		Else If @rn = 3
		Begin
			Insert Into @TmpResultSet (EmpNum,EmpName,WorkDate,TicketType,RawScan,ScanDate,ActualRate,Qty)
			Select @EmpNum,@EmpName,@WorkDate,@TicketType,@RawScan,@ScanDate,@ActualRate,@Qty
		End


		If ((@RowNum - 1) % 24 + 1) = 24
		Begin
			Select @rn = @rn + 1
		End
		If @rn = 4
		Begin
			Set @rn = 1
		End

		-----------------------------------------------------------------------------
        FETCH NEXT FROM cursor_TmpReportDate INTO @EmpNum,@EmpName,@WorkDate,@TicketType,@RawScan,@ScanDate,@ActualRate,@Qty,@Total,@RowNum
    END;
CLOSE cursor_TmpReportDate;
DEALLOCATE cursor_TmpReportDate;

Insert Into @TmpResultSet (EmpNum,EmpName,WorkDate,TicketType,RawScan,ScanDate,ActualRate,Qty)
Values(@EmpNum,@EmpName,@WorkDate,'','','','',''),
	  (@EmpNum,@EmpName,@WorkDate,'','',Concat(@TicketType,' ','Total:'),Cast(@SubTotal As nvarchar(12)),Cast(@SubQtyTotal As nvarchar(12)))

Insert Into @TmpResultSet (EmpNum,EmpName,WorkDate,TicketType,RawScan,ScanDate,ActualRate,Qty)
Values(@EmpNum,@EmpName,@WorkDate,'','','','',''),
      (@EmpNum,@EmpName,@WorkDate,'','','Grand Total:',Cast(@GrandTotal As nvarchar(12)),'')

Set @rn = 1

DECLARE cursor_ReportDate CURSOR FOR
Select 
 EmpNum
,EmpName
,WorkDate
,TicketType
,RawScan
,ScanDate
,ActualRate
,Qty
,ID
From @TmpResultSet
Order By ID

OPEN cursor_ReportDate;
FETCH NEXT FROM cursor_ReportDate INTO @EmpNum,@EmpName,@WorkDate,@TicketType,@RawScan,@ScanDate,@ActualRate,@Qty,@RowNum
WHILE @@FETCH_STATUS = 0
    BEGIN
		-----------------------------------------------------------------
		If @rn = 1
		Begin
			Insert Into @ResultSet (Group_ID,EmpNum,EmpName,WorkDate,TicketType1,RawScan1,ScanDate1,ActualRate1,Qty1)
			Select ((@RowNum - 1) % 24 + 1),@EmpNum,@EmpName,@WorkDate,@TicketType,@RawScan,@ScanDate,@ActualRate,@Qty
		End
		Else If @rn = 2
		Begin
			Update @ResultSet 
			Set EmpNum = @EmpNum
			   ,EmpName = @EmpName
			   ,WorkDate = @WorkDate
			   ,TicketType2 = @TicketType
			   ,RawScan2= @RawScan
			   ,ScanDate2 = @ScanDate
			   ,ActualRate2 = @ActualRate
			   ,Qty2 = @Qty
			Where Group_ID = ((@RowNum - 1) % 24 + 1) And TicketType2 Is Null
		End
		Else If @rn = 3
		Begin
			Update @ResultSet 
			Set EmpNum = @EmpNum
			   ,EmpName = @EmpName
			   ,WorkDate = @WorkDate
			   ,TicketType3 = @TicketType
			   ,RawScan3 = @RawScan
			   ,ScanDate3 = @ScanDate
			   ,ActualRate3 = @ActualRate
			   ,Qty3 = @Qty
			Where Group_ID = ((@RowNum - 1) % 24 + 1) And TicketType3 Is Null
		End


		If ((@RowNum - 1) % 24 + 1) = 24
		Begin
			Select @rn = @rn + 1
		End
		If @rn = 4
		Begin
			Set @rn = 1
		End

		-----------------------------------------------------------------------------
        FETCH NEXT FROM cursor_ReportDate INTO @EmpNum,@EmpName,@WorkDate,@TicketType,@RawScan,@ScanDate,@ActualRate,@Qty,@RowNum
    END;
CLOSE cursor_ReportDate;
DEALLOCATE cursor_ReportDate;



Select 
 ID
,EmpNum
,EmpName
,WorkDate
,TicketType1
,RawScan1
,ScanDate1
,ActualRate1
,Qty1
,TicketType2
,RawScan2
,ScanDate2
,ActualRate2
,Qty2
,TicketType3
,RawScan3
,ScanDate3
,ActualRate3
,Qty3
From @ResultSet
Order By ID
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
                //createLog("ue_JLI_DailyOrderRecieptSummaryReport", "ue_JLI_Rpt_DailyOrderRecieptSummary", 56, "ex - " + ex.Message);
                return null;
            }



        }

    }
}
