using JLI_JPScans.Properties;
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

namespace JLI_JPScans
{
    public class JLI_JPScans : IDOExtensionClass
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
        public int ue_JLI_SavePieceScan(out string infobar,
										string inpEmpNum = null,
										string inpWorkDate = null,
										string inpIsGroup = null,
										string inpSessionID = null,
										string inpScanData = null )
        {
            //createLog("JLI_JPScans", "ue_JLI_SavePieceScan", 56, "inpScanData - " + inpScanData);

            string returnValue = string.Empty; 
			string query = string.Empty;
            DataTable dt_Resultset = new DataTable();
            try
            {
				query = Resources.ue_JLI_SavePieceScan;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {					
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query, inpIsGroup, inpSessionID, inpEmpNum, inpWorkDate, inpScanData);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    returnValue = sqlCommand.ExecuteScalar()?.ToString();                    
                }
            }
            catch (Exception ex)
            {
				returnValue = ex.Message;
                //createLog("JLI_JPScans", "ue_JLI_SavePieceScan", 190, "ex - " + ex.Message);
            }
            infobar = returnValue;
            return 0; // Indicating success

        }
        public int ue_JLI_SaveTimeScan(out string infobar,
										string inpEmpNum = null,
										string inpWorkDate = null,
										string inpIsGroup = null,
										string inpSessionID = null,
										string inpTimeType = null,
										string inpTimeQty = null,
										string inpTimeDesc = null)
        {

            string returnValue = null;

			string query = string.Empty;

            DataTable dt_Resultset = new DataTable();
            try
            {
				query = Resources.ue_JLI_SaveTimeScan;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query, inpIsGroup, inpSessionID, inpEmpNum, inpWorkDate, inpTimeType, inpTimeQty, inpTimeDesc);
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    returnValue = sqlCommand.ExecuteScalar()?.ToString();
                }
            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
                //createLog("JLI_JPScans", "ue_JLI_SavePieceScan", 190, "ex - " + ex.Message);
            }
            infobar = returnValue;
            return 0; // Indicating success


        }
        public int ue_JLI_SaveSpecialScan(out string infobar,
                                        string inpEmpNum = null,
                                        string inpWorkDate = null,
                                        string inpIsGroup = null,
                                        string inpSessionID = null,
                                        string inpSpecialRate = null,
                                        string inpSpecialQty = null,
                                        string inpSpecialDesc = null)
        {
            //createLog("JLI_JPScans", "ue_JLI_SavePieceScan", 56, "inpScanData - " + inpScanData);

            if (string.IsNullOrEmpty(inpEmpNum)) { inpEmpNum = null; }
            if (string.IsNullOrEmpty(inpWorkDate)) { inpWorkDate = null; }
            if (string.IsNullOrEmpty(inpIsGroup)) { inpIsGroup = null; }
            if (string.IsNullOrEmpty(inpSessionID)) { inpSessionID = null; }
            if (string.IsNullOrEmpty(inpSpecialRate)) { inpSpecialRate = null; }
            if (string.IsNullOrEmpty(inpSpecialQty)) { inpSpecialQty = null; }
            if (string.IsNullOrEmpty(inpSpecialDesc)) { inpSpecialDesc = null; }

            string returnValue = null;

            string str = $@"


Declare	
     @IsGroup		ListYesNoType		= '{inpIsGroup}'
	,@SessionID		Uniqueidentifier	= '{inpSessionID}'
	,@emp_num		EmpNumType			= '{inpEmpNum}'
	,@work_date		DateType			= '{inpWorkDate}'
	,@SpecialRate	decimal(9,2)		= {inpSpecialRate}
	,@SpecialQty	int					= {inpSpecialQty}
	,@SpecialDesc	nvarchar(100)		= '{inpSpecialDesc}'

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @err         int
	       ,@EmpCount    int
		   ,@actual_rate decimal(9,2)
		   ,@scan_date   datetime
		   ,@scan_by     EmpNumType
		   ,@item        ItemType
		   ,@job         JobType
		   ,@seq         int
		   ,@ser_num     SerNumType
		   ,@wc_name     nvarchar(25)
		   ,@base_rate   decimal(9,2)
		   ,@RowPointer  RowPointerType
		   ,@delimiter   nchar(1)
           ,@stringVal   nvarchar(100)
           ,@pos         int
		   ,@close_date  DateType
		   
    declare @StringValues table
	(
		seq         int
	   ,stringValue nvarchar(100)
	)

	declare @EmpList table
	(
		emp_num        EmpNumType
	   ,Uf_BuildingNum WhseType
	   ,shift          ShiftType
	   ,dept           DeptType	   
	)

	delete from @EmpList
	delete from @StringValues

	set @scan_date = getdate()
	set @scan_by   = @emp_num

	set @IsGroup     = isnull(@IsGroup,0)
	set @emp_num     = isnull(@emp_num,'')
	set @SpecialRate = isnull(@SpecialRate,0)
	set @SpecialQty  = isnull(@SpecialQty,0)
	set @SpecialDesc = isnull(@SpecialDesc,'')
	set @delimiter = '-'

	if @work_date is null begin
		select 'Invalid Work Date.  Unable to save data.'
		return
	end

	if @SpecialDesc = '' begin
		select 'Invalid Description, cannot be blank.  Unable to save data.'
		return
	end

	if @SpecialQty <= 0 begin
		select 'Invalid Qty, must be greater than zero.  Unable to save data.'
		return
	end

	if @SpecialRate <= 0 begin
		select 'Invalid Rate, must be greater than zero.  Unable to save data.'
		return
	end

	/* make sure work date has not been closed - must be greater than Closed Date in custom parms */
	select @close_date = Datefld1 from ue_JLI_CustParms where ParmId = 'PieceTickets' and ParmKey = 'CloseDate'
	if @close_date is not null begin
		if @work_date < @close_date begin
			select 'Invalid Work Date.  Payroll is Closed for this Work Date.'
			return
		end
	end

	if @IsGroup = 1 and @SessionID is null begin
		select 'Invalid Scan.  Group = True, but No Additional Employees Found for this session.'
		return
	end

	Select @EmpCount = count(*)
	From UserDefinedTypeValues
	Where TypeName = 'JLI_JPGroupScans' And [Description] = @SessionID
    set @EmpCount = isnull(@EmpCount,0)	

	if @IsGroup = 1 and @EmpCount <= 0 begin
		select 'Invalid Scan.  Group = True, but No Additional Employees Found for this session.'
		return
	end

	set @EmpCount = @EmpCount + 1
	set @emp_num = dbo.ExpandKy(7, ltrim(rtrim(@emp_num)))

	if @emp_num = '' begin
		select 'Invalid Employee Number, unable to save scan data.'
		return
	end

	if not exists(select top 1 * from employee_mst where emp_num = @emp_num) begin
		select 'Invalid Employee Number, unable to save scan data.'
		return
	end
	
	set @base_rate = @SpecialRate

	if @IsGroup = 1 begin		
		set @actual_rate = @base_rate / @EmpCount

		Insert Into @EmpList 
		Select employee_mst.emp_num, Uf_BuildingNum, shift, dept
	    From UserDefinedTypeValues
        Inner Join employee_mst 
		On employee_mst.emp_num = UserDefinedTypeValues.value
        Where UserDefinedTypeValues.TypeName = 'JLI_JPGroupScans' And UserDefinedTypeValues.[Description] = @SessionID

		insert into @EmpList 
		     select employee_mst.emp_num, Uf_BuildingNum, shift, dept
		       from employee_mst where employee_mst.emp_num = @emp_num

	end
	else begin
		set @actual_rate = @base_rate

		insert into @EmpList 
		     select employee_mst.emp_num, Uf_BuildingNum, shift, dept
		       from employee_mst where employee_mst.emp_num = @emp_num
	end

	insert into ue_JLI_JPScans (type, emp_num, scan_by, scan_date, work_date, base_rate, group_scan, actual_rate, posted, approved, qty,status, time_desc,last_edit_user,last_edit_date, Uf_BuildingNum, shift, dept)		                  
	     select 'S', emp_num,@emp_num,@scan_date,@work_date,@base_rate, @IsGroup,@actual_rate,0,0, @SpecialQty,'P', @SpecialDesc, @emp_num, @scan_date, Uf_BuildingNum, shift, dept
	       from @EmpList

	select 'record saved successfully.'
	return
END




";


            DataTable dt_Resultset = new DataTable();
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {

                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = str;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    returnValue = sqlCommand.ExecuteScalar()?.ToString();
                }
            }
            catch (Exception ex)
            {
                //createLog("JLI_JPScans", "ue_JLI_SavePieceScan", 190, "ex - " + ex.Message);
            }
            infobar = returnValue;
            return 0; // Indicating success


        }
        public int ue_JLI_SaveOtherScan(out string infobar,
                                        string inpEmpNum = null,
                                        string inpWorkDate = null,
                                        string inpIsGroup = null,
                                        string inpSessionID = null,
                                        string inpOtherRate = null,
                                        string inpOtherQty = null,
                                        string inpOtherDesc = null)
        {
            //createLog("JLI_JPScans", "ue_JLI_SavePieceScan", 56, "inpScanData - " + inpScanData);

            string returnValue = null;

            string query = string.Empty;

            DataTable dt_Resultset = new DataTable();
            try
            {
				query = Resources.ue_JLI_SaveOtherScan;
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = string.Format(query, inpIsGroup, inpSessionID, inpEmpNum, inpWorkDate, inpOtherRate, inpOtherQty, inpOtherDesc);            
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    returnValue = sqlCommand.ExecuteScalar()?.ToString();
                }
            }
            catch (Exception ex)
            {
				returnValue = ex.Message;
                //createLog("JLI_JPScans", "ue_JLI_SavePieceScan", 190, "ex - " + ex.Message);
            }
            infobar = returnValue;
            return 0; // Indicating success


        }
        public int ue_JLI_GetScanTotals(out string outScanTicketsQty
									   ,out string outScanTicketsAmount
									   ,out string outTimeTicketsQty
									   ,out string outTimeTicketsAmount
									   ,out string outOtherTicketsQty
									   ,out string outOtherTicketsAmount
									   ,out string outSpecialTicketsQty
									   ,out string outSpecialTicketsAmount
									   ,out string outTotalTicketsQty
									   ,out string outTotalTicketsAmount
									   ,out string outPayShortFall	
									   ,string inpEmpNum = null
									   ,string inpWorkDate = null
									   ,string inpHrsWorked = null
									   ,string inpRate = null)
        {
            string ScanTicketsQty = string.Empty;
            string ScanTicketsAmount = string.Empty;
            string TimeTicketsQty = string.Empty;
            string TimeTicketsAmount = string.Empty;
            string OtherTicketsQty = string.Empty;
            string OtherTicketsAmount = string.Empty;
            string SpecialTicketsQty = string.Empty;
            string SpecialTicketsAmount = string.Empty;
            string TotalTicketsQty = string.Empty;
            string TotalTicketsAmount = string.Empty;
            string PayShortFall = string.Empty;

            if (string.IsNullOrEmpty(inpEmpNum)) { inpEmpNum = null; }
            if (string.IsNullOrEmpty(inpWorkDate)) { inpWorkDate = null; }
            if (string.IsNullOrEmpty(inpHrsWorked)) { inpHrsWorked = "8"; }
            if (string.IsNullOrEmpty(inpRate)) { inpRate = "0"; }

            string str = $@"
Declare
 @emp_num		EmpNumType		= '{inpEmpNum}'
,@work_date		DateType		= '{inpWorkDate}'
,@hrs_worked	int				= '{inpHrsWorked}'
,@rate			decimal(9,3)	= {inpRate}

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @err                  int
           ,@ScanTicketsQty       int
           ,@ScanTicketsAmount    decimal(18,5)
           ,@TimeTicketsQty       int
           ,@TimeTicketsAmount    decimal(18,5)
           ,@OtherTicketsQty      int
           ,@OtherTicketsAmount   decimal(18,5)
           ,@SpecialTicketsQty    int
           ,@SpecialTicketsAmount decimal(18,5)
           ,@TotalTicketsQty      int
           ,@TotalTicketsAmount   decimal(18,5)
		   ,@pay_shortfall        decimal(18,5)
		   ,@must_pay             decimal(18,5)

	set @emp_num = isnull(@emp_num,'')
	select @emp_num = dbo.ExpandKy(7, ltrim(rtrim(@emp_num)))
	set @hrs_worked = isnull(@hrs_worked,0)
	set @rate = isnull(@rate,0)

	if @hrs_worked = 0 begin
		set @hrs_worked = 8
	end

	set @must_pay = @rate * @hrs_worked

	select @ScanTicketsAmount = sum(actual_rate)
	      ,@ScanTicketsQty    = count(*)
	  from ue_JLI_JPScans J
     where J.emp_num   = @emp_num
	   and J.work_date = @work_date
	   and J.[type]    = 'P'
	   and status <> 'V' -- AIT JCW 06/21/19 Added

	set @ScanTicketsQty       = isnull(@ScanTicketsQty,0)
	set @ScanTicketsAmount    = isnull(@ScanTicketsAmount,0)

	select @TimeTicketsAmount = sum(J.actual_rate * J.time_qty)
	      ,@TimeTicketsQty    = count(*)
	  from ue_JLI_JPScans J
     where J.emp_num   = @emp_num
	   and J.work_date = @work_date
	   and J.[type]    = 'T'
	   and status <> 'V' -- AIT JCW 06/21/19 Added

	set @TimeTicketsQty       = isnull(@TimeTicketsQty,0)
	set @TimeTicketsAmount    = isnull(@TimeTicketsAmount,0)

	select @OtherTicketsAmount = sum(J.actual_rate * J.qty)
	      ,@OtherTicketsQty    = count(*)
	  from ue_JLI_JPScans J
     where J.emp_num   = @emp_num
	   and J.work_date = @work_date
	   and J.[type]    = 'O'
	   and status <> 'V' -- AIT JCW 06/21/19 Added

	set @OtherTicketsQty      = isnull(@OtherTicketsQty,0)
	set @OtherTicketsAmount   = isnull(@OtherTicketsAmount,0)
	
	select @SpecialTicketsAmount = sum(J.actual_rate * J.qty)
	      ,@SpecialTicketsQty    = count(*)
	  from ue_JLI_JPScans J
     where J.emp_num   = @emp_num
	   and J.work_date = @work_date
	   and J.[type]    = 'S'
	   and status <> 'V' -- AIT JCW 06/21/19 Added

	set @SpecialTicketsQty    = isnull(@SpecialTicketsQty,0)
	set @SpecialTicketsAmount = isnull(@SpecialTicketsAmount,0)

	set @TotalTicketsQty    = @ScanTicketsQty + @TimeTicketsQty + @OtherTicketsQty + @SpecialTicketsQty
	set @TotalTicketsAmount = @ScanTicketsAmount + @TimeTicketsAmount + @OtherTicketsAmount + @SpecialTicketsAmount

	set @TotalTicketsQty      = isnull(@TotalTicketsQty,0)
	set @TotalTicketsAmount   = isnull(@TotalTicketsAmount,0)

	set @pay_shortfall = @must_pay - @TotalTicketsAmount
	if @pay_shortfall is null or @pay_shortfall < 0 begin
		set @pay_shortfall = 0
	end

	select IsNull(@ScanTicketsQty,0)		As ScanTicketsQty
		  ,IsNull(@ScanTicketsAmount,0)		As ScanTicketsAmount
		  ,IsNull(@TimeTicketsQty,0)		As TimeTicketsQty
		  ,IsNull(@TimeTicketsAmount,0)		As TimeTicketsAmount
		  ,IsNull(@OtherTicketsQty,0)		As OtherTicketsQty
		  ,IsNull(@OtherTicketsAmount,0)	As OtherTicketsAmount
		  ,IsNull(@SpecialTicketsQty,0)		As SpecialTicketsQty
		  ,IsNull(@SpecialTicketsAmount,0)	As SpecialTicketsAmount
		  ,IsNull(@TotalTicketsQty,0)		As TotalTicketsQty
		  ,IsNull(@TotalTicketsAmount,0)	As TotalTicketsAmount
		  ,IsNull(@pay_shortfall,0)			As PayShortFall

	return


END";
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
					foreach (DataRow row in dt_Resultset.Rows)
					{
						ScanTicketsQty = row["ScanTicketsQty"].ToString();
						ScanTicketsAmount = row["ScanTicketsAmount"].ToString();
						TimeTicketsQty = row["TimeTicketsQty"].ToString();
						TimeTicketsAmount = row["TimeTicketsAmount"].ToString();
						OtherTicketsQty = row["OtherTicketsQty"].ToString();
						OtherTicketsAmount = row["OtherTicketsAmount"].ToString();
						SpecialTicketsQty = row["SpecialTicketsQty"].ToString();
						SpecialTicketsAmount = row["SpecialTicketsAmount"].ToString();
						TotalTicketsQty = row["TotalTicketsQty"].ToString();
						TotalTicketsAmount = row["TotalTicketsAmount"].ToString();
						PayShortFall = row["PayShortFall"].ToString();
					}//foreach (DataRow row in dt_Resultset.Rows)
                }
            }
            catch (Exception ex)
            {
                //createLog("JLI_JPScans", "ue_JLI_GetScanTotals", 56, "ex - " + ex.Message);
            }
            outScanTicketsQty = ScanTicketsQty;
			outScanTicketsAmount = ScanTicketsAmount;
			outTimeTicketsQty = TimeTicketsQty;
			outTimeTicketsAmount = TimeTicketsAmount;
			outOtherTicketsQty = OtherTicketsQty;
			outOtherTicketsAmount = OtherTicketsAmount;
			outSpecialTicketsQty = SpecialTicketsQty;
			outSpecialTicketsAmount = SpecialTicketsAmount;
			outTotalTicketsQty = TotalTicketsQty;
			outTotalTicketsAmount = TotalTicketsAmount;
            outPayShortFall = PayShortFall;

            return 0;

        }







    }
}
