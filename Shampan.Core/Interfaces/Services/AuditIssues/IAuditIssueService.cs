using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Core.Interfaces.Services.AuditIssues;

public interface IAuditIssueService : IBaseService<AuditIssue>
{
    ResultModel<AuditIssue> MultiplePost(AuditIssue model);
	ResultModel<AuditIssue> MultipleUnPost(AuditIssue model);

	ResultModel<AuditIssue> ReportStatusUpdate(AuditIssue model);

}