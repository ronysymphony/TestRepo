using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Shampan.Core.ExtentionMethod;
using Shampan.Core.Interfaces.Repository.Audit;
using Shampan.Models;
using Shampan.Models.AuditModule;
using SixLabors.ImageSharp.ColorSpaces;

namespace Shampan.Repository.SqlServer.Audit
{
	public class AuditMasterRepository : Repository, IAuditMasterRepository
	{
		private DbConfig _dbConfig;
		public AuditMasterRepository(SqlConnection context, SqlTransaction transaction, DbConfig dbConfig)
		{
			this._context = context;
			this._transaction = transaction;
			this._dbConfig = dbConfig;
		}


		public List<AuditMaster> GetAll(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";
			List<AuditMaster> VMs = new List<AuditMaster>();
			DataTable dt = new DataTable();

			try
			{
				sqlText = @"
SELECT  [Id]
      ,[Code]
      ,[Name]
      ,[AuditTypeId]
      ,[IsPlaned]
      ,[Location]
      ,[TeamId]
      ,[BranchID]
      ,[StartDate]
      ,[EndDate]
      ,[Duratiom]
      ,[BusinessTarget]
      ,[AuditStatus]
      ,[ReportStatus]
      ,[CreatedBy]
      ,[CreatedOn]
      ,[CreatedFrom]
      ,[LastUpdateBy]
      ,[LastUpdateOn]
      ,[LastUpdateFrom]
      ,[IsPosted]
      ,[PostedBy]
      ,[PostedOn]
      ,[PostedFrom]
      ,[ReasonOfUnPost]
      ,[CompanyId]
      ,[IsPost]
      ,[Remarks]
      ,(select AuditType from AuditTypes where Id = A_Audits.AuditTypeId)AuditTypeName
      ,(select TeamName from A_Teams where Id = A_Audits.TeamId)TeamName
  FROM [A_Audits]  

where 1=1  

";
				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.Fill(dt);

				VMs = dt.ToList<AuditMaster>();

				return VMs;


			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public List<AuditUser> GetAuditUserByTeamId(string TeamId)
		{
			string sqlText = "";
			List<AuditUser> VMs = new List<AuditUser>();
			DataTable dt = new DataTable();

			//Format(ICReceipts.ReceiptDate, 'yyyy-MM-dd') ReceiptDate,


			try
			{
				sqlText = @"
select TM.UserId,u.Email EmailAddress from A_TeamMembers TM
left Join [SSLAuditAuthDB].[dbo].[AspNetUsers] u on u.Id=TM.UserId
where TeamId=@TeamId
";

				// sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, true);

				// ToDo Escape Sql Injection
				//sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
				//sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";

				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, null, null);

				objComm.SelectCommand.Parameters.AddWithValue("@TeamId", TeamId);

				objComm.Fill(dt);

				VMs = dt
					.ToList<AuditUser>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public List<AuditMaster> FeedBackGetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";
			List<AuditMaster> VMs = new List<AuditMaster>();
			DataTable dt = new DataTable();

			//Format(ICReceipts.ReceiptDate, 'yyyy-MM-dd') ReceiptDate,

			try
			{
				sqlText = @"

declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);
declare @UserId varchar(max);

select @UserId=Id  from SSLAuditAuthDB.dbo.AspNetUsers where UserName=@UserName

Create table #TempId(Id int)
select @A1=AuditFeedbackApproval1,@A2=AuditFeedbackApproval2,@A3=AuditFeedbackApproval3,@A4=AuditFeedbackApproval4  from UserRolls
where  IsAuditFeedback=1 and UserId=@UserId

if(@A4=1)
begin
insert into  #TempId(Id)
select Id from A_Audits where IssueIsApprovedL4=1 and  isnull(BFIsRejected,0)=0 and BFIsApprovedL1=1and BFIsApprovedL2=1 and BFIsApprovedL3=1and BFIsApprovedL4=0
end
if(@A3=1)
begin
insert into  #TempId(Id)
 
select id from A_Audits where IssueIsApprovedL4=1 and  BFIsRejected=0 and BFIsApprovedL1=1and BFIsApprovedL2=1 and BFIsApprovedL3=0 and BFIsApprovedL4=0
end
if(@A2=1)
begin
insert into  #TempId(Id)
select id from A_Audits where IssueIsApprovedL4=1 and  BFIsRejected=0 and BFIsApprovedL1=1and BFIsApprovedL2=0 and BFIsApprovedL3=0 and BFIsApprovedL4=0
end
if(@A1=1)
begin
insert into  #TempId(Id)
select id from A_Audits where IssueIsApprovedL4=1 and  BFIsRejected=0 and BFIsApprovedL1=0and BFIsApprovedL2=0 and BFIsApprovedL3=0 and BFIsApprovedL4=0
end

 select 
 ad.Id
,ad.[Code]
,isnull(ad.[Name] ,'')Name
,Format(ad.[StartDate], 'yyyy-MM-dd') StartDate
,Format(ad.[EndDate], 'yyyy-MM-dd') EndDate

,case 
				 when isnull(ad.BFIsRejected,0)=1 then 'Reject'
				 when isnull(ad.BFIsApprovedL4,0)=1 then 'Approveed' 
				 when isnull(ad.BFIsApprovedL3,0)=1 then 'Waiting For Approval 4' 
				 when isnull(ad.BFIsApprovedL2,0)=1 then 'Waiting For Approval 3' 
				 when isnull(ad.BFIsApprovedL1,0)=1 then 'Waiting For Approval 2' 
				 else 'Waiting For Approval 1' 
				 end ApproveStatus
				 ,case when isnull(ad.IsAudited,0)=1 then 'Audited' else 'Not yet Audited' end AuditStatus
,ad.[IsPost]
from A_Audits ad 
  where 1=1
and IsApprovedL4=1
and ad.id in( select id from #TempId)

";

				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, false);

				// ToDo Escape Sql Injection
				sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
				sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";
				sqlText += @"  drop table #TempId  ";


				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.SelectCommand.Parameters.Add("@UserName", SqlDbType.NChar).Value = index.UserName;

				//objComm.SelectCommand.Parameters.AddWithValue("@BranchId", index.CurrentBranchid);

				objComm.Fill(dt);

				VMs = dt
					.ToList<AuditMaster>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}


		public List<AuditMaster> IssueGetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";
			List<AuditMaster> VMs = new List<AuditMaster>();
			DataTable dt = new DataTable();

			//Format(ICReceipts.ReceiptDate, 'yyyy-MM-dd') ReceiptDate,

			try
			{
				sqlText = @"


 declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);
declare @UserId varchar(max);

select @UserId=Id  from SSLAuditAuthDB.dbo.AspNetUsers where UserName=@UserName

Create table #TempId(Id int)
select @A1=AuditIssueApproval1,@A2=AuditIssueApproval2,@A3=AuditIssueApproval3,@A4=AuditIssueApproval4  from UserRolls
where  IsAuditIssue=1 and UserId=@UserId


if(@A4=1)
begin
insert into  #TempId(Id)
select Id from A_Audits where IsApprovedL4=1 and  isnull(IssueIsRejected,0)=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=1 and IssueIsApprovedL3=1and IssueIsApprovedL4=0
end
if(@A3=1)
begin
insert into  #TempId(Id)
 
select id from A_Audits where IsApprovedL4=1 and  IssueIsRejected=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=1 and IssueIsApprovedL3=0 and IssueIsApprovedL4=0
end
if(@A2=1)
begin
insert into  #TempId(Id)
select id from A_Audits where IsApprovedL4=1 and  IssueIsRejected=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=0 and IssueIsApprovedL3=0 and IssueIsApprovedL4=0
end
if(@A1=1)
begin
insert into  #TempId(Id)
select id from A_Audits where IsApprovedL4=1 and  IssueIsRejected=0 and IssueIsApprovedL1=0and IssueIsApprovedL2=0 and IssueIsApprovedL3=0 and IssueIsApprovedL4=0
end

 select 
 ad.Id
,ad.[Code]
,isnull(ad.[Name] ,'')Name
,Format(ad.[StartDate], 'yyyy-MM-dd') StartDate
,Format(ad.[EndDate], 'yyyy-MM-dd') EndDate

,case 
				 when isnull(ad.IssueIsRejected,0)=1 then 'Reject'
				 when isnull(ad.IssueIsApprovedL4,0)=1 then 'Approveed' 
				 when isnull(ad.IssueIsApprovedL3,0)=1 then 'Waiting For Approval 4' 
				 when isnull(ad.IssueIsApprovedL2,0)=1 then 'Waiting For Approval 3' 
				 when isnull(ad.IssueIsApprovedL1,0)=1 then 'Waiting For Approval 2' 
				 else 'Waiting For Approval 1' 
				 end ApproveStatus
				 ,case when isnull(ad.IsAudited,0)=1 then 'Audited' else 'Not yet Audited' end AuditStatus
,ad.[IsPost]
from A_Audits ad 
  where 1=1
and IsApprovedL4=1
and ad.id in( select id from #TempId)

";

				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, false);

				// ToDo Escape Sql Injection
				sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
				sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";
				sqlText += @"  drop table #TempId";

				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.SelectCommand.Parameters.Add("@UserName", SqlDbType.NChar).Value = index.UserName;

				//objComm.SelectCommand.Parameters.AddWithValue("@BranchId", index.CurrentBranchid);

				objComm.Fill(dt);

				VMs = dt
					.ToList<AuditMaster>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}






		public List<AuditMaster> GetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";
			List<AuditMaster> VMs = new List<AuditMaster>();
			DataTable dt = new DataTable();

			//Format(ICReceipts.ReceiptDate, 'yyyy-MM-dd') ReceiptDate,

			try
			{
				sqlText = @"

declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);
declare @UserId varchar(max);

select @UserId=Id  from SSLAuditAuthDB.dbo.AspNetUsers where UserName=@UserName

Create table #TempId(Id int)
select @A1=AuditApproval1,@A2=AuditApproval2,@A3=AuditApproval3,@A4=AuditApproval4  from UserRolls
where  IsAudit=1 and UserId=@UserId

if(@A4=1)
begin
insert into  #TempId(Id)
select Id from A_Audits where IsRejected=0 and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=1and IsApprovedL4=0
end
if(@A3=1)
begin
insert into  #TempId(Id)
 
select id from A_Audits where IsRejected=0 and IsApprovedL1=1 and IsApprovedL2=1 and IsApprovedL3=0 and IsApprovedL4=0
end
if(@A2=1)
begin
insert into  #TempId(Id)
select id from A_Audits where IsRejected=0 and IsApprovedL1=1 and IsApprovedL2=0 and IsApprovedL3=0 and IsApprovedL4=0
end
if(@A1=1)
begin
insert into  #TempId(Id)
select id from A_Audits where IsRejected=0 and IsApprovedL1=0 and IsApprovedL2=0 and IsApprovedL3=0 and IsApprovedL4=0
end

 select 
 ad.Id
,ad.[Code]
,isnull(ad.[Name] ,'')Name
,Format(ad.[StartDate], 'yyyy-MM-dd') StartDate
,Format(ad.[EndDate], 'yyyy-MM-dd') EndDate

,case 
				 when isnull(ad.IsRejected,0)=1 then 'Reject'
				 when isnull(ad.IsApprovedL4,0)=1 then 'Approveed' 
				 when isnull(ad.IsApprovedL3,0)=1 then 'Waiting For Approval 4' 
				 when isnull(ad.IsApprovedL2,0)=1 then 'Waiting For Approval 3' 
				 when isnull(ad.IsApprovedL1,0)=1 then 'Waiting For Approval 2' 
				 else 'Waiting For Approval 1' 
				 end ApproveStatus
				 ,case when isnull(ad.IsAudited,0)=1 then 'Audited' else 'Not yet Audited' end AuditStatus




,ad.[IsPost]


from A_Audits ad 


where 1=1

and ad.id in( select id from #TempId)

";
				if (index.self)
				{
					
				sqlText += @"  and ad.CreatedBy = @UserName";
				}

				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, false);

				// ToDo Escape Sql Injection
				sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
				sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";
				sqlText += @"  drop table #TempId  ";


				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.SelectCommand.Parameters.Add("@UserName", SqlDbType.NChar).Value = index.UserName;

				//objComm.SelectCommand.Parameters.AddWithValue("@BranchId", index.CurrentBranchid);

				objComm.Fill(dt);

				VMs = dt
					.ToList<AuditMaster>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public List<AuditMaster> GetIndexDataSelfStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";
			List<AuditMaster> VMs = new List<AuditMaster>();
			DataTable dt = new DataTable();

			//Format(ICReceipts.ReceiptDate, 'yyyy-MM-dd') ReceiptDate,

			try
			{
				sqlText = @"

declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);
declare @UserId varchar(max);

select @UserId=Id  from SSLAuditAuthDB.dbo.AspNetUsers where UserName=@UserName

Create table #TempId(Id int)
select @A1=AuditApproval1,@A2=AuditApproval1,@A3=AuditApproval1,@A4=AuditApproval1  from UserRolls
where  IsAudit=1 and UserId=@UserId

if(@A4=1)
begin
insert into  #TempId(Id)
select Id from A_Audits where IsRejected=0 and IsApprovedL1=1and IsApprovedL2=1 and IsApprovedL3=1and IsApprovedL4=0
end
if(@A3=1)
begin
insert into  #TempId(Id)
 
select id from A_Audits where IsRejected=0 and IsApprovedL1=1and IsApprovedL2=1 and IsApprovedL3=0 and IsApprovedL4=0
end
if(@A2=1)
begin
insert into  #TempId(Id)
select id from A_Audits where IsRejected=0 and IsApprovedL1=1and IsApprovedL2=0 and IsApprovedL3=0 and IsApprovedL4=0
end
if(@A1=1)
begin
insert into  #TempId(Id)
select id from A_Audits where IsRejected=0 and IsApprovedL1=0and IsApprovedL2=0 and IsApprovedL3=0 and IsApprovedL4=0
end

 select 
 ad.Id
,ad.[Code]
,isnull(ad.[Name] ,'')Name
,Format(ad.[StartDate], 'yyyy-MM-dd') StartDate
,Format(ad.[EndDate], 'yyyy-MM-dd') EndDate

,case 
				 when isnull(ad.IsRejected,0)=1 then 'Reject'
				 when isnull(ad.IsApprovedL4,0)=1 then 'Approveed' 
				 when isnull(ad.IsApprovedL3,0)=1 then 'Waiting For Approval 4' 
				 when isnull(ad.IsApprovedL2,0)=1 then 'Waiting For Approval 3' 
				 when isnull(ad.IsApprovedL1,0)=1 then 'Waiting For Approval 2' 
				 else 'Waiting For Approval 1' 
				 end ApproveStatus
				 ,case when isnull(ad.IsAudited,0)=1 then 'Audited' else 'Not yet Audited' end AuditStatus




,ad.[IsPost]


from A_Audits ad 


  where 1=1  and ad.CreatedBy=@UserName

and ad.id in( select id from #TempId)

";

				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, false);

				// ToDo Escape Sql Injection
				sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
				sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";
				sqlText += @"  drop table #TempId  ";


				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.SelectCommand.Parameters.Add("@UserName", SqlDbType.NChar).Value = index.UserName;

				//objComm.SelectCommand.Parameters.AddWithValue("@BranchId", index.CurrentBranchid);

				objComm.Fill(dt);

				VMs = dt
					.ToList<AuditMaster>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}



