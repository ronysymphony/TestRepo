using Microsoft.AspNetCore.Identity;

namespace ShampanERP.Models;

public class ApplicationUser : IdentityUser
{
    public string SageUserName { get; set; }
    public int PFNo { get; set; }
    public string Designation { get; set; }
    public bool IsPushAllow { get; set; }



}