using Shampan.Core.Interfaces.Services.Settings;
using Shampan.Models;
using UnitOfWork.Interfaces;

namespace Shampan.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private IUnitOfWork _unitOfWork;

        public SettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int Archive(string tableName, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            throw new NotImplementedException();
        }

        public ResultModel<SettingsModel> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public ResultModel<List<SettingsModel>> GetAll(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            using (var context = _unitOfWork.Create())
            {

                try
                {
                    var records = context.Repositories.SettingsRepository.GetAll(conditionalFields, conditionalValue);
                    context.SaveChanges();

                    return new ResultModel<List<SettingsModel>>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.DataLoaded,
                        Data = records
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<List<SettingsModel>>()
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
            throw new NotImplementedException();
        }

        public ResultModel<List<SettingsModel>> GetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            throw new NotImplementedException();
        }

        public ResultModel<int> GetIndexDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null)
        {
            throw new NotImplementedException();
        }

        public ResultModel<SettingsModel> Insert(SettingsModel model)
        {

            using (var context = _unitOfWork.Create())
            {
                try
                {
                    if (model == null)
                    {
                        return new ResultModel<SettingsModel>()
                        {
                            Status = Status.Warning,
                            Message = MessageModel.NotFoundForSave,
                        };
                    }

                    string[] conditionField = { "SettingGroup", "SettingName" };
                    string[] conditionValue = { model.SettingGroup.Trim(), model.SettingName.Trim() };

                    bool exist = true;// context.Repositories.IPOReceiptsMasterRepository.CheckExists("Settings", conditionField, conditionValue);


                    if (!exist)
                    {

                        SettingsModel master = context.Repositories.SettingsRepository.Insert(model);

                        if (master.Id <= 0)
                        {
                            return new ResultModel<SettingsModel>()
                            {
                                Status = Status.Fail,
                                Message = MessageModel.MasterInsertFailed,
                                Data = master
                            };
                        }


                        context.SaveChanges();

                        return new ResultModel<SettingsModel>()
                        {
                            Status = Status.Success,
                            Message = MessageModel.InsertSuccess,
                            Data = master
                        };


                    }
                    else
                    {
                        return new ResultModel<SettingsModel>()
                        {
                            Status = Status.Fail,
                            Message = MessageModel.DataLoadedFailed,

                        };
                    }



                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<SettingsModel>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.InsertFail,
                        Exception = e
                    };
                }
            }

        }

        public ResultModel<SettingsModel> Update(SettingsModel model)
        {
            using (var context = _unitOfWork.Create())
            {

                try
                {



                    SettingsModel master = context.Repositories.SettingsRepository.Update(model);

                    if (master.Id == 0)
                    {
                        return new ResultModel<SettingsModel>()
                        {
                            Status = Status.Fail,
                            Message = MessageModel.DetailInsertFailed,
                            Data = master
                        };
                    }

                    context.SaveChanges();


                    return new ResultModel<SettingsModel>()
                    {
                        Status = Status.Success,
                        Message = MessageModel.UpdateSuccess,
                        Data = model
                    };

                }
                catch (Exception e)
                {
                    context.RollBack();

                    return new ResultModel<SettingsModel>()
                    {
                        Status = Status.Fail,
                        Message = MessageModel.UpdateFail,
                        Exception = e
                    };
                }
            }
        }


    }
}


