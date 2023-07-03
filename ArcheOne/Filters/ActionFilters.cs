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


            if (controllerName != "LogIn" && controllerName != "Dashboard" && controllerName != "Error")
            {
                bool isAjaxRequest = filterContext.HttpContext.Request.Headers.ContainsKey("X-Requested-With");
                List<IndexDashboardResModel> permissionList = _commonHelper.GetPermissionList();
                //if ((!isAjaxRequest || (isAjaxRequest && !permissionList.Any(x => x.IsAjaxRoute && x.PermissionRoute.Contains($"{controllerName}/{actionName}")))) && !permissionList.Any(x => x.PermissionRoute.Contains($"{controllerName}/{actionName}")))
                if (!permissionList.Any(x => x.PermissionRoute.Contains($"{controllerName}/{actionName}")))
                {
                    // Create a RouteValueDictionary to store the route values
                    var routeValues = new RouteValueDictionary(new
                    {
                        controller = "Error",
                        action = "Forbidden",
                        isAjax = isAjaxRequest // Set the value of isAjax parameter
                    });

                    // Redirect to the new controller, action, and pass the route values
                    filterContext.Result = new RedirectToActionResult("Forbidden", "Error", routeValues);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