		public List<AuditMaster> GetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";
			List<AuditMaster> VMs = new List<AuditMaster>();
			DataTable dt = new DataTable();

			



			try
			{
				sqlText = @"



select 
 ad.Id
,ad.[Code]
,isnull(ad.[Name] ,'')Name
,Format(ad.[StartDate], 'yyyy-MM-dd') StartDate
,Format(ad.[EndDate], 'yyyy-MM-dd') EndDate





,case 
				 when isnull(ad.IsRejected,0)=1 then 'Reject'
				 when isnull(ad.IsApprovedL4,0)=1 then 'Approveed' 
				 when isnull(ad.IsApprovedL3,0)=1 then 'Waiting For Approval 4' 
				 when isnull(ad.IsApprovedL2,0)=1 then 'Waiting For Approval 3' 
				 when isnull(ad.IsApprovedL1,0)=1 then 'Waiting For Approval 2' 
				 else 'Waiting For Approval 1' 
				 end ApproveStatus
				 ,case when isnull(ad.IsAudited,0)=1 then 'Audited' else 'Not yet Audited' end AuditStatus




,ad.[IsPost]


from A_Audits ad 


  where 1=1


";

				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, false);

				// ToDo Escape Sql Injection
				sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
				sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";

				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				//objComm.SelectCommand.Parameters.AddWithValue("@BranchId", index.CurrentBranchid);

