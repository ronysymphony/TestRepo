using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Shampan.Core.ExtentionMethod;
using Shampan.Core.Interfaces.Repository;
using Shampan.Core.Interfaces.Repository.AuditIssues;
using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Repository.SqlServer.AuditIssues
{
	public class AuditIssueRepository : Repository, IAuditIssueRepository
	{
		private DbConfig _dbConfig;

		public AuditIssueRepository(SqlConnection context, SqlTransaction transaction, DbConfig dbConfig)
		{
			this._context = context;
			this._transaction = transaction;
			this._dbConfig = dbConfig;
		}

		public List<AuditIssue> GetAll(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";
			List<AuditIssue> VMs = new List<AuditIssue>();
			DataTable dt = new DataTable();

			try
			{
				sqlText = @"

SELECT  Id
      ,AuditId
      ,IssueName
      ,IssueDetails
      ,DateOfSubmission
      ,IssuePriority
      ,AuditType
      ,IssueStatus
      ,ReportStatus
      ,Risk


      ,CreatedBy
      ,CreatedOn
      ,CreatedFrom
      ,LastUpdateBy
      ,LastUpdateOn
      ,LastUpdateFrom
      ,isnull(IsPosted,0)IsPosted
      ,PostedBy
      ,PostedOn
      ,PostedFrom
      ,IsPost
  FROM A_AuditIssues

where 1=1  

";
				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.Fill(dt);

				VMs = dt.ToList<AuditIssue>();

				return VMs;


			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public List<AuditIssue> GetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			string sqlText = "";
			List<AuditIssue> VMs = new List<AuditIssue>();
			DataTable dt = new DataTable();

			try
			{
				sqlText = @"
SELECT A_AuditIssues.Id
      ,AuditId
      ,IssueName
      ,IssueDetails
      --,DateOfSubmission
      ,Format(DateOfSubmission , 'yyyy-MM-dd') DateOfSubmission
      ,Enums.EnumValue IssuePriority
      ,CreatedBy
      ,CreatedOn
      ,CreatedFrom
      ,LastUpdateBy
      ,LastUpdateOn
      ,LastUpdateFrom
      ,isnull(IsPosted,0)IsPosted
      ,PostedBy
      ,PostedOn
      ,PostedFrom
      ,IsPost
  FROM A_AuditIssues left outer join Enums 
  on A_AuditIssues.IssuePriority = Enums.Id
  

  where 1=1 
    
 and AuditId = @AuditId  
";

				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, true);

				// ToDo Escape Sql Injection
				sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
				sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";

				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.SelectCommand.Parameters.AddWithValue("@AuditId", index.AuditId);


				objComm.Fill(dt);



				VMs = dt
					.ToList<AuditIssue>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public int GetIndexDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel
			vm = null)

		{
			string sqlText = "";

			DataTable dt = new DataTable();
			try
			{
				sqlText = @"
select 
 count(Id)FilteredCount
FROM A_AuditIssues
where 1=1 
and AuditId = @AuditId

";

				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);
				SqlDataAdapter objComm = CreateAdapter(sqlText);

				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

				objComm.SelectCommand.Parameters.AddWithValue("@AuditId", index.AuditId);

				objComm.Fill(dt);
				return Convert.ToInt32(dt.Rows);

			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public AuditIssue Insert(AuditIssue model)
		{
			try
			{
				string sqlText = "";
				int Id = 0;


				sqlText = @"
insert into A_AuditIssues(
 AuditId
,IssueName
,IssueDetails
,DateOfSubmission
,IssuePriority
,AuditType
,IssueStatus
,Risk

,CreatedBy
,CreatedOn
,IsPost
)
values( 
 @AuditId
,@IssueName
,@IssueDetails
,@DateOfSubmission
,@IssuePriority
,@AuditType
,@IssueStatus
,@Risk

,@CreatedBy
,@CreatedOn
 ,@IsPost
)     SELECT SCOPE_IDENTITY() ";

				var command = CreateCommand(sqlText);

				command.Parameters.Add("@AuditId", SqlDbType.Int).Value = model.AuditId;
				command.Parameters.Add("@IssueName", SqlDbType.NVarChar).Value = model.IssueName;
				//command.Parameters.Add("@IssueDetails", SqlDbType.NVarChar).Value = model.IssueDetails;
                command.Parameters.Add("@IssueDetails", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(model.IssueDetails) ? (object)DBNull.Value : model.IssueDetails;

                command.Parameters.Add("@DateOfSubmission", SqlDbType.DateTime).Value = model.DateOfSubmission;
				command.Parameters.Add("@IssuePriority", SqlDbType.NVarChar).Value = model.IssuePriority;

				command.Parameters.Add("@AuditType", SqlDbType.NVarChar).Value = model.AuditType;
				command.Parameters.Add("@IssueStatus", SqlDbType.NVarChar).Value = model.IssueStatus;
				//command.Parameters.Add("@Risk", SqlDbType.NVarChar).Value = model.Risk;
                command.Parameters.Add("@Risk", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(model.Risk) ? (object)DBNull.Value : model.Risk;


                command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = model.Audit.CreatedBy;
				command.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = model.Audit.CreatedOn;
				command.Parameters.Add("@IsPost", SqlDbType.NChar).Value = "N";


				model.Id = Convert.ToInt32(command.ExecuteScalar());

				return model;

			}
			catch (Exception ex)
			{
				throw;
			}


		}

		public AuditIssue MultiplePost(AuditIssue vm)
		{
			try
			{
				string sqlText = "";

				int rowcount = 0;

				string query = @"  ";
				SqlCommand command = CreateCommand(query);

				foreach (string ID in vm.IDs)
				{

					query = @"  update A_AuditIssues set 
     IsPost=@IsPost
   ,PostedBy=@PostedBy
,PostedOn=@PostedOn
,PostedFrom=@PostedFrom
     where  Id= @Id ";
					command = CreateCommand(query);

					command.Parameters.Add("@IsPost", SqlDbType.NChar).Value = "Y";
					command.Parameters.Add("@Id", SqlDbType.BigInt).Value = ID;
					command.Parameters.Add("@PostedBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Audit.PostedBy.ToString()) ? (object)DBNull.Value : vm.Audit.PostedBy.ToString();
					command.Parameters.Add("@PostedOn", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Audit.PostedOn.ToString()) ? (object)DBNull.Value : vm.Audit.PostedOn.ToString();
					command.Parameters.Add("@PostedFrom", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.Audit.PostedFrom.ToString()) ? (object)DBNull.Value : vm.Audit.PostedFrom.ToString();

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


		public AuditIssue MultipleUnPost(AuditIssue vm)
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
						query = @"   update A_AuditIssues set 

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
						command.Parameters.Add("@ReasonOfUnPost", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.UnPostReasonOfIssue.ToString()) ? (object)DBNull.Value : vm.UnPostReasonOfIssue.ToString();

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

		public AuditIssue ReportStatusUpdate(AuditIssue model)
		{
			try
			{
				string sql = @"

        UPDATE A_AuditIssues
        SET
        ReportStatus = @ReportStatus
        
        WHERE Id = @Id
        ";

				
				var command = CreateCommand(sql);

				
				command.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;
				command.Parameters.Add("@ReportStatus", SqlDbType.VarChar).Value = model.ReportStatus;
			
				int rowCount = Convert.ToInt32(command.ExecuteNonQuery());


				if (rowCount <= 0)
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

		public AuditIssue Update(AuditIssue model)
		{
			try
			{
				string sql = @"

        UPDATE A_AuditIssues
        SET
        AuditId = @AuditId,
        IssueName = @IssueName,
        IssueDetails = @IssueDetails,
        DateOfSubmission = @DateOfSubmission,
        IssuePriority = @IssuePriority,
        AuditType = @AuditType,
        IssueStatus = @IssueStatus,
        Risk = @Risk,

        LastUpdateBy = @LastUpdateBy,
        LastUpdateOn = @LastUpdateOn
        WHERE Id = @Id
        ";

				// Create a SQL command using the SQL statement
				var command = CreateCommand(sql);

				// Set the parameter values for the command
				command.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;
				command.Parameters.Add("@AuditId", SqlDbType.Int).Value = model.AuditId;
				command.Parameters.Add("@IssueName", SqlDbType.NVarChar).Value = model.IssueName;
				//command.Parameters.Add("@IssueDetails", SqlDbType.NVarChar).Value = model.IssueDetails;
                command.Parameters.Add("@IssueDetails", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(model.IssueDetails) ? (object)DBNull.Value : model.IssueDetails;

                command.Parameters.Add("@DateOfSubmission", SqlDbType.DateTime).Value = model.DateOfSubmission;
				command.Parameters.Add("@IssuePriority", SqlDbType.NVarChar).Value = model.IssuePriority;
				command.Parameters.Add("@AuditType", SqlDbType.NVarChar).Value = model.AuditType;
				command.Parameters.Add("@IssueStatus", SqlDbType.NVarChar).Value = model.IssueStatus;
				//command.Parameters.Add("@Risk", SqlDbType.NVarChar).Value = model.Risk;


				command.Parameters.Add("@Risk", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(model.Risk) ? (object)DBNull.Value : model.Risk;




				command.Parameters.Add("@LastUpdateBy", SqlDbType.NVarChar).Value = model.Audit.LastUpdateBy;
				command.Parameters.Add("@LastUpdateOn", SqlDbType.DateTime).Value = model.Audit.LastUpdateOn;

				// Execute the update query and retrieve the number of affected rows
				int rowCount = Convert.ToInt32(command.ExecuteNonQuery());

				if (rowCount <= 0)
				{
					// If no rows were updated, throw an exception indicating failure
					throw new Exception(MessageModel.UpdateFail);
				}

				// Return the updated model object
				return model;
			}
			catch (Exception ex)
			{
				// Rethrow any exception that occurred during the update process
				throw ex;
			}
		}
	}

}
