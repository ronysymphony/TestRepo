using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shampan.Models.AuditModule;

public class AuditBranchFeedback
{


    public AuditBranchFeedback()
    {
        AttachmentsList = new List<AuditBranchFeedbackAttachments>();

        Audit = new Audit();
    }

    public int Id { get; set; }
    public int AuditId { get; set; }


    [Display(Name = "Issue Name")]
    public int AuditIssueId { get; set; }

    public int AuditBranchIssueId { get; set; }

    [Display(Name = "Feedback Details")]
    public string IssueDetails { get; set; }


    [Display(Name = "Feedback Heading")]

    public string Heading { get; set; }
    [Display(Name = "Status")]
    public string Status { get; set; }
    public bool IsPosted { get; set; }
    public string IsPost { get; set; }
    public string PostedBy { get; set; }
    public string PostedOn { get; set; }
    public string PostedFrom { get; set; }
    public List<string> IDs { get; set; }

    public Audit Audit { get; set; }

    public List<AuditBranchFeedbackAttachments> AttachmentsList { get; set; }
    public IList<IFormFile>? Attachments { get; set; }
    public string Operation { get; set; } = "add";

    public string Edit { get; set; } = "";
    public string AuditName { get; set; }
    public string IssueName { get; set; }
    public string? ReasonOfUnPost { get; set; }
    public string? UnPostReasonOfBranchFeedback { get; set; }
    public string UserName { get; set; }

}