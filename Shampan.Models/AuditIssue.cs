using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Shampan.Models.AuditModule;

namespace Shampan.Models;

public class AuditIssue
{
    public AuditIssue()
    {
        Audit = new Audit();
        AttachmentsList = new List<AuditIssueAttachments>();
    }

    public int Id { get; set; }


    [Display(Name = "Audit Name")]
    public int AuditId { get; set; }



    [Display(Name = "Issue Name")]
    public string IssueName { get; set; }




    [Display(Name = "Issue Details")]
    public string IssueDetails { get; set; }

	[Display(Name = "Issue Detail")]
	public string IssueDetailsUpdate { get; set; }




    [Display(Name = "Date Of Submission")]
    public string DateOfSubmission { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");




    [Display(Name = "Issue Priority")]
    public string IssuePriority { get; set; }
	[Display(Name = "Issue Status")]
	public string IssueStatus { get; set; }
	public string? Risk { get; set; }

	[Display(Name = "Audit Type")]
	public string AuditType { get; set; }

	public IList<IFormFile>? Attachments { get; set; }

    public List<AuditIssueAttachments> AttachmentsList { get; set; }
    public Audit Audit { get; set; }

    public AuditMaster AuditMaster { get; set; }
    public List<string> IDs { get; set; }

    public bool IsPosted { get; set; }
    public string PostedBy { get; set; }
    public string PostedOn { get; set; }
    public string PostedFrom { get; set; }
    public string Operation { get; set; } = "add";
    public string? CreatedBy { get; set; }
    public string? IsPost { get; set; }
    public string? ReasonOfUnPost { get; set; }
    public string? UnPostReasonOfIssue { get; set; }


    public string Edit { get; set; } = "";
    public string ReportStatus { get; set; }
	[Display(Name = "Report Status")]
	public string ReportStatusModal { get; set; }
	[Display(Name = "Issue Priority")]
	public string IssuePriorityUpdate { get; set; } 
}


