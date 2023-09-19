using System.Data;
using Microsoft.Data.SqlClient;
using Shampan.Core.ExtentionMethod;
using Shampan.Core.Interfaces.Repository.AuditFeedbackRepo;
using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Repository.SqlServer.AuditFeedbackRepo
{
    public class AuditFeedbackRepository : Repository, IAuditFeedbackRepository
    {

        private DbConfig _dbConfig;
        public AuditFeedbackRepository(SqlConnection context, SqlTransaction transaction, DbConfig dbConfig)
        {
            this._context = context;
            this._transaction = transaction;
            this._dbConfig = dbConfig;
        }

        public List<AuditFeedback> GetAll(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            string sqlText = "";
            List<AuditFeedback> VMs = new List<AuditFeedback>();
            DataTable dt = new DataTable();


//			sqlText = @"
//SELECT  [Id]
//      ,[AuditId]
//      ,[AuditIssueId]
//      ,[IssueDetails]
//      ,[Heading]
//      ,[CreatedBy]
//      ,[CreatedOn]
//      ,[CreatedFrom]
//      ,[LastUpdateBy]
//      ,[LastUpdateOn]
//      ,[LastUpdateFrom]
//      ,isnull([IsPosted],0)IsPosted
//      ,[PostedBy]
//      ,[PostedOn]
//      ,[PostedFrom]


//  FROM [A_AuditFeedbacks]



//where 1=1  

//";

			try
            {
                sqlText = @"
SELECT af.[Id]
      ,af.[AuditId]
      ,af.[AuditIssueId]
      ,[IssueDetails]
      ,[Heading]
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
	  ,afba.FileName


  FROM [A_AuditFeedbacks] as af

  left outer join A_AuditFeedbackAttachments  afba on af.Id=afba.AuditFeedbackId


where 1=1  

";
                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlDataAdapter objComm = CreateAdapter(sqlText);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                objComm.Fill(dt);

                VMs = dt.ToList<AuditFeedback>();

                return VMs;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

		public List<AuditFeedback> GetAuditUsers(string tableName, string[] conditionalFields, string[] conditionalValue)
		{
			try
			{
				string sqlText = @"

       Select 
       usr.UserName
       From AuditUsers AU

       left outer join SSLAuditAuthDB.dbo.AspNetUsers usr on usr.Id = AU.UserId

       WHERE 1=1

            ";


				sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

				SqlCommand objComm = CreateCommand(sqlText);

				objComm = ApplyParameters(objComm, conditionalFields, conditionalValue);

				SqlDataAdapter adapter = new SqlDataAdapter(objComm);
				DataTable dtResult = new DataTable();
				adapter.Fill(dtResult);

				List<AuditFeedback> vms = dtResult.ToList<AuditFeedback>();
				return vms;

			}
			catch (Exception)
			{

				throw;
			};
		}

		public List<AuditFeedback> GetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            string sqlText = "";
            List<AuditFeedback> VMs = new List<AuditFeedback>();
            DataTable dt = new DataTable();

            try
            {
                sqlText = @"
SELECT af.[Id]
      ,af.[AuditId]
      ,af.[AuditIssueId]
      ,af.[IssueDetails]
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
  FROM [A_AuditFeedbacks] af 
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
                    .ToList<AuditFeedback>();
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

        public AuditFeedback Insert(AuditFeedback model)
        {
            try
            {
                string sqlText = "";
                int Id = 0;


                sqlText = @"

INSERT INTO [dbo].[A_AuditFeedbacks]
           (
[AuditId]
,[AuditIssueId]
,[IssueDetails]
,[Heading]
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
,@CreatedBy
,@CreatedOn
,@CreatedFrom

)   

SELECT SCOPE_IDENTITY() ";

                var command = CreateCommand(sqlText);

                command.Parameters.Add("@AuditId", SqlDbType.Int).Value = model.AuditId;
                command.Parameters.Add("@AuditIssueId", SqlDbType.NVarChar).Value = model.AuditIssueId;
                //command.Parameters.Add("@IssueDetails", SqlDbType.NVarChar).Value = model.IssueDetails;
                command.Parameters.Add("@IssueDetails", SqlDbType.NVarChar).Value = model.FeedbackDetails is null ? DBNull.Value : model.FeedbackDetails;

                command.Parameters.Add("@Heading", SqlDbType.NVarChar).Value = model.Heading;

                command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = model.Audit.CreatedBy;
                command.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = model.Audit.CreatedOn;
                command.Parameters.Add("@CreatedFrom", SqlDbType.NVarChar).Value = model.Audit.CreatedFrom;


                model.Id = Convert.ToInt32(command.ExecuteScalar());

                return model;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public AuditFeedback Update(AuditFeedback model)
        {
            try
            {
                string sql = @"

    UPDATE A_AuditFeedbacks
    SET
    AuditId = @AuditId,
    AuditIssueId = @AuditIssueId,
    IssueDetails = @IssueDetails,
    Heading = @Heading,
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
                command.Parameters.Add("@IssueDetails", SqlDbType.NVarChar).Value = model.FeedbackDetails;
                command.Parameters.Add("@Heading", SqlDbType.NVarChar).Value = model.Heading;
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
