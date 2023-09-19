using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Core.Interfaces.Repository.Audit
{
    public interface IAuditMasterRepository : IBaseRepository<AuditMaster>
    {
        AuditMaster MultiplePost(AuditMaster model);
        AuditMaster MultipleUnPost(AuditMaster model);

        List<AuditUser> GetAuditUserByTeamId(string TeamId);
        List<AuditUser> GetAuditUserByAuditId(string TeamId);
        List<AuditMaster> GetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
        List<AuditMaster> GetIndexDataSelfStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
        List<AuditMaster> IssueGetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
        List<AuditMaster> FeedBackGetIndexDataStatus(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
		int GetAuditApprovedDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
		int GetAuditIssueDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
		int GetAuditFeedBackDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);
		AuditMaster AuditStatusUpdate(AuditMaster model);
		int GetAuditStatusDataCount(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);

		bool CheckUnPostStatus(string tableName, string[] conditionalFields, string[] conditionalValue);
		List<AuditResponse> AuditResponseGetIndexData(IndexModel index, string[] conditionalFields, string[] conditionalValue, PeramModel vm = null);

		MailSetting SaveUrl(MailSetting Url);

		List<UserProfile> GetEamil(UserProfile Email);
		List<MailSetting> GetUrl(MailSetting Email);

	}
}
