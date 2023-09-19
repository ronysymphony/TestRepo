using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SageERP.ExtensionMethods;
using Shampan.Core.Interfaces.Services.Audit;
using Shampan.Core.Interfaces.Services.AuditIssues;
using Shampan.Models;
using Shampan.Models.AuditModule;
using Shampan.Services.Audit;
using ShampanERP.Models;
using ShampanERP.Persistence;
using StackExchange.Exceptional;

namespace SSLAudit.Controllers.Audit
{
	[ServiceFilter(typeof(UserMenuActionFilter))]

	public class AuditIssueController : Controller
    {
        private readonly ApplicationDbContext _applicationDb;
        private readonly IAuditIssueService _auditIssueService;
        private readonly IAuditMasterService _auditMasterService;
        private readonly IAuditIssueAttachmentsService _auditIssueAttachmentsService;
		private readonly IAuditIssueUserService _auditIssueUserService;


		public AuditIssueController(ApplicationDbContext applicationDb, IAuditIssueUserService auditIssueUserService, IAuditIssueService auditIssueService, IAuditIssueAttachmentsService auditIssueAttachmentsService, IAuditMasterService auditMasterService)
        {
            _applicationDb = applicationDb;
            _auditIssueService = auditIssueService;
            _auditIssueAttachmentsService = auditIssueAttachmentsService;
            _auditMasterService = auditMasterService;
			_auditIssueUserService = auditIssueUserService;
        }

        public IActionResult Index(int? id)
        {
            if (id is null || id == 0)
            {
                return RedirectToAction("Index", "Audit");
            }

            AuditIssue auditIssue = new AuditIssue()
            {
                AuditId = id.Value
            };

            var auditMaster = _auditMasterService.GetAll(new[] { "Id" }, new[] { id.Value.ToString() }).Data;

            if (auditMaster != null && auditMaster.Count > 0)
            {
                auditIssue.AuditMaster = auditMaster.FirstOrDefault();
            }

            return View(auditIssue);
        }



        public IActionResult Create(int? id)
        {

            if (id is null || id == 0)
            {
                return RedirectToAction("Index", "Audit");
            }

            AuditIssue auditIssue = new AuditIssue()
            {
                Operation = "add",
                AuditId = id.Value
            };
            return View(auditIssue);
        }