				objComm.Fill(dt);

				VMs = dt
					.ToList<AuditMaster>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public int GetIndexDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";

			DataTable dt = new DataTable();
			try
			{
				sqlText = @"
              select count(ad.ID) FilteredCount
             --select count(ad.IsApprovedL4) FilteredCount
            from  A_Audits ad 

             where 1=1  

";

				//sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);
				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);
				//objComm.SelectCommand.Parameters.AddWithValue("@BranchId", index.CurrentBranchid);
				objComm.Fill(dt);
				return Convert.ToInt32(dt.Rows[0][0]);

			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public int GetAuditApprovedDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";

			DataTable dt = new DataTable();
			try
			{
				sqlText = @"
              declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);
declare @UserId varchar(max);

select @UserId = Id from SSLAuditAuthDB.dbo.AspNetUsers where UserName = 'erp';

Create table #TempId(Id int);

select @A1 = AuditApproval1, @A2 = AuditApproval2, @A3 = AuditApproval3, @A4 = AuditApproval4  
from UserRolls
where IsAudit = 1 and UserId = @UserId;

if (@A4 = 1)
begin
    insert into #TempId(Id)
    select Id from A_Audits where IsRejected = 0 and IsApprovedL1 = 1 and IsApprovedL2 = 1 and IsApprovedL3 = 1 and IsApprovedL4 = 0;
end
if (@A3 = 1)
begin
    insert into #TempId(Id)
    select id from A_Audits where IsRejected = 0 and IsApprovedL1 = 1 and IsApprovedL2 = 1 and IsApprovedL3 = 0 and IsApprovedL4 = 0;
end
if (@A2 = 1)
begin
    insert into #TempId(Id)
    select id from A_Audits where IsRejected = 0 and IsApprovedL1 = 1 and IsApprovedL2 = 0 and IsApprovedL3 = 0 and IsApprovedL4 = 0;
end
if (@A1 = 1)
begin
    insert into #TempId(Id)
    select id from A_Audits where IsRejected = 0 and IsApprovedL1 = 0 and IsApprovedL2 = 0 and IsApprovedL3 = 0 and IsApprovedL4 = 0;
end;

-- Count the number of rows in the temporary table
SELECT Count(*)-1 AS RowCoun FROM #TempId;

-- Drop the temporary table when you're done
DROP TABLE #TempId;
 

";

				//sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);
				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);
			
				objComm.Fill(dt);
				return Convert.ToInt32(dt.Rows[0][0]);

			}
			catch (Exception e)
			{
				throw e;
			}
		}


		public int GetAuditIssueDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";

			DataTable dt = new DataTable();
			try
			{
				sqlText = @"
              declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);
declare @UserId varchar(max);

select @UserId = Id from SSLAuditAuthDB.dbo.AspNetUsers where UserName = 'erp';

Create table #TempId(Id int);

select @A1 = AuditApproval1, @A2 = AuditApproval2, @A3 = AuditApproval3, @A4 = AuditApproval4  
from UserRolls
where IsAudit = 1 and UserId = @UserId;

if (@A4 = 1)
begin
    insert into #TempId(Id)
    select Id from A_Audits where IsRejected = 0 and IsApprovedL1 = 1 and IsApprovedL2 = 1 and IsApprovedL3 = 1 and IsApprovedL4 = 0;
end
if (@A3 = 1)
begin
    insert into #TempId(Id)
    select id from A_Audits where IsRejected = 0 and IsApprovedL1 = 1 and IsApprovedL2 = 1 and IsApprovedL3 = 0 and IsApprovedL4 = 0;
end
if (@A2 = 1)
begin
    insert into #TempId(Id)
    select id from A_Audits where IsRejected = 0 and IsApprovedL1 = 1 and IsApprovedL2 = 0 and IsApprovedL3 = 0 and IsApprovedL4 = 0;
end
if (@A1 = 1)
begin
    insert into #TempId(Id)
    select id from A_Audits where IsRejected = 0 and IsApprovedL1 = 0 and IsApprovedL2 = 0 and IsApprovedL3 = 0 and IsApprovedL4 = 0;
end;

-- Count the number of rows in the temporary table
SELECT Count(*)-1 AS RowCoun FROM #TempId;

-- Drop the temporary table when you're done
DROP TABLE #TempId;
 

";

				//sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);
				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.Fill(dt);
				return Convert.ToInt32(dt.Rows[0][0]);

			}
			catch (Exception e)
			{
				throw e;
			}
		}


		public int GetAuditFeedBackDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";

			DataTable dt = new DataTable();
			try
			{
				sqlText = @"
              declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);
declare @UserId varchar(max);

select @UserId = Id from SSLAuditAuthDB.dbo.AspNetUsers where UserName = 'erp';

Create table #TempId(Id int);

select @A1 = AuditApproval1, @A2 = AuditApproval2, @A3 = AuditApproval3, @A4 = AuditApproval4  
from UserRolls
where IsAudit = 1 and UserId = @UserId;

if (@A4 = 1)
begin
    insert into #TempId(Id)
    select Id from A_Audits where IsRejected = 0 and IsApprovedL1 = 1 and IsApprovedL2 = 1 and IsApprovedL3 = 1 and IsApprovedL4 = 0;
end
if (@A3 = 1)
begin
    insert into #TempId(Id)
    select id from A_Audits where IsRejected = 0 and IsApprovedL1 = 1 and IsApprovedL2 = 1 and IsApprovedL3 = 0 and IsApprovedL4 = 0;
