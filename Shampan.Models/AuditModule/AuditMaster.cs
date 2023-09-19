using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shampan.Models.AuditModule;

public class AuditMaster
{

    public AuditMaster()
    {
        AuditAreas = new List<AuditAreas>();
        Audit = new Audit();
        Approval = new Approval();

    }

    public Audit Audit { get; set; }
    public Approval Approval;

    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; }


    [Display(Name = "Audit Type")]
    public string AuditTypeId { get; set; }

    [Display(Name = "Planned")]
    public bool IsPlaned { get; set; }


    public string Location { get; set; }


    [Display(Name = "Team Name")]
    public string TeamId { get; set; }

    [Display(Name = "Branch Name")]
    public string BranchID { get; set; }
	[Display(Name = "Branch Name")]

	public string BranchIDStatus { get; set; }

    [Display(Name = "Start Date")]
    public string StartDate { get; set; }

    [Display(Name = "End Date")]
    public string EndDate { get; set; }
    public string Duratiom { get; set; }


    [Display(Name = "Business Target")]
    public string BusinessTarget { get; set; }

    [Display(Name = "Audit Status")]
    public string AuditStatus { get; set; }


    [Display(Name = "Report Status")]
    public string ReportStatus { get; set; }


    public string Remarks { get; set; }

    [Display(Name = "Audit Type Name")]
    public string AuditTypeName { get; set; }

    [Display(Name = "Team Name")]
    public string TeamName { get; set; }

    public string IsPosted { get; set; }
    public string PostedBy { get; set; }
    public string PostedOn { get; set; }
    public string PostedFrom { get; set; }
    public string? ReasonOfUnPost { get; set; }
    public string CompanyId { get; set; }
    public string IsPost { get; set; }
	[Display(Name = "Approve Comments")]
	public string? CommentsL1 { set; get; }
	[Display(Name = "Reject Comments")]

	public string? RejectedComments { set; get; }
    public string? UserId { set; get; }
    public string? Email { set; get; }

    public string? ApproveStatus { set; get; }
	[Display(Name = "Report Status")]

	public string ReportStatusModal { get; set; }

    public List<string> IDs { get; set; }

    public List<AuditAreas> AuditAreas { get; set; }
    public string Operation { get; set; }
    public string Edit { get; set; } = "Audit";
    public string IssuePriority { get; set; } = "Audit";
    public int PlanCount { get; set; }
    public string Url { get; set; }
	public IList<IFormFile>? Attachments { get; set; }

}