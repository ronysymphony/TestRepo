using Shampan.Models;
using Shampan.Models.AuditModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shampan.Core.Interfaces.Repository.AuditIssues
{
    public interface IAuditIssueRepository : IBaseRepository<AuditIssue>
    {
        AuditIssue MultiplePost(AuditIssue objAdvances);
		AuditIssue MultipleUnPost(AuditIssue model);
		AuditIssue ReportStatusUpdate(AuditIssue model);

	}
}
