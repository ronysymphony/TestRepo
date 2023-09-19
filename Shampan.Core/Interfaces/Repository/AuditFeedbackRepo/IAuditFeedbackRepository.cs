using Shampan.Models.AuditModule;

namespace Shampan.Core.Interfaces.Repository.AuditFeedbackRepo
{
    public interface IAuditFeedbackRepository : IBaseRepository<AuditFeedback>
    {
		List<AuditFeedback> GetAuditUsers(string tableName, string[] conditionalFields, string[] conditionalValue);

	}
}
