using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shampan.Core.Interfaces.Services.UserRoll;
using Shampan.Models;

namespace SSLAudit.Controllers
{
	[ServiceFilter(typeof(UserMenuActionFilter))]
	[Authorize]
	public class UserMenuActionFilter : IActionFilter
    {
        private readonly IUserRollsService _userRollsService;

        public UserMenuActionFilter(IUserRollsService userRollsService)
        {
            _userRollsService = userRollsService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                
                string userName = context.HttpContext.User.Identity.Name;
                List<UserManuInfo> userMenu = _userRollsService.GetUserManu(userName);
                List<SubmanuList> usereSubManu = _userRollsService.GetUserSubManu(userName);

                

                context.HttpContext.Items["UserMenu"] = userMenu; // Store the user menu in HttpContext.Items
                context.HttpContext.Items["usereSubManu"] = usereSubManu; // Store the user menu in HttpContext.Items
            }
			
		}

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // This method is called after the action method is executed
        }
        


      
    }
}
