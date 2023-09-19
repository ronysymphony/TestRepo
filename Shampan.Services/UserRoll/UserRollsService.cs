﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shampan.Core.Interfaces.Services.Advance;
using Shampan.Core.Interfaces.Services.Team;
using Shampan.Core.Interfaces.Services.Tour;
using Shampan.Core.Interfaces.Services.UserRoll;
using Shampan.Models;
using Shampan.Models.AuditModule;
using UnitOfWork.Interfaces;

namespace Shampan.Services.UserRoll
{
	public class UserRollsService : IUserRollsService
	{
		private IUnitOfWork _unitOfWork;

		public UserRollsService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;

		}
		public int Archive(string tableName, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
			throw new NotImplementedException();
		}

		public ResultModel<UserRolls> Delete(int id)
		{
			throw new NotImplementedException();
		}

		public ResultModel<List<UserRolls>> GetAll(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
            using (var context = _unitOfWork.Create())
            {

                try
                {
                    var records = context.Repositories.UserRollsRepository.GetAll(conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<List<UserRolls>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<UserRolls>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }



        }

        public int GetCount(string tableName, string fieldName, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
            using (var context = _unitOfWork.Create())
            {

                try
                {
                    int count =
                        context.Repositories.UserRollsRepository.GetCount(tableName,
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

		public ResultModel<List<UserRolls>> GetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
            using (var context = _unitOfWork.Create())
            {

                try
                {
                    var records = context.Repositories.UserRollsRepository.GetIndexData(index, conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<List<UserRolls>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<UserRolls>>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.DataLoadedFailed,
                        Exception = e
                    };
                }

            }



        }

        public ResultModel<int> GetIndexDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
		{
            using (var context = _unitOfWork.Create())
            {

                try
                {
                    var records = context.Repositories.UserRollsRepository.GetIndexDataCount(index, conditionalFields, conditionalValue);
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

		public ResultModel<UserRolls> Insert(UserRolls model)
		{
            string CodeGroup = "TR";
            string CodeName = "Tours";

            using (var context = _unitOfWork.Create())
            {
                try
                {
                    if (model == null)
                    {
                        return new ResultModel<UserRolls>()
                        {
                            Status = Status.Warning,
                            Message = MessageModel.NotFoundForSave,
                        };
                    }


                    //string Code = context.Repositories.ToursRepository.CodeGeneration(CodeGroup, CodeName);


                    if (model != null)
                    {
                      //  model.Code = Code;

						UserRolls master = context.Repositories.UserRollsRepository.Insert(model);

                        if (master.Id <= 0)
                        {
                            return new ResultModel<UserRolls>()
                            {
                                Status = Status.Fail,
                                Message = MessageModel.MasterInsertFailed,
                                Data = master
                            };
                        }


                        context.SaveChanges();


                        return new ResultModel<UserRolls>()
                        {
                            Status = Status.Success,
                            Message = MessageModel.InsertSuccess,
                            Data = master
                        };

                    }
                    else
                    {
                        return new ResultModel<UserRolls>()
                        {
                            Status = Status.Fail,
                            Message = MessageModel.DataLoadedFailed,

                        };
                    }



                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<UserRolls>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.InsertFail,
                        Exception = e
                    };
                }
            }

           

        }

		

		public ResultModel<UserRolls> Update(UserRolls model)
		{
            using (var context = _unitOfWork.Create())
            {

                try
                {

                    //bool CheckPostStatus = false;
                    //CheckPostStatus = context.Repositories.AdvancesRepository.CheckPostStatus("UserRolls", new[] { "Id" }, new[] { model.Id.ToString() });
                    //if (CheckPostStatus)
                    //{
                    //    return new ResultModel<UserRolls>()
                    //    {
                    //        Status = Status.Fail,
                    //        Message = MessageModel.PostAlready,

                    //    };
                    //}


					UserRolls master = context.Repositories.UserRollsRepository.Update(model);

                    context.SaveChanges();


                    return new ResultModel<UserRolls>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.UpdateSuccess,
                        Data = model
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<UserRolls>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.UpdateFail,
                        Exception = e
                    };
                }
            }

            

        }

		public List<UserManuInfo> GetUserManu(string Username)
		{
			using (IUnitOfWorkAdapter context = _unitOfWork.Create())
			{

				try
				{
					List<UserManuInfo> records = context.Repositories.UserRollsRepository.GetUserManu(Username);
					context.SaveChanges();

                    return records;



                }
				catch (Exception e)
				{
					context.RollBack();

                    throw e;
					
				}

			}
		}

		public List<SubmanuList> GetUserSubManu(string Username)
		{
			using (IUnitOfWorkAdapter context = _unitOfWork.Create())
			{

				try
				{
					List<SubmanuList> records = context.Repositories.UserRollsRepository.GetUserSubManu(Username);
					context.SaveChanges();

					return records;



				}
				catch (Exception e)
				{
					context.RollBack();

					throw e;

				}

			}
		}
	}
}
