using Mongoose.IDO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ue_JLI_UnusualOrderEntryDataNotify
{
    public class ue_JLI_UnusualOrderEntryDataNotify : IDOExtensionClass
    {
        public interface IIDOExtensionClass
        {
            void SetContext(Mongoose.IDO.IIDOExtensionClassContext context);
        }
        public int ue_JLI_UnusualOrderEntryDataNotifySp()
        {

            string str = $@"	

--Exec SetSiteSp 'JLI',Null
DECLARE
  @Context VARBINARY(128)
, @Site NCHAR(8) = 'JLI'
, @PadSite NCHAR(8)

SELECT @Site = site FROM parms_mst



DECLARE
  @UserName				UsernameType	= 'sa'
, @ToEmails				nvarchar(300)	= NULL
, @Emails				nvarchar(200)
, @Subject				nvarchar(200)
, @MailMsg				nvarchar(Max)
--, @SqlCrLf				nchar(2)
, @xml					varchar(max)	= ''

Declare 
 @CoNum			CoNumType
,@CoLine		Nvarchar(5)
,@Item			ItemType
,@OrderStatus	Nchar(1)
,@UnitPrice		Int
,@QtyOrdered	Int
,@Customer		Nvarchar(10)
,@CreateDate	Nvarchar(10)
,@DaysPassed	Nvarchar(10)
,@TakenBy		Nvarchar(50)

Declare @ResultSet Table(
 Remark			Nchar(20)
,CoNum			CoNumType
,CoLine			Nvarchar(5)
,Item			ItemType
,OrderStatus	Nchar(1)
,UnitPrice		Int
,QtyOrdered		Int
,Customer		Nvarchar(10)
,CreateDate		Nvarchar(10)
,DaysPassed		Nvarchar(10)
,TakenBy		Nvarchar(50)
)




-- Declare a local static cursor
DECLARE MyCursor CURSOR LOCAL STATIC FOR
Select 
 co.co_num					As 'Order Number'
,coitem.co_line				As 'Order Line'
,coitem.item				As 'Item'
,coitem.stat				As 'Order Status'
,CAST(coitem.price_conv AS FLOAT)		As 'Unit Price'
,CAST(coitem.qty_ordered_conv AS FLOAT)	As 'Qty Ordered'
,co.cust_num				As 'Customer #'
,Cast(coitem.CreateDate As Date)	As 'Create Date'
,DATEDIFF(DAY, Cast(coitem.CreateDate As Date), Cast(GetDate() As Date))	 As 'Days Passed'
,co.taken_by				As 'Taken By'
From co_mst co 
Inner Join coitem_mst coitem 
On co.co_num = coitem.co_num
Where co.stat <> 'C' And coitem.stat <> 'C' And co.cust_num <> 'C000410'
And Left(coitem.item,2) Not In ('PT','FU','FR','-9') And SUBSTRING(item,3,3) <> '999'
And co.Uf_Warranty Is Null
And (price_conv <= 0 Or price_conv >= 1000 Or qty_ordered_conv >= 100)
Order By coitem.CreateDate Desc;

-- Open and iterate
OPEN MyCursor;

FETCH NEXT FROM MyCursor INTO @CoNum,@CoLine,@Item,@OrderStatus,@UnitPrice,@QtyOrdered,@Customer,@CreateDate,@DaysPassed,@TakenBy;
WHILE @@FETCH_STATUS = 0
BEGIN
	--------------------------------
	If Not Exists(Select Top 1 1 From ue_JLI_CO_Valid Where co_num = @CoNum And co_line = @CoLine)
	Begin
		If @UnitPrice = '0'
			Insert Into @ResultSet(Remark,CoNum,CoLine,Item,OrderStatus,UnitPrice,QtyOrdered,Customer,CreateDate,DaysPassed,TakenBy)
			Select 'Unit Price is Zero',@CoNum,@CoLine,@Item,@OrderStatus,@UnitPrice,@QtyOrdered,@Customer,@CreateDate,@DaysPassed,@TakenBy
		If @UnitPrice >= 1500
			Insert Into @ResultSet(Remark,CoNum,CoLine,Item,OrderStatus,UnitPrice,QtyOrdered,Customer,CreateDate,DaysPassed,TakenBy)
			Select 'Unit Price >1500',@CoNum,@CoLine,@Item,@OrderStatus,@UnitPrice,@QtyOrdered,@Customer,@CreateDate,@DaysPassed,@TakenBy
		If @QtyOrdered = 0
			Insert Into @ResultSet(Remark,CoNum,CoLine,Item,OrderStatus,UnitPrice,QtyOrdered,Customer,CreateDate,DaysPassed,TakenBy)
			Select 'Qty Ordered is Zero',@CoNum,@CoLine,@Item,@OrderStatus,@UnitPrice,@QtyOrdered,@Customer,@CreateDate,@DaysPassed,@TakenBy
		If @QtyOrdered > 100
			Insert Into @ResultSet(Remark,CoNum,CoLine,Item,OrderStatus,UnitPrice,QtyOrdered,Customer,CreateDate,DaysPassed,TakenBy)
			Select 'Qty Ordered >100',@CoNum,@CoLine,@Item,@OrderStatus,@UnitPrice,@QtyOrdered,@Customer,@CreateDate,@DaysPassed,@TakenBy
	End
	--------------------------------
    FETCH NEXT FROM MyCursor INTO @CoNum,@CoLine,@Item,@OrderStatus,@UnitPrice,@QtyOrdered,@Customer,@CreateDate,@DaysPassed,@TakenBy;
END

-- Clean up
CLOSE MyCursor;
DEALLOCATE MyCursor;

Set @Subject = 'Order Entry unusual data : ' + Format(Getdate(),'MM/dd/yyyy hh:mm:ss tt')

Select @xml = CAST((Select Remark			AS 'td',''
						,CoNum				AS 'td',''
						,CoLine				AS 'td',''
						,Item				AS 'td',''
						,OrderStatus		AS 'td',''
						,UnitPrice			AS 'td',''
						,QtyOrdered			AS 'td',''
						,Customer			AS 'td',''
						,CreateDate			AS 'td',''
						,DaysPassed			AS 'td',''
						,TakenBy			AS 'td',''
					From @ResultSet 
					Where CAST(CreateDate As Date) = CAST(GETDATE() As date) 
					Order By CreateDate Desc
					FOR XML PATH('tr'), ELEMENTS ) AS NVARCHAR(MAX))

Set @xml = REPLACE(@xml,'<tr>','<tr style=''background-color: #fff63d;''>')

Select @xml = ISNULL(@xml,'') + CAST((Select Remark			AS 'td',''
										,CoNum				AS 'td',''
										,CoLine				AS 'td',''
										,Item				AS 'td',''
										,OrderStatus		AS 'td',''
										,UnitPrice			AS 'td',''
										,QtyOrdered			AS 'td',''
										,Customer			AS 'td',''
										,CreateDate			AS 'td',''
										,DaysPassed			AS 'td',''
										,TakenBy			AS 'td',''
									From @ResultSet 
									Where CAST(CreateDate As Date) <> CAST(GETDATE() As date) 
									Order By CreateDate Desc
									FOR XML PATH('tr'), ELEMENTS ) AS NVARCHAR(MAX))
If @xml > ''
Begin
	Set @MailMsg = '<table border=1 style=width: auto; height: auto;>
				<tr> <th colspan=11 style=text-align: center;>'+@Subject+'</th> </tr>
				<tr>
					<th>Remark</th>
					<th>Order Number</th>
					<th>Order Line</th>
					<th>Item</th>
					<th>Order Status</th>
					<th>Unit Price</th>
					<th>Qty Ordered</th>
					<th>Customer #</th>
					<th>Create Date</th>
					<th>Days Passed</th>
					<th>Taken By</th>
				</tr>'
	Set @MailMsg = @MailMsg + @xml + '</table>'

	Select Top 1 @ToEmails = Charfld1 From ue_JLI_CustParms Where parmid='ue_JLI_UnusualOrderEntryDataNotify'
	--Test
	--Set @ToEmails = 'ramakrishna.balimidi@zeshtit.com;ushan.dalwis@jonathanlouis.net'

	Insert Into ue_JLI_EmailQueue(EmailDoc,email_sub,email_body,email_to,submitted_by,body_format )
	Select 1,@Subject,@MailMsg,@ToEmails,@UserName,'HTML' 
  		
End

";


            DataTable dt_Resultset = new DataTable();
            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = str;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_UnusualOrderEntryDataNotify", "ue_JLI_UnusualOrderEntryDataNotifySp", 190, "ex - " + ex.Message);
            }
            return 0;
        }
    }
}
