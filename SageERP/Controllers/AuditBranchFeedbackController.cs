﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Shampan.Core.Interfaces.Repository.AuditFeedbackRepo;
using Shampan.Core.Interfaces.Services.Audit;
using Shampan.Core.Interfaces.Services.AuditFeedbackService;
using Shampan.Core.Interfaces.Services.AuditIssues;
using Shampan.Models;
using Shampan.Models.AuditModule;
using Shampan.Services.Audit;
using Shampan.Services.AuditFeedbackService;
using Shampan.Services.AuditIssues;
using ShampanERP.Models;
using ShampanERP.Persistence;
using StackExchange.Exceptional;

namespace SSLAudit.Controllers
{
	[ServiceFilter(typeof(UserMenuActionFilter))]

	public class AuditBranchFeedbackController : Controller
    {

        private readonly ApplicationDbContext _applicationDb;
        private readonly IAuditFeedbackService _auditFeedbackService;
        private readonly IAuditBranchFeedbackService _auditBranchFeedbackService;
        private readonly IAuditFeedbackAttachmentsService _auditFeedbackAttachmentsService;
        private readonly IAuditBranchFeedbackAttachmentsService _auditBranchFeedbackAttachmentsService;
		private readonly IAuditMasterService _auditMasterService;


		public AuditBranchFeedbackController(ApplicationDbContext applicationDb, IAuditBranchFeedbackService auditBranchFeedbackService,
            IAuditFeedbackAttachmentsService auditFeedbackAttachmentsService, IAuditBranchFeedbackAttachmentsService
             auditBranchFeedbackAttachmentsService, IAuditMasterService auditMasterService)
        {
            _applicationDb = applicationDb;
            //_auditFeedbackService = auditFeedbackService;
            _auditBranchFeedbackService = auditBranchFeedbackService;
            _auditFeedbackAttachmentsService = auditFeedbackAttachmentsService;
            _auditBranchFeedbackAttachmentsService = auditBranchFeedbackAttachmentsService;
			_auditMasterService = auditMasterService;

		}



		public IActionResult Index(int? id)
        {
            if (id is null || id == 0)
            {
                return RedirectToAction("Index", "Audit");
            }

            AuditBranchFeedback auditIssue = new AuditBranchFeedback()
            {
                AuditId = id.Value
            };

            return View(auditIssue);
        }

        public IActionResult Create(int? id)
        {

            if (id is null || id == 0)
            {
                return RedirectToAction("Index", "Audit");
            }

            AuditBranchFeedback auditIssue = new AuditBranchFeedback()
            {
                Operation = "add",
                AuditId = id.Value
            };
            return View(auditIssue);
        }


