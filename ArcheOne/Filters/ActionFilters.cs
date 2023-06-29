using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArcheOne.Filters
{
    public class ActionFilters : ActionFilterAttribute
    {
        private readonly CommonHelper _commonHelper;
        public ActionFilters(CommonHelper commonHelper)
        {
            _commonHelper = commonHelper;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // This code will execute before the Index method is called
            string controllerName = filterContext.ActionDescriptor.RouteValues["controller"] ?? "";
            string actionName = filterContext.ActionDescriptor.RouteValues["action"] ?? "";


            if (controllerName != "LogIn" && controllerName != "Dashboard")
            {
                List<IndexDashboardResModel> permissionList = _commonHelper.GetPermissionList();
                if (!permissionList.Any(x => x.PermissionRoute.Contains($"{controllerName}/{actionName}")))
                {
                    // Update the route values
                    filterContext.RouteData.Values["controller"] = "Home";
                    filterContext.RouteData.Values["action"] = "Error";

                    // Redirect to the new controller and action
                    filterContext.Result = new RedirectToActionResult("Home", "Error", null);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
