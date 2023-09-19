using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Core.Interfaces.Services.Audit
{
    public interface IAuditMasterService :IBaseService<AuditMaster>
    {
        ResultModel<AuditMaster> MultiplePost(AuditMaster model);
        ResultModel<AuditMaster> MultipleUnPost(AuditMaster model);

         ResultModel<List<AuditUser>> GetAuditUserTeamId(string TeamId);
         ResultModel<List<AuditUser>> GetAuditUserAuditId(string TeamId);
        ResultModel<List<AuditMaster>> GetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
        ResultModel<List<AuditMaster>> GetIndexDataSelfStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);

        ResultModel<List<AuditMaster>> IssueGetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
        ResultModel<List<AuditMaster>> FeedBackGetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
		ResultModel<int> GetAuditApprovDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
		ResultModel<int> GetAuditIssueDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
		ResultModel<int> GetAuditFeedBackDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);

		ResultModel<AuditMaster> AuditStatusUpdate(AuditMaster model);

        ResultModel<int> GetAuditStatusDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
		//ResultModel<bool> CheckUnPostStatus(AuditMaster model);
		ResultModel<List<AuditResponse>> AuditResponseGetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);

		ResultModel<MailSetting> SaveUrl(MailSetting url);
		ResultModel<List<UserProfile>> GetEamil(UserProfile Email);
		ResultModel<List<MailSetting>> GetUrl(MailSetting Url);

	}
}
