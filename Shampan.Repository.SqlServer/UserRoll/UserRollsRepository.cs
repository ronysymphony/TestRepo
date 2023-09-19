using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Shampan.Core.ExtentionMethod;
using Shampan.Core.Interfaces.Repository.Advance;
using Shampan.Core.Interfaces.Repository.Team;
using Shampan.Core.Interfaces.Repository.Tour;
using Shampan.Core.Interfaces.Repository.UserRoll;
using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Repository.SqlServer.UserRoll
{
	public class UserRollsRepository : Repository, IUserRollsRepository
	{
		public UserRollsRepository(SqlConnection context, SqlTransaction transaction)
		{
			this._context = context;
			this._transaction = transaction;
		}
		public int Archive(string tableName, string[] conditionalFields, string[] conditionalValue)
		{
			throw new NotImplementedException();
		}

		public bool CheckExists(string tableName, string[] conditionalFields, string[] conditionalValue)
		{
			throw new NotImplementedException();
		}

		public List<UserManuInfo> GetUserManu(string Username)
		{
			string sqlText = "";
			List<UserManuInfo> VMs = new List<UserManuInfo>();
			DataTable dt = new DataTable();

			


			try
			{
				sqlText =
@"
SELECT  MO.Id,MO.Modul
  --FROM [SSLAuditDB].[dbo].[TBLModul] MO
  FROM [TBLModul] MO
  left join ModulPermission MP ON MP.ModulId=MO.Id
  where MP.IsActive=1 and MP.UserId=@Username
";

		
				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, null, null);

				objComm.SelectCommand.Parameters.AddWithValue("@Username", Username);

				objComm.Fill(dt);

				VMs = dt
					.ToList<UserManuInfo>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public bool CheckPostStatus(string tableName, string[] conditionalFields, string[] conditionalValue)
		{
            try
            {
                bool ÌsPost = false;
                string Post = "";

                DataTable dt = new DataTable();

                // ToDo sql injection
                string sqlText = "select IsPost  from " + tableName + " where 1=1 ";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlCommand command = CreateCommand(sqlText);

                command = ApplyParameters(command, conditionalFields, conditionalValue);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    Post = dt.Rows[0]["IsPost"].ToString();
                    return (Post == "Y");
                }


                return ÌsPost;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

		public bool CheckPushStatus(string tableName, string[] conditionalFields, string[] conditionalValue)
		{
			throw new NotImplementedException();
		}

		public string CodeGeneration(string CodeGroup, string CodeName)
		{
			try
			{

				string codeGeneration = GenerateCode(CodeGroup, CodeName);
				return codeGeneration;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public int Delete(string tableName, string[] conditionalFields, string[] conditionalValue)
		{
            throw new NotImplementedException();

        }

        public List<UserRolls> GetAll(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
            try
            {
                string sqlText = @"select 
 Id

 ,UserId
,IsAudit
,AuditApproval1
,AuditApproval2
,AuditApproval3
,AuditApproval4

,IsTour
,TourApproval1
,TourApproval2
,TourApproval3
,TourApproval4

,IsAdvance
,AdvanceApproval1
,AdvanceApproval2
,AdvanceApproval3
,AdvanceApproval4

,IsTa
,IsTaApproval1
,IsTaApproval2
,IsTaApproval3
,IsTaApproval4

,IsTourCompletionReport
,TourCompletionReportApproval1
,TourCompletionReportApproval2
,TourCompletionReportApproval3
,TourCompletionReportApproval4

,IsAuditIssue
,AuditIssueApproval1
,AuditIssueApproval2
,AuditIssueApproval3
,AuditIssueApproval4

,IsAuditFeedback
,AuditFeedbackApproval1
,AuditFeedbackApproval2
,AuditFeedbackApproval3
,AuditFeedbackApproval4


,CreatedBy
,CreatedOn
,CreatedFrom
,LastUpdateBy
,LastUpdateOn
,LastUpdateFrom


from UserRolls
where 1=1";


//	,Code
//,Description
//,AuditId
//,TeamId
//,TourDate
//,IsPost


                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);


                SqlCommand objComm = CreateCommand(sqlText);

                objComm = ApplyParameters(objComm, conditionalFields, conditionalValue);

                SqlDataAdapter adapter = new SqlDataAdapter(objComm);
                DataTable dtResult = new DataTable();
                adapter.Fill(dtResult);

                List<UserRolls> vms = dtResult.ToList<UserRolls>();
                return vms;


            }
            catch (Exception ex)
            {

                throw ex;
            }
		}

		public int GetCount(string tableName, string fieldName, string[] conditionalFields, string[] conditionalValue)
		{
            string sqlText = "";
            List<UserRolls> VMs = new List<UserRolls>();
            DataTable dt = new DataTable();

            try
            {
                sqlText = @"
                 select count(UserRolls.Id)FilteredCount
                from UserRolls  where 1=1 ";


                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);


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

        public List<UserRolls> GetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
            string sqlText = "";
            List<UserRolls> VMs = new List<UserRolls>();
            DataTable dt = new DataTable();

            try
            {
                sqlText = @"select 
                  UserRolls.Id
                 ,UserRolls.IsAudit
                 ,UserRolls.IsTour
                 ,UserRolls.IsAdvance
                 ,UserRolls.IsTa
                 ,UserRolls.IsTourCompletionReport
                               
                 ,ur.UserName

                 from UserRolls 

                --left outer join  AuthDB.dbo.AspNetUsers  ur on UserRolls.UserId=ur.Id
                left outer join  SSLAuditAuthDB.dbo.AspNetUsers  ur on UserRolls.UserId=ur.Id


                 where 1=1 ";



                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, true);

                // ToDo Escape Sql Injection
                sqlText += @"  order by  " + index.OrderName + "  " + index.orderDir;
                sqlText += @" OFFSET  " + index.startRec + @" ROWS FETCH NEXT " + index.pageSize + " ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(sqlText);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                objComm.Fill(dt);
                var req = new UserRolls();

                VMs.Add(req);


                VMs = dt.ToList<UserRolls>();

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
            List<UserRolls> VMs = new List<UserRolls>();
            DataTable dt = new DataTable();

            try
            {
                sqlText = @"
                 select count(Id)FilteredCount
                from UserRolls  where 1=1 ";


                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);


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

		public string GetSettingsValue(string[] conditionalFields, string[] conditionalValue)
		{
			throw new NotImplementedException();
		}

		public string GetSingleValeByID(string tableName, string ReturnFields, string[] conditionalFields, string[] conditionalValue)
		{
			throw new NotImplementedException();
		}

		public UserRolls Insert(UserRolls model)
		{
			try
			{
				string sqlText = "";

				var command = CreateCommand(@" INSERT INTO UserRolls(


 UserId
,IsAudit
,AuditApproval1
,AuditApproval2
,AuditApproval3
,AuditApproval4

,IsTour
,TourApproval1
,TourApproval2
,TourApproval3
,TourApproval4

,IsAdvance
,AdvanceApproval1
,AdvanceApproval2
,AdvanceApproval3
,AdvanceApproval4

,IsTa
,IsTaApproval1
,IsTaApproval2
,IsTaApproval3
,IsTaApproval4

,IsTourCompletionReport
,TourCompletionReportApproval1
,TourCompletionReportApproval2
,TourCompletionReportApproval3
,TourCompletionReportApproval4

,IsAuditIssue
,AuditIssueApproval1
,AuditIssueApproval2
,AuditIssueApproval3
,AuditIssueApproval4

,IsAuditFeedback
,AuditFeedbackApproval1
,AuditFeedbackApproval2
,AuditFeedbackApproval3
,AuditFeedbackApproval4




,CreatedBy
,CreatedOn
,CreatedFrom


) VALUES (


 @UserId
,@IsAudit
,@AuditApproval1
,@AuditApproval2
,@AuditApproval3
,@AuditApproval4

,@IsTour
,@TourApproval1
,@TourApproval2
,@TourApproval3
,@TourApproval4

,@IsAdvance
,@AdvanceApproval1
,@AdvanceApproval2
,@AdvanceApproval3
,@AdvanceApproval4

,@IsTa
,@IsTaApproval1
,@IsTaApproval2
,@IsTaApproval3
,@IsTaApproval4

,@IsTourCompletionReport
,@TourCompletionReportApproval1
,@TourCompletionReportApproval2
,@TourCompletionReportApproval3
,@TourCompletionReportApproval4

,@IsAuditIssue
,@AuditIssueApproval1
,@AuditIssueApproval2
,@AuditIssueApproval3
,@AuditIssueApproval4

,@IsAuditFeedback
,@AuditFeedbackApproval1
,@AuditFeedbackApproval2
,@AuditFeedbackApproval3
,@AuditFeedbackApproval4




,@CreatedBy
,@CreatedOn
,@CreatedFrom


)SELECT SCOPE_IDENTITY()");

				//command.Parameters.Add("@Code", SqlDbType.NChar).Value = model.Code;
				

				//command.Parameters.Add("@Description", SqlDbType.NChar).Value = string.IsNullOrEmpty(model.Description) ? (object)DBNull.Value : model.Description;

				//command.Parameters.Add("@AuditId", SqlDbType.Int).Value = model.AuditId;
				//command.Parameters.Add("@TeamId", SqlDbType.Int).Value = model.TeamId;

				command.Parameters.Add("@UserId", SqlDbType.NChar).Value = model.UserId;

				command.Parameters.Add("@IsAudit", SqlDbType.Bit).Value = model.IsAudit;
				command.Parameters.Add("@AuditApproval1", SqlDbType.Bit).Value = model.AuditApproval1;
				command.Parameters.Add("@AuditApproval2", SqlDbType.Bit).Value = model.AuditApproval2;
				command.Parameters.Add("@AuditApproval3", SqlDbType.Bit).Value = model.AuditApproval3;
				command.Parameters.Add("@AuditApproval4", SqlDbType.Bit).Value = model.AuditApproval4;

				command.Parameters.Add("@IsTour", SqlDbType.Bit).Value = model.IsTour;
				command.Parameters.Add("@TourApproval1", SqlDbType.Bit).Value = model.TourApproval1;
				command.Parameters.Add("@TourApproval2", SqlDbType.Bit).Value = model.TourApproval2;
				command.Parameters.Add("@TourApproval3", SqlDbType.Bit).Value = model.TourApproval3;
				command.Parameters.Add("@TourApproval4", SqlDbType.Bit).Value = model.TourApproval4;

				command.Parameters.Add("@IsAdvance", SqlDbType.Bit).Value = model.IsAdvance;
				command.Parameters.Add("@AdvanceApproval1", SqlDbType.Bit).Value = model.AdvanceApproval1;
				command.Parameters.Add("@AdvanceApproval2", SqlDbType.Bit).Value = model.AdvanceApproval2;
				command.Parameters.Add("@AdvanceApproval3", SqlDbType.Bit).Value = model.AdvanceApproval3;
				command.Parameters.Add("@AdvanceApproval4", SqlDbType.Bit).Value = model.AdvanceApproval4;

				command.Parameters.Add("@IsTa", SqlDbType.Bit).Value = model.IsTa;
				command.Parameters.Add("@IsTaApproval1", SqlDbType.Bit).Value = model.IsTaApproval1;
				command.Parameters.Add("@IsTaApproval2", SqlDbType.Bit).Value = model.IsTaApproval2;
				command.Parameters.Add("@IsTaApproval3", SqlDbType.Bit).Value = model.IsTaApproval3;
				command.Parameters.Add("@IsTaApproval4", SqlDbType.Bit).Value = model.IsTaApproval3;

				command.Parameters.Add("@IsTourCompletionReport", SqlDbType.Bit).Value = model.IsTourCompletionReport;
				command.Parameters.Add("@TourCompletionReportApproval1", SqlDbType.Bit).Value = model.TourCompletionReportApproval1;
				command.Parameters.Add("@TourCompletionReportApproval2", SqlDbType.Bit).Value = model.TourCompletionReportApproval2;
				command.Parameters.Add("@TourCompletionReportApproval3", SqlDbType.Bit).Value = model.TourCompletionReportApproval3;
				command.Parameters.Add("@TourCompletionReportApproval4", SqlDbType.Bit).Value = model.TourCompletionReportApproval4;

				command.Parameters.Add("@IsAuditIssue", SqlDbType.Bit).Value = model.IsAuditIssue;
				command.Parameters.Add("@AuditIssueApproval1", SqlDbType.Bit).Value = model.AuditIssueApproval1;
				command.Parameters.Add("@AuditIssueApproval2", SqlDbType.Bit).Value = model.AuditIssueApproval2;
				command.Parameters.Add("@AuditIssueApproval3", SqlDbType.Bit).Value = model.AuditIssueApproval3;
				command.Parameters.Add("@AuditIssueApproval4", SqlDbType.Bit).Value = model.AuditIssueApproval4;

				command.Parameters.Add("@IsAuditFeedback", SqlDbType.Bit).Value = model.IsAuditFeedback;
				command.Parameters.Add("@AuditFeedbackApproval1", SqlDbType.Bit).Value = model.AuditFeedbackApproval1;
				command.Parameters.Add("@AuditFeedbackApproval2", SqlDbType.Bit).Value = model.AuditFeedbackApproval2;
				command.Parameters.Add("@AuditFeedbackApproval3", SqlDbType.Bit).Value = model.AuditFeedbackApproval3;
				command.Parameters.Add("@AuditFeedbackApproval4", SqlDbType.Bit).Value = model.AuditFeedbackApproval4;


				command.Parameters.Add("@CreatedBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(model.Audit.CreatedBy.ToString()) ? (object)DBNull.Value : model.Audit.CreatedBy.ToString();
				command.Parameters.Add("@CreatedOn", SqlDbType.NChar).Value = string.IsNullOrEmpty(model.Audit.CreatedOn.ToString()) ? (object)DBNull.Value : model.Audit.CreatedOn.ToString();
				command.Parameters.Add("@CreatedFrom", SqlDbType.NChar).Value = string.IsNullOrEmpty(model.Audit.CreatedFrom.ToString()) ? (object)DBNull.Value : model.Audit.CreatedFrom.ToString();

				//command.Parameters.Add("@IsPost", SqlDbType.NChar).Value = "N";


				model.Id = Convert.ToInt32(command.ExecuteScalar());


				return model;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		

		public UserRolls Update(UserRolls model)
		{
			try
			{
				string sqlText = "";
				int count = 0;

				string query = @"  update UserRolls set




 IsAudit  =@IsAudit
,AuditApproval1=@AuditApproval1
,AuditApproval2=@AuditApproval2
,AuditApproval3=@AuditApproval3
,AuditApproval4=@AuditApproval4


,IsTour = @IsTour
,TourApproval1 = @TourApproval1
,TourApproval2 = @TourApproval2
,TourApproval3 = @TourApproval3
,TourApproval4 = @TourApproval4


,IsAdvance = @IsAdvance 
,AdvanceApproval1 = @AdvanceApproval1
,AdvanceApproval2 = @AdvanceApproval2
,AdvanceApproval3 = @AdvanceApproval3
,AdvanceApproval4 = @AdvanceApproval4


,IsTa=@IsTa
,IsTaApproval1=@IsTaApproval1
,IsTaApproval2=@IsTaApproval2
,IsTaApproval3=@IsTaApproval3
,IsTaApproval4=@IsTaApproval4


,IsTourCompletionReport=@IsTourCompletionReport
,TourCompletionReportApproval1=@TourCompletionReportApproval1
,TourCompletionReportApproval2=@TourCompletionReportApproval2
,TourCompletionReportApproval3=@TourCompletionReportApproval3
,TourCompletionReportApproval4=@TourCompletionReportApproval4


,IsAuditIssue = @IsAuditIssue
,AuditIssueApproval1=@AuditIssueApproval1
,AuditIssueApproval2=@AuditIssueApproval2
,AuditIssueApproval3=@AuditIssueApproval3
,AuditIssueApproval4=@AuditIssueApproval4


,IsAuditFeedback = @IsAuditFeedback
,AuditFeedbackApproval1 =@AuditFeedbackApproval1
,AuditFeedbackApproval2 =@AuditFeedbackApproval2
,AuditFeedbackApproval3 =@AuditFeedbackApproval3
,AuditFeedbackApproval4 =@AuditFeedbackApproval4

  

,LastUpdateBy              =@LastUpdateBy  
,LastUpdateOn              =@LastUpdateOn  
,LastUpdateFrom            =@LastUpdateFrom 
                       
where  Id= @Id ";

//Code = @Code
//,Description = @Description
//,AuditId = @AuditId
//,TeamId = @TeamId
//,TourDate = @TourDate


				SqlCommand command = CreateCommand(query);

				command.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;

                //UserId = @UserId
                //command.Parameters.Add("@UserId", SqlDbType.NChar).Value = model.UserId;

                command.Parameters.Add("@IsAudit", SqlDbType.Bit).Value = model.IsAudit;
				command.Parameters.Add("@AuditApproval1", SqlDbType.Bit).Value = model.AuditApproval1;
				command.Parameters.Add("@AuditApproval2", SqlDbType.Bit).Value = model.AuditApproval2;
				command.Parameters.Add("@AuditApproval3", SqlDbType.Bit).Value = model.AuditApproval3;
				command.Parameters.Add("@AuditApproval4", SqlDbType.Bit).Value = model.AuditApproval4;

				command.Parameters.Add("@IsTour", SqlDbType.Bit).Value = model.IsTour;
				command.Parameters.Add("@TourApproval1", SqlDbType.Bit).Value = model.TourApproval1;
				command.Parameters.Add("@TourApproval2", SqlDbType.Bit).Value = model.TourApproval2;
				command.Parameters.Add("@TourApproval3", SqlDbType.Bit).Value = model.TourApproval3;
				command.Parameters.Add("@TourApproval4", SqlDbType.Bit).Value = model.TourApproval4;

				command.Parameters.Add("@IsAdvance", SqlDbType.Bit).Value = model.IsAdvance;
				command.Parameters.Add("@AdvanceApproval1", SqlDbType.Bit).Value = model.AdvanceApproval1;
				command.Parameters.Add("@AdvanceApproval2", SqlDbType.Bit).Value = model.AdvanceApproval2;
				command.Parameters.Add("@AdvanceApproval3", SqlDbType.Bit).Value = model.AdvanceApproval3;
				command.Parameters.Add("@AdvanceApproval4", SqlDbType.Bit).Value = model.AdvanceApproval4;

				command.Parameters.Add("@IsTa", SqlDbType.Bit).Value = model.IsTa;
				command.Parameters.Add("@IsTaApproval1", SqlDbType.Bit).Value = model.IsTaApproval1;
				command.Parameters.Add("@IsTaApproval2", SqlDbType.Bit).Value = model.IsTaApproval2;
				command.Parameters.Add("@IsTaApproval3", SqlDbType.Bit).Value = model.IsTaApproval3;
				command.Parameters.Add("@IsTaApproval4", SqlDbType.Bit).Value = model.IsTaApproval3;

				command.Parameters.Add("@IsTourCompletionReport", SqlDbType.Bit).Value = model.IsTourCompletionReport;
				command.Parameters.Add("@TourCompletionReportApproval1", SqlDbType.Bit).Value = model.TourCompletionReportApproval1;
				command.Parameters.Add("@TourCompletionReportApproval2", SqlDbType.Bit).Value = model.TourCompletionReportApproval2;
				command.Parameters.Add("@TourCompletionReportApproval3", SqlDbType.Bit).Value = model.TourCompletionReportApproval3;
				command.Parameters.Add("@TourCompletionReportApproval4", SqlDbType.Bit).Value = model.TourCompletionReportApproval4;

				command.Parameters.Add("@IsAuditIssue", SqlDbType.Bit).Value = model.IsAuditIssue;
				command.Parameters.Add("@AuditIssueApproval1", SqlDbType.Bit).Value = model.AuditIssueApproval1;
				command.Parameters.Add("@AuditIssueApproval2", SqlDbType.Bit).Value = model.AuditIssueApproval2;
				command.Parameters.Add("@AuditIssueApproval3", SqlDbType.Bit).Value = model.AuditIssueApproval3;
				command.Parameters.Add("@AuditIssueApproval4", SqlDbType.Bit).Value = model.AuditIssueApproval4;

				command.Parameters.Add("@IsAuditFeedback", SqlDbType.Bit).Value = model.IsAuditFeedback;
				command.Parameters.Add("@AuditFeedbackApproval1", SqlDbType.Bit).Value = model.AuditFeedbackApproval1;
				command.Parameters.Add("@AuditFeedbackApproval2", SqlDbType.Bit).Value = model.AuditFeedbackApproval2;
				command.Parameters.Add("@AuditFeedbackApproval3", SqlDbType.Bit).Value = model.AuditFeedbackApproval3;
				command.Parameters.Add("@AuditFeedbackApproval4", SqlDbType.Bit).Value = model.AuditFeedbackApproval4;

				command.Parameters.Add("@LastUpdateBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(model.Audit.LastUpdateBy.ToString()) ? (object)DBNull.Value : model.Audit.LastUpdateBy.ToString();
				command.Parameters.Add("@LastUpdateOn", SqlDbType.NChar).Value = string.IsNullOrEmpty(model.Audit.LastUpdateOn.ToString()) ? (object)DBNull.Value : model.Audit.LastUpdateOn.ToString();
				command.Parameters.Add("@LastUpdateFrom ", SqlDbType.NChar).Value = string.IsNullOrEmpty(model.Audit.LastUpdateFrom.ToString()) ? (object)DBNull.Value : model.Audit.LastUpdateFrom.ToString();


				int rowcount = command.ExecuteNonQuery();

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

		public List<SubmanuList> GetUserSubManu(string Username)
		{
			string sqlText = "";
			List<SubmanuList> VMs = new List<SubmanuList>();
			DataTable dt = new DataTable();




			try
			{
				sqlText =
@"
SELECT 
     mo.Id,mp.Modul,np.Node,np.Url,np.IsActive
  --FROM [SSLAuditDB].[dbo].[NodePermission] np
  FROM [NodePermission] np

  left join TBLModul as mo on np.ModulId=mo.Id
  left Join ModulPermission as mp on mp.Id=np.ModulId
where np.IsActive=1 and np.UserId=@Username 
";


				SqlDataAdapter objComm = CreateAdapter(sqlText);
				objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, null, null);

				objComm.SelectCommand.Parameters.AddWithValue("@Username", Username);

				objComm.Fill(dt);

				VMs = dt
					.ToList<SubmanuList>();
				return VMs;
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}
