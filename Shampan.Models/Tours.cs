using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Shampan.Models
{
    public class Tours
    {
        public int Id { get; set; }
        [Display(Name = "Code")]
        public string? Code { get; set; }
        public string? Operation { get; set; }
		[Display(Name = "Audit Name")]

		public int AuditId { get; set; }
		[Display(Name = "Team Name")]

		public int TeamId { set; get; }
		public string TeamName { set; get; }
        public string Description { set; get; }
        [Display(Name = "Tour Date")]
        public string TourDate { set; get; }
        public string IsPost { set; get; }
        public string? ReasonOfUnPost { set; get; }
		[Display(Name = "Rejected Comments")]
		public string? RejectedComments { set; get; }
		[Display(Name = "Approved Comments")]
		public string? CommentsL1 { set; get; }
        public string? ApproveStatus { set; get; }
		public List<string> IDs { get; set; }

		public string Edit { get; set; } = "Audit";

		public Audit Audit;
		public Approval Approval;

        public Tours()
        {
            Audit = new Audit();
            Approval = new Approval();
        }


    }

}