end
if (@A2 = 1)
begin
    insert into #TempId(Id)
    select id from A_Audits where IsRejected = 0 and IsApprovedL1 = 1 and IsApprovedL2 = 0 and IsApprovedL3 = 0 and IsApprovedL4 = 0;
end
if (@A1 = 1)
begin
    insert into #TempId(Id)
    select id from A_Audits where IsRejected = 0 and IsApprovedL1 = 0 and IsApprovedL2 = 0 and IsApprovedL3 = 0 and IsApprovedL4 = 0;
end;

-- Count the number of rows in the temporary table
SELECT Count(*)-1 AS RowCoun FROM #TempId;

-- Drop the temporary table when you're done
DROP TABLE #TempId;
 

";

				//sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);
				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.Fill(dt);
				return Convert.ToInt32(dt.Rows[0][0]);

			}
			catch (Exception e)
			{
				throw e;
			}
		}


		public AuditMaster Insert(AuditMaster model)
		{
			try
			{
				string sqlText = "";
				int Id = 0;


				sqlText = @"
insert into A_Audits(
[Code]
,[Name]
,[AuditTypeId]
,[IsPlaned]
--,[Location]
,[TeamId]
,[BranchID]
,[StartDate]
,[EndDate]
,[Duratiom]
,[BusinessTarget]
,[AuditStatus]
,[ReportStatus]
,[CreatedBy]
,[CreatedOn]
,[CreatedFrom]
,[LastUpdateBy]
,[LastUpdateOn]
,[LastUpdateFrom]
,[CompanyId]
,[Remarks]

,[IsPost]


,IsApprovedL1
,IsApprovedL2
,IsApprovedL3
,IsApprovedL4
,IsAudited
,IsRejected
,IssueIsApprovedL1
,IssueIsApprovedL2
,IssueIsApprovedL3
,IssueIsApprovedL4
,IssueIsAudited
,IssueIsRejected
,BFIsApprovedL1
,BFIsApprovedL2
,BFIsApprovedL3
,BFIsApprovedL4
,BFIsAudited
,BFIsRejected



)
values( 
 @Code
,@Name
,@AuditTypeId
,@IsPlaned
--,@Location
,@TeamId
,@BranchID
,@StartDate
,@EndDate
,@Duratiom
,@BusinessTarget
,@AuditStatus
,@ReportStatus
,@CreatedBy
,@CreatedOn
,@CreatedFrom
,@LastUpdateBy
,@LastUpdateOn
,@LastUpdateFrom
,@CompanyId
,@Remarks

,@IsPost


,@IsApprovedL1
,@IsApprovedL2
,@IsApprovedL3
,@IsApprovedL4
,@IsAudited
,@IsRejected
,@IssueIsApprovedL1
,@IssueIsApprovedL2
,@IssueIsApprovedL3
,@IssueIsApprovedL4
,@IssueIsAudited
,@IssueIsRejected
,@BFIsApprovedL1
,@BFIsApprovedL2
,@BFIsApprovedL3
,@BFIsApprovedL4
,@BFIsAudited
,@BFIsRejected

     
)     SELECT SCOPE_IDENTITY() ";

				var command = CreateCommand(sqlText);
				int value = (Convert.ToDateTime(model.EndDate) - Convert.ToDateTime(model.StartDate)).Days;


				command.Parameters.Add("@Code", SqlDbType.NVarChar).Value = model.Code;
				command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = model.Name;
				command.Parameters.Add("@AuditTypeId", SqlDbType.Int).Value = model.AuditTypeId;
				command.Parameters.Add("@IsPlaned", SqlDbType.Bit).Value = model.IsPlaned;
				//command.Parameters.Add("@Location", SqlDbType.NVarChar).Value = model.Location;
				command.Parameters.Add("@TeamId", SqlDbType.Int).Value = model.TeamId;
				command.Parameters.Add("@BranchID", SqlDbType.Int).Value = model.BranchID;
				command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = model.StartDate;
				command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = model.EndDate;
				command.Parameters.Add("@Duratiom", SqlDbType.Int).Value = value;
				command.Parameters.Add("@BusinessTarget", SqlDbType.NVarChar).Value = model.BusinessTarget;
				command.Parameters.Add("@AuditStatus", SqlDbType.NVarChar).Value = model.AuditStatus;
				command.Parameters.Add("@ReportStatus", SqlDbType.NVarChar).Value = model.ReportStatus;
				command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = model.Audit.CreatedBy;
				command.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = model.Audit.CreatedOn;
				command.Parameters.Add("@CreatedFrom", SqlDbType.NVarChar).Value = model.Audit.CreatedFrom;
				command.Parameters.Add("@LastUpdateBy", SqlDbType.NVarChar).Value = model.Audit.LastUpdateBy;
				command.Parameters.Add("@LastUpdateOn", SqlDbType.NVarChar).Value = model.Audit.LastUpdateOn;
				command.Parameters.Add("@LastUpdateFrom", SqlDbType.NVarChar).Value = model.Audit.LastUpdateFrom;
				command.Parameters.Add("@CompanyId", SqlDbType.Int).Value = model.CompanyId;
				command.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = model.Remarks is null ? DBNull.Value : model.Remarks;

				command.Parameters.Add("@IsPost", SqlDbType.NChar).Value = "N";



				command.Parameters.Add("@IsApprovedL1", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IsApprovedL2", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IsApprovedL3", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IsApprovedL4", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IsAudited", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IsRejected", SqlDbType.Bit).Value = 0;


				command.Parameters.Add("@IssueIsApprovedL1", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IssueIsApprovedL2", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IssueIsApprovedL3", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IssueIsApprovedL4", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IssueIsRejected", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@IssueIsAudited", SqlDbType.Bit).Value = 0;

				command.Parameters.Add("@BFIsApprovedL1", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@BFIsApprovedL2", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@BFIsApprovedL3", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@BFIsApprovedL4", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@BFIsAudited", SqlDbType.Bit).Value = 0;
				command.Parameters.Add("@BFIsRejected", SqlDbType.Bit).Value = 0;

				model.Id = Convert.ToInt32(command.ExecuteScalar());




				return model;

			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public AuditMaster MultiplePost(AuditMaster vm)
		{
			try
			{
				string sqlText = "";

				int rowcount = 0;

				string query = @"  ";
				SqlCommand command = CreateCommand(query);

				foreach (string ID in vm.IDs)
				{

					query = @"  update A_Audits set 
     IsPost=@IsPost
   ,PostedBy=@PostedBy
   ,PostedOn=@PostedOn
   ,PostedFrom=@PostedFrom

   ,IsRejected=@IsRejected 


     where  Id= @Id ";
					command = CreateCommand(query);

					command.Parameters.Add("@IsPost", SqlDbType.NChar).Value = "Y";
					command.Parameters.Add("@Id", SqlDbType.BigInt).Value = ID;
					command.Parameters.Add("@PostedBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Audit.PostedBy.ToString()) ? (object)DBNull.Value : vm.Audit.PostedBy.ToString();
					command.Parameters.Add("@PostedOn", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Audit.PostedOn.ToString()) ? (object)DBNull.Value : vm.Audit.PostedOn.ToString();
					command.Parameters.Add("@PostedFrom", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Audit.PostedFrom.ToString()) ? (object)DBNull.Value : vm.Audit.PostedFrom.ToString();

					command.Parameters.Add("@IsRejected", SqlDbType.Bit).Value = 0;



					rowcount = command.ExecuteNonQuery();




				}
				if (rowcount <= 0)
				{
					throw new Exception(MessageModel.UpdateFail);
				}
				return vm;
			}


			catch (Exception ex)
			{
				throw ex;
			}
		}

		public AuditMaster MultipleUnPost(AuditMaster vm)
		{
			try
			{
				string sqlText = "";

				int rowcount = 0;
				string query = @"  ";
				SqlCommand command = CreateCommand(query);



				foreach (string ID in vm.IDs)
				{


					if (vm.Operation == "unpost")
					{
						query = @"   update A_Audits set 

 IsPost=@Post                   
,ReasonOfUnPost=@ReasonOfUnPost
,LastUpdateBy=@LastUpdateBy
,LastUpdateOn=@LastUpdateOn
,LastUpdateFrom=@LastUpdateFrom

 where  Id= @Id ";
						command = CreateCommand(query);

						command.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;
						command.Parameters.Add("@Post", SqlDbType.NChar).Value = "N";
						command.Parameters.Add("@LastUpdateBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Audit.PostedBy.ToString()) ? (object)DBNull.Value : vm.Audit.PostedBy.ToString();
						command.Parameters.Add("@LastUpdateOn", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Audit.PostedOn.ToString()) ? (object)DBNull.Value : vm.Audit.PostedOn.ToString();
						command.Parameters.Add("@LastUpdateFrom", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Audit.PostedFrom.ToString()) ? (object)DBNull.Value : vm.Audit.PostedFrom.ToString();
						command.Parameters.Add("@ReasonOfUnPost", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.ReasonOfUnPost.ToString()) ? (object)DBNull.Value : vm.ReasonOfUnPost.ToString();
						rowcount = command.ExecuteNonQuery();

					}

					else if (vm.Operation == "reject")
					{

						query = @"update A_Audits set 

      IsPost=@IsPost  
     ,IsRejected=@IsRejected 
     ,RejectedBy=@RejectedBy  
     ,RejectedDate=@RejectedDate
     ,RejectedComments=@RejectedComments

     ,IsApprovedL1=@IsApprovedL1
     ,IsApprovedL2=@IsApprovedL2
     ,IsApprovedL3=@IsApprovedL3
     ,IsApprovedL4=@IsApprovedL4


      where  Id= @Id "
						;


						command = CreateCommand(query);

						command.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;
						command.Parameters.Add("@IsPost", SqlDbType.NChar).Value = "N";
						command.Parameters.Add("@IsRejected", SqlDbType.Bit).Value = vm.Approval.IsRejected;


						command.Parameters.Add("@RejectedComments", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.RejectedComments) ? (object)DBNull.Value : vm.RejectedComments;
						command.Parameters.Add("@RejectedBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Approval.RejectedBy) ? (object)DBNull.Value : vm.Approval.RejectedBy;
						command.Parameters.Add("@RejectedDate", SqlDbType.DateTime).Value = vm.Approval.RejectedDate;

						command.Parameters.Add("@IsApprovedL1", SqlDbType.NChar).Value = 0;
						command.Parameters.Add("@IsApprovedL2", SqlDbType.NChar).Value = 0;
						command.Parameters.Add("@IsApprovedL3", SqlDbType.NChar).Value = 0;
						command.Parameters.Add("@IsApprovedL4", SqlDbType.NChar).Value = 0;



						rowcount = command.ExecuteNonQuery();

					}

					//else if (vm.Operation == "approved")
					else if (vm.ApproveStatus == "auditStatus")
					{

						query = @"


declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);


declare @AC1 varchar(1);
declare @AC2 varchar(1);
declare @AC3 varchar(1);
declare @AC4 varchar(1);

declare @UserId varchar(max);
Create table #TempId(Id int)

select @UserId=Id  from SSLAuditAuthDB.dbo.AspNetUsers where UserName=@UserName

select @A1=AuditApproval1,@A2=AuditApproval2,@A3=AuditApproval3,@A4=AuditApproval4  from UserRolls

where  IsAudit=1 and UserId=@UserId
select @AC1=IsApprovedL1,@AC2=IsApprovedL2,@AC3=IsApprovedL3,@AC4=IsApprovedL4 from A_Audits   where id=@Id and IsPost='Y' and  IsRejected=0 

if(@A1=1 and @AC1=0)
begin
	update A_Audits set IsApprovedL1=1 ,ApprovedByL1=@UserName, ApprovedDateL1=@date,CommentsL1=@Comments  where id=@Id and IsPost='Y' and  IsRejected=0 and IsApprovedL1=0 and IsApprovedL2=0 and IsApprovedL3=0 and IsApprovedL4=0
	if(@A2=1)
	begin
		update A_Audits set IsApprovedL2=1  ,ApprovedByL2=@UserName, ApprovedDateL2=@date,CommentsL2=@Comments   where id=@Id and   IsPost='Y' and IsRejected=0 and IsApprovedL1=1 and IsApprovedL2=0 and IsApprovedL3=0 and IsApprovedL4=0
		if(@A3=1)
			begin
			update A_Audits set IsApprovedL3=1  ,ApprovedByL3=@UserName, ApprovedDateL3=@date,CommentsL3=@Comments   where id=@Id and  IsPost='Y' and  IsRejected=0 and IsApprovedL1=1and IsApprovedL2=1 and IsApprovedL3=0 and IsApprovedL4=0
			if(@A4=1)
			begin
			update A_Audits set IsApprovedL4=1 ,ApprovedByL4=@UserName, ApprovedDateL4=@date,CommentsL4=@Comments   where id=@Id and   IsPost='Y' and IsRejected=0 and IsApprovedL1=1and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=0
			end
		end 
	end 
end 
else if(@A2=1 and @AC2=0)
begin
	update A_Audits set IsApprovedL2=1  ,ApprovedByL2=@UserName, ApprovedDateL2=@date,CommentsL2=@Comments   where id=@Id and   IsPost='Y' and IsRejected=0 and IsApprovedL1=1 and IsApprovedL2=0 and IsApprovedL3=0 and IsApprovedL4=0
	if(@A3=1)
		begin
		update A_Audits set IsApprovedL3=1  ,ApprovedByL3=@UserName, ApprovedDateL3=@date,CommentsL3=@Comments   where id=@Id and  IsPost='Y' and  IsRejected=0 and IsApprovedL1=1and IsApprovedL2=1 and IsApprovedL3=0 and IsApprovedL4=0
		if(@A4=1)
		begin
		update A_Audits set IsApprovedL4=1 ,ApprovedByL4=@UserName, ApprovedDateL4=@date,CommentsL4=@Comments   where id=@Id and   IsPost='Y' and IsRejected=0 and IsApprovedL1=1and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=0
		end
	end 
end 
else if(@A3=1 and @AC3=0)
begin
	update A_Audits set IsApprovedL3=1  ,ApprovedByL3=@UserName, ApprovedDateL3=@date,CommentsL3=@Comments   where id=@Id and  IsPost='Y' and  IsRejected=0 and IsApprovedL1=1and IsApprovedL2=1 and IsApprovedL3=0 and IsApprovedL4=0
	if(@A4=1)
	begin
		update A_Audits set IsApprovedL4=1 ,ApprovedByL4=@UserName, ApprovedDateL4=@date,CommentsL4=@Comments   where id=@Id and   IsPost='Y' and IsRejected=0 and IsApprovedL1=1and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=0
	end
end 
else if(@A4=1 and @AC4=0)
begin
	update A_Audits set IsApprovedL4=1 ,ApprovedByL4=@UserName, ApprovedDateL4=@date,CommentsL4=@Comments   where id=@Id and   IsPost='Y' and IsRejected=0 and IsApprovedL1=1and IsApprovedL2=1 and IsApprovedL3=1 and IsApprovedL4=0
end

"
						;


						command = CreateCommand(query);

						command.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;
						//command.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;
						command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = vm.Audit.PostedBy;
						command.Parameters.Add("@date", SqlDbType.VarChar).Value = vm.Audit.PostedOn;
						//command.Parameters.Add("@IsApprovedL1", SqlDbType.Bit).Value = vm.Approval.IsApprovedL1;
						command.Parameters.Add("@Comments", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.CommentsL1) ? (object)DBNull.Value : vm.CommentsL1;

						//if(vm.IsPost == "N")
						//{
						//	throw new Exception("Data is not post yet.");
						//}

						rowcount = command.ExecuteNonQuery();

					}
					else if (vm.ApproveStatus == "issueApprove")
					{

						query = @"


declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);


declare @AC1 varchar(1);
declare @AC2 varchar(1);
declare @AC3 varchar(1);
declare @AC4 varchar(1);

declare @UserId varchar(max);
Create table #TempId(Id int)

select @UserId=Id  from SSLAuditAuthDB.dbo.AspNetUsers where UserName=@UserName

select @A1=AuditIssueApproval1,@A2=AuditIssueApproval2,@A3=AuditIssueApproval3,@A4=AuditIssueApproval4  from UserRolls

where  IsAuditIssue=1 and UserId=@UserId
select @AC1=IssueIsApprovedL1,@AC2=IssueIsApprovedL2,@AC3=IssueIsApprovedL3,@AC4=IssueIsApprovedL4 from A_Audits   where id=@Id and IsPost='Y' and  IssueIsRejected=0 

if(@A1=1 and @AC1=0)
begin
	update A_Audits set IssueIsApprovedL1=1 ,IssueApprovedByL1=@UserName, IssueApprovedDateL1=@date,IssueCommentsL1=@Comments  where id=@Id and IsPost='Y' and  IssueIsRejected=0 and IssueIsApprovedL1=0 and IssueIsApprovedL2=0 and IssueIsApprovedL3=0 and IssueIsApprovedL4=0
	if(@A2=1)
	begin
		update A_Audits set IssueIsApprovedL2=1  ,IssueApprovedByL2=@UserName, IssueApprovedDateL2=@date,IssueCommentsL2=@Comments   where id=@Id and   IsPost='Y' and IssueIsRejected=0 and IssueIsApprovedL1=1 and IssueIsApprovedL2=0 and IssueIsApprovedL3=0 and IssueIsApprovedL4=0
		if(@A3=1)
			begin
			update A_Audits set IssueIsApprovedL3=1  ,IssueApprovedByL3=@UserName, IssueApprovedDateL3=@date,IssueCommentsL3=@Comments   where id=@Id and  IsPost='Y' and  IssueIsRejected=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=1 and IssueIsApprovedL3=0 and IssueIsApprovedL4=0
			if(@A4=1)
			begin
			update A_Audits set IssueIsApprovedL4=1 ,IssueApprovedByL4=@UserName, IssueApprovedDateL4=@date,IssueCommentsL4=@Comments   where id=@Id and   IsPost='Y' and IssueIsRejected=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=1 and IssueIsApprovedL3=1 and IssueIsApprovedL4=0
			end
		end 
	end 
end 
else if(@A2=1 and @AC2=0)
begin
	update A_Audits set IssueIsApprovedL2=1  ,IssueApprovedByL2=@UserName, IssueApprovedDateL2=@date,IssueCommentsL2=@Comments   where id=@Id and   IsPost='Y' and IssueIsRejected=0 and IssueIsApprovedL1=1 and IssueIsApprovedL2=0 and IssueIsApprovedL3=0 and IssueIsApprovedL4=0
	if(@A3=1)
		begin
		update A_Audits set IssueIsApprovedL3=1  ,IssueApprovedByL3=@UserName, IssueApprovedDateL3=@date,IssueCommentsL3=@Comments   where id=@Id and  IsPost='Y' and  IssueIsRejected=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=1 and IssueIsApprovedL3=0 and IssueIsApprovedL4=0
		if(@A4=1)
		begin
		update A_Audits set IssueIsApprovedL4=1 ,IssueApprovedByL4=@UserName, IssueApprovedDateL4=@date,IssueCommentsL4=@Comments   where id=@Id and   IsPost='Y' and IssueIsRejected=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=1 and IssueIsApprovedL3=1 and IssueIsApprovedL4=0
		end
	end 
end 
else if(@A3=1 and @AC3=0)
begin
	update A_Audits set IssueIsApprovedL3=1  ,IssueApprovedByL3=@UserName, IssueApprovedDateL3=@date,IssueCommentsL3=@Comments   where id=@Id and  IsPost='Y' and  IssueIsRejected=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=1 and IssueIsApprovedL3=0 and IssueIsApprovedL4=0
	if(@A4=1)
	begin
		update A_Audits set IssueIsApprovedL4=1 ,IssueApprovedByL4=@UserName, IssueApprovedDateL4=@date,IssueCommentsL4=@Comments   where id=@Id and   IsPost='Y' and IssueIsRejected=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=1 and IssueIsApprovedL3=1 and IssueIsApprovedL4=0
	end
end 
else if(@A4=1 and @AC4=0)
begin
	update A_Audits set IssueIsApprovedL4=1 ,IssueApprovedByL4=@UserName, IssueApprovedDateL4=@date,IssueCommentsL4=@Comments   where id=@Id and   IsPost='Y' and IssueIsRejected=0 and IssueIsApprovedL1=1and IssueIsApprovedL2=1 and IssueIsApprovedL3=1 and IssueIsApprovedL4=0
end

"
						;


						command = CreateCommand(query);

						command.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;
						//command.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;
						command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = vm.Audit.PostedBy;
						command.Parameters.Add("@date", SqlDbType.VarChar).Value = vm.Audit.PostedOn;
						//command.Parameters.Add("@IsApprovedL1", SqlDbType.Bit).Value = vm.Approval.IsApprovedL1;
						command.Parameters.Add("@Comments", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.CommentsL1) ? (object)DBNull.Value : vm.CommentsL1;



						rowcount = command.ExecuteNonQuery();

					}

					else if (vm.ApproveStatus == "branchFeedbackApprove")
					{

						query = @"


declare @A1 varchar(1);
declare @A2 varchar(1);
declare @A3 varchar(1);
declare @A4 varchar(1);


declare @AC1 varchar(1);
declare @AC2 varchar(1);
declare @AC3 varchar(1);
declare @AC4 varchar(1);

declare @UserId varchar(max);
Create table #TempId(Id int)

select @UserId=Id  from SSLAuditAuthDB.dbo.AspNetUsers where UserName=@UserName

select @A1=AuditFeedbackApproval1,@A2=AuditFeedbackApproval2,@A3=AuditFeedbackApproval3,@A4=AuditFeedbackApproval4  from UserRolls

where  IsAuditFeedback=1 and UserId=@UserId
select @AC1=BFIsApprovedL1,@AC2=BFIsApprovedL2,@AC3=BFIsApprovedL3,@AC4=BFIsApprovedL4 from A_Audits   where id=@Id and IsPost='Y' and  BFIsRejected=0 

if(@A1=1 and @AC1=0)
begin
	update A_Audits set BFIsApprovedL1=1 ,BFApprovedByL1=@UserName, BFApprovedDateL1=@date,BFCommentsL1=@Comments  where id=@Id and IsPost='Y' and  BFIsRejected=0 and BFIsApprovedL1=0 and BFIsApprovedL2=0 and BFIsApprovedL3=0 and BFIsApprovedL4=0
	if(@A2=1)
	begin
		update A_Audits set BFIsApprovedL2=1  ,BFApprovedByL2=@UserName, BFApprovedDateL2=@date,BFCommentsL2=@Comments   where id=@Id and   IsPost='Y' and BFIsRejected=0 and BFIsApprovedL1=1 and BFIsApprovedL2=0 and BFIsApprovedL3=0 and BFIsApprovedL4=0
		if(@A3=1)
			begin
			update A_Audits set BFIsApprovedL3=1  ,BFApprovedByL3=@UserName, BFApprovedDateL3=@date,BFCommentsL3=@Comments   where id=@Id and  IsPost='Y' and  BFIsRejected=0 and BFIsApprovedL1=1and BFIsApprovedL2=1 and BFIsApprovedL3=0 and BFIsApprovedL4=0
			if(@A4=1)
			begin
			update A_Audits set BFIsApprovedL4=1 ,BFApprovedByL4=@UserName, BFApprovedDateL4=@date,BFCommentsL4=@Comments   where id=@Id and   IsPost='Y' and BFIsRejected=0 and BFIsApprovedL1=1and BFIsApprovedL2=1 and BFIsApprovedL3=1 and BFIsApprovedL4=0
			end
		end 
	end 
end 
else if(@A2=1 and @AC2=0)
begin
	update A_Audits set BFIsApprovedL2=1  ,BFApprovedByL2=@UserName, BFApprovedDateL2=@date,BFCommentsL2=@Comments   where id=@Id and   IsPost='Y' and BFIsRejected=0 and BFIsApprovedL1=1 and BFIsApprovedL2=0 and BFIsApprovedL3=0 and BFIsApprovedL4=0
	if(@A3=1)
		begin
		update A_Audits set BFIsApprovedL3=1  ,BFApprovedByL3=@UserName, BFApprovedDateL3=@date,BFCommentsL3=@Comments   where id=@Id and  IsPost='Y' and  BFIsRejected=0 and BFIsApprovedL1=1and BFIsApprovedL2=1 and BFIsApprovedL3=0 and BFIsApprovedL4=0
		if(@A4=1)
		begin
		update A_Audits set BFIsApprovedL4=1 ,BFApprovedByL4=@UserName, BFApprovedDateL4=@date,BFCommentsL4=@Comments   where id=@Id and   IsPost='Y' and BFIsRejected=0 and BFIsApprovedL1=1and BFIsApprovedL2=1 and BFIsApprovedL3=1 and BFIsApprovedL4=0
		end
	end 
end 
else if(@A3=1 and @AC3=0)
begin
	update A_Audits set BFIsApprovedL3=1  ,BFApprovedByL3=@UserName, BFApprovedDateL3=@date,BFCommentsL3=@Comments   where id=@Id and  IsPost='Y' and  BFIsRejected=0 and BFIsApprovedL1=1and BFIsApprovedL2=1 and BFIsApprovedL3=0 and BFIsApprovedL4=0
	if(@A4=1)
	begin
		update A_Audits set BFIsApprovedL4=1 ,BFApprovedByL4=@UserName, BFApprovedDateL4=@date,BFCommentsL4=@Comments   where id=@Id and   IsPost='Y' and BFIsRejected=0 and BFIsApprovedL1=1and BFIsApprovedL2=1 and BFIsApprovedL3=1 and BFIsApprovedL4=0
	end
end 
else if(@A4=1 and @AC4=0)
begin
	update A_Audits set BFIsApprovedL4=1 ,BFApprovedByL4=@UserName, BFApprovedDateL4=@date,BFCommentsL4=@Comments   where id=@Id and   IsPost='Y' and BFIsRejected=0 and BFIsApprovedL1=1and BFIsApprovedL2=1 and BFIsApprovedL3=1 and BFIsApprovedL4=0
end

"
						;


						command = CreateCommand(query);

						command.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;
						//command.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;
						command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = vm.Audit.PostedBy;
						command.Parameters.Add("@date", SqlDbType.VarChar).Value = vm.Audit.PostedOn;
						//command.Parameters.Add("@IsApprovedL1", SqlDbType.Bit).Value = vm.Approval.IsApprovedL1;
						command.Parameters.Add("@Comments", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.CommentsL1) ? (object)DBNull.Value : vm.CommentsL1;



						rowcount = command.ExecuteNonQuery();

					}




				}
				if (rowcount <= 0)
				{
					throw new Exception(MessageModel.PostFail);
				}

				return vm;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public AuditMaster Update(AuditMaster model)
		{
			try
			{

				string sql = "";

				sql = @"

Update [A_Audits]
set
 Name=@Name
,AuditTypeId=@AuditTypeId
,IsPlaned=@IsPlaned
--,Location=@Location
,TeamId=@TeamId
,BranchID=@BranchID
,StartDate=@StartDate
,EndDate=@EndDate
,Duratiom=@Duratiom
,BusinessTarget=@BusinessTarget
,AuditStatus=@AuditStatus
,Remarks=@Remarks
,ReportStatus=@ReportStatus
,LastUpdateBy=@LastUpdateBy
,LastUpdateOn=@LastUpdateOn
,LastUpdateFrom=@LastUpdateFrom


where Id=@Id
";


				var command = CreateCommand(sql);
				int value = (Convert.ToDateTime(model.EndDate) - Convert.ToDateTime(model.StartDate)).Days;

				command.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;
				command.Parameters.Add("@Code", SqlDbType.NVarChar).Value = model.Code;
				command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = model.Name;
				command.Parameters.Add("@AuditTypeId", SqlDbType.Int).Value = model.AuditTypeId;
				command.Parameters.Add("@IsPlaned", SqlDbType.Bit).Value = model.IsPlaned;
				//command.Parameters.Add("@Location", SqlDbType.NVarChar).Value = model.Location;
				command.Parameters.Add("@TeamId", SqlDbType.Int).Value = model.TeamId;
				command.Parameters.Add("@BranchID", SqlDbType.Int).Value = model.BranchID;
				command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = model.StartDate;
				command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = model.EndDate;
				command.Parameters.Add("@Duratiom", SqlDbType.Int).Value = value;
				command.Parameters.Add("@BusinessTarget", SqlDbType.NVarChar).Value = model.BusinessTarget;
				command.Parameters.Add("@AuditStatus", SqlDbType.NVarChar).Value = model.AuditStatus;
				command.Parameters.Add("@ReportStatus", SqlDbType.NVarChar).Value = model.ReportStatus;
				command.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = model.Remarks == null ? DBNull.Value : model.Remarks;

				command.Parameters.Add("@LastUpdateBy", SqlDbType.NVarChar).Value = model.Audit.LastUpdateBy;
				command.Parameters.Add("@LastUpdateOn", SqlDbType.NVarChar).Value = model.Audit.LastUpdateOn;
				command.Parameters.Add("@LastUpdateFrom", SqlDbType.NVarChar).Value = model.Audit.LastUpdateFrom;




				int rowcount = Convert.ToInt32(command.ExecuteNonQuery());

				if (rowcount <= 0)
				{
					throw new Exception(MessageModel.UpdateFail);
				}

				return model;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public AuditMaster AuditStatusUpdate(AuditMaster model)
		{
			try
			{

				string sql = "";

				sql = @"

Update [A_Audits]

 set

AuditStatus=@AuditStatus

where Id=@Id
";


				var command = CreateCommand(sql);


				command.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;
				
				command.Parameters.Add("@AuditStatus", SqlDbType.NVarChar).Value = model.AuditStatus;
				




				int rowcount = Convert.ToInt32(command.ExecuteNonQuery());

				if (rowcount <= 0)
				{
					throw new Exception(MessageModel.UpdateFail);
				}

				return model;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public int GetAuditStatusDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";

			DataTable dt = new DataTable();
			try
			{
				sqlText = @"
              select count(A_Audits.IsPlaned) PlanCount
             
             from  A_Audits 

             where 1=1  and A_Audits.IsPlaned = 1

             ";

				//sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);
				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);
				//objComm.SelectCommand.Parameters.AddWithValue("@BranchId", index.CurrentBranchid);
				objComm.Fill(dt);

				List<AuditMaster> VMs = new List<AuditMaster>();
				VMs = dt.ToList<AuditMaster>();


				return Convert.ToInt32(dt.Rows[0][0]);

			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public List<AuditResponse> AuditResponseGetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";
			List<AuditResponse> VMs = new List<AuditResponse>();
			DataTable dt = new DataTable();





			try
			{
				sqlText = @"

        select 
        AI.Id
       ,A.Name AuditName
       ,AI.IssueName
       ,E.EnumValue IssuePriority
       ,format(AI.DateOfSubmission,'dd/MM/yyyy') DateOfSubmission 
       --,AI.DateOfSubmission

       from A_AuditIssues AI

       left outer join A_Audits A on AI.AuditId=A.Id
       left outer join Enums E on E.Id=AI.IssuePriority

       WHERE 1=1 and AI.DateOfSubmission < CAST(GETDATE() AS DATE)

	   --and AI.Id not in(select AuditId from A_AuditBranchFeedbacks)

	   and AI.Id not in(select AuditIssueId from A_AuditBranchFeedbacks)";

				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, true);

				// ToDo Escape Sql Injection
				sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
				sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";

				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				//objComm.SelectCommand.Parameters.AddWithValue("@BranchId", index.CurrentBranchid);

				objComm.Fill(dt);

				VMs = dt
					.ToList<AuditResponse>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public MailSetting SaveUrl(MailSetting model)
		{
			try
			{
				string sqlText = "";
				int Id = 0;

				sqlText = @"
insert into MailSetting(
 [AutidId]
,[AuditIssueId]
,[BranchFeedbackId]
,[ApprovedUrl]
,[ApproveOparetion]
,[Status]
,[IsMailed]


)
values( 

 @AutidId
,@AuditIssueId
,@BranchFeedbackId
,@ApprovedUrl
,@ApproveOparetion
,@Status
,@IsMailed


     
) SELECT SCOPE_IDENTITY() ";

				var command = CreateCommand(sqlText);


				command.Parameters.Add("@AutidId", SqlDbType.Int).Value = model.AutidId;
				command.Parameters.Add("@ApprovedUrl", SqlDbType.NVarChar).Value = model.ApprovedUrl;
				command.Parameters.Add("@AuditIssueId", SqlDbType.Int).Value = model.AuditIssueId == 0 ? DBNull.Value : model.AuditIssueId;
				command.Parameters.Add("@BranchFeedbackId", SqlDbType.Int).Value = model.BranchFeedbackId == 0 ? DBNull.Value : model.BranchFeedbackId;
				command.Parameters.Add("@ApproveOparetion", SqlDbType.NVarChar).Value = model.ApproveOparetion is null ? DBNull.Value : model.ApproveOparetion;
				command.Parameters.Add("@Status", SqlDbType.NVarChar).Value = model.Status;
				command.Parameters.Add("@IsMailed", SqlDbType.Bit).Value = model.IsMailed;


				model.Id = Convert.ToInt32(command.ExecuteScalar());




				return model;

			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public List<UserProfile> GetEamil(UserProfile Email)
		{
			string sqlText = "";
			List<UserProfile> VMs = new List<UserProfile>();
			DataTable dt = new DataTable();

			try
			{
				sqlText = @"

declare @AL1 varchar(1);
declare @AL2 varchar(1);
declare @AL3 varchar(1);
declare @AL4 varchar(1);


select @AL1=  t.IsApprovedL1, @AL2=  t.IsApprovedL2, @AL3=  t.IsApprovedL3, @AL4=  t.IsApprovedL4 from A_Audits  t where   t.Id=@AuditId 


if (@AL1=0)
begin
select UserName,Email from SSLAuditAuthDB.dbo.AspNetUsers au
left join UserRolls up on up.UserId=au.Id 
where up.AuditApproval1 =1 and au.UserName = @userName
end
else  if (@AL2=0)
begin
select UserName,Email from SSLAuditAuthDB.dbo.AspNetUsers au
left join UserRolls up on up.UserId=au.Id 
where up.AuditApproval2 =1 and au.UserName = @userName
end
else if (@AL3=0)
begin
select UserName,Email from SSLAuditAuthDB.dbo.AspNetUsers au
left join UserRolls up on up.UserId=au.Id 
where up.AuditApproval3 =1 and au.UserName = @userName
end
else if (@AL4=0)
begin
select UserName,Email from SSLAuditAuthDB.dbo.AspNetUsers au
left join UserRolls up on up.UserId=au.Id 
where up.AuditApproval4 =1 and au.UserName = @userName
end

 ";
				sqlText = ApplyConditions(sqlText, null, null);

				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, null, null);

				objComm.SelectCommand.Parameters.AddWithValue("@AuditId", Email.Id);
				objComm.SelectCommand.Parameters.AddWithValue("@userName", Email.UserName);


				objComm.Fill(dt);

				VMs = dt.ToList<UserProfile>();

				return VMs;


			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public List<MailSetting> GetUrl(MailSetting model)
		{
			string sqlText = "";
			List<MailSetting> VMs = new List<MailSetting>();
			DataTable dt = new DataTable();

			try
			{
				sqlText = @"
       SELECT [Id]
      ,[ApprovedUrl]
     
       FROM [MailSetting]  

       where 1=1 and AutidId = @AutidId and Status=@status";

				sqlText = ApplyConditions(sqlText, null, null);

				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, null, null);


				objComm.SelectCommand.Parameters.AddWithValue("@AutidId", model.Id);
				objComm.SelectCommand.Parameters.AddWithValue("@status", "audit");

				objComm.Fill(dt);

				VMs = dt.ToList<MailSetting>();

				return VMs;


			}
			catch (Exception e)
			{
				throw e;
			}
		}

        public List<AuditUser> GetAuditUserByAuditId(string AuditId)
        {
            string sqlText = "";
            List<AuditUser> VMs = new List<AuditUser>();
            DataTable dt = new DataTable();

            //Format(ICReceipts.ReceiptDate, 'yyyy-MM-dd') ReceiptDate,


            try
            {
                sqlText = @"
select TM.UserId,u.Email EmailAddress from AuditUsers TM
left Join [SSLAuditAuthDB].[dbo].[AspNetUsers] u on u.Id=TM.UserId
where AuditId=@AuditId
";

                // sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, true);

                // ToDo Escape Sql Injection
                //sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
                //sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(sqlText);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, null, null);

                objComm.SelectCommand.Parameters.AddWithValue("@AuditId", AuditId);

                objComm.Fill(dt);

                VMs = dt
                    .ToList<AuditUser>();
                return VMs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
