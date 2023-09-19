using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Core.Interfaces.Repository.AuditFeedbackRepo
{
    public interface IAuditBranchFeedbackRepository : IBaseRepository<AuditBranchFeedback>
    {
        AuditBranchFeedback MultiplePost(AuditBranchFeedback objAdvances);
        AuditBranchFeedback MultipleUnPost(AuditBranchFeedback model);
		List<AuditBranchFeedback> GetAuditIssueUsers(string tableName, string[] conditionalFields, string[] conditionalValue);


	}
}
