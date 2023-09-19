using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SageERP.ExtensionMethods;
using Shampan.Core.Interfaces.Services.Audit;
using Shampan.Core.Interfaces.Services.AuditFeedbackService;
using Shampan.Core.Interfaces.Services.AuditIssues;
using Shampan.Core.Interfaces.Services.UserRoll;
using Shampan.Models;
using Shampan.Models.AuditModule;
using Shampan.Services.Audit;
using Shampan.Services.AuditFeedbackService;
using Shampan.Services.AuditIssues;
using ShampanERP.Controllers;
using ShampanERP.Models;
using ShampanERP.Persistence;
using StackExchange.Exceptional;
using System.Net.Mail;
using System.Net;
using EmailSettings = Shampan.Models.EmailSettings;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Net.Mime;
using MessagePack.Formatters;
using Humanizer;
using System.Security.Policy;
using Shampan.Services;
using Microsoft.Extensions.Caching.Memory;
using SSLAudit.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.CodeAnalysis.Differencing;
using System;
using StackExchange.Exceptional.Internal;
using System.Data.SqlTypes;
using OfficeOpenXml;
using Shampan.Core.Interfaces.Services.Team;
using Serilog;
using Microsoft.AspNetCore.Http.Extensions;

namespace SSLAudit.Controllers.Audit
{
    [ServiceFilter(typeof(UserMenuActionFilter))]




    [Authorize]
    public class AuditController : Controller
    {
        private readonly ApplicationDbContext _applicationDb;
        private readonly IAuditMasterService _auditMasterService;
        private readonly IAuditAreasService _auditAreasService;
        private readonly IAuditIssueService _auditIssueService;
        private readonly IAuditIssueAttachmentsService _auditIssueAttachmentsService;
        private readonly IAuditFeedbackService _auditFeedbackService;
        private readonly IAuditBranchFeedbackService _auditBranchFeedbackService;
        private readonly IAuditFeedbackAttachmentsService _auditFeedbackAttachmentsService;
        private readonly IAuditBranchFeedbackAttachmentsService _auditBranchFeedbackAttachmentsService;
        private readonly IAuditUserService _auditUserService;
        private readonly IAuditIssueUserService _auditIssueUserService;
        private readonly IUserRollsService _userRollsService;
        private readonly IMemoryCache _memoryCache;


        private readonly ITeamsService _teamsService;

        private readonly ILogger<AuditController> _logger;



        public AuditController(ApplicationDbContext applicationDb,
            IAuditMasterService auditMasterService,
            IAuditAreasService auditAreasService,
            IAuditIssueService auditIssueService,
            IAuditIssueAttachmentsService auditIssueAttachmentsService,
            IAuditFeedbackService auditFeedbackService,
            IAuditBranchFeedbackService auditBranchFeedbackService,
            IUserRollsService userRollsService,
            IMemoryCache memoryCache,
            ITeamsService teamsService,
            ILogger<AuditController> logger,



        IAuditFeedbackAttachmentsService auditFeedbackAttachmentsService, IAuditUserService auditUserService,
        IAuditIssueUserService auditIssueUserService,
        IAuditBranchFeedbackAttachmentsService auditBranchFeedbackAttachmentsService)
        {
            _applicationDb = applicationDb;
            _auditMasterService = auditMasterService;
            _auditAreasService = auditAreasService;
            _auditIssueService = auditIssueService;
            _auditIssueAttachmentsService = auditIssueAttachmentsService;
            _auditFeedbackService = auditFeedbackService;
            _auditFeedbackAttachmentsService = auditFeedbackAttachmentsService;
            _auditUserService = auditUserService;
            _auditIssueUserService = auditIssueUserService;
            _auditBranchFeedbackService = auditBranchFeedbackService;
            _userRollsService = userRollsService;
            _auditBranchFeedbackAttachmentsService = auditBranchFeedbackAttachmentsService;
            _auditUserService = auditUserService;
            _memoryCache = memoryCache;
            _teamsService = teamsService;
            _logger = logger;


        }


        public async Task<IActionResult> Index()
        //public  IActionResult Index()
        {

            var cacheKey = $"{User.Identity.Name}_BranchClaim";
            if (_memoryCache.TryGetValue(cacheKey, out BranchClaim branchClaim))
            {
                string name = branchClaim.BranchName;

                var identity = new ClaimsIdentity(User.Identity);
                identity.AddClaim(new Claim(ClaimNames.CurrentBranch, branchClaim.BranchId.ToString()));
                identity.AddClaim(new Claim(ClaimNames.CurrentBranchName, branchClaim.BranchName.ToString().Trim()));

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties() { IssuedUtc = DateTimeOffset.Now, });

            }


            string userName = User.Identity.Name;
            List<UserManuInfo> manu = _userRollsService.GetUserManu(userName);
            return View();
        }



        public IActionResult Create()
        {
            //string userName = User.Identity.Name;
            //List<UserManuInfo> manu = _userRollsService.GetUserManu(userName);
            //ViewBag.ManuList = manu;
            AuditMaster auditMaster = new AuditMaster()
            {
                Operation = "add",
                IsPlaned = true,
                AuditStatus = "NA",
                ReportStatus = "NA"
            };
            return View(auditMaster);
        }


