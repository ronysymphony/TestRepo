using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shampan.Core.Interfaces.Services;
using Shampan.Core.Interfaces.Services.Branch;
using Shampan.Core.Interfaces.Services.Company;
using Shampan.Core.Interfaces.Services.User;
using Shampan.Models;
using ShampanERP.Models;
using ShampanERP.Persistence;
using SSLAudit.Controllers;
using StackExchange.Exceptional;
using System.Security.Claims;

namespace SageERP.Controllers
{
	[ServiceFilter(typeof(UserMenuActionFilter))]
	[Authorize]
	public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _applicationDb;

        private readonly ICommonService _commonService;
        private readonly DbConfig _dbConfig;
        public UserProfileController(ApplicationDbContext applicationDb,
            ICommonService commonService,
              UserManager<ApplicationUser> userManager, DbConfig dbConfig)
        {
            _userManager = userManager;
            _applicationDb = applicationDb;

            _commonService = commonService;
            _dbConfig = dbConfig;

        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {

            ModelState.Clear();
            UserProfile vm = new UserProfile();
            vm.Operation = "add";
            return View("CreateEdit", vm);
        }
        public IActionResult Edit(string id)
        {
            var user = _userManager.Users.SingleOrDefault(x => x.Id == id);
            UserProfile vm = new UserProfile();
            vm.UserName = user.UserName;
            //vm.UserName = user.UserName;

            vm.Operation = "update";
            return View("ChangePassword", vm);
        }

        public IActionResult _index(string userName)
        {
            var search = Request.Form["search[value]"].FirstOrDefault();
            var users = _userManager.Users;


            if (!string.IsNullOrEmpty(userName))
            {
                users = users.Where(u => u.UserName.Contains(userName));
            }
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.UserName.Contains(search));
            }

            var result = users.Count(); // Get the total number of records
            var namesList = users.ToList();
            List<ApplicationUser> data = namesList;

            string draw = Request.Form["draw"].ToString();

            return Ok(new { data = data, draw = draw, recordsTotal = result, recordsFiltered = result });
        }


        public async Task<ActionResult> CreateEditAsync(UserProfile model)
        {
            ResultModel<UserProfile> result = new ResultModel<UserProfile>();
            try
            {
                string SageDbName = _dbConfig.SageDbName;
                var claims = new List<Claim>
              {
                //new Claim("Database", "SageSSL"),
                new Claim("Database", "SSLAuditDB"),
                new Claim("SageDatabase", SageDbName),
                //add
                //new Claim("AuthDatabase", "SSLAuditAuthDB"),

              };
              if (model.Operation == "update")
              {


                    var user = await _userManager.FindByNameAsync(model.UserName);

                    if (user == null)
                    {
                        result.Message = "User not found.";
                        return Ok(result);
                    }
                    if (string.IsNullOrEmpty(model.Password) && string.IsNullOrEmpty(model.ConfirmPassword))
                    {
                        // Update the user profile without changing the password
                        user.Email = model.Email;
                        user.PhoneNumber = model.PhoneNumber;
                        user.SageUserName = model.SageUserName;
                        user.PFNo = model.PFNo;
                        user.Designation = model.Designation;

                        var updateResult = await _userManager.UpdateAsync(user);
                        if (!updateResult.Succeeded)
                        {
                            result.Message = "Failed to update user profile.";
                            return Ok(result);
                        }

                        result.Status = Status.Success;
                        result.Message = "User profile updated successfully.";
                        result.Data = model;
                        return Ok(result);
                    }

                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);

                    if (!changePasswordResult.Succeeded)
                    {
                        result.Message = "Failed to change the password.";
                        return Ok(result);
                    }

                    result.Status = Status.Success;
                    result.Message = "Password successfully updated.";
                    result.Data = model;


                    return Ok(result);
              }
                else
                {

                    if (model.Password != model.ConfirmPassword)
                    {
                        result.Message = "Passwords do not match.";
                        return Ok(result);
                    }

                    var _user = new ApplicationUser { UserName = model.UserName, PhoneNumber = model.PhoneNumber, Email = model.Email, SageUserName = model.SageUserName, Designation =model.Designation,PFNo=model.PFNo };
                    //var _phone = new ApplicationUser { PhoneNumber = model.PhoneNumber };
                    //var _email= new ApplicationUser { Email = model.Email };

                    var _result = await _userManager.CreateAsync(_user, model.Password);


                    if (!_result.Succeeded)
                    {
                        foreach (var error in _result.Errors)
                        {
                            result.Message = error.Description;
                            return Ok(result);
                        }
                    }
                    var userClaimsresult = await _userManager.AddClaimsAsync(_user, claims);


                    result.Status = Status.Success;
                    result.Message = "Successfully Saved";
                    result.Data = model;

                }
              
               

            }
            catch (Exception ex)
            {
                ex.LogAsync(ControllerContext.HttpContext);
            }

            return Ok(result);
        }

        public ActionResult Profile(string id)
        {
           
                var user = _userManager.Users.SingleOrDefault(x => x.Id == id);
                UserProfile vm = new UserProfile();
                vm.UserName = user.UserName;
                vm.PhoneNumber = user.PhoneNumber;
                vm.SageUserName = user.SageUserName;
                vm.Email = user.Email;
                vm.PFNo = user.PFNo;
                vm.Designation = user.Designation;
            //vm.UserName = user.UserName;

            vm.Operation = "update";
                return View("EditProfile", vm);
            



        }
    }
}
