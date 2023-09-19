using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Shampan.Core.Interfaces.Services.Audit;
using Shampan.Models;
using Shampan.Models.AuditModule;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;
using UnitOfWork.Interfaces;

namespace Shampan.Services.Audit
{
    public class AuditMasterService : IAuditMasterService
    {

        private IUnitOfWork _unitOfWork;

        public AuditMasterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResultModel<List<AuditMaster>> GetAll(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    List<AuditMaster> records = context.Repositories.AuditMasterRepository.GetAll(conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }
        }



        public ResultModel<List<AuditMaster>> FeedBackGetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    List<AuditMaster> records = context.Repositories.AuditMasterRepository.FeedBackGetIndexDataStatus(index, conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }
        }
        public ResultModel<List<AuditMaster>> IssueGetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    List<AuditMaster> records = context.Repositories.AuditMasterRepository.IssueGetIndexDataStatus(index, conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }
        }

        public ResultModel<List<AuditMaster>> GetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    List<AuditMaster> records = context.Repositories.AuditMasterRepository.GetIndexDataStatus(index, conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }
        }

        public ResultModel<List<AuditMaster>> GetIndexDataSelfStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    List<AuditMaster> records = context.Repositories.AuditMasterRepository.GetIndexDataSelfStatus(index, conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }
        }




        public ResultModel<List<AuditMaster>> GetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    List<AuditMaster> records = context.Repositories.AuditMasterRepository.GetIndexData(index, conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<AuditMaster>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }
        }

        public ResultModel<int> GetIndexDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue,
            PeramModel vm = null)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    int records = context.Repositories.AuditMasterRepository.GetIndexDataCount(index, conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<int>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<int>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }
        }

        public int GetCount(string tableName, string fieldName, string[] conditionalFields, string[] conditionalValue,
            PeramModel vm = null)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    int count =
                        context.Repositories.AuditMasterRepository.GetCount(tableName,
                            "Id", null, null);
                    context.SaveChanges();


                    return count;

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return 0;

                }

            }
        }

        public ResultModel<AuditMaster> Insert(AuditMaster model)
        {
            string CodeGroup = "Audit";
            string CodeName = "AuditMaster";

            using IUnitOfWorkAdapter context = _unitOfWork.Create();
            try
            {
                if (model is null)
                {
                    return new ResultModel<AuditMaster>()
                    {
                        Status = Status.Warning,
                        Message = MessageModel.NotFoundForSave,
                    };
                }


                string code = context.Repositories.AuditMasterRepository.GenerateCode(CodeGroup, CodeName, Convert.ToInt32(model.BranchID));

                if (code == "" && code == null)
                {
                    return new ResultModel<AuditMaster>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        //Exception = e
                    };
                }

                model.Code = code;

                AuditMaster master = context.Repositories.AuditMasterRepository.Insert(model);

                if (master.Id <= 0)
                {
                    return new ResultModel<AuditMaster>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.MasterInsertFailed,
                        Data = master
                    };
                }


                context.SaveChanges();


                return new ResultModel<AuditMaster>()
                {
                    Status = Status.Success,
                    Message = MessageModel.InsertSuccess,
                    Data = master
                };
            }
            catch (Exception e)
            {
                context.RollBack();

                return new ResultModel<AuditMaster>()
                {
                    Status = Status.Fail,
                    Message = MessageModel.InsertFail,
                    Exception = e
                };
            }
        }

        public ResultModel<AuditMaster> Update(AuditMaster model)
        {
            using IUnitOfWorkAdapter context = _unitOfWork.Create();
            try
            {


                bool CheckPostStatus = false;
                CheckPostStatus = context.Repositories.AuditMasterRepository.CheckPostStatus("A_Audits", new[] { "Id" }, new[] { model.Id.ToString() });
                if (CheckPostStatus)
                {
                    return new ResultModel<AuditMaster>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.PostAlready,

                    };
                }



                AuditMaster master = context.Repositories.AuditMasterRepository.Update(model);

                context.SaveChanges();


                return new ResultModel<AuditMaster>()
                {
                    Status = Status.Success,
                    Message = MessageModel.UpdateSuccess,
                    Data = model
                };

            }
            catch (Exception e)
            {
                context.RollBack();

                return new ResultModel<AuditMaster>()
                {
                    Status = Status.Fail,
                    Message = MessageModel.UpdateFail,
                    Exception = e
                };
            }
        }

        public ResultModel<AuditMaster> Delete(int id)
        {
			using IUnitOfWorkAdapter context = _unitOfWork.Create();
            try
            {


				string stringValue = id.ToString();

				int master = context.Repositories.AuditMasterRepository.Delete("AuditUsers", new[] { "Id" }, new[] { stringValue });

				context.SaveChanges();


				return new ResultModel<AuditMaster>()
				{
					Status = Status.Success,
					Message = MessageModel.DeleteSuccess
					//Data = model
				};

			}
			catch (Exception e)
			{
				context.RollBack();

				return new ResultModel<AuditMaster>()
				{
					Status = Status.Fail,
					Message = MessageModel.UpdateFail,
					Exception = e
				};
			}
		}

        public int Archive(string tableName, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            throw new NotImplementedException();
        }

        public ResultModel<AuditMaster> MultiplePost(AuditMaster model)
        {
            using (var context = _unitOfWork.Create())
            {

                try
                {

                    bool CheckPostStatus = false;
                    CheckPostStatus = context.Repositories.AuditMasterRepository.CheckPostStatus("A_Audits", new[] { "Id" }, new[] { model.Id.ToString() });
                    if (CheckPostStatus)
                    {
                        return new ResultModel<AuditMaster>()
                        {
                            Status = Status.Fail,
                            Message = MessageModel.PostAlready,

                        };
                    }

                    AuditMaster master = context.Repositories.AuditMasterRepository.MultiplePost(model);



                    context.SaveChanges();


                    return new ResultModel<AuditMaster>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.PostSuccess,
                        Data = model
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<AuditMaster>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.UpdateFail,
                        Exception = e
                    };
                }
            }
        }

        public ResultModel<AuditMaster> MultipleUnPost(AuditMaster model)
        {
            using (var context = _unitOfWork.Create())
            {

                try
                {

                    if (model.Operation == "unpost")
                    {
						bool CheckUpPostStatus = false;
						CheckUpPostStatus = context.Repositories.AuditMasterRepository.CheckUnPostStatus("A_Audits", new[] { "Id" }, new[] { model.Id.ToString() });
						if (CheckUpPostStatus)
						{
							return new ResultModel<AuditMaster>()
							{
								Status = Status.Fail,
								Message = MessageModel.UpPostAlready,

							};
						}
					}

                    AuditMaster master = context.Repositories.AuditMasterRepository.MultipleUnPost(model);



                    context.SaveChanges();


                    return new ResultModel<AuditMaster>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.UnPostSuccess,
                        Data = model
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<AuditMaster>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.UnPostFail,
						Data = model,
						Exception = e
                    };
                }
            }
        }

        public ResultModel<List<AuditUser>> GetAuditUserTeamId(string TeamId)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    List<AuditUser> records = context.Repositories.AuditMasterRepository.GetAuditUserByTeamId(TeamId);
                    context.SaveChanges();

                    return new ResultModel<List<AuditUser>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<AuditUser>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }
        }

		public ResultModel<int> GetAuditApprovDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			using (IUnitOfWorkAdapter context = _unitOfWork.Create())
			{

				try
				{
					int records = context.Repositories.AuditMasterRepository.GetAuditApprovedDataCount(index, conditionalFields, conditionalValue);
					context.SaveChanges();

					return new ResultModel<int>()
					{
						Status = Status.Success,
						Message = MessageModel.DataLoaded,
						Data = records
					};

				}
				catch (Exception e)
				{
					context.RollBack();

					return new ResultModel<int>()
					{
						Status = Status.Fail,
						Message = MessageModel.DataLoadedFailed,
						Exception = e
					};
				}

			}
		}

		public ResultModel<int> GetAuditIssueDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			using (IUnitOfWorkAdapter context = _unitOfWork.Create())
			{

				try
				{
					int records = context.Repositories.AuditMasterRepository.GetAuditIssueDataCount(index, conditionalFields, conditionalValue);
					context.SaveChanges();

					return new ResultModel<int>()
					{
						Status = Status.Success,
						Message = MessageModel.DataLoaded,
						Data = records
					};

				}
				catch (Exception e)
				{
					context.RollBack();

					return new ResultModel<int>()
					{
						Status = Status.Fail,
						Message = MessageModel.DataLoadedFailed,
						Exception = e
					};
				}

			}
		}
		public ResultModel<int> GetAuditFeedBackDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			using (IUnitOfWorkAdapter context = _unitOfWork.Create())
			{

				try
				{
					int records = context.Repositories.AuditMasterRepository.GetAuditFeedBackDataCount(index, conditionalFields, conditionalValue);
					context.SaveChanges();

					return new ResultModel<int>()
					{
						Status = Status.Success,
						Message = MessageModel.DataLoaded,
						Data = records
					};

				}
				catch (Exception e)
				{
					context.RollBack();

					return new ResultModel<int>()
					{
						Status = Status.Fail,
						Message = MessageModel.DataLoadedFailed,
						Exception = e
					};
				}

			}
		}

		public ResultModel<AuditMaster> AuditStatusUpdate(AuditMaster model)
		{
			using IUnitOfWorkAdapter context = _unitOfWork.Create();
			try
			{



				AuditMaster master = context.Repositories.AuditMasterRepository.AuditStatusUpdate(model);

				context.SaveChanges();


				return new ResultModel<AuditMaster>()
				{
					Status = Status.Success,
					Message = MessageModel.UpdateSuccess,
					Data = model
				};

			}
			catch (Exception e)
			{
				context.RollBack();

				return new ResultModel<AuditMaster>()
				{
					Status = Status.Fail,
					Message = MessageModel.UpdateFail,
					Exception = e
				};
			}
		}

		public ResultModel<int> GetAuditStatusDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			using (IUnitOfWorkAdapter context = _unitOfWork.Create())
			{

				try
				{
					int records = context.Repositories.AuditMasterRepository.GetAuditStatusDataCount(index, conditionalFields, conditionalValue);
					context.SaveChanges();

					return new ResultModel<int>()
					{
						Status = Status.Success,
						Message = MessageModel.DataLoaded,
						Data = records
					};

				}
				catch (Exception e)
				{
					context.RollBack();

					return new ResultModel<int>()
					{
						Status = Status.Fail,
						Message = MessageModel.DataLoadedFailed,
						Exception = e
					};
				}

			}
		}

		public ResultModel<bool> CheckUnPostStatus(AuditMaster model)
		{

			using (IUnitOfWorkAdapter context = _unitOfWork.Create())
			{

				try
				{
					bool CheckUpPostStatus = false;
					CheckUpPostStatus = context.Repositories.AuditMasterRepository.CheckUnPostStatus("A_Audits", new[] { "Id" }, new[] { model.Id.ToString() });
					if (CheckUpPostStatus)
					{
						return new ResultModel<bool>()
						{
							Status = Status.Fail,
							Message = MessageModel.PostAlready,

						};
					}

					return new ResultModel<bool>()
					{
						Status = Status.Fail,
						Message = MessageModel.PostSuccess,

					};

				}
				catch (Exception e)
				{
					context.RollBack();

					return new ResultModel<bool>()
					{
						Status = Status.Fail,
						Message = MessageModel.DataLoadedFailed,
						Exception = e
					};
				}

			}

			
		}

		public ResultModel<List<AuditResponse>> AuditResponseGetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			using (IUnitOfWorkAdapter context = _unitOfWork.Create())
			{

				try
				{
					List<AuditResponse> records = context.Repositories.AuditMasterRepository.AuditResponseGetIndexData(index, conditionalFields, conditionalValue);
					context.SaveChanges();

					return new ResultModel<List<AuditResponse>>()
					{
						Status = Status.Success,
						Message = MessageModel.DataLoaded,
						Data = records
					};

				}
				catch (Exception e)
				{
					context.RollBack();

					return new ResultModel<List<AuditResponse>>()
					{
						Status = Status.Fail,
						Message = MessageModel.DataLoadedFailed,
						Exception = e
					};
				}

			}
		}

		public ResultModel<MailSetting> SaveUrl(MailSetting model)
		{
	

			using IUnitOfWorkAdapter context = _unitOfWork.Create();
			try
			{
				if (model is null)
				{
					return new ResultModel<MailSetting>()
					{
						Status = Status.Warning,
						Message = MessageModel.NotFoundForSave,
					};
				}

				MailSetting master = context.Repositories.AuditMasterRepository.SaveUrl(model);

				if (master.Id <= 0)
				{
					return new ResultModel<MailSetting>()
					{
						Status = Status.Fail,
						Message = MessageModel.MasterInsertFailed,
						Data = master
					};
				}


				context.SaveChanges();


				return new ResultModel<MailSetting>()
				{
					Status = Status.Success,
					Message = MessageModel.InsertSuccess,
					Data = master
				};
			}
			catch (Exception e)
			{
				context.RollBack();

				return new ResultModel<MailSetting>()
				{
					Status = Status.Fail,
					Message = MessageModel.InsertFail,
					Exception = e
				};
			}
		}

		public ResultModel<List<UserProfile>> GetEamil(UserProfile Email)
		{
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    List<UserProfile> records = context.Repositories.AuditMasterRepository.GetEamil(Email);
                    context.SaveChanges();

                    return new ResultModel<List<UserProfile>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<UserProfile>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }

		}

		public ResultModel<List<MailSetting>> GetUrl(MailSetting Url)
		{
			using (IUnitOfWorkAdapter context = _unitOfWork.Create())
			{

				try
				{
					List<MailSetting> records = context.Repositories.AuditMasterRepository.GetUrl(Url);
					context.SaveChanges();

					return new ResultModel<List<MailSetting>>()
					{
						Status = Status.Success,
						Message = MessageModel.DataLoaded,
						Data = records
					};

				}
				catch (Exception e)
				{
					context.RollBack();

					return new ResultModel<List<MailSetting>>()
					{
						Status = Status.Fail,
						Message = MessageModel.DataLoadedFailed,
						Exception = e
					};
				}

			}
		}

        public ResultModel<List<AuditUser>> GetAuditUserAuditId(string AuditId)
        {
            using (IUnitOfWorkAdapter context = _unitOfWork.Create())
            {

                try
                {
                    List<AuditUser> records = context.Repositories.AuditMasterRepository.GetAuditUserByAuditId(AuditId);
                    context.SaveChanges();

                    return new ResultModel<List<AuditUser>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<AuditUser>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }
        }
    }
}
