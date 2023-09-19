﻿using Shampan.Models;
using Shampan.Models.AuditModule;

namespace Shampan.Core.Interfaces.Services.AuditFeedbackService
{
    public interface IAuditBranchFeedbackService : IBaseService<Models.AuditModule.AuditBranchFeedback>
    {
        ResultModel<AuditBranchFeedback> MultiplePost(AuditBranchFeedback model);
        ResultModel<AuditBranchFeedback> MultipleUnPost(AuditBranchFeedback model);

    }
}
