//using Repository.Interfaces;

using Shampan.Core.Interfaces.Repository;
using Shampan.Core.Interfaces.Repository.Advance;
using Shampan.Core.Interfaces.Repository.Audit;
using Shampan.Core.Interfaces.Repository.AuditFeedbackRepo;
using Shampan.Core.Interfaces.Repository.AuditIssues;
using Shampan.Core.Interfaces.Repository.Branch;
using Shampan.Core.Interfaces.Repository.Calender;
using Shampan.Core.Interfaces.Repository.Circular;
using Shampan.Core.Interfaces.Repository.Company;
using Shampan.Core.Interfaces.Repository.CompanyInfos;
using Shampan.Core.Interfaces.Repository.ModulePermissions;
using Shampan.Core.Interfaces.Repository.Modules;
using Shampan.Core.Interfaces.Repository.Oragnogram;
using Shampan.Core.Interfaces.Repository.Settings;
using Shampan.Core.Interfaces.Repository.Team;
using Shampan.Core.Interfaces.Repository.TeamMember;
using Shampan.Core.Interfaces.Repository.Tour;
using Shampan.Core.Interfaces.Repository.TransportAllownace;
using Shampan.Core.Interfaces.Repository.User;
using Shampan.Core.Interfaces.Repository.UserRoll;

namespace Shampan.Core.Interfaces.UnitOfWork
{
    public interface IUnitOfWorkRepository
    {
        ICommonRepository CommonRepository { get; }


        IAuditFeedbackRepository AuditFeedbackRepository { get; }
        IAuditBranchFeedbackRepository AuditBranchFeedbackRepository { get; }
        IAuditFeedbackAttachmentsRepository AuditFeedbackAttachmentsRepository { get; }
        IAuditUserRepository AuditUserRepository { get; }

        IAuditIssueUserRepository AuditIssueUserRepository { get; }
        //change
        ISettingsRepository SettingsRepository { get; }
        ICompanyInfoRepository CompanyInfoRepository { get; }
        IBranchProfileRepository BranchProfileRepository { get; }
        ICompanyinfosRepository CompanyInfosRepository { get; }
        IUserBranchRepository UserBranchRepository { get; }
        ITeamsRepository TeamsRepository { get; }
        ITeamMembersRepository TeamMembersRepository { get; }
        IAdvancesRepository AdvancesRepository { get; }
        IToursRepository ToursRepository { get; }
        ITransportAllownacesRepository TransportAllownacesRepository { get; }
		IUserRollsRepository UserRollsRepository { get; }

        IAuditAreasRepository AuditAreasRepository { get; }
        IAuditMasterRepository AuditMasterRepository { get; }
        ICircularsRepository CircularsRepository { get; }
		ICalendersRepository CalendersRepository { get; }
		IOragnogramRepository OragnogramRepository { get; }


        IAuditIssueAttachmentsRepository AuditIssueAttachmentsRepository { get; }
        IAuditIssueRepository AuditIssueRepository { get; }
		ICircularAttachmentsRepository CircularAttachmentsRepository { get; }
		IEmployeesHiAttachmentsRepository EmployeesHiAttachmentsRepository { get; }

        IAuditBranchFeedbackAttachmentsRepository AuditBranchFeedbackAttachmentsRepository { get; }
		IModuleRepository ModuleRepository { get; }
		IModulePermissionRepository ModulePermissionRepository { get; }

    }
}