        [HttpPost]
        public ActionResult CreateEdit(AuditIssue master)
        {
            ResultModel<AuditIssue> result = new ResultModel<AuditIssue>();
            try
            {
                string userName = User.Identity.Name;
                ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                if (master.Operation == "update")
                {

                    master.Audit.LastUpdateBy = user.UserName;
                    master.Audit.LastUpdateOn = DateTime.Now;
                    master.Audit.LastUpdateFrom = "";

                    result = _auditIssueService.Update(master);

                    //if (result.Status == Status.Fail)
                    //{
                    //    throw result.Exception;
                    //}


                    return Ok(result);
                }
                else
                {

                    master.Audit.CreatedBy = user.UserName;
                    master.Audit.CreatedOn = DateTime.Now;
                    master.Audit.CreatedFrom = "";
                    master.Audit.LastUpdateBy = user.UserName;
                    master.Audit.LastUpdateOn = DateTime.Now;
                    master.Audit.LastUpdateFrom = "";
                    result = _auditIssueService.Insert(master);

					//if (result.Status == Status.Fail)
					//{
					//    throw result.Exception;
					//}

					//save link of audit address

					//https://localhost:7031/Audit/Edit/28?edit=issue

					//Uri returnurl = new Uri(Request.Host.ToString());
					//string urlString = returnurl.ToString();
					//string Url = "https://" + urlString + "/Audit/Edit/" + result.Data.Id + "?edit=issueApprove";


					//MailSetting ms = new MailSetting();
					//ms.ApprovedUrl = Url;
					//ms.AuditIssueId = result.Data.Id;
					//_auditMasterService.SaveUrl(ms);


					//end




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
        public ActionResult<IList<AuditIssue>> _index(int? id)
        {
            try
            {
                //if (id is null || id == 0)
                //{
                //    return  Ok(new { Data = new List<AuditIssue>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
                //}


                IndexModel index = new IndexModel();
                string userName = User.Identity.Name;
                ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);
                string? search = Request.Form["search[value]"].FirstOrDefault();




                string issuename = Request.Form["issuename"].ToString();
                string issuepriority = Request.Form["issuepriority"].ToString();
                string dateofsubmission = Request.Form["dateofsubmission"].ToString();




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
                index.AuditId = id.Value;



                string[] conditionalFields = new[]
                {
                    "IssueName like",
                    "Enums.EnumValue like",
                    "DateOfSubmission like"
                };
                string?[] conditionalValue = new[] { issuename, issuepriority, dateofsubmission };


                ResultModel<List<AuditIssue>> indexData =
                    _auditIssueService.GetIndexData(index,
                        conditionalFields, conditionalValue);


                ResultModel<int> indexDataCount =
                    _auditIssueService.GetIndexDataCount(index,
                        conditionalFields, conditionalValue);

                int result = _auditIssueService.GetCount(TableName.A_AuditIssues, "Id", new[]
                        {
                            "createdBy",
                        }, new[] { userName });

                // data, draw, recordsTotal = TotalReocrd, recordsFiltered = data.Count
                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditIssue>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }
        }




        public ActionResult<IList<AuditIssue>> Edit(int id)
        {
            try
            {
                ResultModel<List<AuditIssue?>> result =
                    _auditIssueService.GetAll(new[] { "Id" }, new[] { id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                AuditIssue? auditMaster = result.Data.FirstOrDefault();

                auditMaster.AttachmentsList  = _auditIssueAttachmentsService.GetAll(new[] {"AuditIssueId"}, new[] {id.ToString()}).Data;

                auditMaster.Operation = "update";

                return View("Create", auditMaster);
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return RedirectToAction("Index");
            }
        }


		public ActionResult<IList<AuditIssueUser>> Delete(AuditIssueUser master)
		{
			try
			{

                ResultModel<AuditIssueUser> result = _auditIssueUserService.Delete(master.Id);
                //ResultModel<AuditMaster> result = new ResultModel<AuditMaster>();

				return Ok(result);

			}
			catch (Exception e)
			{
				e.LogAsync(ControllerContext.HttpContext);
				return RedirectToAction("Index");
			}
		}

		public ActionResult MultiplePost(AuditIssue master)
        {
            ResultModel<AuditIssue> result = new ResultModel<AuditIssue>();

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
                    result = _auditIssueService.MultiplePost(master);


                }




                return Ok(result);


            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok("");
        }

        public ActionResult MultipleUnPost(AuditIssue master)
        {
            ResultModel<AuditIssue> result = new ResultModel<AuditIssue>();

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
					result = _auditIssueService.MultipleUnPost(master);
				}


				return Ok(result);
            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok("");
        }







        [HttpPost]
        public IActionResult DeleteFile(string filePath, string id)
        {
            string saveDirectory = "wwwroot\\files";

            ResultModel<AuditIssueAttachments> result = new ResultModel<AuditIssueAttachments>
            {
                Message = "File could not be deleted"
            };

            try
            {
                var path = Path.Combine(saveDirectory, filePath);
                if (!System.IO.File.Exists(path)) return Ok(result);

                result = _auditIssueAttachmentsService.Delete(Convert.ToInt32(id.Replace("file-", "")));

                if (result.Status == Status.Success)
                {
                    System.IO.File.Delete(path);
                }


                return Ok(result);
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);

                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<IActionResult> DownloadFile(string filePath)
        {
            string saveDirectory = "wwwroot\\files";

            try
            {
                var path = Path.Combine(saveDirectory, filePath);
                var memory = new MemoryStream();
                await using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var ext = Path.GetExtension(path).ToLowerInvariant();
                return File(memory, GetMimeType(ext), Path.GetFileName(path));
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);

                return RedirectToAction("Index");
            }
        }


        private string GetMimeType(string ext)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(ext, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

    }
}
