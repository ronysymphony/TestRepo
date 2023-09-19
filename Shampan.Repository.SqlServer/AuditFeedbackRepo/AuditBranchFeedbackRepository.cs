using System.Data;
using Microsoft.Data.SqlClient;
using Shampan.Core.ExtentionMethod;
using Shampan.Core.Interfaces.Repository.AuditFeedbackRepo;
using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Repository.SqlServer.AuditFeedbackRepo
{
    public class AuditBranchFeedbackRepository : Repository, IAuditBranchFeedbackRepository
    {

        private DbConfig _dbConfig;
        public AuditBranchFeedbackRepository(SqlConnection context, SqlTransaction transaction, DbConfig dbConfig)
        {
            this._context = context;
            this._transaction = transaction;
            this._dbConfig = dbConfig;
        }

        public List<AuditBranchFeedback> GetAll(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            string sqlText = "";
            List<AuditBranchFeedback> VMs = new List<AuditBranchFeedback>();
            DataTable dt = new DataTable();

            try
            {
                sqlText = @"
SELECT  [Id]
      ,[AuditId]
      ,[AuditIssueId]
      ,[IssueDetails]
      ,[Heading]

      ,[IsPost]
      ,[Status]

      ,[CreatedBy]
      ,[CreatedOn]
      ,[CreatedFrom]
      ,[LastUpdateBy]
      ,[LastUpdateOn]
      ,[LastUpdateFrom]
      ,isnull([IsPosted],0)IsPosted
      ,[PostedBy]
      ,[PostedOn]
      ,[PostedFrom]
  FROM [A_AuditBranchFeedbacks]

where 1=1  

";
                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlDataAdapter objComm = CreateAdapter(sqlText);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                objComm.Fill(dt);

                VMs = dt.ToList<AuditBranchFeedback>();

                return VMs;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<AuditBranchFeedback> GetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            string sqlText = "";
            List<AuditBranchFeedback> VMs = new List<AuditBranchFeedback>();
            DataTable dt = new DataTable();

            try
            {
                sqlText = @"
SELECT af.[Id]
      ,af.[AuditId]
      ,af.[AuditIssueId]
      ,af.[IssueDetails]
      ,af.[IsPost]
      ,af.[CreatedBy]
      ,af.[CreatedOn]
      ,af.[CreatedFrom]
      ,af.[LastUpdateBy]
      ,af.[LastUpdateOn]
      ,af.[LastUpdateFrom]
      ,af.[Heading]
--     ,af.[IsPosted]
--     ,af.[PostedBy]
--     ,af.[PostedOn]
--     ,af.[PostedFrom]
	  ,ad.Name AuditName
	  ,ai.IssueName
  FROM [A_AuditBranchFeedbacks] af 
  left outer join A_Audits ad on af.AuditId = ad.Id
  left outer join A_AuditIssues ai on ai.Id = af.AuditIssueId
   
where 1=1
and af.AuditId = @AuditId  


";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, true);

                // ToDo Escape Sql Injection
                sqlText += @"  order by  " + index.OrderName.Replace("AuditName","ad.Name") + "  " + index.orderDir;
                sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(sqlText);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                objComm.SelectCommand.Parameters.AddWithValue("@AuditId", index.AuditId);


                objComm.Fill(dt);

                VMs = dt
                    .ToList<AuditBranchFeedback>();
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

SELECT count(af.Id)
  FROM [A_AuditFeedbacks] af 
  left outer join A_Audits ad on af.AuditId = ad.Id
   
where 1=1
and af.AuditId = @AuditId  

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

		public List<AuditBranchFeedback> GetAuditIssueUsers(string tableName, string[] conditionalFields, string[] conditionalValue)
		{

			try
			{
				string sqlText = @"

       Select 
       usr.UserName
       From AuditIssueUsers IU

       left outer join SSLAuditAuthDB.dbo.AspNetUsers usr on usr.Id = IU.UserId

       WHERE 1=1

            ";


				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

				SqlCommand objComm = CreateCommand(sqlText);

				objComm = ApplyParameters(objComm, conditionalFields, conditionalValue);

				SqlDataAdapter adapter = new SqlDataAdapter(objComm);
				DataTable dtResult = new DataTable();
				adapter.Fill(dtResult);

				List<AuditBranchFeedback> vms = dtResult.ToList<AuditBranchFeedback>();
				return vms;

			}
			catch (Exception)
			{

				throw;
			};
		}

		public AuditBranchFeedback Insert(AuditBranchFeedback model)
        {
            try
            {
                string sqlText = "";
                int Id = 0;


                sqlText = @"

INSERT INTO [dbo].[A_AuditBranchFeedbacks]
           (
[AuditId]
,[AuditIssueId]
,[IssueDetails]
,[Heading]
,[Status]
,[IsPost]

,[CreatedBy]
,[CreatedOn]
,[CreatedFrom]

		   )
VALUES
(
 @AuditId
,@AuditIssueId
,@IssueDetails
,@Heading
,@Status
,@IsPost

,@CreatedBy
,@CreatedOn
,@CreatedFrom

)   

SELECT SCOPE_IDENTITY() ";

                var command = CreateCommand(sqlText);

                command.Parameters.Add("@AuditId", SqlDbType.Int).Value = model.AuditId;
                command.Parameters.Add("@AuditIssueId", SqlDbType.NVarChar).Value = model.AuditIssueId;
                command.Parameters.Add("@IssueDetails", SqlDbType.NVarChar).Value = model.IssueDetails;
                command.Parameters.Add("@Heading", SqlDbType.NVarChar).Value = model.Heading;
                command.Parameters.Add("@Status", SqlDbType.NVarChar).Value = model.Status;

                command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = model.Audit.CreatedBy;
                command.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = model.Audit.CreatedOn;
                command.Parameters.Add("@CreatedFrom", SqlDbType.NVarChar).Value = model.Audit.CreatedFrom;

				command.Parameters.Add("@IsPost", SqlDbType.NVarChar).Value = "N";


				model.Id = Convert.ToInt32(command.ExecuteScalar());

                return model;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public AuditBranchFeedback MultiplePost(AuditBranchFeedback vm)
        {
            try
            {
                string sqlText = "";

                int rowcount = 0;

                string query = @"  ";
                SqlCommand command = CreateCommand(query);

                foreach (string ID in vm.IDs)
                {

                    query = @"  update A_AuditBranchFeedbacks set 
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

        public AuditBranchFeedback MultipleUnPost(AuditBranchFeedback vm)
        {
            try
            {
                string sqlText = "";

                int rowcount = 0;

                string query = @"  ";
                SqlCommand command = CreateCommand(query);

                foreach (string ID in vm.IDs)
                {

                    query = @"  update A_AuditBranchFeedbacks set 
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
                    command.Parameters.Add("@ReasonOfUnPost", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.UnPostReasonOfBranchFeedback.ToString()) ? (object)DBNull.Value : vm.UnPostReasonOfBranchFeedback.ToString();
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

        public AuditBranchFeedback Update(AuditBranchFeedback model)
        {
            try
            {
                string sql = @"

    UPDATE A_AuditBranchFeedbacks
    SET
    AuditId = @AuditId,
    AuditIssueId = @AuditIssueId,
    IssueDetails = @IssueDetails,
    Heading = @Heading,
    Status = @Status,
    LastUpdateBy = @LastUpdateBy,
    LastUpdateOn = @LastUpdateOn,
    LastUpdateFrom = @LastUpdateFrom
    WHERE Id = @Id
    ";

                // Create a SQL command using the SQL statement
                var command = CreateCommand(sql);

                // Set the parameter values for the command
                command.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;
                command.Parameters.Add("@AuditId", SqlDbType.Int).Value = model.AuditId;
                command.Parameters.Add("@AuditIssueId", SqlDbType.Int).Value = model.AuditIssueId;
                command.Parameters.Add("@IssueDetails", SqlDbType.NVarChar).Value = model.IssueDetails;
                command.Parameters.Add("@Heading", SqlDbType.NVarChar).Value = model.Heading;
                command.Parameters.Add("@Status", SqlDbType.NVarChar).Value = model.Status;
                command.Parameters.Add("@LastUpdateBy", SqlDbType.NVarChar).Value = model.Audit.LastUpdateBy;
                command.Parameters.Add("@LastUpdateOn", SqlDbType.DateTime).Value = model.Audit.LastUpdateOn;
                command.Parameters.Add("@LastUpdateFrom", SqlDbType.NVarChar).Value = model.Audit.LastUpdateFrom;

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
            catch
            {
                // Rethrow any exception that occurred during the update process
                throw;
            }
        }
    }
}