        [HttpPost]
        public ActionResult CreateEdit(AuditBranchFeedback master)
        {
            ResultModel<AuditBranchFeedback> result = new ResultModel<AuditBranchFeedback>();
            try
            {
                master.AuditIssueId = master.AuditBranchIssueId;


                string userName = User.Identity.Name;
                ApplicationUser? user = _applicationDb.Users.FirstOrDefault(model => model.UserName == userName);

                if (master.Operation == "update")
                {

                    master.Audit.LastUpdateBy = user.UserName;
                    master.Audit.LastUpdateOn = DateTime.Now;
                    master.Audit.LastUpdateFrom = "";

                    result = _auditBranchFeedbackService.Update(master);

                    //if (result.Status == Status.Fail)
                    //{
                    //    throw result.Exception;
                    //}


                    return Ok(result);
                }


                master.Audit.CreatedBy = user.UserName;
                master.Audit.CreatedOn = DateTime.Now;
                master.Audit.CreatedFrom = "";
                master.Audit.LastUpdateBy = user.UserName;
                master.Audit.LastUpdateOn = DateTime.Now;
                master.Audit.LastUpdateFrom = "";

				


				result = _auditBranchFeedbackService.Insert(master);

				//if (result.Status == Status.Fail)
				//{
				//    throw result.Exception;
				//}

				//https://localhost:7031/Audit/Edit/32?edit=branchFeedbackApprove
				Uri returnurl = new Uri(Request.Host.ToString());
				string urlString = returnurl.ToString();
				string Url = "https://" + urlString + "/Audit/Edit/" + result.Data.Id + "?edit=branchFeedbackApprove";
				MailSetting ms = new MailSetting();
				ms.ApprovedUrl = Url;
				ms.BranchFeedbackId = result.Data.Id;
				//_auditMasterService.SaveUrl(ms);




				return Ok(result);


            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }

        public ActionResult<IList<AuditFeedback>> Edit(int id)
        {
            try
            {
                ResultModel<List<AuditBranchFeedback?>> result =
                    _auditBranchFeedbackService.GetAll(new[] { "Id" }, new[] { id.ToString() });

                if (result.Status == Status.Fail)
                {
                    throw result.Exception;
                }

                AuditBranchFeedback? auditMaster = result.Data.FirstOrDefault();

                //change of _audit and id name

                auditMaster.AttachmentsList = _auditBranchFeedbackAttachmentsService
                    .GetAll(new[] { "AuditBranchFeedbackId" }, new[] {id.ToString()}).Data;

                auditMaster.Operation = "update";

                return View("Create", auditMaster);
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public ActionResult<IList<AuditBranchFeedback>> _index(int? id)
        {
            try
            {
                //if (id is null || id == 0)
                //{
                //    return Ok(new { Data = new List<AuditFeedback>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
                //}


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
                index.AuditId = id.Value;



                string[] conditionalFields = new[]
                {
                    "ai.IssueName like",
                    "ad.Name like",
					"af.Heading like",
					"af.IsPost like"
				};
                string?[] conditionalValue = new[] { search, search,search,search };


                ResultModel<List<AuditBranchFeedback>> indexData =
                    _auditBranchFeedbackService.GetIndexData(index,
                conditionalFields, conditionalValue);


                ResultModel<int> indexDataCount =
                    _auditBranchFeedbackService.GetIndexDataCount(index,
                conditionalFields, conditionalValue);

                int result = _auditBranchFeedbackService.GetCount(TableName.A_Audits, "Id", null,null);

                // data, draw, recordsTotal = TotalReocrd, recordsFiltered = data.Count
                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditIssue>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }
        }

        [HttpPost]
        public ActionResult<IList<AuditBranchFeedback>> _indexBranchFeedback(int? id)
        {
            try
            {
                //if (id is null || id == 0)
                //{
                //    return Ok(new { Data = new List<AuditFeedback>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
                //}


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
                index.OrderName = orderName;
                index.orderDir = orderDir;
                index.startRec = Convert.ToInt32(startRec);
                index.pageSize = Convert.ToInt32(pageSize);

                index.createdBy = userName;
                index.AuditId = id.Value;



                string[] conditionalFields = new[]
                {
                    "ai.IssueName like",
                    "ad.Name like"
                };
                string?[] conditionalValue = new[] { search, search };


                ResultModel<List<AuditBranchFeedback>> indexData =
                    _auditBranchFeedbackService.GetIndexData(index,
                conditionalFields, conditionalValue);


                ResultModel<int> indexDataCount =
                    _auditFeedbackService.GetIndexDataCount(index,
                conditionalFields, conditionalValue);

                int result = _auditFeedbackService.GetCount(TableName.A_Audits, "Id", null, null);

                // data, draw, recordsTotal = TotalReocrd, recordsFiltered = data.Count
                return Ok(new { data = indexData.Data, draw, recordsTotal = result, recordsFiltered = indexDataCount.Data });
            }
            catch (Exception e)
            {
                e.LogAsync(ControllerContext.HttpContext);
                return Ok(new { Data = new List<AuditIssue>(), draw = "", recordsTotal = 0, recordsFiltered = 0 });
            }
        }



        public ActionResult MultiplePost(AuditBranchFeedback master)
        {
            ResultModel<AuditBranchFeedback> result = new ResultModel<AuditBranchFeedback>();

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
                    result = _auditBranchFeedbackService.MultiplePost(master);


                }




                return Ok(result);


            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok("");
        }


        public ActionResult MultipleUnPost(AuditBranchFeedback master)
        {
            ResultModel<AuditBranchFeedback> result = new ResultModel<AuditBranchFeedback>();

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
                    result = _auditBranchFeedbackService.MultipleUnPost(master);
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

            ResultModel<AuditBranchFeedbackAttachments> result = new ResultModel<AuditBranchFeedbackAttachments>
            {
                Message = "File could not be deleted"
            };

            try
            {
                var path = Path.Combine(saveDirectory, filePath);
                if (!System.IO.File.Exists(path)) return Ok(result);

                //result = _auditFeedbackAttachmentsService.Delete(Convert.ToInt32(id.Replace("file-", "")));
                result = _auditBranchFeedbackAttachmentsService.Delete(Convert.ToInt32(id.Replace("file-", "")));

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