        [HttpPost]
        public ActionResult ReportCreateEdit(AuditIssue master)
        {
            ResultModel<AuditIssue> result = new ResultModel<AuditIssue>();
            try
            {

                if (master.Operation == "update")
                {


                    result = _auditIssueService.ReportStatusUpdate(master);



                    return Ok(result);
                }
                else
                {



                    if (result.Status == Status.Fail)
                    {
                        throw result.Exception;
                    }


                    return Ok(result);
                }


            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }


        [HttpPost]
        public ActionResult AuditStatusCreateEdit(AuditMaster master)
        {
            ResultModel<AuditMaster> result = new ResultModel<AuditMaster>();
            try
            {

                if (master.Operation == "update")
                {


                    result = _auditMasterService.AuditStatusUpdate(master);



                    return Ok(result);
                }
                else
                {



                    if (result.Status == Status.Fail)
                    {
                        throw result.Exception;
                    }


                    return Ok(result);
                }


            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }

        //for excel

        [HttpPost]
        public ActionResult ExcelCreateEdit(AuditMaster master, IFormFile Attachments)
        {
            ResultModel<AuditMaster> result = new ResultModel<AuditMaster>();
            try
            {

                using (var stream = new MemoryStream())
                {
                    Attachments.CopyTo(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int lastRow = worksheet.Cells["A" + worksheet.Dimension.End.Row.ToString()].End.Row;
                        int rowCountWithValues = 1;

                        for (int row = 2; row <= lastRow; row++)
                        {
                            if (

                                worksheet.Cells[row, 1].Value != null || worksheet.Cells[row, 2].Value != null ||
                                worksheet.Cells[row, 3].Value != null || worksheet.Cells[row, 4].Value != null ||
                                worksheet.Cells[row, 5].Value != null || worksheet.Cells[row, 6].Value != null ||
                                worksheet.Cells[row, 7].Value != null || worksheet.Cells[row, 9].Value != null ||
                                worksheet.Cells[row, 8].Value != null || worksheet.Cells[row, 10].Value != null

                            )
                            {
                                rowCountWithValues++;

                            }
                        }


                        //for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        for (int row = 2; row <= rowCountWithValues; row++)
                        {


                            master.Name = worksheet.Cells[row, 1].Value?.ToString();
                            master.StartDate = worksheet.Cells[row, 2].Value?.ToString();
                            master.EndDate = worksheet.Cells[row, 3].Value?.ToString();
                            var planed = worksheet.Cells[row, 4].Value?.ToString();
                            if (planed == "True" || planed == "true") { master.IsPlaned = true; } else { master.IsPlaned = false; }
                            master.AuditStatus = worksheet.Cells[row, 6].Value?.ToString();
                            master.BusinessTarget = worksheet.Cells[row, 9].Value?.ToString();



                            master.TeamName = worksheet.Cells[row, 7].Value?.ToString();
                            master.TeamId = _teamsService.GetSingleValeByID(null, master.TeamName, null, null);

                            master.BranchID = worksheet.Cells[row, 8].Value?.ToString();
                            master.AuditTypeId = worksheet.Cells[row, 5].Value?.ToString();





                            string userName = User.Identity.Name;
                            ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);
                            master.Audit.CreatedBy = user.UserName;
                            master.Audit.CreatedOn = DateTime.Now;
                            master.Audit.CreatedFrom = "";
                            master.Audit.LastUpdateBy = user.UserName;
                            master.Audit.LastUpdateOn = DateTime.Now;
                            master.Audit.LastUpdateFrom = "";
                            master.ReportStatus = "NA";
                            master.AuditStatus = "NA";
                            master.CompanyId = "1";


                            result = _auditMasterService.Insert(master);



                            Uri returnurl = new Uri(Request.Host.ToString());
                            string urlString = returnurl.ToString();
                            string AuditUrl = "https://" + urlString + "/Audit/Edit/" + result.Data.Id + "?edit=auditStatus";
                            string IssueUrl = "https://" + urlString + "/Audit/Edit/" + result.Data.Id + "?edit=issueApprove";
                            string BranchFeedUrl = "https://" + urlString + "/Audit/Edit/" + result.Data.Id + "?edit=branchFeedbackApprove";

                            string AuditPreviewUrl = "https://" + urlString + "/Audit/Edit/" + result.Data.Id + "?edit=audit";

                            MailSetting ms = new MailSetting();
                            ms.ApprovedUrl = AuditUrl;
                            ms.AutidId = result.Data.Id;
                            ms.Status = "Audit";
                            _auditMasterService.SaveUrl(ms);

                            ms.ApprovedUrl = IssueUrl;
                            ms.AutidId = result.Data.Id;
                            ms.Status = "Issue";
                            _auditMasterService.SaveUrl(ms);

                            ms.ApprovedUrl = BranchFeedUrl;
                            ms.AutidId = result.Data.Id;
                            ms.Status = "BranchFeedback";
                            _auditMasterService.SaveUrl(ms);

                            ResultModel<List<AuditUser>> UserData = _auditMasterService.GetAuditUserTeamId(master.TeamId);

                            foreach (var User in UserData.Data)
                            {
                                User.Audit.CreatedBy = user.UserName;
                                User.AuditId = result.Data.Id;
                                User.Audit.CreatedOn = DateTime.Now;
                                User.Audit.CreatedFrom = "";
                                User.Audit.LastUpdateBy = user.UserName;
                                User.Audit.LastUpdateOn = DateTime.Now;
                                _auditUserService.Insert(User);
                                MailService.SendAuditApprovalMail(User.EmailAddress, AuditPreviewUrl);
                            }





                            if (result.Status == Status.Fail)
                            {
                                throw result.Exception;
                            }



                        }
                    }

                }


                return Ok(result);



            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }

        //end of excel

        //for Email 
        [HttpPost]
        public ActionResult SendEmailCreateEdit(AuditMaster master)
        {
            ResultModel<AuditMaster> result = new ResultModel<AuditMaster>();
            try
            {

                string userName = User.Identity.Name;
                ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                master.Audit.CreatedBy = user.UserName;
                master.Audit.CreatedOn = DateTime.Now;
                master.Audit.CreatedFrom = "";
                master.Audit.LastUpdateBy = user.UserName;
                master.Audit.LastUpdateOn = DateTime.Now;
                master.Audit.LastUpdateFrom = "";

                master.ReportStatus = "NA";
                master.AuditStatus = "NA";

                master.CompanyId = "1";

                string[] parts = new string[] { "", "" };
                var currentUrl = HttpContext.Request.GetDisplayUrl();
                parts = currentUrl.TrimStart('/').Split('/');
                string urlhttp = parts[0];
                string HostUrl = parts[2];

                //string AuditPreviewUrl = /*"https://" + */ HostUrl + "/Audit/Edit/" + master.Id + "?edit=audit";
                string AuditPreviewUrl = urlhttp + "//" + HostUrl + "/Audit/Edit/" + master.Id + "?edit=audit";


                ResultModel<List<AuditUser>> UserData = _auditMasterService.GetAuditUserAuditId(master.Id.ToString());

                foreach (var User in UserData.Data)
                {
                    User.Audit.CreatedBy = user.UserName;
                    User.AuditId = master.Id;
                    User.Audit.CreatedOn = DateTime.Now;
                    User.Audit.CreatedFrom = "";
                    User.Audit.LastUpdateBy = user.UserName;
                    User.Audit.LastUpdateOn = DateTime.Now;
                    //_auditUserService.Insert(User);
                    MailService.SendAuditApprovalMail(User.EmailAddress, AuditPreviewUrl);
                }



                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }


                return Ok(result);



            }
            catch (Exception ex)
            {

                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }

        //End of Email



        [HttpPost]
        public ActionResult CreateEdit(AuditMaster master)
        {
            ResultModel<AuditMaster> result = new ResultModel<AuditMaster>();
            try
            {






                if (master.Operation == "update")
                {
                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                    master.Audit.LastUpdateBy = user.UserName;
                    master.Audit.LastUpdateOn = DateTime.Now;
                    master.Audit.LastUpdateFrom = "";

                    if (master.AuditStatus == null)
                    {
                        master.AuditStatus = "NA";
                    }
                    if (master.ReportStatus == null)
                    {
                        master.ReportStatus = "NA";
                    }

                    //master.BranchID = User.GetCurrentBranchId();
                    result = _auditMasterService.Update(master);

                    //if (result.Status == Status.Fail)
                    //{
                    //throw result.Exception;
                    //}


                    return Ok(result);
                }
                else
                {
                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                    master.Audit.CreatedBy = user.UserName;
                    master.Audit.CreatedOn = DateTime.Now;
                    master.Audit.CreatedFrom = "";
                    master.Audit.LastUpdateBy = user.UserName;
                    master.Audit.LastUpdateOn = DateTime.Now;
                    master.Audit.LastUpdateFrom = "";

                    master.ReportStatus = "NA";
                    master.AuditStatus = "NA";

                    //master.BranchID = User.GetCurrentBranchId();
                    master.CompanyId = "1";



                    result = _auditMasterService.Insert(master);





					var currentUrl = HttpContext.Request.GetDisplayUrl();
					
					string[] parts = new string[] { "", "" };
					parts = currentUrl.TrimStart('/').Split('/');
					string urlhttp = parts[0]; 
											   
					string HostUrl = parts[2];



                    //Uri returnurl = new Uri(Request.Host.ToString());
                    //string urlString = returnurl.ToString();

                    string AuditUrl = urlhttp + "//" + HostUrl + "/Audit/Edit/" + result.Data.Id + "?edit=auditStatus";
                    string IssueUrl = urlhttp + "//" + HostUrl + "/Audit/Edit/" + result.Data.Id + "?edit=issueApprove";
                    string BranchFeedUrl = urlhttp + "//" + HostUrl + "/Audit/Edit/" + result.Data.Id + "?edit=branchFeedbackApprove";

                    string AuditPreviewUrl = urlhttp + "//" + HostUrl + "/Audit/Edit/" + result.Data.Id + "?edit=audit";

                    MailSetting ms = new MailSetting();
                    ms.ApprovedUrl = AuditPreviewUrl;
                    ms.AutidId = result.Data.Id;
                    ms.Status = "Audit";
                    _auditMasterService.SaveUrl(ms);

                    ms.ApprovedUrl = AuditPreviewUrl;
                    ms.AutidId = result.Data.Id;
                    ms.Status = "Issue";
                    _auditMasterService.SaveUrl(ms);

                    ms.ApprovedUrl = AuditPreviewUrl;
                    ms.AutidId = result.Data.Id;
                    ms.Status = "BranchFeedback";
                    _auditMasterService.SaveUrl(ms);


                    //end



                    //AuditEmailSent(master.Name);



                    ResultModel<List<AuditUser>> UserData = _auditMasterService.GetAuditUserTeamId(master.TeamId);



                    foreach (var User in UserData.Data)
                    {
                        User.Audit.CreatedBy = user.UserName;
                        User.AuditId = result.Data.Id;
                        User.Audit.CreatedOn = DateTime.Now;
                        User.Audit.CreatedFrom = "";
                        User.Audit.LastUpdateBy = user.UserName;
                        User.Audit.LastUpdateOn = DateTime.Now;
                        _auditUserService.Insert(User);
                        MailService.SendAuditApprovalMail(User.EmailAddress, AuditPreviewUrl);
                    }







                    if (result.Status == Status.Fail)
                    {
                        //Exception ex = new Exception();
                        //_logger.LogError(ex, "An error occurred in the Index action.");
                        throw result.Exception;
                    }


                    return Ok(result);
                }


            }
            catch (Exception ex)
            {

                //var logFilePath = @"F:\errorlog.txt";
                //var logMessages = new List<string>() { "test"};

                //if (System.IO.File.Exists(logFilePath))
                //{
                //	// Read log messages from the log file
                //	var logLines = System.IO.File.ReadAllLines(logFilePath);

                //	// Add log messages to the list
                //	logMessages.AddRange(logLines);
                //}


                ////var logEvents = Log.ReadFrom.File(@"F:\errorlog.txt").ReadFrom(DateTime.Now.AddMonths(-1)); // Adjust the time frame as needed




                //_logger.LogError(ex, "An error occurred in the Index action.");
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }





        [HttpPost]
        public ActionResult AreaCreateEdit(AuditAreas master)
        {
            ResultModel<AuditAreas> result = new ResultModel<AuditAreas>();
            try
            {

                if (master.Operation == "update")
                {

                    result = _auditAreasService.Update(master);

                    if (result.Status == Status.Fail)
                    {
                        throw result.Exception;
                    }


                    return Ok(result);
                }
                else
                {

                    result = _auditAreasService.Insert(master);

                    if (result.Status == Status.Fail)
                    {
                        throw result.Exception;
                    }

                    return Ok(result);
                }


            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }





        [HttpPost]
        public ActionResult AuditUserCreateEdit(AuditUser master)
        {
            ResultModel<AuditUser> result = new ResultModel<AuditUser>();
            try
            {

                if (master.Operation == "update")
                {
                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                    master.Audit.LastUpdateBy = user.UserName;
                    master.Audit.LastUpdateOn = DateTime.Now;
                    master.Audit.LastUpdateFrom = "";

                    result = _auditUserService.Update(master);

                    if (result.Status == Status.Fail)
                    {
                        throw result.Exception;
                    }


                    return Ok(result);
                }
                else
                {
                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                    master.Audit.CreatedBy = user.UserName;
                    master.Audit.CreatedOn = DateTime.Now;
                    master.Audit.CreatedFrom = "";

                    //Uri returnurl = new Uri(Request.Host.ToString());
                    //string urlString = returnurl.ToString();
                    //string Url = "https://" + urlString + "/Audit/Edit/" + result.Data.Id + "?edit=auditStatus";
                    result = _auditUserService.Insert(master);

                    UserProfile urp = new UserProfile();
                    urp.Id = master.AuditId;
                    urp.UserName = user.UserName;
                    MailSetting mailSetting = new MailSetting();
                    mailSetting.Id = master.AuditId;

                    var notifiEmai = _auditMasterService.GetEamil(urp);

                    var email = notifiEmai.Data.FirstOrDefault();


                    //var AuditUrl = _auditMasterService.GetUrl(mailSetting);

                    //var url = AuditUrl.Data.FirstOrDefault();
                    //Uri returnurl = new Uri(Request.Host.ToString());
                    //string urlString = returnurl.ToString();
                    //string AuditPreviewUrl = "https://" + urlString + "/Audit/Edit/" + result.Data.AuditId + "?edit=audit";


                    var currentUrl = HttpContext.Request.GetDisplayUrl();
                    string[] parts = new string[] { "", "" };
                    parts = currentUrl.TrimStart('/').Split('/');
                    string urlhttp = parts[0]; // "Audit"
                                               //string b1 = parts[1]; // "Edit"
                    string HostUrl = parts[2]; // "155"

                    string AuditPreviewUrl = urlhttp + "//" + HostUrl + "/Audit/Edit/" + result.Data.AuditId + "?edit=audit";
                    MailService.SendAuditApprovalMail(master.EmailAddress, AuditPreviewUrl);
                    if (result.Status == Status.Fail)
                    {
                        throw result.Exception;
                    }

                    return Ok(result);
                }


            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }



        [HttpPost]
        public ActionResult AuditIssueUserCreateEdit(AuditIssueUser master)
        {
            ResultModel<AuditIssueUser> result = new ResultModel<AuditIssueUser>();
            try
            {

                if (master.Operation == "update")
                {
                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                    master.Issue.LastUpdateBy = user.UserName;
                    master.Issue.LastUpdateOn = DateTime.Now;
                    master.Issue.LastUpdateFrom = "";

                    result = _auditIssueUserService.Update(master);

                    if (result.Status == Status.Fail)
                    {
                        throw result.Exception;
                    }



                    return Ok(result);
                }
                else
                {
                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                    master.Issue.CreatedBy = user.UserName;
                    master.Issue.CreatedOn = DateTime.Now;
                    master.Issue.CreatedFrom = "";

                    result = _auditIssueUserService.Insert(master);

                    UserProfile urp = new UserProfile();
                    urp.Id = master.AuditId;
                    urp.UserName = user.UserName;
                    MailSetting mailSetting = new MailSetting();
                    mailSetting.Id = master.AuditId;

                    var notifiEmai = _auditMasterService.GetEamil(urp);

                    var email = notifiEmai.Data.FirstOrDefault();


                    //var AuditUrl = _auditMasterService.GetUrl(mailSetting);

                    //var url = AuditUrl.Data.FirstOrDefault();

                    Uri returnurl = new Uri(Request.Host.ToString());
                    string urlString = returnurl.ToString();
                    string AuditPreviewUrl = "https://" + urlString + "/Audit/Edit/" + result.Data.AuditId + "?edit=audit";


                    MailService.SendAuditApprovalMail(master.EmailAddress, AuditPreviewUrl);


                    if (result.Status == Status.Fail)
                    {
                        throw result.Exception;
                    }

                    return Ok(result);
                }


            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }



        //public ActionResult<IList<AuditMaster>> Edit(int id, string edit = "audit")
        public async Task<ActionResult<IList<AuditMaster>>> Edit(int id, string edit = "audit")
        {
            try
            {
                //For Branch Name
                var cacheKey = $"{User.Identity.Name}_BranchClaim";
                if (_memoryCache.TryGetValue(cacheKey, out BranchClaim branchClaim))
                {
                    string name = branchClaim.BranchName;

                    var identity = new ClaimsIdentity(User.Identity);
                    identity.AddClaim(new Claim(ClaimNames.CurrentBranch, branchClaim.BranchId.ToString()));
                    identity.AddClaim(new Claim(ClaimNames.CurrentBranchName, branchClaim.BranchName.ToString().Trim()));

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties() { IssuedUtc = DateTimeOffset.Now, });

                }
                //End of Branch 







                ResultModel<List<AuditMaster?>> result =
                    _auditMasterService.GetAll(new[] { "Id" }, new[] { id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                AuditMaster? auditMaster = result.Data.FirstOrDefault();

                if (auditMaster is null)
                {
                    return RedirectToAction("Index");
                }

                auditMaster.AuditAreas = _auditAreasService.GetAll(new[] { "AuditId" }, new[] { id.ToString() }).Data;

                auditMaster.Operation = "update";
                auditMaster.Edit = edit;
                auditMaster.ApproveStatus = edit;


                return View("Create", auditMaster);
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return RedirectToAction("Index");
            }
        }
        public ActionResult<IList<AuditMaster>> Delete(AuditMaster master)
        {
            try
            {

                ResultModel<AuditMaster> result = _auditMasterService.Delete(master.Id);


                //ResultModel<List<AuditMaster?>> result =
                //	_auditMasterService.GetAll(new[] { "Id" }, new[] { id.ToString() });

                //if (result.Status == Status.Fail)
                //{
                //	throw result.Exception;
                //}

                //AuditMaster? auditMaster = result.Data.FirstOrDefault();

                //if (auditMaster is null)
                //{
                //	return RedirectToAction("Index");
                //}

                //auditMaster.AuditAreas = _auditAreasService.GetAll(new[] { "AuditId" }, new[] { id.ToString() }).Data;

                //auditMaster.Operation = "update";
                //auditMaster.Edit = edit;

                //return View("Create", auditMaster);
                return Ok(result);

            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return RedirectToAction("Index");
            }
        }

        //Audit Approve 
        //public IActionResult ApproveStatusIndex()
        public async Task<IActionResult> ApproveStatusIndex()
        {

            //For Branch Name
            var cacheKey = $"{User.Identity.Name}_BranchClaim";
            if (_memoryCache.TryGetValue(cacheKey, out BranchClaim branchClaim))
            {
                string name = branchClaim.BranchName;

                var identity = new ClaimsIdentity(User.Identity);
                identity.AddClaim(new Claim(ClaimNames.CurrentBranch, branchClaim.BranchId.ToString()));
                identity.AddClaim(new Claim(ClaimNames.CurrentBranchName, branchClaim.BranchName.ToString().Trim()));

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties() { IssuedUtc = DateTimeOffset.Now, });

            }
            //End of Branch 


            //string userName = User.Identity.Name;
            //List<UserManuInfo> manu = _userRollsService.GetUserManu(userName);
            //ViewBag.ManuList = manu;

            return View();
        }



        public IActionResult _approveStatusIndex(string status)
        {
            try
            {
                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                index.UserName = userName;
                ApplicationUser user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                var search = Request.Form["search[value]"].FirstOrDefault();
                string users = Request.Form["BranchCode"].ToString();
                string branch = Request.Form["BranchName"].ToString();
                string Address = Request.Form["Address"].ToString();
                string TelephoneNo = Request.Form["TelephoneNo"].ToString();



                string code = Request.Form["code"].ToString();
                string name = Request.Form["name"].ToString();
                string description = Request.Form["description"].ToString();
                string approveStatus = Request.Form["approveStatus"].ToString();


                string post = Request.Form["ispost"].ToString();


                if (post == "Select")
                {
                    post = "";

                }



                string draw = Request.Form["draw"].ToString();
                var startRec = Request.Form["start"].FirstOrDefault();
                var pageSize = Request.Form["length"].FirstOrDefault();
                var orderName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][Name]"].FirstOrDefault();

                var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();

                index.SearchValue = Request.Form["search[value]"].FirstOrDefault();

                index.OrderName = "Id";

                index.orderDir = orderDir;
                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);


                index.createdBy = userName;

                string[] conditionalFields = null;
                string?[] conditionalValue = null;

                if (status == "self")
                {
                    conditionalFields = new[]

                    {
                             "Code like",
                            "Name like",
                            "ad.IsPost ",
                            "ad.createdBy ",
                     };

                    conditionalValue = new[] { code, name, "Y", index.createdBy };
                }

                else
                {
                    conditionalFields = new[]
                    {
                            "Code like",
                            "Name like",
                            "ad.IsPost "
                    };

                    conditionalValue = new[] { code, name, "Y" };
                }



                //string[] conditionalFields = new[]
                //{
                //            "Code like",
                //            "Name like",
                //            "ad.IsPost "

                //};

                //string?[] conditionalValue = new[] { code, name, "Y" };

                ResultModel<List<AuditMaster>> indexData =
                    _auditMasterService.GetIndexDataStatus(index, conditionalFields, conditionalValue);



                ResultModel<int> indexDataCount =
                 _auditMasterService.GetAuditApprovDataCount(index, conditionalFields, conditionalValue);


                int result = _auditMasterService.GetCount(TableName.A_Audits, "Id", new[] { "A_Audits.createdBy", }, new[] { userName });


                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditMaster>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }


        }

        public IActionResult SelfApproveStatusIndex()
        {
            //string userName = User.Identity.Name;
            //List<UserManuInfo> manu = _userRollsService.GetUserManu(userName);
            //ViewBag.ManuList = manu;
            return View();
        }

        public IActionResult _selfapproveStatusIndex()
        {
            try
            {
                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                index.UserName = userName;
                ApplicationUser user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                var search = Request.Form["search[value]"].FirstOrDefault();
                string users = Request.Form["BranchCode"].ToString();
                string branch = Request.Form["BranchName"].ToString();
                string Address = Request.Form["Address"].ToString();
                string TelephoneNo = Request.Form["TelephoneNo"].ToString();



                string code = Request.Form["code"].ToString();
                string name = Request.Form["name"].ToString();
                string description = Request.Form["description"].ToString();
                string approveStatus = Request.Form["approveStatus"].ToString();


                string post = Request.Form["ispost"].ToString();


                if (post == "Select")
                {
                    post = "";

                }



                string draw = Request.Form["draw"].ToString();
                var startRec = Request.Form["start"].FirstOrDefault();
                var pageSize = Request.Form["length"].FirstOrDefault();
                var orderName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][Name]"].FirstOrDefault();

                var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();

                index.SearchValue = Request.Form["search[value]"].FirstOrDefault();

                index.OrderName = "Id";

                index.orderDir = orderDir;
                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);


                index.createdBy = userName;


                string[] conditionalFields = new[]
                {
                            "Code like",
                            "Name like",
                            "ad.IsPost "

                };

                string?[] conditionalValue = new[] { code, name, "Y" };

                ResultModel<List<AuditMaster>> indexData =
                    _auditMasterService.GetIndexDataSelfStatus(index, conditionalFields, conditionalValue);



                ResultModel<int> indexDataCount =
                _auditMasterService.GetIndexDataCount(index, conditionalFields, conditionalValue);


                int result = _auditMasterService.GetCount(TableName.A_Audits, "Id", new[] { "A_Audits.createdBy", }, new[] { userName });


                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditMaster>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }


        }


        //issue approve
        //public IActionResult IssueApproveStatusIndex()
        public async Task<IActionResult> IssueApproveStatusIndex()
        {

            //For Branch Name
            var cacheKey = $"{User.Identity.Name}_BranchClaim";
            if (_memoryCache.TryGetValue(cacheKey, out BranchClaim branchClaim))
            {
                string name = branchClaim.BranchName;

                var identity = new ClaimsIdentity(User.Identity);
                identity.AddClaim(new Claim(ClaimNames.CurrentBranch, branchClaim.BranchId.ToString()));
                identity.AddClaim(new Claim(ClaimNames.CurrentBranchName, branchClaim.BranchName.ToString().Trim()));

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties() { IssuedUtc = DateTimeOffset.Now, });

            }
            //End of Branch

            //string userName = User.Identity.Name;
            //List<UserManuInfo> manu = _userRollsService.GetUserManu(userName);
            //ViewBag.ManuList = manu;
            return View();
        }

        public IActionResult _issueApproveStatusIndex()
        {
            try
            {
                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                index.UserName = userName;
                ApplicationUser user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                var search = Request.Form["search[value]"].FirstOrDefault();
                string users = Request.Form["BranchCode"].ToString();
                string branch = Request.Form["BranchName"].ToString();
                string Address = Request.Form["Address"].ToString();
                string TelephoneNo = Request.Form["TelephoneNo"].ToString();



                string code = Request.Form["code"].ToString();
                string name = Request.Form["name"].ToString();
                string description = Request.Form["description"].ToString();
                string approveStatus = Request.Form["approveStatus"].ToString();


                string post = Request.Form["ispost"].ToString();


                if (post == "Select")
                {
                    post = "";

                }



                string draw = Request.Form["draw"].ToString();
                var startRec = Request.Form["start"].FirstOrDefault();
                var pageSize = Request.Form["length"].FirstOrDefault();
                var orderName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][Name]"].FirstOrDefault();

                var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();

                index.SearchValue = Request.Form["search[value]"].FirstOrDefault();

                index.OrderName = "Id";

                index.orderDir = orderDir;
                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);


                index.createdBy = userName;


                string[] conditionalFields = new[]
                {
                            "Code like",
                            "Name like",
                            "ad.IsPost "


                };

                string?[] conditionalValue = new[] { code, name, "Y" };

                ResultModel<List<AuditMaster>> indexData =
                    _auditMasterService.IssueGetIndexDataStatus(index, conditionalFields, conditionalValue);



                ResultModel<int> indexDataCount =
                _auditMasterService.GetAuditIssueDataCount(index, conditionalFields, conditionalValue);


                int result = _auditMasterService.GetCount(TableName.A_Audits, "Id", new[] { "A_Audits.createdBy", }, new[] { userName });


                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditMaster>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }

        }
        //Branch FeedBack approve
        //public IActionResult FeedBackApproveStatusIndex()
        public async Task<IActionResult> FeedBackApproveStatusIndex()
        {

            //For Branch Name
            var cacheKey = $"{User.Identity.Name}_BranchClaim";
            if (_memoryCache.TryGetValue(cacheKey, out BranchClaim branchClaim))
            {
                string name = branchClaim.BranchName;

                var identity = new ClaimsIdentity(User.Identity);
                identity.AddClaim(new Claim(ClaimNames.CurrentBranch, branchClaim.BranchId.ToString()));
                identity.AddClaim(new Claim(ClaimNames.CurrentBranchName, branchClaim.BranchName.ToString().Trim()));

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties() { IssuedUtc = DateTimeOffset.Now, });

            }
            //End of Branch


            //string userName = User.Identity.Name;
            //List<UserManuInfo> manu = _userRollsService.GetUserManu(userName);
            //ViewBag.ManuList = manu;
            return View();
        }

        public IActionResult _feedBackApproveStatusIndex()
        {
            try
            {
                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                index.UserName = userName;
                ApplicationUser user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                var search = Request.Form["search[value]"].FirstOrDefault();
                string users = Request.Form["BranchCode"].ToString();
                string branch = Request.Form["BranchName"].ToString();
                string Address = Request.Form["Address"].ToString();
                string TelephoneNo = Request.Form["TelephoneNo"].ToString();



                string code = Request.Form["code"].ToString();
                string name = Request.Form["name"].ToString();
                string description = Request.Form["description"].ToString();
                string approveStatus = Request.Form["approveStatus"].ToString();


                string post = Request.Form["ispost"].ToString();


                if (post == "Select")
                {
                    post = "";

                }



                string draw = Request.Form["draw"].ToString();
                var startRec = Request.Form["start"].FirstOrDefault();
                var pageSize = Request.Form["length"].FirstOrDefault();
                var orderName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][Name]"].FirstOrDefault();

                var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();

                index.SearchValue = Request.Form["search[value]"].FirstOrDefault();

                index.OrderName = "Id";

                index.orderDir = orderDir;
                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);


                index.createdBy = userName;


                string[] conditionalFields = new[]
                {
                            "Code like",
                            "Name like",
                            "ad.IsPost "

                };

                string?[] conditionalValue = new[] { code, name, "Y" };

                ResultModel<List<AuditMaster>> indexData =
                    _auditMasterService.FeedBackGetIndexDataStatus(index, conditionalFields, conditionalValue);



                ResultModel<int> indexDataCount =
                _auditMasterService.GetAuditFeedBackDataCount(index, conditionalFields, conditionalValue);


                int result = _auditMasterService.GetCount(TableName.A_Audits, "Id", new[] { "A_Audits.createdBy", }, new[] { userName });


                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditMaster>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }

        }


        public ActionResult MultiplePost(AuditMaster master)
        {
            ResultModel<AuditMaster> result = new ResultModel<AuditMaster>();

            try
            {

                foreach (string ID in master.IDs)
                {


                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);
                    master.Audit.PostedBy = user.UserName;
                    master.Audit.PostedOn = DateTime.Now;
                    master.Audit.PostedFrom = HttpContext.Connection.RemoteIpAddress.ToString();
                    master.Operation = "post";

                    //string stringValue = ID;
                    //master.Id = int.Parse(stringValue);


                    result = _auditMasterService.MultiplePost(master);


                }




                return Ok(result);


            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok("");
        }


        public ActionResult MultipleUnPost(AuditMaster master)
        {
            ResultModel<AuditMaster> result = new ResultModel<AuditMaster>();

            try
            {

                foreach (string ID in master.IDs)
                {

                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);
                    master.Audit.PostedBy = user.UserName;
                    master.Audit.PostedOn = DateTime.Now;
                    master.Audit.PostedFrom = HttpContext.Connection.RemoteIpAddress.ToString();
                    master.Operation = "unpost";
                    result = _auditMasterService.MultipleUnPost(master);
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok("");
        }

        public ActionResult MultipleReject(AuditMaster master)
        {
            ResultModel<AuditMaster> result = new ResultModel<AuditMaster>();

            try
            {

                foreach (string ID in master.IDs)
                {

                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);
                    master.Approval.IsRejected = true;
                    master.Approval.RejectedBy = user.UserName;
                    master.Approval.RejectedDate = DateTime.Now;

                    //master.Approval.RejectedComments = DateTime.Now;
                    master.Operation = "reject";
                    result = _auditMasterService.MultipleUnPost(master);
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok("");
        }


        public ActionResult MultipleApproved(AuditMaster master)
        {
            ResultModel<AuditMaster> result = new ResultModel<AuditMaster>();
            //ResultModel<UserProfile> result = new ResultModel<UserProfile>();

            try
            {

                foreach (string ID in master.IDs)
                {

                    string userName = User.Identity.Name;
                    ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);
                    master.Audit.PostedBy = user.UserName;
                    master.Audit.PostedOn = DateTime.Now;
                    master.Audit.PostedFrom = HttpContext.Connection.RemoteIpAddress.ToString();

                    //master.Approval.RejectedComments = DateTime.Now;
                    master.Operation = "approved";


                    UserProfile urp = new UserProfile();
                    urp.Id = master.Id;
                    urp.UserName = userName;
                    MailSetting mailSetting = new MailSetting();
                    mailSetting.Id = master.Id;

                    var notifiEmai = _auditMasterService.GetEamil(urp);

                    var email = notifiEmai.Data.FirstOrDefault();


                    var AuditUrl = _auditMasterService.GetUrl(mailSetting);

                    var url = AuditUrl.Data.FirstOrDefault();
                    if (email != null)
                    {
                        result = _auditMasterService.MultipleUnPost(master);
                        MailService.SendAuditApprovalMail(email.Email, url.ApprovedUrl);
                    }


                    //result.Data.Edit = email.Email;
                    //result.Data.Url = url.ApprovedUrl;



                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            //return Ok("");
            return Ok(result);
        }





        //end of change






        [HttpPost]
        public ActionResult<IList<AuditMaster>> _index()
        {
            try
            {
                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);



                string? search = Request.Form["search[value]"].FirstOrDefault();



                string draw = Request.Form["draw"].ToString();
                var startRec = Request.Form["start"].FirstOrDefault();
                var pageSize = Request.Form["length"].FirstOrDefault();
                var orderName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][Name]"].FirstOrDefault();
                var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();


                string code = Request.Form["code"].ToString();
                string name = Request.Form["name"].ToString();
                string startdate = Request.Form["startdate"].ToString();
                string enddate = Request.Form["enddate"].ToString();
                string ispost = Request.Form["ispost"].ToString();

                if (ispost == "Select")
                {
                    ispost = "";

                }



                index.SearchValue = Request.Form["search[value]"].FirstOrDefault();
                index.OrderName = orderName;
                index.orderDir = orderDir;
                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);

                index.createdBy = userName;
                index.CurrentBranchid = User.GetCurrentBranchId(); ;


                string[] conditionalFields = new[]
                {
                    "ad.[Code] like",
                    "ad.[Name] like",
                    "ad.[StartDate] like",
                    "ad.[EndDate] like",
                    "ad.[IsPost] like"
                };
                string?[] conditionalValue = new[] { code, name, startdate, enddate, ispost };


                ResultModel<List<AuditMaster>> indexData =
                    _auditMasterService.GetIndexData(index,
                        conditionalFields, conditionalValue);


                ResultModel<int> indexDataCount =
                    _auditMasterService.GetIndexDataCount(index,
                        conditionalFields, conditionalValue);

                int result = _auditMasterService.GetCount(TableName.A_Audits, "Id", null, null);

                // data, draw, recordsTotal = TotalReocrd, recordsFiltered = data.Count
                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditMaster>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }
        }






        [HttpPost]
        public ActionResult<IList<AuditAreas>> _indexArea(int id)
        {
            try
            {
                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                string? search = Request.Form["search[value]"].FirstOrDefault();

                string draw = Request.Form["draw"].ToString();
                var startRec = Request.Form["start"].FirstOrDefault();
                var pageSize = Request.Form["length"].FirstOrDefault();
                var orderName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][Name]"].FirstOrDefault();

                var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();

                index.SearchValue = Request.Form["search[value]"].FirstOrDefault();
                //index.OrderName = orderName;
                index.OrderName = "Id";
                //index.orderDir = orderDir;
                index.orderDir = "desc";
                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);

                index.createdBy = userName;
                index.AuditId = id;


                string[] conditionalFields = {
                    "AuditArea like"
                };
                string?[] conditionalValue = { search };


                ResultModel<List<AuditAreas>> indexData =
                    _auditAreasService.GetIndexData(index,
                        conditionalFields, conditionalValue);


                ResultModel<int> indexDataCount =
                    _auditAreasService.GetIndexDataCount(index,
                        conditionalFields, conditionalValue);

                int result = _auditMasterService.GetCount(TableName.A_AuditAreas, "Id", null, null);

                // data, draw, recordsTotal = TotalReocrd, recordsFiltered = data.Count
                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditAreas>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }
        }





        [HttpPost]
        public ActionResult<IList<AuditUser>> _indexAuditUser(int id)
        {
            try
            {
                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                string? search = Request.Form["search[value]"].FirstOrDefault();

                string draw = Request.Form["draw"].ToString();
                var startRec = Request.Form["start"].FirstOrDefault();
                var pageSize = Request.Form["length"].FirstOrDefault();
                var orderName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][Name]"].FirstOrDefault();

                var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();

                index.SearchValue = Request.Form["search[value]"].FirstOrDefault();

                //index.OrderName = orderName;
                //index.orderDir = orderDir;

                index.OrderName = "Id";
                index.orderDir = "desc";

                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);

                index.createdBy = userName;
                index.AuditId = id;


                string[] conditionalFields = {
                    "EmailAddress like",
                    "usr.UserName like"
                };
                string?[] conditionalValue = { search, search };


                ResultModel<List<AuditUser>> indexData =
                    _auditUserService.GetIndexData(index,
                        conditionalFields, conditionalValue);


                ResultModel<int> indexDataCount =
                    _auditAreasService.GetIndexDataCount(index,
                        conditionalFields, conditionalValue);

                int result = _auditMasterService.GetCount(TableName.AuditUsers, "Id", null, null);

                // data, draw, recordsTotal = TotalReocrd, recordsFiltered = data.Count
                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditUser>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }
        }





        [HttpPost]
        public ActionResult<IList<AuditIssueUser>> _indexAuditIssueUser(int id)
        {
            try
            {
                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                string? search = Request.Form["search[value]"].FirstOrDefault();

                string draw = Request.Form["draw"].ToString();
                var startRec = Request.Form["start"].FirstOrDefault();
                var pageSize = Request.Form["length"].FirstOrDefault();
                var orderName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][Name]"].FirstOrDefault();

                var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();

                index.SearchValue = Request.Form["search[value]"].FirstOrDefault();
                //index.OrderName = orderName;
                //index.orderDir = orderDir;
                index.OrderName = "Id";
                index.orderDir = "desc";
                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);

                index.createdBy = userName;
                index.AuditIssueId = id;


                string[] conditionalFields = {
                    "EmailAddress like"
                };
                string?[] conditionalValue = { search };


                ResultModel<List<AuditIssueUser>> indexData =
                    _auditIssueUserService.GetIndexData(index,
                        conditionalFields, conditionalValue);


                ResultModel<int> indexDataCount =
                    _auditAreasService.GetIndexDataCount(index,
                        conditionalFields, conditionalValue);

                int result = _auditMasterService.GetCount(TableName.AuditUsers, "Id", null, null);

                // data, draw, recordsTotal = TotalReocrd, recordsFiltered = data.Count
                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditIssueUser>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }
        }



        //Audit Response
        [HttpPost]
        public ActionResult<IList<AuditResponse>> _indexAuditResponse()
        {
            try
            {
                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);



                string? search = Request.Form["search[value]"].FirstOrDefault();



                string draw = Request.Form["draw"].ToString();
                var startRec = Request.Form["start"].FirstOrDefault();
                var pageSize = Request.Form["length"].FirstOrDefault();
                var orderName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][Name]"].FirstOrDefault();
                var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();


                string code = Request.Form["code"].ToString();
                string name = Request.Form["name"].ToString();
                string startdate = Request.Form["startdate"].ToString();
                string enddate = Request.Form["enddate"].ToString();
                string ispost = Request.Form["ispost"].ToString();

                if (ispost == "Select")
                {
                    ispost = "";

                }



                index.SearchValue = Request.Form["search[value]"].FirstOrDefault();
                index.OrderName = "Code";
                index.orderDir = orderDir;
                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);

                index.createdBy = userName;
                index.CurrentBranchid = User.GetCurrentBranchId(); ;


                string[] conditionalFields = new[]
                {
                    "A.Name like",
                    "AI.IssueName like",
                    "AI.DateOfSubmission like",
                    "E.EnumValue like"
                };
                string?[] conditionalValue = new[] { search, search, search, search };


                ResultModel<List<AuditResponse>> indexData = _auditMasterService.AuditResponseGetIndexData(index,
                    conditionalFields, conditionalValue);


                ResultModel<int> indexDataCount = _auditMasterService.GetIndexDataCount(index,
                    conditionalFields, conditionalValue);

                int result = _auditMasterService.GetCount(TableName.A_Audits, "Id", null, null);


                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditMaster>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }
        }




        [HttpPost]
        public ActionResult AuditAreaModal(AuditAreas auditAreas)
        {

            ModelState.Clear();

            string edit = auditAreas.Edit;

            if (auditAreas.Id != 0)
            {
                ResultModel<List<AuditAreas?>> result =
                    _auditAreasService.GetAll(new[] { "Id" }, new[] { auditAreas.Id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                auditAreas = result.Data.FirstOrDefault();

                //auditAreas.AreaDetails = Convert.FromBase64String(auditAreas.AreaDetails).ToString();

                auditAreas.Operation = "update";
            }

            auditAreas.Edit = edit;

            return PartialView("_AreaDetail", auditAreas);
        }



        [HttpPost]
        //public ActionResult AuditStatusModal(AuditIssue AuditIssue)
        public ActionResult AuditStatusModal(AuditMaster master)
        {
            ModelState.Clear();

            string edit = master.Edit;

            if (master.Id != 0)
            {
                ResultModel<List<AuditMaster>> result =
                    _auditMasterService.GetAll(new[] { "Id" }, new[] { master.Id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                master = result.Data.FirstOrDefault();

                //master.AttachmentsList = _auditIssueAttachmentsService
                //   .GetAll(new[] { "AuditIssueId" }, new[] { master.Id.ToString() }).Data;

                //master.ReportStatusModal = master.ReportStatus;

                master.Operation = "update";
                master.BranchIDStatus = master.BranchID;
            }


            master.Edit = edit;


            return PartialView("_AuditStatus", master);
        }
        [HttpPost]
        public ActionResult AuditIssueModal(AuditIssue AuditIssue)
        {
            ModelState.Clear();

            string edit = AuditIssue.Edit;

            if (AuditIssue.Id != 0)
            {
                ResultModel<List<AuditIssue?>> result =
                    _auditIssueService.GetAll(new[] { "Id" }, new[] { AuditIssue.Id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                AuditIssue = result.Data.FirstOrDefault();

                AuditIssue.AttachmentsList = _auditIssueAttachmentsService
                   .GetAll(new[] { "AuditIssueId" }, new[] { AuditIssue.Id.ToString() }).Data;

                AuditIssue.Operation = "update";
            }


            AuditIssue.Edit = edit;


            return PartialView("_AuditIssue", AuditIssue);
        }


        [HttpPost]
        public ActionResult AuditReportStatusModal(AuditIssue master)
        {
            ModelState.Clear();

            string edit = master.Edit;

            if (master.Id != 0)
            {
                ResultModel<List<AuditIssue>> result =
                    _auditIssueService.GetAll(new[] { "Id" }, new[] { master.Id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                master = result.Data.FirstOrDefault();

                master.IssueDetailsUpdate = master.IssueDetails;
                master.IssuePriorityUpdate = master.IssuePriority;

                //master.AttachmentsList = _auditIssueAttachmentsService
                //   .GetAll(new[] { "AuditIssueId" }, new[] { master.Id.ToString() }).Data;

                //master.ReportStatusModal = master.ReportStatus;
                master.Operation = "update";
                master.ReportStatusModal = master.ReportStatus;
            }


            master.Edit = edit;


            return PartialView("_AuditReportStatus", master);
        }




        [HttpPost]
        public ActionResult AuditFeedbackModal(AuditFeedback auditFeedback)
        {
            ModelState.Clear();

            string edit = auditFeedback.Edit;

            if (auditFeedback.Id != 0)
            {
                ResultModel<List<AuditFeedback?>> result =
                    //_auditFeedbackService.GetAll(new[] { "Id" }, new[] { auditFeedback.Id.ToString() });
                    _auditFeedbackService.GetAll(new[] { "af.Id" }, new[] { auditFeedback.Id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                auditFeedback = result.Data.FirstOrDefault();
                auditFeedback.FeedbackDetails = auditFeedback.IssueDetails;
                auditFeedback.AttachmentsList = _auditFeedbackAttachmentsService
                    .GetAll(new[] { "AuditFeedbackId" }, new[] { auditFeedback.Id.ToString() }).Data;





                auditFeedback.Operation = "update";
            }
            auditFeedback.Edit = edit;


            return PartialView("_AuditIssueFeedback", auditFeedback);
        }


        [HttpPost]
        public ActionResult AuditBranchFeedbackModal(AuditBranchFeedback AuditBranchFeedback)
        {
            ModelState.Clear();

            string edit = AuditBranchFeedback.Edit;

            if (AuditBranchFeedback.Id != 0)
            {
                ResultModel<List<AuditBranchFeedback?>> result =
                    _auditBranchFeedbackService.GetAll(new[] { "Id" }, new[] { AuditBranchFeedback.Id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                AuditBranchFeedback = result.Data.FirstOrDefault();


                //change
                AuditBranchFeedback.AuditBranchIssueId = AuditBranchFeedback.AuditIssueId;

                //change of _audit and id name

                AuditBranchFeedback.AttachmentsList = _auditBranchFeedbackAttachmentsService
                    .GetAll(new[] { "AuditBranchFeedbackId" }, new[] { AuditBranchFeedback.Id.ToString() }).Data;

                AuditBranchFeedback.Operation = "update";
            }
            AuditBranchFeedback.Edit = edit;


            return PartialView("_AuditIssueBranchFeedback", AuditBranchFeedback);
        }




        [HttpPost]
        public ActionResult AuditUserModal(AuditUser auditUser)
        {
            ModelState.Clear();
            string edit = auditUser.Edit;


            if (auditUser.Id != 0)
            {
                ResultModel<List<AuditUser?>> result =
                    _auditUserService.GetAll(new[] { "Id" }, new[] { auditUser.Id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                auditUser = result.Data.FirstOrDefault();

                auditUser.Operation = "update";
            }

            auditUser.Edit = edit;

            return PartialView("_AuditUser", auditUser);
        }

        [HttpPost]
        public ActionResult AuditIssueUserModal(AuditIssueUser auditIssueUser)
        {
            ModelState.Clear();
            string edit = auditIssueUser.Edit;


            if (auditIssueUser.Id != 0)
            //if (auditIssueUser.AuditId != 0)
            {
                ResultModel<List<AuditIssueUser?>> result =
                    _auditIssueUserService.GetAll(new[] { "Id" }, new[] { auditIssueUser.Id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                auditIssueUser = result.Data.FirstOrDefault();

                auditIssueUser.Operation = "update";
            }

            auditIssueUser.Edit = edit;

            return PartialView("_AuditIssueUser", auditIssueUser);
        }




        [HttpGet]
        public ActionResult GetUserInfo(string userId)
        {

            ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.Id == userId);

            return Ok(new { user.Email });
        }








    }
}
