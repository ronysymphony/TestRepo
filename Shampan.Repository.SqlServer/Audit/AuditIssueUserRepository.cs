using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Shampan.Core.ExtentionMethod;
using Shampan.Core.Interfaces.Repository.Audit;
using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Repository.SqlServer.Audit
{
    public class AuditIssueUserRepository : Repository, IAuditIssueUserRepository
    {

        private DbConfig _dbConfig;
        public AuditIssueUserRepository(SqlConnection context, SqlTransaction transaction, DbConfig dbConfig)
        {
            this._context = context;
            this._transaction = transaction;
            this._dbConfig = dbConfig;

        }

        //,[Remarks]
        //,[IssuePriority]

        public List<AuditIssueUser> GetAll(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            try
            {
                string sqlText = @"

SELECT  [Id]
      ,[AuditId]
      ,[UserId]
      ,(select UserName from SSLAuditAuthDB.dbo.AspNetUsers where Id = UserId) UserName
      ,[EmailAddress]
      
      

  FROM [AuditIssueUsers]
WHERE 1=1

            ";


                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlCommand objComm = CreateCommand(sqlText);

                objComm = ApplyParameters(objComm, conditionalFields, conditionalValue);

                SqlDataAdapter adapter = new SqlDataAdapter(objComm);
                DataTable dtResult = new DataTable();
                adapter.Fill(dtResult);

                List<AuditIssueUser> vms = dtResult.ToList<AuditIssueUser>();
                return vms;

            }
            catch (Exception)
            {

                throw;
            };
        }

        public List<AuditIssueUser> GetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            string sqlText = "";
            List<AuditIssueUser> VMs = new List<AuditIssueUser>();
            DataTable dt = new DataTable();

            try
            {
                sqlText = @"

SELECT  [Id]
      ,[AuditId]
,AuditIssueId
      ,[UserId]
      ,(select UserName from SSLAuditAuthDB.dbo.AspNetUsers where Id = UserId) UserName
      ,[EmailAddress]
  FROM [AuditIssueUsers]
WHERE 1=1 
and AuditIssueId = @AuditIssueId

";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, true);

                // ToDo Escape Sql Injection
                sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
                sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(sqlText);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                objComm.SelectCommand.Parameters.AddWithValue("@AuditIssueId", index.AuditIssueId);

                objComm.Fill(dt);

                VMs = dt
                    .ToList<AuditIssueUser>();
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
from  AuditUsers ad 

where 1=1 
and AuditIssueId = @AuditIssueId

";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);
                SqlDataAdapter objComm = CreateAdapter(sqlText);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);
                objComm.SelectCommand.Parameters.AddWithValue("@AuditIssueId", index.CurrentBranchid);
                objComm.Fill(dt);
                return Convert.ToInt32(dt.Rows[0][0]);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public AuditIssueUser Insert(AuditIssueUser model)
        {
            try
            {
                string sqlText = "";
                int Id = 0;

                sqlText = @"
INSERT INTO [dbo].[AuditIssueUsers]
           ([AuditId]
           ,[AuditIssueId]
           ,[UserId]
           ,[EmailAddress]
)
     VALUES
(@AuditId
,@AuditIssueId
,@UserId
,@EmailAddress) select SCOPE_IDENTITY()
";

                var command = CreateCommand(sqlText);
                command.Parameters.Add("@AuditId", SqlDbType.Int).Value = model.AuditId;
                command.Parameters.Add("@AuditIssueId", SqlDbType.Int).Value = model.AuditIssueId;
                command.Parameters.Add("@UserId", SqlDbType.NVarChar).Value =
                    model.UserId is null ? DBNull.Value : model.UserId;
                command.Parameters.Add("@EmailAddress", SqlDbType.NVarChar).Value = model.EmailAddress;
				//command.Parameters.Add("@IsPost", SqlDbType.NVarChar).Value = "N";




				model.Id = Convert.ToInt32(command.ExecuteScalar());

                return model;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public AuditIssueUser Update(AuditIssueUser model)
        {
            try
            {

                string sql = "";

                sql = @"

Update [AuditIssueUsers]
set

 AuditIssueId = @AuditIssueId
,UserId = @UserId
,EmailAddress = @EmailAddress

where Id=@Id
";


                var command = CreateCommand(sql);


                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@AuditIssueId", model.AuditIssueId);
                command.Parameters.AddWithValue("@UserId", model.UserId);

                command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
        

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
    }
}
