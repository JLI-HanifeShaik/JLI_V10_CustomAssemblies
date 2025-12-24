using Mongoose.Core.Common;
using Mongoose.IDO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ue_JLI_PieceRateEditLogNotify
{
    public class ue_JLI_PieceRateEditLogNotify : IDOExtensionClass
    {
        public interface IIDOExtensionClass
        {
            void SetContext(Mongoose.IDO.IIDOExtensionClassContext context);
        }
        public int ue_JLI_PieceRateEditLogNotifySp()
        {

            string str = $@"	

	

	
	DECLARE
	  @UserName				UsernameType	= 'sa'
	, @ToEmails				nvarchar(300)	= NULL
	, @Emails				nvarchar(200)
	, @Subject				nvarchar(200)
	, @MailMsg				nvarchar(Max)
	--, @SqlCrLf				nchar(2)
	, @xml					varchar(max)	= ''

	Set @Subject = 'Piece Rate Edit Log : ' + Format(Getdate(),'MM/dd/yyyy') + ' UNITED STATES'
	Select @xml = CAST((Select CONCAT(template_country,'-',template_id) AS 'td',''
							,template_whse		AS 'td',''
							,style_code			AS 'td',''
							,style_prefix		AS 'td',''
							,Format(log_date,'MM/dd/yyyy') AS 'td',''
							,log_details		AS 'td',''
							,log_user			AS 'td',''
						From ue_JLI_JPStyleRateAuditLog 
						Where Convert(Date,log_date) >= Convert(Date,Getdate()) And template_country = 'UNITED STATES'		
						FOR XML PATH('tr'), ELEMENTS ) AS NVARCHAR(MAX))
	If @xml > ''
	Begin
		Set @MailMsg = '<table border=""""1"""" width=""""auto"""" height=""""auto"""">
					<tr> <th colspan=""7"" style=""text-align: center;"">'+@Subject+'</th> </tr>
					<tr>
						<th>BUILDING</th>
						<th>TEMPLATE WHSE</th>
						<th>STYLE</th>
						<th>STYLE PREFIX</th>
						<th>LOG DATE</th>
						<th>LOG DETAILS</th>
						<th>USER</th>
					</tr>'
		Set @MailMsg = @MailMsg + @xml + '</table>'

		Select Top 1 @ToEmails = Charfld1 From ue_JLI_CustParms Where parmid='PieceTicketAuditLog-Emails-US'
		--Test
		--Set @ToEmails = 'ramakrishna.balimidi@zeshtit.com;ushan.dalwis@jonathanlouis.net'

		Insert Into ue_JLI_EmailQueue(EmailDoc,email_sub,email_body,email_to,submitted_by,body_format )
		Select 1,@Subject,@MailMsg,@ToEmails,@UserName,'HTML' 
  		
	End
	


	
	Set @xml = ''
	Set @Subject = 'Piece Rate Edit Log : ' + Format(Getdate(),'MM/dd/yyyy') + ' MEXICO'
	Select @xml = CAST((Select CONCAT(template_country,'-',template_id) AS 'td',''
							,template_whse		AS 'td',''
							,style_code			AS 'td',''
							,style_prefix		AS 'td',''
							,Format(log_date,'MM/dd/yyyy') AS 'td',''
							,log_details			AS 'td',''
							,log_user			AS 'td',''
						From ue_JLI_JPStyleRateAuditLog 
						Where Convert(Date,log_date) >= Convert(Date,Getdate()) And template_country = 'MEXICO'		
						FOR XML PATH('tr'), ELEMENTS ) AS NVARCHAR(MAX))
	If @xml > ''
	Begin
		Set @MailMsg = '<table border=""""1"""" width=""""auto"""" height=""""auto"""">
					<tr> <th colspan=""7"" style=""text-align: center;"">'+@Subject+'</th> </tr>
					<tr>
						<th>BUILDING</th>
						<th>TEMPLATE WHSE</th>
						<th>STYLE</th>
						<th>STYLE PREFIX</th>
						<th>LOG DATE</th>
						<th>LOG DETAILS</th>
						<th>USER</th>
					</tr>'
		Set @MailMsg = @MailMsg + @xml + '</table>'

		Select Top 1 @ToEmails = Charfld1 From ue_JLI_CustParms Where parmid='PieceTicketAuditLog-Emails-MX'
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
                //createLog("ue_JLI_PieceRateEditLogNotify", "ue_JLI_Rpt_DailyOrderRecieptSummary", 190, "ex - " + ex.Message);
            }
            return 0;
        }
    }
}
